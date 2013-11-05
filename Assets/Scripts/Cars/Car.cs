using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Diese Klasse enthält alle relevanten Daten eines Autos. 
 * Außerdem verarbeitet sie den Input eines Controller Scripts, dies kann ein Spieler oder eine KI sein.
 * Hier findet Gangwechslen statt, es werden Widerstandskräfte hinzugefügt
 *
 */


public class Car : MonoBehaviour {
	
	//nur zu testzwcken
 	public float SteerAngle = 30;
	
	//um nicht jedes Rad neu ändern zu müssen, werden hier die Daten geändert
	//die maximale Länge der Feder im auseinander gezogenen ZUstand
	public float suspensionDistance;
	//Dämpfungswert für die Feder
	public float suspensionDamper;
	//die Kraft, die die Feder aushalten kann, am Motor meistens stärker, hier für vorne
	public float suspensionSpringFront;
	//die Kraft, die die Feder aushalten kann, am Motor meistens stärker, hier für hinten
	public float suspensionSpringRear;
	
	
	//Eigenschaften des Autos
	
	//Motor, Schaltung
	//Drehmomentkennline des Fahrzeuges. X-Achse entspricht Umdrehungen pro Minute (RPM), Y-Achse enstspricht der daraus resultierende 
	//Drehmonent in Newtonmeter
	public AnimationCurve engineTorqueCurve;
	//Effizienz der Schaltung (es entstehen immer Verluste)
	public float transmissionEfficiency = 0.7f;
	//Übersetzungswerte der Schaltung, kleiner Gang = hohe Übersetzung, hoher Gang = kleine Übersetzung, erster Wert ist Rückwärtsgang
	public float[] gearRatio = {-1.5f, 2.3f, 1.8f, 1.25f, 1.0f, 0.7f, 0.5f};
	//Werte für die Gangschaltung, damit sie weiss ab welcher Umdrehungszahl sie höher schalten soll
	public float[] RPMToGearUp = {3000, 3000, 3000, 3000, 3000, 4500};
	//Werte für die Gangschaltung, damit sie weiss ab welcher Umdrehungszahl sie runter schalten soll
	public float[] RPMToGearDown = {3000, 2000, 2500, 2500, 2500, 4500};
	//Verstärung des Differenzials
	public float differentialMultiplier = 3f;
	//kraft des Bremspedals
	public float breakTorque = 20;
	
/*	
	//höhster lenkwinkel
	public maxSteerAngle = 30;
	//kleinster Lenkwikel
	public minSteerAngle = 10;
*/	
	//Werte für Wiederstandskräfte
	//Wert für Luftwiederstand beim geradeaus fahren, setzt sich unter anderen aus Luftdichte und Fläche zusammen
	public float CDrag = 0.4f;
	//Multiplier für seitlichen Luftwiederstand
	public float sideDragMultiplier = 1.0f;
	//Motorbremse
	public float engineBreakingMultiplier = 0.5f;

	//Zentrum der Masse für den RigiBody
	public Transform CenterOfMass;
	
	//Referenzen zu den Reifen
	public Wheel[] wheels;
	//liste mit lenkrädern
	private List<Wheel> steerWheels;
	//liste mit beschleiunigungsrädern
	private List<Wheel> driveWheels;
	
	//WheelFrictionCurves, sollten für alle Reifen gleich sein. Um das nicht ständig ändern zu müssen, wird das an einer zentralen
	//Stelle gepsichert und an die Reifen weitergegeben
	//vorwärtsfahren
	private WheelFrictionCurve forwardWFC;
	//seitwärsfahren
	private WheelFrictionCurve sidewaysWFC;
	
	//Werte im Bezug auf den Controller
	// Wert für die Beschleinigung, von -1 (rückwärts beschleunigen bzw. bremsen) bis 1 (vorwärts beschleunigen)
	private float throttle = 0.0f;
	//Wert für den Lenker, von -1 (links) bis 1 (rechts)
	private float steer = 0.0f;

	//Aktuelle Werte
	//aktueller Gang, 1 ist für 1. Gang, 0 ist für Rückwärtsgang
	private int currentGear = 1;
	//aktuelle Umdrehungen des Motors pro Minute
	private float currentRPM = 1000;
	//Timer für den letzten Gangwechsel
	private float gearChangeTimer = 0;
	
	//referenz auf aktuelles Objekt
	private Transform thisTransform;
	//referenz auf eigenens rigidBody
	private Rigidbody thisRigidBody;
	
	
	void Awake()
	{
		thisTransform = transform;
		thisRigidBody = rigidbody;
	}
	
	// Use this for initialization
	void Start () {		
		//richte die Räder ein
		setupWheels();

		//Massezemtrum sollte weiter vorne und weiter unten liegen, daher Referenz auf eine anderes in der Hierariche plaziertes Objekt
		//rigidbody.centerOfMass = CenterOfMass.transform.position;
		thisRigidBody.centerOfMass = CenterOfMass.localPosition;
		
		//setForwardFrictionCurve(float asymSlip, float asymValue, float exSlip, float exValue, float stiff);

	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	// Update is called regularly 
	//in dieser Methoden werden die Physikberechnungen durchgeführt
	void FixedUpdate () 
	{
		//relative Geschwindigkeit ausrechnen, von WorldSpace zu LocalSpace
		Vector3 relativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);
		
		//Zeit seit den letzten Gangwechsel verringern
		gearChangeTimer-= Time.deltaTime;
		
		//applyResistanceForces(relativeVelocity);
		//updateWFC();
		
		//nur Beschleinigung hinzufügen, wenn der Gang nicht gewechselt wird
		if(gearChangeTimer<0)
		{
			applyMotorTorque(relativeVelocity);	
		}
		
		applyMotorTorque(relativeVelocity);	
		
		applyBrakeTorque(relativeVelocity);
		calculateRPM();
		updateGear(relativeVelocity);
		
		applySteering();
		
		Debug.Log ("Gear: " + currentGear + " RPM: " + currentRPM);
	}
	
	//Der Wert wird vom InputPlayerXXController geändert
	public void setThrottle(float th)
	{
		throttle = th;
	}
	
	//Der Wert wird vom InputPlayerXXController geändert
	public void setSteer(float st)
	{
		steer = st;
	}
	
	private void setupWheels()
	{
		driveWheels = new List<Wheel>();
		steerWheels = new List<Wheel>();
		setupWFC();
		
		//füge der Liste die richtigen Wheels zu und übertrage Federwerte
		foreach(Wheel wheel in wheels)
		{
			if(wheel.isFrontWheel)
			{
				wheel.setSpringValues(suspensionDistance, suspensionDamper, suspensionSpringFront);
				wheel.setFrictionCurves(forwardWFC, sidewaysWFC);
			}
			else
			{
				wheel.setSpringValues(suspensionDistance, suspensionDamper, suspensionSpringRear);
				wheel.setFrictionCurves(forwardWFC, sidewaysWFC);
			}
			
			if(wheel.isDriveWheel)
			{
				driveWheels.Add(wheel);
			}
			if(wheel.isSteerWheel)
			{
				steerWheels.Add(wheel);
			}
		}
	}
	
	private void getInput()
	{
		/*
		//sollte aber nicht kleiner als 0.5 werden
		float engineDamageModifier = EngineHealt / 100;
		
		*/
	}
	
	private void setupWFC()
	{
		//normales geradeaus fahren
		forwardWFC = new WheelFrictionCurve();
		forwardWFC.asymptoteSlip = 2.0f;
		forwardWFC.asymptoteValue = 400f;
		forwardWFC.extremumSlip = 0.5f;
		forwardWFC.extremumValue = 6000;
		forwardWFC.stiffness = 1.0f;
		
		//seitliches rutschen/bewegen
		sidewaysWFC = new WheelFrictionCurve();
		sidewaysWFC.asymptoteSlip = 2.0f;
		sidewaysWFC.asymptoteValue = 150f;
		sidewaysWFC.extremumSlip = 1f;
		sidewaysWFC.extremumValue = 350;
		sidewaysWFC.stiffness = 1.0f;
		
			
	}
	
	//in dieser Methode werden die Widerstandskräfte berechent
	private void applyResistanceForces(Vector3 relativeVelocity)
	{
		//Rollwiderstand
		//Rollwiederstandkoefizient ist abhängig vom Untergrund
		//eventuell kannman auch die Layer bestimmen, auf dem der Wagen fährt
		float GroundCoefRR = 0.015f;
		//Rollwiderstandswert = Rollwiederstandskoeffizient * Masse * Gravitation
		float CoefRR = GroundCoefRR * rigidbody.mass * 9.81f;
		//Rollwiderstandskraft, ist entgegengesetzt der aktuellen Fahrtrichtung des Autos
		Vector3 RollingResistanceForce = -Mathf.Sign(relativeVelocity.z) * thisTransform.forward * CoefRR;
		
		//Rollwiderstand wird nur hinzugefügt, wenn alle Räden den Boden berühren
		bool wheelsAreGrounded = true;
		//gucke, ob eines der Räder den Boden nicht berüht
		foreach(Wheel wheel in wheels)
		{
			if(!wheel.wheelCol.isGrounded)
			{
				wheelsAreGrounded = false;
			}
		}
		if(wheelsAreGrounded)
		{
			rigidbody.AddForce(RollingResistanceForce, ForceMode.Impulse);
		}
		
		//Luftwiderstand
		//Luftwiderstandswert CDrag = 0.5 * Luftwiderstandskoeffiezient * Luftdichte * Fläche in Fahrtrichtung
		
		//Vector3 DragForce = CDrag * 
	}
	
	//die WheelFrictionCurves werden abhängig vom Status des Fahrzeugs verädert (Handbremse, etc...), aber auch abhängig vom Untergrund
	private void updateWFC()
	{
		
	}
	
	//in dieser Methode wird die Motorkraft (Drehmoment) auf die Reifen übertragen
	private void applyMotorTorque(Vector3 relVelocity)
	{
		
		//der finale Drehmoment, der auch auf die Reifen übertragen wird. 
		//beim rückwärtsfahren soll die Kraft nicht so hoch sein
		float motorTorque = Mathf.Abs(throttle) * engineTorqueCurve.Evaluate(currentRPM) * gearRatio[currentGear] * differentialMultiplier * transmissionEfficiency;
		
	
		//vorwärts fahren
		if(throttle > 0.0)
		{
			//geh jedes DriveWheel durch und füge Drehmoment hinzu
			foreach(Wheel wheel in driveWheels)
			{
				//werte reseten, da sich das Auto möglicherweise noch fortbewegt, da er noch den Wert vom vorherigen Frame hat
				wheel.wheelCol.brakeTorque = 0f;
				wheel.wheelCol.motorTorque = motorTorque;
			}
		}
		
		//rückwärtsfahren, nur wenn Bremstaste gedrückt wurde und die geschwindigkeit kleiner 0 ist
		if(throttle < 0.0 && relVelocity.z <= 0)
		{
			//mit der Bremstaste soll das auch auch rückwärts fahren können, daher muss geprüft werden, ob sich das Auto rückwärts bewegt
			//kann man prüfen, in dem man sich die Z Komponente der relativen Geschwindigkeit anschaut
			
			Debug.Log("RÜCKWÄRTS");
			//geh jedes DriveWheel durch und füge Drehmoment hinzu
			foreach(Wheel wheel in driveWheels)
			{
				//werte reseten, da sich das Auto möglicherweise noch fortbewegt, da er noch den Wert vom vorherigen Frame hat
				wheel.wheelCol.brakeTorque = 0f;
				//0.1 ist ein Faktor um beim rückwärtsfahren die Kraft zu verringern
				wheel.wheelCol.motorTorque = -motorTorque * 0.1f;
			}
		}
	}
	
	//in dieser Methode wird gebremst
	private void applyBrakeTorque(Vector3 relVelocity)
	{
		//es soll nur mit dem Bremspedal gebremst werden, wenn sich das Auto vorwärts bewegt
		if(throttle < 0.0 && relVelocity.z >0)
		{
			//gehe jedes Rad durch und bremse
			foreach(Wheel wheel in wheels)
			{
				//werte reseten, da sich das Auto möglicherweise noch fortbewegt, da er noch den Wert vom vorherigen Frame hat

				
				wheel.wheelCol.brakeTorque = breakTorque * -throttle;
			}	
		}
		
		//Motorbremse
		if(throttle == 0.0f)
		{
			foreach(Wheel wheel in driveWheels)
			{
				//werte reseten, da sich das Auto möglicherweise noch fortbewegt, da er noch den Wert vom vorherigen Frame hat
				wheel.wheelCol.motorTorque = 0f;
				wheel.wheelCol.brakeTorque = engineBreakingMultiplier * currentRPM/60;
				
			}	
		}
		
	}
	
	//diesse Methode errechnet die momentane Motordrehzahl in anhängigkeit der reifen
	private void calculateRPM()
	{
		foreach(Wheel wheel in driveWheels)
		{
			//berechne die RPM abhängig von der Rad umdrehung, da die Reifen mit dem Motor verbunden sind
			currentRPM = Mathf.Abs(wheel.wheelCol.rpm) * gearRatio[currentGear] * differentialMultiplier;
			//es reicht wenn man ein Rad hat
			break;
		}
		
		if(currentRPM < 1000)
		{
			currentRPM = 1000;
		}
	}
	
	//in dieser Methode wird der Gang gewechselt. Außerdem soll beim rückwärtsfahren der 0. Gang benutzt werden
	private void updateGear(Vector3 relVelocity)
	{
		//falls die vorwärts Geschwindigkeit relativ niedrig ist (nach einen Crash z.B.), wird wieder in den ersten Gang gewechslt
		if(relVelocity.z > 0 && relVelocity.z < 5)
		{
			currentGear = 1;
			return;
		}
		
		//falls gerade der Gang gewechselt wird, müssen wir nicht nochmal wechslen
		if(gearChangeTimer >= 0.0f)
		{
			return;
		}
		
		//Gang höher schalten wenn nicht der letzt Gang erreicht wurde, die Drehzahl hoch ist und man Gas gibt
		if(currentGear < RPMToGearUp.Length && currentRPM > RPMToGearUp[currentGear] && throttle > 0.0)
		{
			currentGear++;
			//Gang wurde gewechselt, bis dahin darf kein motorTorque auf die Reifen übertragen werden (siehe FixedUpdate)
			gearChangeTimer = 0.5f;
		}
		else
		{
			//beim bremsen und wenn die Motordrehzahl niedrig ist soll der Gang runtergeschalter werden, aber nur wenn man sich vorwärts bewegt
			if(relVelocity.z > 0 && currentGear > 1 && currentRPM < RPMToGearDown[currentGear-1] && throttle <= 0.0f) //
			{
				currentGear--;
				gearChangeTimer = 0.5f;	
			}
			//hier wird der Rückwärtsgang eingelegt
			else if(relVelocity.z <= 0 && throttle <= 0.0f)
			{
				currentGear = 0;
				gearChangeTimer = 0.5f;
			}
		}
	}
	
	//in dieser Methode wird das Auto gelenkt
	private void applySteering()
	{
		//lenken, gehe jedes SteerWheel durch und lenke
		foreach(Wheel wheel in steerWheels)
		{
			wheel.wheelCol.steerAngle = SteerAngle * steer;
		}
	}
}

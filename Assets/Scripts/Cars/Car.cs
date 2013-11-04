using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Diese Klasse enthält alle relevanten Daten eines Autos. 
 * Außerdem verarbeitet sie den Input eines Controller Scripts, dies kann ein Spieler oder eine KI sein.
 *
 */


public class Car : MonoBehaviour {
	
	//nur zu testzwcken
	public float MotorTorque = 20;
	public float BreakTorque = 20;
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
	
	
	
/*	Momentan nicht in Gebrauch
	//Motor, Schaltung
	//Drehmomentkennline des Fahrzeuges. X-Achse entspricht Umdrehungen pro Minute (RPM), Y-Achse enstspricht der daraus resultierende 
	//Drehmonent in Newtonmeter
	public AnimationCurve EngineTorqueCurve;
	//Effizienz der Schaltung (es entstehen immer Verluste)
	public float TransmissionEfficiency = 0.7f;
	//Übersetzungswerte der Schaltung, kleiner Gang = hohe Übersetzung, hoher Gang = kleine Übersetzung
	public float[] GearRatio = {2.3f, 1.8f, 1.25f, 1.0f, 0.7f, 0.5f};
	//Werte für die Gangschaltung, damit sie weiss ab welcher Umdrehungszahl sie höher schalten soll
	public float[] RPMToGearUp = {3000, 3000, 3000, 3000, 4500};
	
	//höhster lenkwinkel
	public maxSteerAngle = 30;
	//kleinster Lenkwikel
	public minSteerAngle = 10;
	
	//Werte für Wiederstandskräfte
	//Wert für Luftwiederstand beim geradeaus fahren, setzt sich unter anderen aus Luftdichte und Fläche zusammen
	public float CDrag = 0.4f;
	//Multiplier für seitlichen Luftwiederstand
	public float sideDragMultiplier = 1.0f;
*/
	//Zentrum der Masse für den RigiBody
	public Transform CenterOfMass;
	
	//Referenzen zu den Reifen
	public Wheel[] wheels;
	//liste mit lenkrädern
	private List<Wheel> steerWheels;
	//liste mit beschleiunigungsrädern
	private List<Wheel> driveWheels;
	
	//WheelFrictionCurves
	private WheelFrictionCurve forwardWFC;
	private WheelFrictionCurve sidewaysWFC;
	
/*	momentan nicht in gebrauch
	//WheelFrictionCurves, sollten für alle Reifen gleich sein. Um das nicht ständig ändern zu müssen, wird das an einer zentralen Stelle gepsichert 
	//und an die Reifen weitergegeben
	private WheelFrictionCurve forwardFrictionCurve;
	private WheelFrictionCurve sidewaysFrictionCurve;
	
*/	
	//Werte im Bezug auf den Controller
	// Wert für die Beschleinigung, von -1 (rückwärts beschleunigen bzw. bremsen) bis 1 (vorwärts beschleunigen)
	private float throttle = 0.0f;
	//Wert für den Lenker, von -1 (links) bis 1 (rechts)
	private float steer = 0.0f;
	//Zeit, wie lange die Kuplung noch nicht drin ist, falls > 0 keine Motorkraft auf Reifen
	//private float gearChange = 0.0f;
/*	
	
	//Aktuelle Werte
	//aktueller Gang
	private int currentGear = 0;
	//aktuelle Umdrehungen des Motors pro Minute
	private float currentRPM = 1000;
*/	
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
	void Update () {

		
	}
	
	// Update is called regularly 
	void FixedUpdate () {
		//relative Geschwindigkeit ausrechnen, von WorldSpace zu LocalSpace
		Vector3 relativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);
		
		applyMotorTorque(relativeVelocity);
		applySteering();
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
		float GroundCoefRR = 1f;
		//Rollwiderstandswert = Rollwiederstandskoeffizient * Masse * Gravitation
		float CoefRR = GroundCoefRR * rigidbody.mass * 9.81f;
		//Rollwiderstandskraft, ist entgegengesetzt der aktuellen Fahrtrichtung des Autos
		Vector3 RollingResistanceForce = -Mathf.Sign(relativeVelocity.z) * transform.forward * CoefRR;
		
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
	
	private void applyMotorTorque(Vector3 relVelocity)
	{
		
		//Gas geben, Drehmoment wird auf reifen übertragen, nur wenen Reifen boden berühren
		if(throttle > 0.0)
		{
			//geh jedes DriveWheel durch und füge Drehmoment hinzu
			foreach(Wheel wheel in driveWheels)
			{
				//werte reseten, da sich das Auto möglicherweise noch fortbewegt, da er noch den Wert vom vorherigen Frame hat
				wheel.wheelCol.brakeTorque = 0f;
				wheel.wheelCol.motorTorque = 0f;
				if(wheel.wheelCol.isGrounded)
				{
					wheel.wheelCol.motorTorque = MotorTorque * throttle;
				}
			}
		}
		
		//bremsen
		if(throttle < 0.0)
		{
			//mit der Bremstaste soll das auch auch rückwärts fahren können, daher muss geprüft werden, ob sich das Auto rückwärts bewegt
			//falls ja, bewege rückwärts, falls nein, bremse
			//kann man prüfen, in dem man sich die Z Komponente der relativen Geschwindigkeit anschaut
			
			//falls größer 0, bremse
			if(relVelocity.z > 0)
			{
				//gehe jedes Rad durch und bremse
				foreach(Wheel wheel in wheels)
				{
					//werte reseten, da sich das Auto möglicherweise noch fortbewegt, da er noch den Wert vom vorherigen Frame hat
					wheel.wheelCol.brakeTorque = 0f;
					wheel.wheelCol.motorTorque = 0f;
					if(wheel.wheelCol.isGrounded)
					{
						wheel.wheelCol.brakeTorque = BreakTorque * -throttle;
					}
				}
			}
			//sonst fahre rückwärts
			else
			{
				//geh jedes DriveWheel durch und füge Drehmoment hinzu
				foreach(Wheel wheel in driveWheels)
				{
					//werte reseten, da sich das Auto möglicherweise noch fortbewegt, da er noch den Wert vom vorherigen Frame hat
					wheel.wheelCol.brakeTorque = 0f;
					wheel.wheelCol.motorTorque = 0f;
					if(wheel.wheelCol.isGrounded)
					{
						wheel.wheelCol.motorTorque = MotorTorque * throttle;
					}
				}
			}
		}
	}
	
	private void applySteering()
	{
		//lenken, gehe jedes SteerWheel durch und lenke
		foreach(Wheel wheel in steerWheels)
		{
			if(wheel.wheelCol.isGrounded)
			{
				wheel.wheelCol.steerAngle = SteerAngle * steer;
			}
		}
	}
	
}

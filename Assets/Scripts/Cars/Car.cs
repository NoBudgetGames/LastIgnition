using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Diese Klasse enthält alle relevanten Daten eines Autos. z.B. die Kennlinie der Motors (verhältniss Drehzahl zu Drehmoment)
 * oder die Übersetzungsverhältnsse der Getriebes, es wird ein Gangwechsel simuliert, Widerstandskräfte werden hinzugefügt 
 * Außerdem verarbeitet sie den Input eines Controller Scripts, dies kann ein Spieler oder eine KI sein.
 * Daneben hat jedes Autos auch Lebenspunkte, die bei Kollisionen oder durch Waffen verringert werden
 * Dazu gibt es noch mehrere Modele, die in abhängigkeit der Lebenspunkte ausgetauscht werden
 */


public class Car : MonoBehaviour {
	
	
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
	public float[] RPMToGearUp = {3000, 3000, 3000, 3000, 3000, 3500};
	//Werte für die Gangschaltung, damit sie weiss ab welcher Umdrehungszahl sie runter schalten soll
	public float[] RPMToGearDown = {3000, 2000, 2500, 2500, 2500, 2500};
	//Verstärung des Differenzials
	public float differentialMultiplier = 3f;
	//kraft des Bremspedals
	public float breakTorque = 20;
	//der Rutschmultiplikator für das Seitfärtsfahren, wir mit dem stiffnesswert der sideWFC verrechnet
	public float slipMultiplier = 0.7f;
	
	//höhster lenkwinkel
	public float maxSteerAngle = 40;
	//kleinster Lenkwikel
	public float minSteerAngle = 15;

	//Werte für Wiederstandskräfte
	//Wert für Luftwiederstand beim geradeaus fahren, setzt sich unter anderen aus Luftdichte und Fläche zusammen
/*	
	public float CDrag = 0.4f;
	//Multiplier für seitlichen Luftwiederstand
	public float sideDragMultiplier = 1.0f;
*/	
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
	
	//Liste der Schadensmodelle
	//Frontmodelle
	public GameObject[] frontDamageModels;
	//Heckmodelle
	public GameObject[] rearDamageModels;
	//linke Modelle (Türen?)
	public GameObject[] leftDamageModels;
	//rechte Modelle
	public GameObject[] rightDamageModels;
	
	//WheelFrictionCurves, sollten für alle Reifen gleich sein. Um das nicht ständig ändern zu müssen, wird das an einer zentralen
	//Stelle gespeichert und an die Reifen weitergegeben
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
	//momentaner Lenkwinkel,abhängig von der Geschwindigkeit
	//private float currentSteerAngle = maxSteerAngle;
	
	//Health Werte
	//der eigentliche, allgemeine Heatlh Wert
	private int health = 100;
	//Health Wert für die Front, wird benötigt um das korrekte Schadensmodel anzuzeigen
	private int frontHealth = 100;
	//Health Wert für den Heck, wird benötigt um das korrekte Schadensmodel anzuzeigen
	private int rearHealth = 100;
	//Health Wert für die linke Seite, wird benötigt um das korrekte Schadensmodel anzuzeigen
	private int leftHealth = 100;
	//Health Wert für die rechte Seite, wird benötigt um das korrekte Schadensmodel anzuzeigen
	private int rightHealth = 100;
	//Healthwert des Motors
	private int engineHealth = 100;
	//Healtwert der Lenkung
	private int steerHealth = 100;
		
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
	void Start () 
	{		
		//richtet die Räder ein
		setupWheels();

		//Massezemtrum sollte weiter vorne und weiter unten liegen, daher Referenz auf eine anderes in der Hierariche plaziertes Objekt
		thisRigidBody.centerOfMass = CenterOfMass.localPosition;
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
		
		applyResistanceForces(relativeVelocity);
		//updateWFC();
		
		//nur Beschleinigung hinzufügen, wenn der Gang nicht gewechselt wird
		if(gearChangeTimer<0)
		{
			applyMotorTorque(relativeVelocity);	
		}
		
		applyBrakeTorque(relativeVelocity);
		calculateRPM();
		updateGear(relativeVelocity);
		
		applySteering(relativeVelocity);
		
		//Debug.Log ("Gear: " + currentGear + " RPM: " + currentRPM + " Velocity: " + relativeVelocity.magnitude);
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
	
	//diese Methode verarbeitet den Schaden, der auf das Auto ausgeübt wurde, wird vom DamageController aufgerufen
	public void applyDamage(int damageAmount, int direction)
	{
		//Schaden von den Lebenspunkten abziehen
		health -= damageAmount;
		
		//falls das Auto keine Lebenspunkte mehr hat, explodiert das Auto und der TrackManager wird benachrichtigt
		if(health <= 0)
		{
			
		}
		
		//zunächst muss geschaut werden, an welcher Stelle der Schaden angerichtet werden soll
		switch (direction)
		{
			case (int)DamageDirection.FRONT:
				setupFrontDamage(damageAmount);
				break;
		}
			
	}
	
	//in dieser Methode wird das errechnet, welches der Schadensmodelle aktuell dargestellet werden soll
	private void setupFrontDamage(int damageAmount)
	{
		frontHealth -= damageAmount;
		
		//Der Index des zu darstellenden Models, 0 = kein Schaden, 1 = mehr schaden usw...
		int damageModelNumber = 0;
		
		//ab welchen Lebenspunkten soll das Model geändert werden?
		if(frontHealth >= 60)
		{	
			damageModelNumber = 0;
		}
		else if(frontHealth >= 30)
		{	
			damageModelNumber = 1;
		}
		else
		{
			damageModelNumber = 2;	
		}
		for(int i = 0; i < frontDamageModels.Length; i++)
		{
		
			frontDamageModels[i].gameObject.SetActive(false);
			if(damageModelNumber == i)
			{
				frontDamageModels[i].gameObject.SetActive(true);
			}
		}
	}
	
	//diese Methode richtet die Reifen ein und fügt sie den richtigen Listen zu
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

	//diese Methode initialisiert alle WheelFrictionCurves
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
		sidewaysWFC.stiffness = 1.0f * slipMultiplier;
		
			
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
		//bei niedrigen Geschwindigkeiten soll kein Rollwiederstand erzeugt werden
		if(wheelsAreGrounded && (relativeVelocity.z > 10 || relativeVelocity.z < -10))
		{
			rigidbody.AddForce(RollingResistanceForce, ForceMode.Impulse);
		}
		
		//Luftwiderstand
		//Luftwiderstandswert CDrag = 0.5 * Luftwiderstandskoeffiezient * Luftdichte * Fläche in Fahrtrichtung
		
		//Vector3 DragForce = CDrag * 
	}
	
	//die WheelFrictionCurves sollen abhängig vom Status des Fahrzeugs (Handbremse, etc...) und abhängig des gerade befahrenens 
	//Untergrunds verändert werden (dies kann man mit dem stiffness-wert der WFC verändert werden)
	private void updateWFC()
	{
		
	}
	
	//in dieser Methode wird die Motorkraft (Drehmoment) aus der Drehzahl des Motors errechnet , mit der Gangschaltung multipliziert und 
	//auf die Reifen übertragen
	private void applyMotorTorque(Vector3 relVelocity)
	{
		
		//der finale Drehmoment, der auch auf die Reifen übertragen wird. 
		//beim rückwärtsfahren soll die Kraft nicht so hoch sein
		float motorTorque = Mathf.Abs(throttle) * engineTorqueCurve.Evaluate(currentRPM) * gearRatio[currentGear] * differentialMultiplier * transmissionEfficiency;
		
	
		//vorwärts fahren
		if(throttle > 0.0f)
		{
			//geh jedes DriveWheel durch und füge Drehmoment hinzu
			foreach(Wheel wheel in driveWheels)
			{
					//werte reseten, da sich das Auto möglicherweise noch fortbewegt/bremst, da er noch den Wert vom vorherigen Frame hat
					//verurschat ein paar selstsame Fehler
					wheel.wheelCol.brakeTorque = 0f;
					wheel.wheelCol.motorTorque = motorTorque;	
			}
		}
		
		//rückwärtsfahren
		//mit der Bremstaste soll man auch rückwärts fahren können, daher muss geprüft werden, ob sich das Auto rückwärts bewegt
		//kann man prüfen, in dem man sich die Z Komponente der relativen Geschwindigkeit anschaut
		if(throttle < 0.0f && relVelocity.z <= 0.1f)
		{
			//geh jedes DriveWheel durch und füge Drehmoment hinzu
			foreach(Wheel wheel in driveWheels)
			{
					//werte reseten, da sich das Auto möglicherweise noch fortbewegt, da er noch den Wert vom vorherigen Frame hat
					wheel.wheelCol.brakeTorque = 0f;
					//0.1 ist ein Faktor um beim rückwärtsfahren die Kraft zu verringern, das Auto soll rückwärtsfahrend schließlich 
					//keine neuen Geschwindigkeitsrekorde aufstellen
					wheel.wheelCol.motorTorque = -motorTorque * 0.1f;
			}
		}
	}
	
	//in dieser Methode wird gebremst
	//zum einen mit dem Bremspedal/-taste, 
	//zum anderen mit der Motorbremse
	private void applyBrakeTorque(Vector3 relVelocity)
	{
		//es soll nur mit dem Bremspedal gebremst werden, wenn sich das Auto vorwärts bewegt
		if(throttle < 0.0f && relVelocity.z > 0.1f)
		{
			//gehe jedes Rad durch und bremse
			foreach(Wheel wheel in wheels)
			{
				//werte reseten, da sich das Auto möglicherweise noch fortbewegt, da er noch den Wert vom vorherigen Frame hat
				wheel.wheelCol.motorTorque = 0;
				wheel.wheelCol.brakeTorque = breakTorque * -throttle;
			}	
		}
		
		//Motorbremse, nur wenn kein Gas gegeben wirdoder  nicht gebremst wird oder keine niedrige Geschwindigkeit vorhanden ist
		if(throttle == 0.0f && (relVelocity.z > 10 || relVelocity.z < -10))
		{
			foreach(Wheel wheel in driveWheels)
			{
				//werte reseten, da sich das Auto möglicherweise noch fortbewegt, da er noch den Wert vom vorherigen Frame hat
				wheel.wheelCol.motorTorque = 0f;
				wheel.wheelCol.brakeTorque = engineBreakingMultiplier * currentRPM/60;
				
			}	
		}
		
	}
	
	//diesse Methode errechnet die momentane Motordrehzahl in abhängigkeit eines der DriveWheels (weil die mit dem Motor verbunden sind)
	private void calculateRPM()
	{
		foreach(Wheel wheel in driveWheels)
		{
			//berechne die RPM abhängig von der umdrehungszahl des Rades, da die Reifen mit dem Motor verbunden sind
			//faktor am Ende ist um die sonst niedrigen Drehzahlen etwas zu kompensieren
			currentRPM = Mathf.Abs(wheel.wheelCol.rpm) * gearRatio[currentGear] * differentialMultiplier * 1.0f;
			//es reicht wenn man ein Rad hat
			break;
		}
		
		//die Motordrehzahl soll nicht weniger als 1000 sein (sonst würd im Reallife der Motor ausgehen)
		if(currentRPM < 1000)
		{
			currentRPM = 1000;
		}
	}
	
	//in dieser Methode wird der Gang gewechselt. Außerdem soll beim rückwärtsfahren der 0. Gang benutzt werden
	private void updateGear(Vector3 relVelocity)
	{
		//falls die vorwärts Geschwindigkeit relativ niedrig ist (nach einen Crash z.B.), wird wieder in den ersten Gang gewechslt
		if(relVelocity.z > 0 && relVelocity.magnitude < 10)
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
			gearChangeTimer = 0.25f;
		}
		else
		{
			//beim bremsen und wenn die Motordrehzahl niedrig ist soll der Gang runtergeschalter werden, aber nur wenn man sich vorwärts bewegt
			if(relVelocity.z > 0 && currentGear > 1 && currentRPM < RPMToGearDown[currentGear-1]) //
			{
				currentGear--;
				gearChangeTimer = 0.25f;	
			}
			//hier wird der Rückwärtsgang eingelegt
			else if(relVelocity.z <= 0 && throttle <= 0.0f)
			{
				currentGear = 0;
				gearChangeTimer = 0.25f;
			}
		}
	}
	
	//in dieser Methode wird das Auto gelenkt und der Schaden auf die Lenkung übertragen
	private void applySteering(Vector3 relVelocity)
	{
		
		//Lenkwinkel abhängig von Geschwindigkeit, sonst zu starke Lenkung bei hohen Geschwindigkeiten
		float currentSteerAngle = Mathf.Lerp(maxSteerAngle, minSteerAngle, relVelocity.magnitude/100 * 0.5f);
		Debug.Log ("SteerAngle " + currentSteerAngle);
	
		
		//lenken, gehe jedes SteerWheel durch und lenke
		foreach(Wheel wheel in steerWheels)
		{
			wheel.wheelCol.steerAngle = currentSteerAngle * steer;
		}
	}
}
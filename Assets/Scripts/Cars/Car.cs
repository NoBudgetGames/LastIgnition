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

public class Car : MonoBehaviour 
{
	
//// AUFHÄNGUNG
	
	//um nicht jedes Rad neu ändern zu müssen, werden hier die Daten geändert
	//die maximale Länge der Feder im auseinander gezogenen ZUstand
	public float suspensionDistance;
	//Dämpfungswert für die Feder
	public float suspensionDamper;
	//die Kraft, die die Feder aushalten kann, am Motor meistens stärker, hier für vorne
	public float suspensionSpringFront;
	//die Kraft, die die Feder aushalten kann, am Motor meistens stärker, hier für hinten
	public float suspensionSpringRear;
	
//// EIGENSCHAFTEN DES AUTOS
	
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
	public float brakeTorque = 20;
	//der Rutschmultiplikator für das Seitfärtsfahren, wir mit dem stiffnesswert der sideWFC verrechnet
	public float slipMultiplier = 0.7f;
	
	//höhster lenkwinkel
	public float maxSteerAngle = 30;
	//kleinster Lenkwikel
	public float minSteerAngle = 12;

	//Werte für Wiederstandskräfte
	//Motorbremse
	public float engineBrakingMultiplier = 0.5f;
	//Wert für Schaden einen anderen Auto bei einer Kollision
	public float crashDamage = 0.1f;

/*	
 	//Wert für Luftwiederstand beim geradeaus fahren, setzt sich unter anderen aus Luftdichte und Fläche zusammen
	public float CDrag = 0.4f;
	//Multiplier für seitlichen Luftwiederstand
	public float sideDragMultiplier = 1.0f;
*/	
	
//// REFERENZEN AUF OBJEKTE 
	
	//Zentrum der Masse für den RigiBody (während er in der Luft ist)
	public Transform CenterOfMass;
	//Zentrumd der Masse dür den RigidBody, während dem fahren (damit er nicht beim Kurvenfahren umkippt)
	public Transform CenterOfMassDown;

	//Liste der Schadensmodelle
	//Frontmodelle
	public GameObject[] frontDamageModels;
	//Heckmodelle
	public GameObject[] rearDamageModels;
	//linke Modelle
	public GameObject[] leftDamageModels;
	//rechte Modelle
	public GameObject[] rightDamageModels;
	//vordere linke Modelle
	public GameObject[] frontLeftDamageModels;
	//vordere rechte Modelle
	public GameObject[] frontRightDamageModels;
	//hintere linke Modelle
	public GameObject[] rearLeftDamageModels;
	//hintere rechte Modelle
	public GameObject[] rearRightDamageModels;
	
	//Referenzen zu den Reifen
	public List<Wheel> wheels;
	//Referenz auf loses Rad 
	public GameObject loseWheel;
	//referenz auf linke Tür
	public GameObject leftDoor;
	//referenz auf rechte Tür
	public GameObject rightDoor;
	//referenz auf Motorhaube
	public GameObject frontDoor;
	//referenz auf Kofferraumtür
	public GameObject rearDoor;
	//Objekt um das Absenken des Autos zu verhindern, wenn es einen Reifen verliert
	public GameObject wheelSphereCol;
	//liste mit lenkrädern
	private List<Wheel> steerWheels;
	//liste mit beschleiunigungsrädern
	private List<Wheel> driveWheels;
	
	//referenz auf aktuelles Objekt, ist einfacher die Referenz zu setzen anstadt mit GetComponent die Komponente zu suchen
	private Transform thisTransform;
	//referenz auf eigenens rigidBody, 
	private Rigidbody thisRigidBody;
	
//// WHEELFRICTIONCURVES
	
	//WheelFrictionCurves, sollten für alle Reifen gleich sein
	//vorwärtsfahren
	private WheelFrictionCurve forwardWFC;
	//seitwärsfahren bzw. rutschen
	private WheelFrictionCurve sidewaysWFC;
	//seitwärtsrutschen falls Handbremse benutzt wird
	private WheelFrictionCurve sidewaysHandbrakeWFC;
	
//// INPUT WERTE
	
	// Wert für die Beschleinigung, von -1 (rückwärts beschleunigen bzw. bremsen) bis 1 (vorwärts beschleunigen)
	private float throttle = 0.0f;
	//Wert für den Lenker, von -1 (links) bis 1 (rechts)
	private float steer = 0.0f;
	//bool für die Handbremse
	private bool handbrake = false;
	
//// AKTUELLE WERTE
	
	//aktueller Gang, 1 ist für 1. Gang, 0 ist für Rückwärtsgang
	private int currentGear = 1;
	//aktuelle Umdrehungen des Motors pro Minute
	private float currentRPM = 1000;
	//Timer für den letzten Gangwechsel
	private float gearChangeTimer = 0;
	//sind alle Reifen auf dem Boden?
	private bool areAllWheelsGrounded = false;
	//ist mindestens ein Rad auf dem Boden?
	private bool isOneWheelGrounded = false;
	//ist das Auto am vorwärts beschleunigen?
	private bool isAccelearting = false;
	//ist das Auto am bremsen
	private bool isBraking = false;
	//ist das Auto am rückwärts fahren?
	private bool isReversing = false;
	//Aktuelle Gewschwindigkeit
	private float currentVelocity = 0.0f;
	//Relative Neigungsänderung gegenüber dem letzten Frame, wird benötigt um den WheelColider Bug zu umgehen
	private Vector3 previousInclination = Vector3.zero;
	//hat das Auto schon einen Reifen verloren?
	private bool hasLostWheel = false;

//// HEALTH

	//gesamter Health Wert
	private float health = 100f;
	//Health Wert für die Front, wird benötigt um das korrekte Schadensmodel anzuzeigen
	private int frontHealth = 100;
	//Health Wert für den Heck, wird benötigt um das korrekte Schadensmodel anzuzeigen
	private int rearHealth = 100;
	//Health Wert für die linke Seite, wird benötigt um das korrekte Schadensmodel anzuzeigen
	private int leftHealth = 100;
	//Health Wert für die rechte Seite, wird benötigt um das korrekte Schadensmodel anzuzeigen
	private int rightHealth = 100;
	//Health Wert für die vordere linke Seite, wird benötigt um das korrekte Schadensmodel anzuzeigen
	private int frontLeftHealth = 100;
	//Health Wert für die vordere rechte Seite, wird benötigt um das korrekte Schadensmodel anzuzeigen
	private int frontRightHealth = 100;
	//Health Wert für die hintere linke Seite, wird benötigt um das korrekte Schadensmodel anzuzeigen
	private int rearLeftHealth = 100;
	//Health Wert für die hintere rechte Seite, wird benötigt um das korrekte Schadensmodel anzuzeigen
	private int rearRightHealth = 100;

/*
	//Healthwert des Motors
	private int engineHealth = 100;
	//Healtwert der Lenkung
	private int steerHealth = 100;
*/		

//// START UND UPDATE METHODEN

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
		thisRigidBody.centerOfMass = CenterOfMassDown.localPosition;
		
		//richtige Schadensmodelle aktivieren
		applyVisualDamage(0, (int)DamageDirection.FRONT);
	}
	
	//in dieser Methode werden die Physikberechnungen durchgeführt
	void FixedUpdate () 
	{
		//relative Geschwindigkeit ausrechnen, von WorldSpace zu LocalSpace
		Vector3 relativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);
		//momentane Geschwindigkeit
		currentVelocity = relativeVelocity.magnitude;
		
		//Zeit seit den letzten Gangwechsel verringern
		gearChangeTimer-= Time.deltaTime;
		//Status des Autos feststellen
		calculateStatus(relativeVelocity);		
		
		//Widerstanskräfte berechnen
		applyResistanceForces(relativeVelocity);
		//WFC updaten
		updateWFC();
		//Auto stabilisieren
		stabilizeCar();
		
		//nur Beschleinigung hinzufügen, wenn gerade der Gang nicht gewechselt wird
		if(gearChangeTimer<0)
		{
			applyMotorTorque(relativeVelocity);	
		}
		//bremskräfte hinzufügen
		applyBrakeTorque(relativeVelocity);
		
		//Gangwechslen falls nötig
		updateGear(relativeVelocity);
		//Motordrehzahl ausrechnen
		calculateRPM();
		//Lenkung hinzufügen		
		applySteering(relativeVelocity);
		
		//Debug.Log ("Gear: " + currentGear + " RPM: " + currentRPM + " Velocity: " + relativeVelocity.magnitude);		
	}

//// GET METHODEN
	
	//lefert die aktuelle Geschwindigkeit zurück
	public float getVelocity()
	{
		return currentVelocity;
	}

	//liefert den Geschwindigkeitsvektor zurück
	public Vector3 getVelocityVector()
	{
		return thisRigidBody.velocity;
	}
	
	//liefert aktuelle Drehzahl zurück
	public float getRPM()
	{
		return currentRPM;
	}
	
	//liefert aktuellen Gang zurück
	public int getGear()
	{
		return currentGear;
	}

	//liefert aktuellen Gang zurück
	public float getHealth()
	{
		return health;
	}

	//ist das Auto gerade am rückwärtsfahren?
	public bool isCarReversing()
	{
		return isReversing;
	}
	
//// INPUT METHODEN
	
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
	
	//in dieser Methode wird das Auto nach einen Unfall (wenn er z.B. auf dem Dach liegt) wieder auf die Straße gesetzt
	public void resetCar(float reset)
	{
		if(reset > 0.0f)
		{
			//falls das Auto auf dem Dach oder schief liegt wird es wieder zurückgesetzt
			Vector3 tempAngles = thisTransform.eulerAngles;
			tempAngles.z = 0.0f;
			tempAngles.x = 0.0f;
			thisTransform.eulerAngles = tempAngles;
			
			//die Geschwindigkeit wird auch resetet
			thisRigidBody.velocity = Vector3.zero;			
			
			//Die Position muss wieder angepasst werden, es wird ein Ray nach unten geschoßen und den Aufprallpunkt 
			//als neue Position gewählt
			RaycastHit rayHit;
			if(Physics.Raycast(thisTransform.position, -Vector3.up, out rayHit))
			{
				Vector3 tempPos = thisTransform.position;
				//+4 um das Auto nicht im Boden versinken zu lassen
				tempPos.y = rayHit.point.y + 4;
				thisTransform.position = tempPos;
			}
		}
	}
	
	//in dieser Methode wird überprüft, ob die Handbremse betätigt wurde, wird vom InputControlle aufgerufen
	public void setHandbrake(bool handBrake)
	{
		if(handBrake)
		{
			handbrake = true;
		}
		else
		{
			handbrake = false;
		}
	}
	
//// SCHADENSMODELLE AUFSETZEN, EXPLODIEREN

	public void applyDamage(DamageDirection direction, float damageAmount)
	{
		health -= damageAmount;	
		//Wenn die Lebenspunkte 0 erreichen wird das Objekt zerstört
		applyVisualDamage(direction, (int)damageAmount);
		if(health<=0.0f)
		{
			explodeCar();
		}

	}

	//diese Methode sorgt dafür, dass das Auto einen Reifen verliert
	private void loseAWheel(DamageDirection direction)
	{
		Wheel wheelToDestroy = null;
		hasLostWheel = true;
		//gehe durch jedes Rad durch und such dasd richtige raus
		foreach(Wheel wheel in wheels)
		{	
			if(wheel.direction == direction)
			{
				wheelToDestroy = wheel;
				break;
			}
		}
		//call by reference durch ref
		if(wheelToDestroy != null)
		{
			removeWheelFromList(ref wheelToDestroy);
		}
	}
	
	//lösche das Rad aus den Listen, lösche es und instanziere ein neues Rad
	private void removeWheelFromList(ref Wheel wheel)
	{
		//Rad aus zugehöriger Liste entfernen
		if(wheel.isDriveWheel)
		{
			driveWheels.Remove(wheel);
		}
		if(wheel.isSteerWheel)
		{
			steerWheels.Remove(wheel);
		}
		//neues Rad um in der Welt rumzufliegen
		GameObject.Instantiate(loseWheel, wheel.transform.position, wheel.transform.rotation);
				
		//den Collider dahin stellen, wo vorher das Rad war (um ein absenken des Auto an dieser Stelle zu verhindern)
		wheelSphereCol.transform.localPosition = wheel.transform.localPosition;
		
		//Rad aus Liste entfernen und löschen
		wheels.Remove(wheel);
		wheel.transform.parent = null;
		//hier muss explizit auf das dazugehörige gameObject zugegriffen werden
		GameObject.Destroy(wheel.gameObject);
	}

	//diese Methode löst die Tür vom Auto
	private void removeDoor(GameObject door)
	{
		//falls die noch nicht entfernt wurde
		if(door.GetComponent<HingeJoint>() != null)
		{
			//auszuhaltende Kraft soll 0 sein
			door.GetComponent<HingeJoint>().breakForce = 0;
			//damit man mi einer gerigen Kraft die TÜr lösen kann
			door.GetComponent<Rigidbody>().AddForceAtPosition(door.transform.up, door.transform.position);
		}
	}

	//diese Methode lässt das Auto explodieren, alle Reifen und Türen entfernen
	public void explodeCar()
	{
		wheelSphereCol.SetActive(false);
		//solange noch reifen drin sind, enfferne sie
		while(wheels.Count != 0)
		{
			//immer das erste Elemebt löschen
			Wheel wheelToDestroy = wheels[0];
			removeWheelFromList(ref wheelToDestroy);
		}
		//gehe durch alle Schadenszonen durch und füge (visuellen )schaden hinzu
		foreach(DamageDirection direction in DamageDirection.GetValues(typeof(DamageDirection)))
		{
			applyVisualDamage(direction, 200);
		}
		removeDoor(frontDoor);
 		removeDoor(rearDoor);
		removeDoor(rightDoor);
		removeDoor(leftDoor);
	}
	
	//diese Methode verarbeitet den Schaden und schaut, aus welcher Richtung er kam und ruf entsprechende Methode auf
	private void applyVisualDamage(DamageDirection direction, int damageAmount)
	{
		//zunächst muss geschaut werden, an welcher Stelle der Schaden angerichtet werden soll
		switch (direction)
		{
			case DamageDirection.FRONT:
				setupFrontDamage(damageAmount);
				break;
			case DamageDirection.REAR:
				setupRearDamage(damageAmount);
				break;
			case DamageDirection.RIGHT:
				setupRightDamage(damageAmount);
				break;
			case DamageDirection.LEFT:
				setupLeftDamage(damageAmount);
				break;
			case DamageDirection.FRONT_LEFT:
				setupFrontLeftDamage(damageAmount);
				break;
			case DamageDirection.FRONT_RIGHT:
				setupFrontRightDamage(damageAmount);
				break;
			case DamageDirection.REAR_LEFT:
				setupRearLeftDamage(damageAmount);
				break;
			case DamageDirection.REAR_RIGHT:
				setupRearRightDamage(damageAmount);
				break;
		}
	}
	
	//in dieser Methode wird das graphische Objekt für die Front des Autos aktiviert
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
		//gehe durch die Damage Models durch und aktiviere das richtige
		for(int i = 0; i < frontDamageModels.Length; i++)
		{
			frontDamageModels[i].gameObject.SetActive(false);
			if(damageModelNumber == i)
			{
				frontDamageModels[i].gameObject.SetActive(true);
			}
		}
		//falls das Auto berits schaden hat, wird die Tür aktiviert, damit sie in Kurven auch aufgehen kann
		if(damageModelNumber >= 1)
		{
			frontDoor.SetActive(true);
		}
	}
	
	//in dieser Methode wird das graphische Objekt für den Heck des Autos aktiviert
	private void setupRearDamage(int damageAmount)
	{
		rearHealth -= damageAmount;
		//Der Index des zu darstellenden Models, 0 = kein Schaden, 1 = mehr schaden usw...
		int damageModelNumber = 0;	
		//ab welchen Lebenspunkten soll das Model geändert werden?
		if(rearHealth >= 60)
		{	
			damageModelNumber = 0;
		}
		else if(rearHealth >= 30)
		{	
			damageModelNumber = 1;
		}
		else
		{
			damageModelNumber = 2;	
		}
		for(int i = 0; i < rearDamageModels.Length; i++)
		{
			rearDamageModels[i].gameObject.SetActive(false);
			if(damageModelNumber == i)
			{
				rearDamageModels[i].gameObject.SetActive(true);
			}
		}
		if(damageModelNumber >= 1)
		{
			rearDoor.SetActive(true);
		}
	}
	
	//in dieser Methode wird das graphische Objekt für die linke  Seite des Autos aktiviert
	private void setupLeftDamage(int damageAmount)
	{
		leftHealth -= damageAmount;
		//Der Index des zu darstellenden Models, 0 = kein Schaden, 1 = mehr schaden usw...
		int damageModelNumber = 0;
		//ab welchen Lebenspunkten soll das Model geändert werden?
		if(leftHealth >= 60)
		{	
			damageModelNumber = 0;
		}
		else if(leftHealth >= 30)
		{	
			damageModelNumber = 1;
		}
		else
		{
			damageModelNumber = 2;	
		}
		for(int i = 0; i < leftDamageModels.Length; i++)
		{
			leftDamageModels[i].gameObject.SetActive(false);
			if(damageModelNumber == i)
			{
				leftDamageModels[i].gameObject.SetActive(true);

			}
		}
		if(damageModelNumber >= 1)
		{
			leftDoor.SetActive(true);
		}
	}
	
	//in dieser Methode wird das graphische Objekt für die rechte Seite des Autos aktiviert
	private void setupRightDamage(int damageAmount)
	{
		rightHealth -= damageAmount;
		//Der Index des zu darstellenden Models, 0 = kein Schaden, 1 = mehr schaden usw...
		int damageModelNumber = 0;
		//ab welchen Lebenspunkten soll das Model geändert werden?
		if(rightHealth >= 60)
		{	
			damageModelNumber = 0;
		}
		else if(rightHealth >= 30)
		{	
			damageModelNumber = 1;
		}
		else
		{
			damageModelNumber = 2;	
		}
		for(int i = 0; i < rightDamageModels.Length; i++)
		{
			rightDamageModels[i].gameObject.SetActive(false);
			if(damageModelNumber == i)
			{
				rightDamageModels[i].gameObject.SetActive(true);
			}
		}
		if(damageModelNumber >= 1)
		{
			rightDoor.SetActive(true);
		}
	}

	//in dieser Methode wird das graphische Objekt für die vordere linke  Seite des Autos aktiviert
	private void setupFrontLeftDamage(int damageAmount)
	{
		frontLeftHealth -= damageAmount;
		//Der Index des zu darstellenden Models, 0 = kein Schaden, 1 = mehr schaden usw...
		int damageModelNumber = 0;
		//ab welchen Lebenspunkten soll das Model geändert werden?
		if(frontLeftHealth >= 60)
		{	
			damageModelNumber = 0;
		}
		else if(frontLeftHealth >= 30)
		{	
			damageModelNumber = 1;
		}
		else
		{
			damageModelNumber = 2;	
		}
		for(int i = 0; i < frontLeftDamageModels.Length; i++)
		{
			frontLeftDamageModels[i].gameObject.SetActive(false);
			if(damageModelNumber == i)
			{
				frontLeftDamageModels[i].gameObject.SetActive(true);
			}
		}
		//falls noch kein Reifen verloren worden ist und die Health Punkte relative klein sind
		if(hasLostWheel == false && frontLeftHealth <= 15)
		{
			loseAWheel(DamageDirection.FRONT_LEFT);
		}
	}

	//in dieser Methode wird das graphische Objekt für die vordere rechte Seite des Autos aktiviert
	private void setupFrontRightDamage(int damageAmount)
	{
		frontRightHealth -= damageAmount;
		//Der Index des zu darstellenden Models, 0 = kein Schaden, 1 = mehr schaden usw...
		int damageModelNumber = 0;
		//ab welchen Lebenspunkten soll das Model geändert werden?
		if(frontRightHealth >= 60)
		{	
			damageModelNumber = 0;
		}
		else if(frontRightHealth >= 30)
		{	
			damageModelNumber = 1;
		}
		else
		{
			damageModelNumber = 2;	
		}
		for(int i = 0; i < frontRightDamageModels.Length; i++)
		{
			frontRightDamageModels[i].gameObject.SetActive(false);
			if(damageModelNumber == i)
			{
				frontLeftDamageModels[i].gameObject.SetActive(true);
			}
		}
		//falls noch kein Reifen verloren worden ist und die Health Punkte relative klein sind
		if(hasLostWheel == false && frontRightHealth <= 15)
		{
			loseAWheel(DamageDirection.FRONT_RIGHT);
		}
	}

	//in dieser Methode wird das graphische Objekt für die hintere linke  Seite des Autos aktiviert
	private void setupRearLeftDamage(int damageAmount)
	{
		rearLeftHealth -= damageAmount;
		//Der Index des zu darstellenden Models, 0 = kein Schaden, 1 = mehr schaden usw...
		int damageModelNumber = 0;
		//ab welchen Lebenspunkten soll das Model geändert werden?
		if(rearLeftHealth >= 60)
		{	
			damageModelNumber = 0;
		}
		else if(rearLeftHealth >= 30)
		{	
			damageModelNumber = 1;
		}
		else
		{
			damageModelNumber = 2;	
		}
		for(int i = 0; i < rearLeftDamageModels.Length; i++)
		{
			rearLeftDamageModels[i].gameObject.SetActive(false);
			if(damageModelNumber == i)
			{
				rearLeftDamageModels[i].gameObject.SetActive(true);
			}
		}
		//falls noch kein Reifen verloren worden ist und die Health Punkte relative klein sind
		if(hasLostWheel == false && rearLeftHealth <= 15)
		{
			loseAWheel(DamageDirection.REAR_LEFT);
		}
	}

	//in dieser Methode wird das graphische Objekt für die hintere rechte Seite des Autos aktiviert
	private void setupRearRightDamage(int damageAmount)
	{
		rearRightHealth -= damageAmount;
		//Der Index des zu darstellenden Models, 0 = kein Schaden, 1 = mehr schaden usw...
		int damageModelNumber = 0;
		//ab welchen Lebenspunkten soll das Model geändert werden?
		if(rearRightHealth >= 60)
		{	
			damageModelNumber = 0;
		}
		else if(rearRightHealth >= 30)
		{	
			damageModelNumber = 1;
		}
		else
		{
			damageModelNumber = 2;	
		}
		for(int i = 0; i < rearRightDamageModels.Length; i++)
		{
			rearRightDamageModels[i].gameObject.SetActive(false);
			if(damageModelNumber == i)
			{
				rearRightDamageModels[i].gameObject.SetActive(true);
			}
		}
		//falls noch kein Reifen verloren worden ist und die Health Punkte relative klein sind
		if(hasLostWheel == false && rearRightHealth <= 15)
		{
			loseAWheel(DamageDirection.REAR_LEFT);
		}
	}

//// RÄDER EINRICHTEN
	
	//diese Methode richtet die Reifen ein und fügt sie den richtigen Listen zu
	private void setupWheels()
	{
		driveWheels = new List<Wheel>();
		steerWheels = new List<Wheel>();
		setupWFC();
		
		//füge der Liste die richtigen Wheels zu und übertrage Federwerte
		foreach(Wheel wheel in wheels)
		{
			wheel.wheelCol.motorTorque = 0f;
			wheel.wheelCol.brakeTorque = 0f;
			wheel.wheelCol.steerAngle = 0f;
					
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
		forwardWFC.stiffness = 0.98f;
		
		//seitliches rutschen/bewegen
		sidewaysWFC = new WheelFrictionCurve();
		sidewaysWFC.asymptoteSlip = 2.0f;
		sidewaysWFC.asymptoteValue = 150f;
		sidewaysWFC.extremumSlip = 1f;
		sidewaysWFC.extremumValue = 350;
		sidewaysWFC.stiffness = 0.98f * slipMultiplier;	
		
		//seitliches rutschen falls die Handbremse benutzt wird, dadurch kann das Auto besser um die Kurve driften und Donuts fahren
		sidewaysHandbrakeWFC = new WheelFrictionCurve();
		sidewaysHandbrakeWFC.asymptoteSlip = 2.0f;
		sidewaysHandbrakeWFC.asymptoteValue = 150f;
		sidewaysHandbrakeWFC.extremumSlip = 1f;
		sidewaysHandbrakeWFC.extremumValue = 350;
		sidewaysHandbrakeWFC.stiffness = 0.5f * slipMultiplier;	
	}

//// PHYSIK BERECHNUNGEN, FAHRZEUG WERTE
	
	//in dieser Methode wid festgestellt, ob die Reifen den Boden berühren und wie der aktuelle Status des Autos ist
	private void calculateStatus(Vector3 relVelocity)
	{
		//Überprüfe Gaspedal und Bremse
		//weder Gaspedal nich Bremse sind gedrückt
		if(throttle == 0.0f)
		{
			isAccelearting = false;
			isBraking = false;
			isReversing = false;
		}
		//wenn gaspedal gedrückt ist
		else if(throttle > 0.0f)
		{
			isAccelearting = true;
			isBraking = false;
			isReversing = false;
		}
		//falls bremse oder rückwärts gefahren wird
		else if(throttle < 0.0f)
		{
			//bremsen
			if(relVelocity.z > 0)
			{
				isAccelearting = false;
				isBraking = true;
				isReversing = false;	
			}
			//rückwärtsfahren
			else if(relVelocity.z <= 0)
			{
				isAccelearting = false;
				isBraking = false;
				isReversing = true;	
			}	
		}
		
		//Überprüfe Reifen
		areAllWheelsGrounded = true;
		isOneWheelGrounded = false;
		foreach(Wheel wheel in wheels)
		{
			//berüht mindestens ein Reifen den Boden?
			if(wheel.wheelCol.isGrounded)
			{
				isOneWheelGrounded = true;	
			}
			//ist ein Reifen in der Luft?
			else
			{
				areAllWheelsGrounded = false;
			}
		}
	}
	
	//in dieser Methode werden die Widerstandskräfte berechent
	private void applyResistanceForces(Vector3 relativeVelocity)
	{
		//Rollwiderstand
		//Rollwiederstandkoefizient ist abhängig vom Untergrund, eventuell kannman auch die Layer bestimmen, auf dem der Wagen fährt
		//gehe durch jedes Rad durch und bestimme die Layer auf der er fährt
		float GroundCoefRR = 0.0f;
		int size = 0;
		foreach(Wheel wheel in wheels)
		{
			//falls sich das Rad in der Luft befindet soll es nicht zur Berechnung beitragen
			WheelHit hit;

			if(wheel.wheelCol.GetGroundHit(out hit))
			{
				//schau auf welcher Layer der Refien fährt
				int layer = hit.collider.transform.gameObject.layer;
				//switch erlaubt keine abfragen wie "case LayerMask.NameToLayer("Default"):", daher feste Werte
				//Compiler Error CS0150: A constant value is expected
				switch(layer)
				{
					case 9:
						GroundCoefRR = 0.07f; //fester Sand, SandNormal
						break;
					case 10:
						GroundCoefRR = 0.3f; //loser Sand, SandLose
						break;
					case 11:
						GroundCoefRR = 0.02f; //Schotter, Rubble
						break;
					case 12:
						GroundCoefRR = 0.05f; //Erdweg, Dirt
						break;
					default:
						GroundCoefRR = 0.015f; //asphalt, Default Layer
						break;
				}
				size++;	
			}	
		}
		//bereche den Durchschnitt der wheels
		if(size != 0)
		{
			GroundCoefRR /= size;	
		}

		//Rollwiderstandswert = Rollwiederstandskoeffizient * Masse * Gravitation
		float CoefRR = GroundCoefRR * rigidbody.mass * 9.81f;
		//Rollwiderstandskraft, ist entgegengesetzt der aktuellen Fahrtrichtung des Autos
		Vector3 RollingResistanceForce = -Mathf.Sign(relativeVelocity.z) * thisTransform.forward * CoefRR;
		
		//bei niedrigen Geschwindigkeiten soll kein Rollwiederstand erzeugt werden
		if(isOneWheelGrounded && (relativeVelocity.z > 10 || relativeVelocity.z < -10))
		{
			thisRigidBody.AddForce(RollingResistanceForce, ForceMode.Impulse);
		}
		//Angular Drag soll größer sein, wenn das Auto eine hohe Geschwindigkeit hat
		thisRigidBody.angularDrag = Mathf.Abs(relativeVelocity.z) / 100;
		
		thisRigidBody.drag = 0.1f;
		//falls sich das Auto in der Luft befindet, soll der Luftwiederstand steigen
		if(areAllWheelsGrounded == false)
		{
			thisRigidBody.drag = 0.4f;
		}
		//Luftwiderstand
		//Luftwiderstandswert CDrag = 0.5 * Luftwiderstandskoeffiezient * Luftdichte * Fläche in Fahrtrichtung
		//Vector3 DragForce = CDrag * 
	}
	
	//die WheelFrictionCurves sollen abhängig vom Status des Fahrzeugs (Handbremse, etc...) und abhängig des gerade befahrenens 
	//Untergrunds verändert werden (dies kann man mit dem stiffness-wert der WFC verändert werden)
	private void updateWFC()
	{
		//falls die Handbremse benutzt wird soll eine spezielle WFC für die Hinterreifen verwendet werden
		if(handbrake)
		{
			foreach(Wheel wheel in wheels)
			{
				//falls es ein Vorderrad ist, sollen die normalen WFC bnutzt werden
				if(wheel.isFrontWheel)
				{
					wheel.setFrictionCurves(forwardWFC, sidewaysWFC);
				}
				//ansonsten soll am den HInterrädern eine andere WFC fürs seitliche fahren verwendet werden
				else
				{
					wheel.setFrictionCurves(forwardWFC, sidewaysHandbrakeWFC);
				}
			}
		}
		//ansonsten benutze die normalen WFCs
		else
		{
			foreach(Wheel wheel in wheels)
			{
				wheel.setFrictionCurves(forwardWFC, sidewaysWFC);
			}
		}
	}
	
	//diesse Methode errechnet die momentane Motordrehzahl in abhängigkeit eines der DriveWheels (weil die mit dem Motor verbunden sind)
	private void calculateRPM()
	{
		//resete den Wert und fang von anfang an zu rechnen
		currentRPM = 0;
		int size = 0;
		foreach(Wheel wheel in driveWheels)
		{
			//falls sich das Rad in der Luft befindet soll es nicht zu Berechnung beitragen
			if(wheel.wheelCol.isGrounded)
			{
				//berechne die RPM abhängig von der umdrehungszahl des Rades, da die Reifen mit dem Motor verbunden sind
				currentRPM += Mathf.Abs(wheel.wheelCol.rpm) * gearRatio[currentGear] * differentialMultiplier;
				size++;	
			}	
		}
		//bereche den Durchschnitt der driveWheels
		if(size != 0)
		{
			currentRPM /= size;	
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
		if(isAccelearting && currentGear < RPMToGearUp.Length && currentRPM > RPMToGearUp[currentGear] && areAllWheelsGrounded == true)
		{
			currentGear++;
			currentRPM = RPMToGearDown[currentGear-1];
			//Gang wurde gewechselt, bis dahin darf kein motorTorque auf die Reifen übertragen werden (siehe FixedUpdate)
			gearChangeTimer = 0.25f;
		}
		//wenn die Motordrehzahl niedrig ist soll der Gang runtergeschalter werden, aber nur wenn der zweite oder höher Gänge eingelegt sind
		else if(currentGear > 1 && currentRPM < RPMToGearDown[currentGear-1]) //
		{
			currentGear--;
			gearChangeTimer = 0.25f;	
		}
		//hier wird der Rückwärtsgang eingelegt
		else if(isReversing)
		{
			currentGear = 0;
			gearChangeTimer = 0.25f;
		}
	}
	
	//diese Methode stabiliziert das Auto in Kurven lage. Da die StabilizerBars nicht genug stabiliziert, wird hier einfach der Schwerpunkt nach unten gesetzt
	private void stabilizeCar()
	{
		//nur wenn mindestens 1 Rad den Boden berührt soll das Auto stabiliziert werden
		//so wird verhindert, das das Auto mitten in der Luft sich falsch verhält
		if(isOneWheelGrounded)
		{
			thisRigidBody.centerOfMass = CenterOfMassDown.localPosition;
		}
		else
		{
			thisRigidBody.centerOfMass = CenterOfMass.localPosition;
		}
	}

//// GAS GEBEN, BREMSEN UND LENKEN
	
	//in dieser Methode wird die Motorkraft (Drehmoment) aus der Drehzahl des Motors errechnet , mit der Gangschaltung multipliziert und 
	//auf die Reifen übertragen
	private void applyMotorTorque(Vector3 relVelocity)
	{
		//Drehmoment, der auch auf die Reifen übertragen wird. 
		float motorTorque = Mathf.Abs(throttle) * engineTorqueCurve.Evaluate(currentRPM) * gearRatio[currentGear] * differentialMultiplier * transmissionEfficiency;
		
		//DasAuto beschleunigt plötzlich an einen Hügel. Das ist ein Bug im WheelCollider von Unity. Der Fehler tritt nur auf, wenn sich die neigung der 
		//Straße relativ zum Auto ändert. Um das zu vermeiden wird geschaut, ob sich die Neigung des Autos gegenüber dem letzten Frame geändert hat.
		//Diese Änderung wird mit der Motorkraft verrechnet um diese abzuschwächen
		//Keine X komponente da seitliche Drehung nicht berücksichtigt werden soll
		Vector3 currentForward = new Vector3(0.0f, thisTransform.transform.forward.y,thisTransform.transform.forward.z);
		//winkel zwischen vorwärtsvektor aus dem letzten Frame und den aktuellen.
		float inclinationChange = Vector3.Angle(previousInclination, currentForward);
		previousInclination = currentForward;
	
		//gas geben
		if(isAccelearting)
		{
			//geh jedes DriveWheel durch und füge Drehmoment hinzu
			foreach(Wheel wheel in driveWheels)
			{
				//werte reseten, da sich das Auto möglicherweise noch fortbewegt/bremst, da er noch den Wert vom vorherigen Frame hat
				//verurschat ein paar selstsame Fehler
				wheel.wheelCol.brakeTorque = 0f;
				if(inclinationChange > 0.2)
				{
					//um die beschleinigung zu minimieren wird sie abhängig vom Winkel verkleinert
					motorTorque = motorTorque * Mathf.Lerp(0.2f, 0.01f, inclinationChange);
				}
				wheel.wheelCol.motorTorque = motorTorque;		
			}
		}
		//rückwärtsfahren
		else if(isReversing)
		{
			foreach(Wheel wheel in driveWheels)
			{
				wheel.wheelCol.brakeTorque = 0f;
				if(inclinationChange > 0.2)
				{
					motorTorque = motorTorque * 0.1f;
				}
				wheel.wheelCol.motorTorque = -motorTorque;
			}
		}
	}
	
	//in dieser Methode wird gebremst, zum einen mit dem Bremspedal/-taste, zum anderen mit der Motorbremse
	private void applyBrakeTorque(Vector3 relVelocity)
	{
		//falls die Handbremse gezogen wird, soll auf jeden Reifendie maximale Bremskraft ausgeübt werden
		if(handbrake)
		{
			foreach(Wheel wheel in wheels)
			{
				if(wheel.isFrontWheel)
				{
					wheel.wheelCol.brakeTorque = brakeTorque;	
				}
				else
				{
					//mehr Bremskraft auf Hinterreifen
					wheel.wheelCol.brakeTorque = brakeTorque * 2;
				}
			}	
		}
		//bremsen
		else if(isBraking)
		{
			//gehe jedes Rad durch und bremse
			foreach(Wheel wheel in wheels)
			{
				//werte reseten, da sich das Auto möglicherweise noch fortbewegt, da er noch den Wert vom vorherigen Frame hat
				wheel.wheelCol.motorTorque = 0;
				//throttle ist < 0 daher mit -1 multiplizieren
				wheel.wheelCol.brakeTorque = brakeTorque * -throttle;
			}	
		}
		//Motorbremse, nur wenn kein Gas gegeben wird oder  nicht gebremst wird
		else if(throttle == 0.0f)
		{
			foreach(Wheel wheel in driveWheels)
			{
				//werte reseten, da sich das Auto möglicherweise noch fortbewegt, da er noch den Wert vom vorherigen Frame hat
				wheel.wheelCol.motorTorque = 0f;
				wheel.wheelCol.brakeTorque = engineBrakingMultiplier * currentRPM/60;
			}	
		}
	}
	
	//in dieser Methode wird das Auto gelenkt und der Schaden auf die Lenkung übertragen
	private void applySteering(Vector3 relVelocity)
	{
		//Lenkwinkel abhängig von Geschwindigkeit, sonst zu starke Lenkung bei hohen Geschwindigkeiten, - 75 damit es erst ab 75kmh passiert
		float currentSteerAngle = Mathf.Lerp(maxSteerAngle, minSteerAngle, (relVelocity.magnitude - 75)/100 * 0.8f);
		//lenken, gehe jedes SteerWheel durch und lenke
		foreach(Wheel wheel in steerWheels)
		{
			wheel.wheelCol.steerAngle = currentSteerAngle * steer;
		}
	}
}
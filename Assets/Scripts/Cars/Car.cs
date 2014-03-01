using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Diese Klasse enthält alle relevanten Daten eines Autos. z.B. die Kennlinie der Motors (verhältniss Drehzahl zu Drehmoment)
 * oder die Übersetzungsverhältnsse der Getriebes, es wird ein Gangwechsel simuliert, Widerstandskräfte werden hinzugefügt 
 * Außerdem verarbeitet sie den Input eines Controller Scripts, dies kann ein Spieler oder eine KI sein.
 * Daneben hat jedes Autos auch Lebenspunkte, die bei Kollisionen oder durch Waffen verringert werden
 * Dazu gibt es noch mehrere Modele, die in abhängigkeit der Lebenspunkte ausgetauscht werden
 * (Für eine genaure Beschreibung siehe Doku/Designentscheidungen)
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
	//Multiplier für die Gänge, die ersten Gänge sind zu stark, daher werden diese abgeschwächt. Man könnte auch die gearRation ändern, 
	//allerdings führt das dazu dass das Auto recht spät hochschaltet
	public float[] gearMultiplier = {0.5f, 0.5f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f};
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
	//Glass SchadensModelle
	public GameObject[] glassDamageModels;
	
	//Referenzen zu den Reifen
	public List<Wheel> wheels;
	//Referenz auf loses Rad 
	public GameObject loseWheel;
	//referenz auf das Lenkrad (zweite in der Hierachie über dem Mesh-Objekt)
	public GameObject steeringWheel;
	//referenz auf linke Tür
	public GameObject leftDoor;
	//referenz auf rechte Tür
	public GameObject rightDoor;
	//referenz auf Motorhaube
	public GameObject frontDoor;
	//referenz auf Kofferraumtür
	public GameObject rearDoor;
	//referenz auf vordere Stoßstange
	public GameObject frontBumperPrefab;
	//referenz auf hintere Stoßstange
	public GameObject rearBumperPrefab;
	//referenz auf linken Auspuff
	public GameObject leftExhaustPrefab;
	//referenz auf rechten Auspuff
	public GameObject rightExhaustPrefab;
	//Objekt um das Absenken des Autos zu verhindern, wenn es einen Reifen verliert
	public GameObject wheelSphereCol;
	//Referenz auf den ParticleSystem, der das Feuer erzeugt, wenn das AUto explodiert ist
	public ParticleSystem particleSysForExplosion;
	
	//Referenz auf ein GameObject, zu dem das Auto bei einen drücken der Reset Taste teleportiert wird,
	//normalerweise wäre das die Aktuelle Position des Autos, 
	//beim verlassen der Strecke beim Rundkurs wäre es die Position der zuletzt durchgefahrenen Checkpoints
	private GameObject resetPosition;
	
	//liste mit lenkrädern
	private List<Wheel> steerWheels;
	//liste mit beschleiunigungsrädern
	private List<Wheel> driveWheels;
	
	//referenz auf aktuelles Transfom, 
	private Transform thisTransform;
	//referenz auf eigenens rigidBody, 
	private Rigidbody thisRigidBody;

	//// WHEELFRICTIONCURVES
	
	//WheelFrictionCurves, sollten für alle Reifen gleich sein
	//vorwärtsfahren
	private WheelFrictionCurve forwardWFC;
	//seitwärsfahren für vorne
	private WheelFrictionCurve sidewaysFrontWFC;
	//seitwärsfahren bzw. rutschen für hinten
	private WheelFrictionCurve sidewaysRearWFC;
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
	//hat das Auto schon einen Reifen verloren?
	private bool hasLostWheel = false;
	//hat das Auto die vordere Stoßstange verloren?
	private bool hasLostFrontBumper = false;
	//hat das Auto die hintere Stoßstange verloren?
	private bool hasLostRearBumper = false;
	//hat das Auto die Auspüffe verloren?
	private bool hasLostExhausts = false;
	//Geschwindigkeit im vorherigen Frame
	private float previousVel = 0.0f;
	
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
	//Healthwert ab dem das zweite Schadensmodell angezeigt werden soll (erste Schadensmodell hat keinen sichtbaren Schaden)
	private int secondDamageModelHealthLimit = 70;
	//Healthwert ab dem das dritte Schadensmodell angezeigt werden soll (erste Schadensmodell hat keinen sichtbaren Schaden)
	private int thirdDamageModelHealthLimit = 30;
	
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
		//particleSys deaktivieren
		particleSysForExplosion.enableEmission = false;
		//erstmal deaktivieren, da es noch nicht gebraucht wird
		particleSysForExplosion.gameObject.SetActive(false);
		
		//richtige Schadensmodell aktivieren, gehe durch jede Schadesnzone durch und füge 0 Schaden
		foreach(DamageZone damZone in DamageZone.GetValues(typeof(DamageZone)))
		{
			applyVisualDamage(damZone, 0);
		}
		resetPosition = this.gameObject;
	}
	
	//in dieser Methode wird das Lenkrad gedreht
	void Update()
	{
		if(steeringWheel != null)
		{
			//hier muss eine temporäre Variable erstellt werden
			Vector3 tempRot = steeringWheel.transform.localEulerAngles;
			tempRot.z = -steer * 120;
			steeringWheel.transform.localEulerAngles = tempRot;	
		}
	}
	
	//in dieser Methode werden die Physikberechnungen durchgeführt
	void FixedUpdate () 
	{
		//relative Geschwindigkeit des Fahrzeugs im eigenen LocalSpace umrechnen, von WorldSpace zu LocalSpace
		Vector3 relativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);
		//momentane Geschwindigkeit
		currentVelocity = relativeVelocity.magnitude;
		//Status des Autos feststellen
		calculateStatus(relativeVelocity);		
		
		//Widerstanskräfte berechnen
		applyResistanceForces(relativeVelocity);
		//WFC updaten
		updateWFC();
		//Auto stabilisieren
		stabilizeCar();

		//Zeit seit den letzten Gangwechsel verringern
		gearChangeTimer-= Time.deltaTime;
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
	}
	
	//// GET UND SET METHODEN
	
	//liefert die aktuelle Geschwindigkeit in Kilometer pro Stunde zurück
	//Dazu wird die Unity Geschwinddigkeit in Meter/Sekunde umgerechnet
	//in dem das Größenverhältnisses des Autos verwendet wird
	public float getVelocityInKmPerHour()
	{
		// Länge des Autos (für den Dodge Charger) in Metern (aus Wikipedia): 5283 mm = 5,283 m 
		// Länge des Autos in Unity (vorderes Ende - hinteres Ende, gemessen): 1327.76 - 1306.646 = 21.114
		// Verhältniss: 5,283 / 21.114 = ca. 0,25 ==> durch 4 teilen
		// zunächst in Meter pro Sekunde (/4), dann in Kilometer pro Stunde umrechnen ( * 3.6)
		// 1/4 * 3,6 = 0,9 ==> "Unity-Geschwindigkeit" mit 0.9 multiplizieren
		return currentVelocity * 0.9f;
	}
	
	//liefert den Geschwindigkeitsvektor in WorldSpace zurück
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
	
	//liefert Lebenspunkte zurück
	public float getHealth()
	{
		return health;
	}
	
	//ist das Auto gerade am rückwärtsfahren?
	public bool isCarReversing()
	{
		return isReversing;
	}
	
	//liefert true, wenn das Auto gerade am lenken ist
	public bool isSteering()
	{
		if(steer < 0.1f && steer > -0.1f)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
	
	//setzt die resetPosition
	public void setResetPosition(GameObject obj)
	{
		resetPosition = obj;
	}
	
	//// INPUT METHODEN
	
	//Der Wert wird vom PlayerInputController geändert
	public void setThrottle(float th)
	{
		throttle = th;
	}
	
	//Der Wert wird vom PlayerInputController geändert
	public void setSteer(float st)
	{
		steer = st;
	}
	
	//in dieser Methode wird das Auto nach einen Unfall (wenn er z.B. auf dem Dach liegt) wieder auf die Straße gesetzt
	public void resetCar(bool reset)
	{
		if(reset)
		{
			//falls das Auto auf dem Dach oder schief liegt wird es wieder zurückgesetzt
			Vector3 tempAngles = resetPosition.transform.eulerAngles;
			tempAngles.z = 0.0f;
			tempAngles.x = 0.0f;
			thisTransform.eulerAngles = tempAngles;
			
			//die Geschwindigkeit wird auch resetet
			thisRigidBody.velocity = Vector3.zero;			
			
			//Die Position muss wieder angepasst werden, es wird ein Ray nach unten geschoßen und den Aufprallpunkt 
			//als neue Position gewählt
			RaycastHit rayHit;
			if(Physics.Raycast(resetPosition.transform.position, -Vector3.up, out rayHit))
			{
				Vector3 tempPos = resetPosition.transform.position;
				//+4 um das Auto nicht im Boden versinken zu lassen
				tempPos.y = rayHit.point.y + 3;
				thisTransform.position = tempPos;
			}
		}
	}
	
	//in dieser Methode wird überprüft, ob die Handbremse betätigt wurde, wird vom InputController aufgerufen
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
	
	//diese Methode behandelt den Schaden der einzelnen DestructibleCarParts
	public void applyDamage(DamageZone damZone, float damageAmount)
	{
		health -= damageAmount;	
		
		//die Scheiben sollen als erstes kaputt gehen
		if(health < 90)
		{
			glassDamageModels[0].SetActive(false);
			glassDamageModels[1].SetActive(true);
		}
		//für den sichtbaren Schaden sind keine float Werte nötig
		applyVisualDamage(damZone, (int)damageAmount);
		
		//Wenn die Lebenspunkte 0 sind, soll das Auto explodieren und alle refen verlieren
		if(health<=0.0f)
		{
			explodeCar();
		}
	}
	
	//diese Methode sorgt dafür, dass das Auto einen Reifen verliert
	private void loseAWheel(DamageZone damZone)
	{
		Wheel wheelToDestroy = null;
		hasLostWheel = true;
		//gehe durch jedes Rad durch und such dasd richtige raus
		foreach(Wheel wheel in wheels)
		{	
			if(wheel.damageZone == damZone)
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
			//damit man mit einer gerigen Kraft die TÜr lösen kann
			door.GetComponent<Rigidbody>().AddForceAtPosition(door.transform.up, door.transform.position);
		}
	}
	
	//diese Methode lässt das Auto explodieren sowie alle Reifen und Türen entfernen
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
		foreach(DamageZone damZone in DamageZone.GetValues(typeof(DamageZone)))
		{
			applyVisualDamage(damZone, 200);
		}
		removeDoor(frontDoor);
		removeDoor(rearDoor);
		removeDoor(rightDoor);
		removeDoor(leftDoor);

		//aktiviere den ParticleSystem
		particleSysForExplosion.gameObject.SetActive(true);
		particleSysForExplosion.enableEmission = true;
	}
	
	//diese Methode verarbeitet den Schaden und schaut, aus welcher Richtung er kam und ruf entsprechende Methode auf
	private void applyVisualDamage(DamageZone damZone, int damageAmount)
	{
		//zunächst muss geschaut werden, an welcher Stelle der Schaden angerichtet werden soll
		switch (damZone)
		{
		case DamageZone.FRONT:
			setupFrontDamage(damageAmount);
			break;
		case DamageZone.REAR:
			setupRearDamage(damageAmount);
			break;
		case DamageZone.RIGHT:
			setupRightDamage(damageAmount);
			break;
		case DamageZone.LEFT:
			setupLeftDamage(damageAmount);
			break;
		case DamageZone.FRONT_LEFT:
			setupFrontLeftDamage(damageAmount);
			break;
		case DamageZone.FRONT_RIGHT:
			setupFrontRightDamage(damageAmount);
			break;
		case DamageZone.REAR_LEFT:
			setupRearLeftDamage(damageAmount);
			break;
		case DamageZone.REAR_RIGHT:
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
		//erstes Modell wenn Lebenspunkte zwischen 100 und secondDamageModelHealthLimit
		if(frontHealth >= secondDamageModelHealthLimit)
		{	
			damageModelNumber = 0;
		}
		//zweites Modell wenn Lebenspunkte zwischen secondDamageModelHealthLimit und thirdDamageModelHealthLimit
		else if(frontHealth >= thirdDamageModelHealthLimit)
		{	
			damageModelNumber = 1;
			//falls das Auto noch nicht die vordere Stoßstange verloren hat
			if(hasLostFrontBumper == false)
			{
				hasLostFrontBumper = true;
				GameObject.Instantiate(frontBumperPrefab, thisTransform.position, thisTransform.rotation);
			}
		}
		//ansonsten sind die Lebenspunkt unterhalb von thirdDamageModelHealthLimit und somit wird das 3. Schadensmodel angezeigt
		else
		{
			damageModelNumber = 2;	
			//falls das Auto noch nicht die vordere Stoßstange verloren hat
			if(hasLostFrontBumper == false)
			{
				hasLostFrontBumper = true;
				GameObject.Instantiate(frontBumperPrefab, thisTransform.position, thisTransform.rotation);
			}
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
		int damageModelNumber = 0;	
		if(rearHealth >= secondDamageModelHealthLimit)
		{	
			damageModelNumber = 0;
		}
		else if(rearHealth >= thirdDamageModelHealthLimit)
		{	
			damageModelNumber = 1;
			if(hasLostRearBumper == false)
			{
				hasLostRearBumper = true;
				GameObject.Instantiate(rearBumperPrefab, thisTransform.position, thisTransform.rotation);
			}
		}
		else
		{
			damageModelNumber = 2;	
			//falls die Auspüffe noch nichr verloren wurden
			if(hasLostExhausts == false)
			{
				hasLostExhausts = true;
				GameObject.Instantiate(rightExhaustPrefab, thisTransform.position, thisTransform.rotation);
				GameObject.Instantiate(leftExhaustPrefab, thisTransform.position, thisTransform.rotation);
			}
			//falls Stoßstange nocht nicht verloren
			if(hasLostRearBumper == false)
			{
				hasLostRearBumper = true;
				GameObject.Instantiate(rearBumperPrefab, thisTransform.position, thisTransform.rotation);
			}
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
		int damageModelNumber = 0;
		if(leftHealth >= secondDamageModelHealthLimit)
		{	
			damageModelNumber = 0;
		}
		else if(leftHealth >= thirdDamageModelHealthLimit)
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
		int damageModelNumber = 0;
		if(rightHealth >= secondDamageModelHealthLimit)
		{	
			damageModelNumber = 0;
		}
		else if(rightHealth >= thirdDamageModelHealthLimit)
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
	
	//in dieser Methode wird das graphische Objekt für die vordere linke Seite des Autos aktiviert
	private void setupFrontLeftDamage(int damageAmount)
	{
		frontLeftHealth -= damageAmount;
		int damageModelNumber = 0;
		if(frontLeftHealth >= secondDamageModelHealthLimit)
		{	
			damageModelNumber = 0;
		}
		else if(frontLeftHealth >= thirdDamageModelHealthLimit)
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
			loseAWheel(DamageZone.FRONT_LEFT);
		}
	}
	
	//in dieser Methode wird das graphische Objekt für die vordere rechte Seite des Autos aktiviert
	private void setupFrontRightDamage(int damageAmount)
	{
		frontRightHealth -= damageAmount;
		int damageModelNumber = 0;
		if(frontRightHealth >= secondDamageModelHealthLimit)
		{	
			damageModelNumber = 0;
		}
		else if(frontRightHealth >= thirdDamageModelHealthLimit)
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
				frontRightDamageModels[i].gameObject.SetActive(true);
			}
		}
		//falls noch kein Reifen verloren worden ist und die Health Punkte relative klein sind
		if(hasLostWheel == false && frontRightHealth <= 15)
		{
			loseAWheel(DamageZone.FRONT_RIGHT);
		}
	}
	
	//in dieser Methode wird das graphische Objekt für die hintere linke  Seite des Autos aktiviert
	private void setupRearLeftDamage(int damageAmount)
	{
		rearLeftHealth -= damageAmount;
		int damageModelNumber = 0;
		if(rearLeftHealth >= secondDamageModelHealthLimit)
		{	
			damageModelNumber = 0;
		}
		else if(rearLeftHealth >= thirdDamageModelHealthLimit)
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
			loseAWheel(DamageZone.REAR_LEFT);
		}
	}
	
	//in dieser Methode wird das graphische Objekt für die hintere rechte Seite des Autos aktiviert
	private void setupRearRightDamage(int damageAmount)
	{
		rearRightHealth -= damageAmount;
		int damageModelNumber = 0;
		if(rearRightHealth >= secondDamageModelHealthLimit)
		{	
			damageModelNumber = 0;
		}
		else if(rearRightHealth >= thirdDamageModelHealthLimit)
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
			loseAWheel(DamageZone.REAR_LEFT);
		}
	}
	
	//// RÄDER EINRICHTEN
	
	//diese Methode richtet die Reifen ein und fügt sie den richtigen Listen zu
	private void setupWheels()
	{
		driveWheels = new List<Wheel>();
		steerWheels = new List<Wheel>();
		setupWFC();
		
		//ordne dem  Wheel der richtigen Liste zu und übertrage Federwerte und WFCs
		foreach(Wheel wheel in wheels)
		{
			wheel.wheelCol.motorTorque = 0f;
			wheel.wheelCol.brakeTorque = 0f;
			wheel.wheelCol.steerAngle = 0f;
			
			if(wheel.isFrontWheel)
			{
				wheel.setSpringValues(suspensionDistance, suspensionDamper, suspensionSpringFront);
				wheel.setFrictionCurves(forwardWFC, sidewaysFrontWFC);
			}
			else
			{
				wheel.setSpringValues(suspensionDistance, suspensionDamper, suspensionSpringRear);
				wheel.setFrictionCurves(forwardWFC, sidewaysFrontWFC);
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
		forwardWFC.asymptoteValue = 200; //400f;
		forwardWFC.extremumSlip = 0.5f;
		forwardWFC.extremumValue = 3000; //6000;
		forwardWFC.stiffness = 1.0f;
		
		//seitliches rutschen/bewegen für die Vorderreifen
		sidewaysFrontWFC = new WheelFrictionCurve();
		sidewaysFrontWFC.asymptoteSlip = 2.0f;
		sidewaysFrontWFC.asymptoteValue = 70; //150f;
		sidewaysFrontWFC.extremumSlip = 1f;
		sidewaysFrontWFC.extremumValue = 170; //350;
		sidewaysFrontWFC.stiffness = 1.0f * slipMultiplier;	
		
		//seitliches rutschen/bewegen für die Hintereifen (für Donuts usw.)
		sidewaysRearWFC = new WheelFrictionCurve();
		sidewaysRearWFC.asymptoteSlip = 2.0f;
		sidewaysRearWFC.asymptoteValue = 70; //150f;
		sidewaysRearWFC.extremumSlip = 1f;
		sidewaysRearWFC.extremumValue = 170; //350;
		sidewaysRearWFC.stiffness = 0.9f * slipMultiplier;	
		
		//seitliches rutschen falls die Handbremse benutzt wird
		sidewaysHandbrakeWFC = new WheelFrictionCurve();
		sidewaysHandbrakeWFC.asymptoteSlip = 2.0f;
		sidewaysHandbrakeWFC.asymptoteValue = 70; //150f;
		sidewaysHandbrakeWFC.extremumSlip = 1f;
		sidewaysHandbrakeWFC.extremumValue = 200; //350;
		sidewaysHandbrakeWFC.stiffness = 1.0f;
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
		//bei niedrigen Geschwindigkeiten soll kein Rollwiederstand erzeugt werden
		if(isOneWheelGrounded && (relativeVelocity.z > 10 || relativeVelocity.z < -10))
		{
			//Rollwiederstandkoefizient ist abhängig vom Untergrund
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
						GroundCoefRR += 0.03f; //fester Sand, SandNormal
						break;
					case 10:
						GroundCoefRR += 0.01f; //loser Sand, SandLose
						break;
					case 11:
						GroundCoefRR += 0.025f; //Schotter, Rubble
						break;
					case 12:
						GroundCoefRR += 0.025f; //Erdweg, Dirt
						break;
					case 13:
						GroundCoefRR += 0.04f; //Grass, Grass
						break;
					default:
						GroundCoefRR += 0.015f; //asphalt, Default Layer
						break;
					}
					size++;	
				}	
			}
			//bereche den Durchschnitt der wheels
			if(size != 0)
			{
				GroundCoefRR /= (size * 4);	
			}
			
			//Rollwiderstandswert = Rollwiederstandskoeffizient * Masse * Gravitation
			float CoefRR = GroundCoefRR * rigidbody.mass * Physics.gravity.y;
			//Rollwiderstandskraft, ist entgegengesetzt der aktuellen Fahrtrichtung des Autos
			Vector3 RollingResistanceForce = Mathf.Sign(relativeVelocity.z) * thisTransform.forward * CoefRR;
			//füge die Kraft dem Auto hinzu
			thisRigidBody.AddForce(RollingResistanceForce, ForceMode.Impulse);
		}
		
		//Angular Drag soll größer sein, wenn das Auto eine hohe Geschwindigkeit hat
		thisRigidBody.angularDrag = Mathf.Abs(relativeVelocity.z) / 50;
		
		thisRigidBody.drag = 0.1f;
		//falls sich das Auto in der Luft befindet, soll der Luftwiederstand steigen
		if(areAllWheelsGrounded == false)
		{
			thisRigidBody.drag = 0.2f;
		}
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
					wheel.setFrictionCurves(forwardWFC, sidewaysFrontWFC);
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
				/*				//wenn die Reifen am durchdrehen sind
				if(wheel.getForwardSlip() > 1.0f)
				{
*/					//falls es ein Vorderrad ist, sollen die normalen WFC bnutzt werden
				if(wheel.isFrontWheel)
				{
					wheel.setFrictionCurves(forwardWFC, sidewaysFrontWFC);
				}
				//ansonsten soll am den HInterrädern eine andere WFC verwendet werden, um besser rutschen zu können
				else
				{
					wheel.setFrictionCurves(forwardWFC, sidewaysRearWFC);
				}
				/*				}
				//ansonsten benutzen normale WFC
				else
				{
					wheel.setFrictionCurves(forwardWFC, sidewaysFrontWFC);
				}
*/			}
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
	
	//diese Methode stabiliziert das Auto in Kurvenlage. Da die StabilizerBars nicht genug stabilizieren, wird hier einfach der Schwerpunkt nach unten gesetzt
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
		//falls sich das Auto in der Luft befindet, drücke das Auto nach unten
		//man könnte zwar auch die Gravitaionskraft im Editor größer machen, allerdings würde es das Fahrverhalten auch drastisch
		//ändern, da die Reifen deutlich mehr Grip haben. Um die Werte nicht erneut anpassen zu müssen, wird hier einfach 
		//das Auto nach unter grdrückt
		if(areAllWheelsGrounded == false)
		{
			thisRigidBody.AddForce(new Vector3(0.0f, -5000.0f, 0.0f), ForceMode.Impulse);
		}
	}
	
	//// GAS GEBEN, BREMSEN UND LENKEN
	
	//in dieser Methode wird die Motorkraft (Drehmoment) aus der Drehzahl des Motors errechnet , mit der Gangschaltung multipliziert und 
	//auf die Reifen übertragen
	private void applyMotorTorque(Vector3 relVelocity)
	{
		//gaspedal gedrückt?
		if(isAccelearting || isReversing)
		{
			//um das plötzliche beschleunigen des Autos an Hügeln zu verhindern (Unity Bug, siehe
			//http://forum.unity3d.com/threads/120091-Edy-s-Vehicle-Physics-official-thread/page6?p=1054085&viewfull=1#post1054085 )
			//wird in dieser Variante des Fixes geschaut, wie die momentane Beschleinigung ist. Sollte sie einen Wert überschreiten, wird das Auto 
			//einfach abgebremst
			//beschleiunigung = DeltaV / DeltaT
			float deltaAccelleration = (currentVelocity - previousVel) / Time.fixedDeltaTime;
			previousVel = currentVelocity;
			//maximal Zulässige Beschleunigung
			int maxAccelleration = 60;
			//Debug.Log ("Accel " + deltaAccelleration);
			
/*			//da das Auto z.B. an den Arena Wänden immer noch zu stark beschleuinigt, wird als gegenmaßnahme eine gegenkraft erzeugt, die
			//abhängig vom neigungswinkel des Autos ist
			//allerdings sieht es in der ArenaStadium nicht gut aus, und führt zu komischen Fahrverhalten, wenn man an der Seite hochfahren will
			//nur wenn sich ein Reifen auf dem Boden befindet
			if(isOneWheelGrounded)
			{
				//Vector in World Space, in welcher Richtung das Auto zeigt
				Vector3 currentForward = this.thisTransform.TransformDirection(thisTransform.forward);
				
				//die Kraft soll nur angewendet werden, wenn sich das Auto nach oben neigt bzw. nach oben zeigt und dabei auch den Berg hochfährt
				if((currentForward.y > 0.0f && isAccelearting && relVelocity.z > 0.0f) || (currentForward.y < 0.0f && isReversing && relVelocity.z < 0.0f))
				{
					//Richtung des Autos auf dem Boden, ohne Y-Komponente
					Vector3 groundForward = new Vector3(currentForward.x, 0.0f, currentForward.z);
					//winkel zwischen vorwärtsvektor aus dem letzten Frame und den aktuellen.
					float inclinationAngle = Vector3.Angle(groundForward.normalized, currentForward.normalized);
					//Kraft ist entgegengesetzt der Fahrtrichtung
					Vector3 resistanceForce = -Mathf.Sign(relVelocity.z) * thisTransform.forward * relVelocity.z * 0.07f * inclinationAngle/90;
					thisRigidBody.AddForce(resistanceForce, ForceMode.VelocityChange);
				}
			}
*/
			//Drehmoment, der auch auf die Reifen übertragen wird. Setzt sich zusammen aus: wie weit ist das Gaspedal/Bremspedal (fürs Rückwärtsfahren) 
			//durchgedrückt * Drehmomentkurve * aktueller Gang * Gang Koeffizient * Differenzial Koeffizient * Effizienz des Getriebes 
			float motorTorque = Mathf.Abs(throttle) * engineTorqueCurve.Evaluate(currentRPM) * gearRatio[currentGear] * gearMultiplier[currentGear]
			* differentialMultiplier * transmissionEfficiency;
			
			//falls wir am rückwärtsfahren sind, soll die Motorkraft negativ sein
			if(isReversing)
			{
				motorTorque = -motorTorque;
				//beim rückwärtafahren entstehen größere Beschleunigungen, Ursache noch nicht gefunden
			}
			
			//geh jedes DriveWheel durch und füge Drehmoment hinzu
			foreach(Wheel wheel in driveWheels)
			{
				//werte reseten, da sich das Auto möglicherweise noch fortbewegt/bremst, da er noch den Wert vom vorherigen Frame hat
				//verurschat ein paar selstsame Fehler
				wheel.wheelCol.brakeTorque = 0.0f;
				wheel.wheelCol.motorTorque = motorTorque;		
				//sollte die maximal zugelasse Beschleunigung überschrietten werden, bremsen wir einfach und beschleunigen nicht mehr so stark
				if(deltaAccelleration > maxAccelleration && isReversing == false)
				{
					wheel.wheelCol.motorTorque = motorTorque/2;	
					wheel.wheelCol.brakeTorque = Mathf.Lerp(0.0f, 1000, deltaAccelleration / maxAccelleration);
				}
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
					wheel.wheelCol.brakeTorque = brakeTorque * 1.5f;	
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
		//Motorbremse, nur wenn kein Gas gegeben und nicht gebremst wird
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
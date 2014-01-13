using UnityEngine;
using System.Collections;

/*
 * diese Klasse stellt ein Reifen dar. sie besitzt eine WheelCollider Komponente, sowie eine Referenz auf ein Graphisches Objekt,
 * welches sich auch dreht. Um eine korrekte Aufhänung und eine (Lenk-)Drehung des Reifens zu erreichen muss zwischen dem WheelCollider und dem eigentlichen
 * grapischen Objekt (das sich um die eigene y-Achse rotiert), muss ein Objekt dazwischen schalten, um den Reifen auch um die Lenkachse zu drehen.
 */

public class Wheel : MonoBehaviour 
{
	//Referenz auf GrafikObjekt, benötigt um es zu rotieren
	public Transform tireGraphic;
	//wird dieser Reifen zur Lenkung verwendet?
	public bool isSteerWheel = false;
	//wird dieser Reifen zur beschleunigung verwendet?
	public bool isDriveWheel = false;
	//ist dies ein Vorderrad? die Federn der Aufhängung am Motor sind in der Regel stärker als die anderen (meistens vorne)
	public bool isFrontWheel = false;
	//referenz auf den WheelCollider, sollte nicht verändert werden, nur public um von aussen zuzugreifen
	public WheelCollider wheelCol;
	//DamageDirection
	public DamageZone damageZone;
	
	//die Räder benötigen ein zusätzliches Gameobject, das dazwischen geschaltet ist, um eine korrekte aufhängung und Lenkung darzustellen
	private GameObject steerGraphic;
	//forwärtsslip der Reifen
	private float forwardSlip;
	
//// START UND UPDATE METHODEN
	
	// Use this for initialization
	void Awake()
	{
		wheelCol = GetComponent<WheelCollider>();
	}
	
	void Start () 
	{
		//hier wird ein Objekt dazwischen geschaltet
		steerGraphic = new GameObject(transform.name + "SteerColumn");
		steerGraphic.transform.position = tireGraphic.transform.position;
		steerGraphic.transform.rotation = tireGraphic.transform.rotation;
		steerGraphic.transform.parent = tireGraphic.parent;
		tireGraphic.parent = steerGraphic.transform;
	}
	
	// Update is called once per frame
	//in dieser Methode wird das grapische Objekt des Rades verändert
	void Update()
	{				
		//hier muss noch das grapische Objekt rotiert werden, / 60 in sekunden, * in Grad
		tireGraphic.Rotate(Vector3.right * (wheelCol.rpm / 60 * 360 * Time.deltaTime));	
		
		//falls es ein SteerWheel ist, muss das grapische Objekt noch abhängig von SteerAngle gedreht werden
		if(isSteerWheel)
		{
			//hier muss eine temporäre Variable erstellt werden
			Vector3 tempRot = steerGraphic.transform.localEulerAngles;
			tempRot.y = wheelCol.steerAngle;
			steerGraphic.transform.localEulerAngles = tempRot;	
		}
	}

	//in dieser Methode wird der Slip der Reifenan die CarKLasse weitergelteite und die Skidmarks gerendert
	void FixedUpdate()
	{
		//in WheelHit werden Informationen über die Beührung des Reifens mit dem Bodeb gespeichert
		WheelHit wheelHit;
		//falls das Rad den Boden berührt, soll die Position des Rades nach oben verschoben sein, da die Feder zusammengedrückt wird,
		//außerdem wird das durchdrehen der Reifen an die Car-Klasse weitergeleitet und die Skidmarks gerendert
		if(wheelCol.GetGroundHit(out wheelHit))
		{
			//die position des Rades soll vom Berührungspunkt durch den Radradius nach oben verschoben sein
			steerGraphic.transform.localPosition = wheelCol.transform.up * (wheelCol.radius + wheelCol.transform.InverseTransformPoint(wheelHit.point).y);
			if(wheelHit.sidewaysSlip > 0.5f)
			{
				drawSkidmark(wheelHit.sidewaysSlip);
			}
			if(wheelHit.forwardSlip > 0.5f)
			{
				forwardSlip = wheelHit.forwardSlip;
			}
			else
			{
				forwardSlip = 0;
			}
		}
		//ansonsten werden die Reifen durch die Feder nach aussen gedrückt
		else
		{
			steerGraphic.transform.position = wheelCol.transform.position - (wheelCol.transform.up * wheelCol.suspensionDistance);
		}
	}

//// SETUP METHODEN

	//richtet die Werte der Aufhängung/Feder ein
	public void setSpringValues(float distance, float damper, float springForce)
	{
		//Radius des Reifen ist von der Größe der graphischen Darstellung (BoundingBox) abhängig, /2 da size.y den durchmesser zurückliefert
		//dadurch vermeidet man Clippingfehler
		wheelCol.suspensionDistance = distance;
		wheelCol.mass = 3;
		wheelCol.radius = tireGraphic.renderer.bounds.size.y / 2;
			
		//um an die Feder (JointSpring) "ranzukommen" muss hier eine temporäre Variable erstellt werden
		JointSpring tempJS = wheelCol.suspensionSpring;
		tempJS.damper = damper;
		tempJS.spring = springForce;
		// Zielposition der Feder, 0 = auseinander gezogen
		tempJS.targetPosition = 0f; 
		wheelCol.suspensionSpring = tempJS;
	}
	
	//in dieser Methode werden die WheelFrictionCurves übergeben, man kann sie auch noch nachträlich ändern, z.B. im beim 
	//benutzen der Handbremse oder einen anderen Untergrund ein anderes Verhalten zu haben
	public void setFrictionCurves(WheelFrictionCurve forward, WheelFrictionCurve sideways)
	{
		//falls sich das Rad in der Luft befindet soll es nicht zur Berechnung beitragen
		WheelHit hit;
		if(wheelCol.GetGroundHit(out hit))
		{
			//schau auf welcher Layer der Refien fährt
			int layer = hit.collider.transform.gameObject.layer;
			//switch erlaubt keine abfragen wie "case LayerMask.NameToLayer("Default"):", daher feste Werte
			//Compiler Error CS0150: A constant value is expected
			switch(layer)
			{
			case 9:
				forward.stiffness *= 0.8f; //fester Sand, SandNormal
				sideways.stiffness *= 0.8f;
				break;
			case 10:
				forward.stiffness *= 0.6f; //loser Sand, SandLose
				sideways.stiffness *= 0.8f;
				break;
			case 11:
				forward.stiffness *= 0.75f; //Schotter, Rubble
				sideways.stiffness *= 0.65f;
				break;
			case 12:
				forward.stiffness *= 0.8f; //Erdweg, Dirt
				sideways.stiffness *= 0.7f;
				break;
			case 13:
				forward.stiffness *= 0.7f; //Grass, Grass
				sideways.stiffness *= 0.7f;
				break;
			default:
				//asphalt, Default Layer (mach nichts)
				break;
			}
			wheelCol.forwardFriction = forward;
			wheelCol.sidewaysFriction = sideways;
		}
		wheelCol.forwardFriction = forward;
		wheelCol.sidewaysFriction = sideways;
	}

//// GET METHODEN
	
	//diese Methode liefert den prozentualen Wert zurück, wie sehr die Feder zusammengedrückt ist
	//Wert ist zwischen 0.0 (zusammengrdückt) und 1.0 (auseinandergezogen) 
	public float getSuspensionFactor()
	{
		//falls der Reifen den Boden berührt soll erechne Faktor
		if(wheelCol.isGrounded)
		{
			//momentaner abstand / sollabstand. Soll nicht kleiner als 0 sein (passiert wenn die Feder zu stark gedrückt ist)
			return (wheelCol.transform.InverseTransformPoint(wheelCol.transform.position).y - steerGraphic.transform.localPosition.y) / wheelCol.suspensionDistance;
		}
		//ansonsten liefere 1.0f zurück (Feder ist gedehnt)
		else
		{
			return 1.0f;
		}
	}

	//liefert das Durchdrehen der Reifen zurück
	public float getForwardSlip()
	{
		return forwardSlip;
	}

//// SKIDMARKS

	//in dieser Methode werden die Skidmarks für diesen Reifen gerendert
	private void drawSkidmark(float sideSlip)
	{

	}
}
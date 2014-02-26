using UnityEngine;
using System.Collections;

/*
 * diese Klasse stellt ein Reifen dar. sie besitzt eine WheelCollider Komponente, sowie eine Referenz auf ein Graphisches Objekt,
 * welches sich auch dreht. Um eine korrekte Aufhänung und eine (Lenk-)Drehung des Reifens zu erreichen muss zwischen dem WheelCollider und dem eigentlichen
 * grapischen Objekt (das sich um die eigene y-Achse rotiert), muss ein Objekt dazwischen schalten, um den Reifen auch um die Lenkachse zu drehen.
 * Außerdem besitzt die Klasse einen ParticleSystem, um beim Rutschen Rauch dazustellen
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
	//referenz auf den WheelCollider, sollte nicht verändert werden, nur public um von aussen besser zuzugreifen (anstatt über Getter Methode)
	public WheelCollider wheelCol;
	//DamageDirection
	public DamageZone damageZone;
	//referenzt auf die Skidmark-Prefab mit TrailRenderer
	public GameObject skidTrailPrefab;

	//Referenz auf den Skidmark Object samt TrailRenderer
	private GameObject skidmarkWithTrail;
	//die Räder benötigen ein zusätzliches Gameobject, das dazwischen geschaltet ist, um eine korrekte aufhängung und Lenkung darzustellen
	private GameObject steerGraphic;
	//der ParticleSystem
	private ParticleSystem particleSys;
	
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
		particleSys = gameObject.GetComponentInChildren<ParticleSystem>();
		particleSys.enableEmission = false;
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

		//in WheelHit werden Informationen über die Beührung des Reifens mit dem Boden gespeichert
		WheelHit wheelHit;
		//falls das Rad den Boden berührt, soll die Position des Rades nach oben verschoben sein, da die Feder zusammengedrückt wird,
		//außerdem wird das durchdrehen der Reifen an die Car-Klasse weitergeleitet und die Skidmarks gerendert
		if(wheelCol.GetGroundHit(out wheelHit))
		{
			//die position des Rades soll vom Berührungspunkt durch den Radradius nach oben verschoben sein
			steerGraphic.transform.localPosition = wheelCol.transform.up * (wheelCol.radius + wheelCol.transform.InverseTransformPoint(wheelHit.point).y);

			//falls der Reifen rutscht
			if(Mathf.Abs(wheelHit.forwardSlip) > 10.0f || Mathf.Abs(wheelHit.sidewaysSlip) > 15.0f)
			{	
				//Das Partikel System soll Partikel emitten, 
				particleSys.enableEmission = true;
				//schau auf welcher Layer der Refien fährt
				int layer = wheelHit.collider.transform.gameObject.layer;
				//switch erlaubt keine abfragen wie "case LayerMask.NameToLayer("Default"):", daher feste Werte
				//Compiler Error CS0150: A constant value is expected
				switch(layer)
				{
					case 9:
						//fester Sand, SandNormal
						particleSys.startColor = skidTrailPrefab.GetComponent<SkidmarkWithTrailRenderer>().colorSandNormal;
						break;
					case 10:
						//loser Sand, SandLose
						particleSys.startColor = skidTrailPrefab.GetComponent<SkidmarkWithTrailRenderer>().colorSandLose;
						break;
					case 11:
						//Schotter, Rubble
						particleSys.startColor = skidTrailPrefab.GetComponent<SkidmarkWithTrailRenderer>().colorRubble;
						break;
					case 12:
						//Erdweg, Dirt
						particleSys.startColor = skidTrailPrefab.GetComponent<SkidmarkWithTrailRenderer>().colorDirt;
						break;
					case 13:
					 	//Grass, Grass, es sollen keine Partikel erzeugt werden
						particleSys.enableEmission = false;
						break;
					default:
						//asphalt, Default Layer (mach nichts)
						particleSys.startColor = new Color(1.0f, 1.0f, 1.0f);
						break;
				}

				//falls es momentan bereits eine Skidmark für diesen Reifen gibt
				if(skidmarkWithTrail != null)
				{
					skidmarkWithTrail.transform.position = wheelHit.point;
					//verschiebe die Skidmark ein kleines bischen nach oben, damit sie nicht durch den Boden clippt
					skidmarkWithTrail.transform.Translate(Vector3.up * 0.1f);
				}
				//falls es keine Skidmark gibt, erzeuge eine neue
				else
				{
					skidmarkWithTrail = (GameObject)GameObject.Instantiate(skidTrailPrefab);
					skidmarkWithTrail.transform.position = wheelHit.point;
					skidmarkWithTrail.GetComponent<SkidmarkWithTrailRenderer>().setGroundLayer(wheelHit.collider.transform.gameObject.layer);
				}
			}
			//ansonsten entferne die aktuelle Skidmark
			else
			{
				//Das Partikel System soll keine Partikel emitten
				particleSys.enableEmission = false;
				if(skidmarkWithTrail != null)
				{
					skidmarkWithTrail.GetComponent<SkidmarkWithTrailRenderer>().removeSkidmark();
					skidmarkWithTrail = null;
				}
			}
		}
		//ansonsten werden die Reifen durch die Feder nach aussen gedrückt und das Skidmark Object existiert nicht mehr
		else
		{
			steerGraphic.transform.position = wheelCol.transform.position - (wheelCol.transform.up * wheelCol.suspensionDistance);
			if(skidmarkWithTrail != null)
			{
				skidmarkWithTrail.GetComponent<SkidmarkWithTrailRenderer>().removeSkidmark();
				skidmarkWithTrail = null;
			}
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
	//benutzen der Handbremse oder bei einen anderen Untergrund ein anderes Verhalten zu haben
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
				checkSkidmarkLayer(layer);
				break;
			case 10:
				forward.stiffness *= 0.5f; //loser Sand, SandLose
				sideways.stiffness *= 1.5f;
				checkSkidmarkLayer(layer);
				break;
			case 11:
				forward.stiffness *= 0.75f; //Schotter, Rubble
				sideways.stiffness *= 0.65f;
				checkSkidmarkLayer(layer);
				break;
			case 12:
				forward.stiffness *= 0.8f; //Erdweg, Dirt
				sideways.stiffness *= 0.7f;
				checkSkidmarkLayer(layer);
				break;
			case 13:
				forward.stiffness *= 0.7f; //Grass, Grass
				sideways.stiffness *= 0.7f;
				checkSkidmarkLayer(layer);
				break;
			default:
				//asphalt, Default Layer (mach nichts)
				checkSkidmarkLayer(layer);
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

//// SONSTIGES

	//diese Methode überprüft, ob die Layer der Skidmark mit der des Bodens übereinstimmt, auf dem der Reifen gerade fährt
	private void checkSkidmarkLayer(int layer)
	{
		//überprüfe ob SKidmark vorhanden
		if(skidmarkWithTrail != null)
		{
			//falls die Skidmark GroundLayer verschieden zu der Boden Layer ist, muss eine neue Skidmark erzeugt werden (das macht die Update Methode),
			//da der Untergrund sich geändert hat und nun nicht mehr die richtige Farbe hat
			if(skidmarkWithTrail.GetComponent<SkidmarkWithTrailRenderer>().getGroundLayer() != layer)
			{
				skidmarkWithTrail.GetComponent<SkidmarkWithTrailRenderer>().removeSkidmark();
				skidmarkWithTrail = null;
			}
		}
	}
}
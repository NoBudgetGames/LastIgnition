using UnityEngine;
using System.Collections;

/*
 * diese Klasse stellt ein Reifen dar. sie besitzt eine WheelCollider Komponente, sowie eine Referenz auf ein Graphisches Objekt
 * */
public class Wheel : MonoBehaviour {
	
	//Referenz auf GrafikObjekt, benötigt um es zu rotieren
	public Transform tireGraphic;
	//wird dieser Reifen zur Lenkung verwendet?
	public bool isSteerWheel = false;
	//wird dieser Reifen zur beschleunigung verwendet?
	public bool isDriveWheel = false;
	//ist dies ein Vorderrad? die Federn der Aufhängung am Motor sind in der Regel stärker als die anderen (meistens vorne)
	public bool isFrontWheel = false;
	
	//referenz auf den WheelCollider, sollte nicht verändert werden
	public WheelCollider wheelCol;
	
	//die Räder benötigen ein zusätzliches Gameobject, das dazwischen geschaltet ist, um eine korrekte aufhänhung und Lenkung darzustellen
	private GameObject steerGraphic;
	
	// Use this for initialization
	void Awake()
	{
		wheelCol = GetComponent<WheelCollider>();
	}
	
	void Start () 
	{
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
	/*	
		//falls das Rad den Boden berührt, soll die Position des Rades nach oben verschoben sein, da die Feder zusammengedrückt wird
		if(wheelCol.isGrounded)
		{
			
			steerGraphic.transform.localPosition = steerGraphic.transform.up * (wheelCol.radius + steerGraphic.transform.InverseTransformPoint(wheelHit.point).y);
 		}
		//ansonsten werden die Reifen durch die Feder nach aussen gedrückt
		else
		{
			seerGraphic.transform.position = wheelCol.transform.position - (wheelCol.transform.up * wheelCol.suspensionDistance);
		}
		
	*/		
		

		//hier muss noch das grapische Objekt rotiert werden, die 15 sind dazu da, damit es besser aussieht
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
	
	//richtet die Werte der Feder ein
	public void setSpringValues(float distance, float damper, float springForce)
	{
		
		//Radius des Reifen ist von der Größe der graphischen Abhängig, /2 da size.y den durchmesser zurückliefert
		wheelCol.suspensionDistance = distance;
		wheelCol.mass = 3;
		wheelCol.radius = tireGraphic.renderer.bounds.size.y / 2;
			
		//um an die Feder (JointSpring) "ranzukommen" muss hier eine temporäre Variable erstellt werden
		JointSpring tempJS = wheelCol.suspensionSpring;
		tempJS.damper = damper;
		tempJS.spring = springForce;
		tempJS.targetPosition = 0f;
		wheelCol.suspensionSpring = tempJS;
	}
	
	public void setFrictionCurves(WheelFrictionCurve FWFC, WheelFrictionCurve SWFC)
	{
		wheelCol.forwardFriction = FWFC;
		wheelCol.sidewaysFriction = SWFC;
	}
}

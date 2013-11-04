using UnityEngine;
using System.Collections;

/*
 * diese Klasse stellt ein Reifen dar. sie besitzt eine WheelCollider Komponente, sowie eine Referenz auf ein Graphisches Objekt
 * */
public class Wheel : MonoBehaviour {
	
	//Referenz auf GrafikObjekt, benötigt um es zu rotieren
	public Transform wheelGraphic;
	//wird dieser Reifen zur Lenkung verwendet?
	public bool isSteerWheel = false;
	//wird dieser Reifen zur beschleunigung verwendet?
	public bool isDriveWheel = false;
	//ist dies ein Vorderrad?
	public bool isFrontWheel = false;
	
	//referenz auf den WheelCollider
	public WheelCollider wheelCol;
	
	// Use this for initialization
	void Awake()
	{
		wheelCol = GetComponent<WheelCollider>();
	}
	
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		//hier muss noch das grapische Objekt rotiert werden, die 15 sind dazu da, damit es besser aussieht
		wheelGraphic.Rotate(Vector3.right * (wheelCol.rpm / 60 * 360 * Time.deltaTime));
		
		//falls es ein SteerWheel ist, muss das grapische Objekt noch abhängig von SteerAngle gedreht werden
		//wheelGraphic.Rotate(Vector3.up, wheelCol.steerAngle);
	}
	
	//richtet die Werte der Feder ein
	public void setSpringValues(float distance, float damper, float springForce)
	{
		
		//Radius des Reifen ist von der Größe der graphischen Abhängig, /2 da size.y den durchmesser zurückliefert
		wheelCol.suspensionDistance = distance;
		wheelCol.mass = 3;
		wheelCol.radius = wheelGraphic.renderer.bounds.size.y / 2;
			
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

using UnityEngine;
using System.Collections;

/*
 * diese Klasse stellt ein Reifen dar. sie besitzt eine WheelCollider Komponente, sowie eine Referenz auf ein Graphisches Objekt
 * */
public class Wheel : MonoBehaviour {
	
	//Referenz auf GrafikObjekt, beötigt um es zu rotieren
	public Transform wheelGraphic;
	//wird dieser Reifen zur Lenkung verwendet?
	public bool isSteerWheel = false;
	//wird dieser Reifen zur beschleunigung verwendet?
	public bool isDriveWheel = false;
	//ist dies ein Vorderrad?
	public bool isFrontWheel = false;
	
	
	// Use this for initialization
	void Start () {
	
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//richtet die Werte der Feder ein
	public void setSpringValues(float distance, float damper, float springForce, float radius)
	{
		GetComponent<WheelCollider>().radius = radius;
		GetComponent<WheelCollider>().suspensionDistance = distance;
		JointSpring js = GetComponent<WheelCollider>().suspensionSpring;
		js.damper = damper;
		js.spring = springForce;
	}
	
	public void setFrictionCurves(WheelFrictionCurve FWFC, WheelFrictionCurve SWFC)
	{
		GetComponent<WheelCollider>().forwardFriction = FWFC;
		GetComponent<WheelCollider>().sidewaysFriction = SWFC;
	}
}

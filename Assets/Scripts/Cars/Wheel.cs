using UnityEngine;
using System.Collections;

/*
 * diese Klasse stellt ein Reifen dar. sie besitzt eine WheelCollider Komponente, sowie eine Referenz auf ein Graphisches Objekt
 * */
public class Wheel : MonoBehaviour {
	
	//Referenz auf GrafikObjekt, beötigt um es zu rotieren
	public Transform wheelGraphic;
	//wird dieser Reifen zur Lenkung verwendet
	public bool steerWheel = false;
	//wird dieser Reifen zur beschleunigung verwendet
	public bool driveWheel = false;
	
	
	
	// Use this for initialization
	void Start () {
	
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

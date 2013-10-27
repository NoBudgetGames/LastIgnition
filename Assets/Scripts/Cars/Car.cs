using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {
	
	//Motor, Schaltung
	//Drehmomentkennline des Fahrzeuges. X-Achse entspricht Umdrehungen pro Minute (RPM), Y-Achse enstspricht der daraus resultierende Drehmonent in Newtonmeter
	public AnimationCurve EngineTorqueCurve;
	//Effizienz der Schaltung (es entstehen immer Verluste)
	public float TransmissionEfficiency = 0.7f;
	//Übersetzungswerte der Schaltung, kleiner Gang = hohe Übersetzung, hoher Gang = kleine Übersetzung
	public float[] GearRatio = {2.3f, 1.8f, 1.25f, 1.0f, 0.7f, 0.5f};
	//Werte für die Gangschaltung, damit sie weiss ab welcher Umdrehungszahl sie höher schalten soll
	public float[] RPMToGearUp = {3000, 3000, 3000, 3000, 4500};
	
	//Werte für Wiederstandskräfte
	//Wert für Luftwiederstand beim geradeaus fahren, setzt sich unter anderen aus Luftdichte und Fläche zusammen
	public float CDrag = 0.4f;
	//Multiplier für seitlichen Luftwiederstand
	public float sideDragMultiplier = 1.0f;
	//Zentrum der Masse
	public Transform CenterOfMass;
	
	//Referenzen zu den Reifen
	public WheelCollider[] wheels;
	
	
	// Use this for initialization
	void Start () {
		//Massezemtrum sollte weiter vorne und weiter unten liegen, daher Referenz auf eine anderes in der Hierariche plaziertes Objekt
		//rigidbody.centerOfMass = CenterOfMass.transform.position;
	
	}
	
	// Update is called once per frame
	void Update () {

		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//relative Geschwindigkeit ausrechnen, 
		//Vector3 relativeVelocity = rigidbody.velocity;
		
		getInput();
		
	
	}
	
	private void ApplyResistanceForces(Vector3 relativeVelocity)
	{
		//Rollwiderstand
		//Ground.getRollingResistanceCoefficient(); Rollwiederstandkoefizient ist abhängig vom Untergrund
		float GroundCoefRR = 1f;
		//Rollwiderstandswert = Rollwiederstandskoeffizient * Masse * Gravitation
		float CoefRR = GroundCoefRR * rigidbody.mass * 9.81f;
		//Rollwiderstandskraft, ist entgegengesetzt der Fahrtrichtung des Autos
		Vector3 RollingResistanceForce = -Mathf.Sign(relativeVelocity.z) * transform.forward * CoefRR;
		
		
		//Luftwiderstand
		//Luftwiderstandswert CDrag = 0.5 * Luftwiderstandskoeffiezient * Luftdichte * Fläche in Fahrtrichtung
		//Vector3 DragForce = CDrag * 
		
	}
	
	private void getInput()
	{
		if(Input.GetKey(KeyCode.UpArrow))
		{
			//Gas geben, Drehmoment wird auf reifen übertragen, nur wenen Reifen boden berühren
			if(wheels[2].isGrounded || wheels[3].isGrounded)
			{
				wheels[2].motorTorque = 20;
				wheels[3].motorTorque = 20;
			}
		}
		
		if(Input.GetKey(KeyCode.DownArrow))
		{
			if(wheels[3].isGrounded || wheels[2].isGrounded)
			{
				wheels[2].brakeTorque = 20;
				wheels[3].brakeTorque = 20;
			}
		}
		
		if(Input.GetKey(KeyCode.RightArrow))
		{
			wheels[0].steerAngle = 30;
			wheels[1].steerAngle = 30;
		}
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			wheels[0].steerAngle = -30;
			wheels[1].steerAngle = -30;
		}
		resetTorque();
		
	}
	
	//alle Werte werden wirder auf 0 gesetzt, damit es zu keiner dauerhaften Beschleunigung kommt
	private void resetTorque()
	{
		foreach(WheelCollider wheel in wheels)
		{
			wheel.brakeTorque = Mathf.Lerp(wheel.brakeTorque, 0f, 0.5f);
			wheel.motorTorque = Mathf.Lerp(wheel.motorTorque, 0f, 0.5f);
			wheel.steerAngle = Mathf.Lerp(wheel.steerAngle, 0f, 0.2f);
		}
	}
	
}

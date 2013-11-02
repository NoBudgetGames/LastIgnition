using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Diese Klasse enthält alle Autorelevanten Daten. 
 * Außerdem verarbeitet sie den Input eines Controller Scripts, dies kann ein Spieler oder eine KI sein.
 *
 */


public class Car : MonoBehaviour {
	
	//nur zu testzwcken
	public float MotorTorque = 20;
	public float BreakTorque = 20;
	public float SteerAngle = 30;
	
	
/*	Momentan nicht in Gebrauch
	//Motor, Schaltung
	//Drehmomentkennline des Fahrzeuges. X-Achse entspricht Umdrehungen pro Minute (RPM), Y-Achse enstspricht der daraus resultierende 
	//Drehmonent in Newtonmeter
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
*/
	//Zentrum der Masse für den RigiBody
	public Transform CenterOfMass;
	
	//Referenzen zu den Reifen
	public Wheel[] wheels;
	//liste mit lenkrädern
	private List<Wheel> steerWheels;
	//liste mit beschleiunigungsrädern
	private List<Wheel> driveWheels;
	
/*	momentan nicht in gebrauch
	//WheelFrictionCurves, sollten für alle Reifen gleich sein. Um das nicht ständig ändern zu müssen, wird das an einer zentralen Stelle gepsichert 
	//und an die Reifen weitergegeben
	private WheelFrictionCurve forwardFrictionCurve;
	private WheelFrictionCurve sidewaysFrictionCurve;
	
*/	
	//Werte im Bezug auf den Controller
	// Wert für die Beschleinigung, von -1 (rückwärts beschleunigen bzw. bremsen) bis 1 (vorwärts beschleunigen)
	private float throttle = 0.0f;
	//Wert für den Lenker, von -1 (links) bis 1 (rechts)
	private float steer = 0.0f;
	//Zeit, wie lange die Kuplung noch nicht drin ist, falls > 0 keine Motorkraft auf Reifen
	//private float gearChange = 0.0f;
/*	
	
	//Aktuelle Werte
	//aktueller Gang
	private int currentGear = 0;
	//aktuelle Umdrehungen des Motors pro Minute
	private float currentRPM = 1000;
*/	
	// Use this for initialization
	void Start () {
		driveWheels = new List<Wheel>();
		steerWheels = new List<Wheel>();
		
		//füge der Liste die richtigen Wheels zu
		foreach(Wheel wheel in wheels)
		{
			if(wheel.driveWheel)
			{
				driveWheels.Add(wheel);
			}
			if(wheel.steerWheel)
			{
				steerWheels.Add(wheel);
			}
		}
		
		//Massezemtrum sollte weiter vorne und weiter unten liegen, daher Referenz auf eine anderes in der Hierariche plaziertes Objekt
		//rigidbody.centerOfMass = CenterOfMass.transform.position;
		
		//setForwardFrictionCurve(float asymSlip, float asymValue, float exSlip, float exValue, float stiff);

	}
	
	// Update is called once per frame
	void Update () {

		
	}
	
	// Update is called regularly 
	void FixedUpdate () {
		//relative Geschwindigkeit ausrechnen, 
		//Vector3 relativeVelocity = rigidbody.velocity;
		
		applyTractionForces();
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
	
	
	//in dieser Methode werden die Widerstandskräfte berechent
	private void applyResistanceForces(Vector3 relativeVelocity)
	{
		//Rollwiderstand
		//Ground.getRollingResistanceCoefficient(); Rollwiederstandkoefizient ist abhängig vom Untergrund
		//eventuell kannman auch die Layer bestimmen, auf dem der Wagen fährt
		float GroundCoefRR = 1f;
		//Rollwiderstandswert = Rollwiederstandskoeffizient * Masse * Gravitation
		float CoefRR = GroundCoefRR * rigidbody.mass * 9.81f;
		//Rollwiderstandskraft, ist entgegengesetzt der aktuellen Fahrtrichtung des Autos
		Vector3 RollingResistanceForce = -Mathf.Sign(relativeVelocity.z) * transform.forward * CoefRR;
		
		
		//Luftwiderstand
		//Luftwiderstandswert CDrag = 0.5 * Luftwiderstandskoeffiezient * Luftdichte * Fläche in Fahrtrichtung
		//Vector3 DragForce = CDrag * 
	}
	
	private void applyTractionForces()
	{

		//Gas geben, Drehmoment wird auf reifen übertragen, nur wenen Reifen boden berühren
		if(throttle > 0.0)
		{
			//geh jedes DriveWheel durch und füge Drehmoment hinzu
			foreach(Wheel wheel in driveWheels)
			{
				if(wheel.GetComponent<WheelCollider>().isGrounded)
				{
					wheel.GetComponent<WheelCollider>().motorTorque = MotorTorque * throttle;
				}
			}
		}
		
		//bremsen
		if(throttle < 0.0)
		{
			//gehe jedes Rad durch und bremse
			foreach(Wheel wheel in wheels)
			{
				if(wheel.GetComponent<WheelCollider>().isGrounded)
				{
					wheel.GetComponent<WheelCollider>().brakeTorque = BreakTorque * -throttle;
				}
			}
		}
		
		//lenken, gehe jedes SteerWheel durch und lenke
		foreach(Wheel wheel in steerWheels)
		{
			if(wheel.GetComponent<WheelCollider>().isGrounded)
			{
				wheel.GetComponent<WheelCollider>().steerAngle = SteerAngle * steer;
			}
		}
	}
	
}

using UnityEngine;
using System.Collections;

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
	private Wheel[] steerWheels;
	//liste mit beschleiunigungsrädern
	private Wheel[] driveWheels;
	
/*	momentan nicht in gebrauch
	//WheelFrictionCurves, sollten für alle Reifen gleich sein. Um das nicht ständig ändern zu müssen, wird das an einer zentralen Stelle gepsichert 
	//und an die Reifen weitergegeben
	private WheelFrictionCurve forwardFrictionCurve;
	private WheelFrictionCurve sidewaysFrictionCurve;
	
*/	
	//Werte im Bezug auf den Controller
	// Wert für den durchgedrückten Gaspedal, von -1 (rückwärts) bis 1 (vorwärts)
	private float throttle = 0.0f;
	//Wert für den Lenker, von -1 bis 1
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
	//kann das Auto steuern? (Räder auf dem Boden?)
	private bool canSteer;
	//kann das Auto beschleunigen?
	private bool canDrive;

	
	// Use this for initialization
	void Start () {
		
		//füge der Liste die richtigen Wheels zu

		
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
		
		checkStatus();
		applyTractionForces();
		
	
	}
	
	public void setThrottle(float th)
	{
		throttle = th;
	}
	
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
	
	
	//diese Methode überprüft, ob die Räder des Fahrzeugs den Boden berühren
	private void checkStatus()
	{
		//es wird angenommen, das die Rädern den Boden berühren, sobald einer der Reifen den Boden nicht berüht
		//kann das Fahrzeug nicht lenken bzw. beschleunigen
		
		
		foreach(Wheel wheel in wheels)
		{
			//überprüfe die lenkräder
			if(!wheel.canSteer())
			{
				canSteer = false;
			}
			//überprüfe die beschlunigungsräder
			if(!wheel.canDrive())
			{
				canDrive = false;
			}
			
		}
		
		canSteer = true;
		canDrive = true;
	}
	
	private void applyTractionForces()
	{

		//Gas geben, Drehmoment wird auf reifen übertragen, nur wenen Reifen boden berühren
		if(canDrive && throttle > 0.0)
		{
			wheels[0].GetComponent<WheelCollider>().motorTorque = MotorTorque * throttle;
			wheels[1].GetComponent<WheelCollider>().motorTorque = MotorTorque * throttle;
			//wheels[2].motorTorque = MotorTorque;
			//wheels[3].motorTorque = MotorTorque;
		}
		
		//bremsen
		if(canDrive  && throttle < 0.0)
		{
			wheels[2].GetComponent<WheelCollider>().brakeTorque = BreakTorque * -throttle;
			wheels[3].GetComponent<WheelCollider>().brakeTorque = BreakTorque * -throttle;
		}
		
		//lenken
		if(canSteer)
		{
			wheels[0].GetComponent<WheelCollider>().steerAngle = SteerAngle * steer;
			//wheels[0].wheelGraphic.rotation.y = SteerAngle;
			wheels[1].GetComponent<WheelCollider>().steerAngle = SteerAngle * steer;
			//wheels[1].wheelGraphic.rotation.y = SteerAngle;
		}
		
	}
	
}

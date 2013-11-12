using UnityEngine;
using System.Collections;

/*
 * der InputController diest dazu, den Input vom SPieler auf das Fahrzeug zu übertragen
 * Die Nummer des Spielers ist von einen String abhängig, somit muss nur eine Klasse geschreiben werden
 */

public class PlayerInputController : MonoBehaviour 
{
	//referenz auf Fahrzeug,
	private Car car;
	//Einfüge String für Input, "One" für Spieler 1, "Two" für Spieler 2
	public string playerString = "One";
	
	// Use this for initialization
	void Start () 
	{
		car = GetComponent<Car>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	// Update is called regularly
	void FixedUpdate()
	{
		//hier wird der Input behandelt und auf car übertragen
		car.setThrottle(Input.GetAxis("Player" + playerString + "Throttle"));
		car.setSteer(Input.GetAxis("Player" + playerString + "Steer"));
		car.resetCar(Input.GetAxis("Player" + playerString + "ResetCar"));
		car.setHandbrake(Input.GetAxis("Player" + playerString + "Handbrake"));
	}
	
}

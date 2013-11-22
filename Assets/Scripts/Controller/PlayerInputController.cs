using UnityEngine;
using System.Collections;

/*
 * der InputController diest dazu, den Input vom SPieler auf das Fahrzeug zu übertragen
 * Die Nummer des Spielers ist von einen String abhängig, somit muss nur eine Klasse geschreiben werden
 */

public class PlayerInputController : MonoBehaviour 
{
	//Einfüge String für Input, "One" für Spieler 1, "Two" für Spieler 2
	public string playerString = "One";
	//Wird ein Controller benutz oder nicht?
	public bool usingController = false;
	//referenz auf Fahrzeug,
	private Car car;
	
	// Use this for initialization
	void Start () 
	{
		car = GetComponent<Car>();
	}
	
	// Update is called regularly
	void FixedUpdate()
	{
		//falls Tastatur benutzt wird
		if(usingController == false)
		{
			car.setThrottle(Input.GetAxis("Player" + playerString + "ThrottleKey"));
			car.setSteer(Input.GetAxis("Player" + playerString + "SteerKey"));
			car.resetCar(Input.GetAxis("Player" + playerString + "ResetCarKey"));
			car.setHandbrake(Input.GetAxis("Player" + playerString + "HandbrakeKey"));	
		}
		//falls Controller benutzt wird
		else
		{
			car.setThrottle(Input.GetAxis("Player" + playerString + "Throttle"));
			car.setSteer(Input.GetAxis("Player" + playerString + "Steer"));
			car.resetCar(Input.GetAxis("Player" + playerString + "ResetCar"));
			car.setHandbrake(Input.GetAxis("Player" + playerString + "Handbrake"));	
		}
		
		if(Input.GetKeyDown(KeyCode.F))
		{
			DestructibleObject dest = GetComponent<DestructibleObject>();
			dest.receiveDamage(5);
		}
	}
}

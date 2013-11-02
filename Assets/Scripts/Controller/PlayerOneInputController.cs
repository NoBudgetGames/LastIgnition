using UnityEngine;
using System.Collections;

/*
 * der InputController diest dazu, den Input vom SPieler auf das Fahrzeug zu übertragen
 * Diese Klasse ist für Spieler 1 zuständig
 */
public class PlayerOneInputController : MonoBehaviour {
	
	//referenzt auf Fahrzeug,
	public Car car;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// Update is called regularly
	void FixedUpdate()
	{
		
		//hier wird der Input behandelt und auf car übertragen
		
		car.setThrottle(Input.GetAxis("PlayerOneThrottle"));
		car.setSteer(Input.GetAxis("PlayerOneSteer"));

		
		
		
	}
	
}

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
	//refernz auf den CameraController
	public CameraController cameraCtrl;
	//referenz auf weitere Kameras, z.B. die Kamere auf der Motorhaube
	//reihenfolge: Motorhaube | Kofferraum
	public Transform[] additionalCameras;
	//referenz auf Fahrzeug,
	private Car car;

	private CarInventory inv;

	
	// Use this for initialization
	void Start () 
	{
		car = GetComponent<Car>();
		inv = GetComponentInChildren<CarInventory>();
	}

	void FixedUpdate()
	{
		//falls Tastatur benutzt wird
		if(usingController == false)
		{
			car.setThrottle(Input.GetAxis("Player" + playerString + "ThrottleKey"));
			car.setSteer(Input.GetAxis("Player" + playerString + "SteerKey"));
			car.resetCar(Input.GetAxis("Player" + playerString + "ResetCarKey"));
			car.setHandbrake(Input.GetButton("Player" + playerString + "HandbrakeKey"));

			inv.setFiring(Input.GetButton("Player" + playerString + "FireKey"));
			inv.prevWeapon(Input.GetButtonDown("Player" + playerString + "CycleWeaponDownKey"));
			inv.nextWeapon(Input.GetButtonDown("Player" + playerString + "CycleWeaponUpKey"));

			cameraCtrl.lookBack(Input.GetButton("Player" + playerString + "LookBackKey"));
			cameraCtrl.cycleCamera(Input.GetAxis("Player" + playerString + "ChangeCameraKey"));
		}
		//falls Controller benutzt wird
		else
		{
			car.setThrottle(Input.GetAxis("Player" + playerString + "Throttle"));
			car.setSteer(Input.GetAxis("Player" + playerString + "Steer"));
			car.resetCar(Input.GetAxis("Player" + playerString + "ResetCar"));
			car.setHandbrake(Input.GetButton("Player" + playerString + "Handbrake"));

			inv.setFiring(Input.GetButton("Player" + playerString + "Fire"));
			inv.prevWeapon(Input.GetButtonDown("Player" + playerString + "CycleWeaponDown"));
			inv.nextWeapon(Input.GetButtonDown("Player" + playerString + "CycleWeaponUp"));

			cameraCtrl.lookBack(Input.GetButton("Player" + playerString + "LookBack"));
			cameraCtrl.cycleCamera(Input.GetAxis("Player" + playerString + "ChangeCamera"));
		}
	}
}

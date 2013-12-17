using UnityEngine;
using System.Collections;

/*
 * der InputController diest dazu, den Input vom SPieler auf das Fahrzeug zu übertragen
 * Die Nummer des Spielers ist von einen String abhängig, somit muss nur eine Klasse geschreiben werden
 * AUßerdem besitzt die Klasse eine Referenz auf einen CameraController. Da nur menschliche Spieler eine Kamera brauchen,
 * wird diese Komponente im ebenfalls nur für menschliche Spieler benötigten InputController gespeichert
 */

public class PlayerInputController : MonoBehaviour 
{
	//Einfüge String für Input, "One" für Spieler 1, "Two" für Spieler 2, es ist nicht der richtige Name des Spielers!
	public string playerString = "One";
	//Wird ein Controller benutz oder nicht?
	public bool usingController = false;
	//refernz auf die CameraControllerPrefab, wird instanziert, NICHT VERÄNDERN!
	public GameObject cameraCtrlPrefab;
	//referenz auf weitere Kameras, z.B. die Kamere auf der Motorhaube
	//reihenfolge wichtig! 1. Motorhaube | 2. Kofferraum
	public Transform[] additionalCameraPositions;
	//refernz auf den CameraController
	public CameraController cameraCtrl;
	//referenz auf Fahrzeug,
	private Car car;
	//Referenz auf das Inventar des Autos
	private CarInventory inv;

	public GameObject hudPrefab;
	public HUD hud;

	void Awake()
	{
		car = GetComponent<Car>();
		inv = GetComponentInChildren<CarInventory>();
		
		//neuen CameraController aufsetzen
		GameObject camObj = (GameObject)GameObject.Instantiate(cameraCtrlPrefab, car.transform.position, car.transform.rotation);
		cameraCtrl = camObj.GetComponent<CameraController>();
		
		cameraCtrl.targetCar = car;
		//erstes Element ist Motorhaubenkamera
		cameraCtrl.hoodCamera = additionalCameraPositions[0];
		//zweites Element ist Kofferraumkamera
		cameraCtrl.hoodCameraLookBack = additionalCameraPositions[1];
		
			GameObject hudObj = (GameObject) GameObject.Instantiate(hudPrefab);
			hud = hudObj.GetComponent<HUD>();

			hud.cameraObject = camObj;
		



	}

	public void setupHUD(){
		if(playerString == "One"){
			hud.layer = LayerMask.NameToLayer("Test");
			cameraCtrl.camera.cullingMask |= 1 << LayerMask.NameToLayer("Test");
		} else {
			hud.layer = LayerMask.NameToLayer("Test2");
			cameraCtrl.camera.cullingMask |= 1 << LayerMask.NameToLayer("Test2");
		}
		hud.player = playerString;
	}

	//in dieser Methode wird der Input des Spielers an die jeweiligen Komponenten weitergereicht
	void FixedUpdate()
	{

		//falls Tastatur benutzt wird, wird später rausgenohmen
		if(usingController == false)
		{
			car.setThrottle(Input.GetAxis("Player" + playerString + "ThrottleKey"));
			car.setSteer(Input.GetAxis("Player" + playerString + "SteerKey"));
			car.resetCar(Input.GetButton("Player" + playerString + "ResetCarKey"));
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
			car.resetCar(Input.GetButtonDown("Player" + playerString + "ResetCar"));
			car.setHandbrake(Input.GetButton("Player" + playerString + "Handbrake"));

			inv.setFiring(Input.GetButton("Player" + playerString + "Fire"));
			inv.prevWeapon(Input.GetButtonDown("Player" + playerString + "CycleWeaponDown"));
			inv.nextWeapon(Input.GetButtonDown("Player" + playerString + "CycleWeaponUp"));

			cameraCtrl.lookBack(Input.GetButton("Player" + playerString + "LookBack"));
			cameraCtrl.cycleCamera(Input.GetAxis("Player" + playerString + "ChangeCamera"));
		}
	}
}

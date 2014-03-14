using UnityEngine;
using System.Collections;

/*
 * der InputController diest dazu, den Input vom Spieler auf das Fahrzeug zu übertragen
 * Die Nummer des Spielers ist von einen String abhängig, somit muss nur eine Klasse geschrieben werden
 * AUßerdem besitzt die Klasse eine Referenz auf einen CameraController. Da nur menschliche Spieler eine Kamera brauchen,
 * wird diese Komponente im ebenfalls nur für menschliche Spieler benötigten PlayerInputController gespeichert
 * Die Klasse wird als Script-Komponente in das Auto-Object hinzugefügt
 */

public class PlayerInputController : MonoBehaviour 
{
	//Einfüge String für Input, "One" für Spieler 1, "Two" für Spieler 2, es ist nicht der richtige Name des Spielers!
	public string numberOfControllerString = "One";
	//refernz auf die CameraControllerPrefab, wird instanziert, NICHT VERÄNDERN!
	public GameObject cameraCtrlPrefab;
	//referenz auf weitere Kameras im Auto, z.B. die Kamere auf der Motorhaube
	//reihenfolge wichtig! 1. Motorhaube | 2. Kofferraum
	public Transform[] additionalCameraPositions;
	//refernz auf den CameraController
	public CameraController cameraCtrl;

	//referenz auf Fahrzeug,
	private Car car;
	//Referenz auf das Inventar des Autos
	private CarInventory inv;
	//hilfsvariable, die überprüft, ob ein Controller für den Spieler angeschloßen wurde oder nicht
	private bool controllerAttached = false;

	public GameObject hudPrefab;
	public HUD hud;

	void Awake()
	{
		car = GetComponent<Car>();
		inv = GetComponentInChildren<CarInventory>();
		
		//neuen CameraController aufsetzen
		GameObject camObj;
		if(Network.connections.Length > 0){
			camObj = (GameObject)Network.Instantiate(cameraCtrlPrefab, car.transform.position, car.transform.rotation,0);
		} else {
			camObj = (GameObject)GameObject.Instantiate(cameraCtrlPrefab, car.transform.position, car.transform.rotation);
		}
		cameraCtrl = camObj.GetComponent<CameraController>();
		
		cameraCtrl.targetCar = car;
		//erstes Element ist Motorhaubenkamera
		cameraCtrl.hoodCamera = additionalCameraPositions[0];
		//zweites Element ist Kofferraumkamera
		cameraCtrl.hoodCameraLookBack = additionalCameraPositions[1];
		
		GameObject hudObj;
		if(Network.connections.Length > 0){
			hudObj = (GameObject) Network.Instantiate(hudPrefab,hudPrefab.transform.position,hudPrefab.transform.rotation,0);
		} else {
			hudObj = (GameObject) GameObject.Instantiate(hudPrefab);
		}
		hud = hudObj.GetComponent<HUD>();
		hud.cameraObject = camObj;
		setupHUD();
	}

	//in dieser Methode wird geschaut, ob für den Spieler ein Controller angeschloßen wurde oder nicht
	void Start()
	{
		//gehe alle angeschloßenen Controller durch
		//GetJoystickNames ist ein String Array mit den angeschloßenen Joysticks
		//falls seine Länge kleiner als 2 ist, wurden nicht alle Controller angeschloßen
		//d.h. Spieler 2 wird bei nur 1 angeschloßenen Controller immer auf der Tastatur spielen
		for(int i = 0; i < Input.GetJoystickNames().Length; i++)
		{
			//überprüfe für Spieler One
			if(i == 0 && numberOfControllerString.Equals("One") && Input.GetJoystickNames()[i] != null)
			{
				controllerAttached = true;
			}
			//überprüfe für Spieler Two
			if(i == 1 && numberOfControllerString.Equals("Two") && Input.GetJoystickNames()[i] != null)
			{
				controllerAttached = true;
			}
		}

	}

	public void setupHUD(){
		if(numberOfControllerString == "One"){
			hud.layer = LayerMask.NameToLayer("Test");
			cameraCtrl.camera.cullingMask |= 1 << LayerMask.NameToLayer("Test");
			cameraCtrl.camera.cullingMask ^= 1 << LayerMask.NameToLayer("Test2");
		} else {
			hud.layer = LayerMask.NameToLayer("Test2");
			cameraCtrl.camera.cullingMask ^= 1 << LayerMask.NameToLayer("Test");
			cameraCtrl.camera.cullingMask |= 1 << LayerMask.NameToLayer("Test2");
		}
		hud.player = numberOfControllerString;
	}

	//in dieser Methode wird der Input des Spielers an die jeweiligen Komponenten weitergereicht
	void FixedUpdate()
	{
		//falls für diesen Spieler kein Controller angeschloßen ist, benutze die Tastatur
		if(controllerAttached == false)
		{
			car.setThrottle(Input.GetAxis("Player" + numberOfControllerString + "ThrottleKey"));
			car.setSteer(Input.GetAxis("Player" + numberOfControllerString + "SteerKey"));
			car.resetCar(Input.GetButton("Player" + numberOfControllerString + "ResetCarKey"));
			car.setHandbrake(Input.GetButton("Player" + numberOfControllerString + "HandbrakeKey"));

			inv.setFiring(Input.GetButton("Player" + numberOfControllerString + "FireKey"));
			inv.prevWeapon(Input.GetButtonDown("Player" + numberOfControllerString + "CycleWeaponDownKey"));
			inv.nextWeapon(Input.GetButtonDown("Player" + numberOfControllerString + "CycleWeaponUpKey"));

			cameraCtrl.lookBack(Input.GetButton("Player" + numberOfControllerString + "LookBackKey"));
			cameraCtrl.cycleCamera(Input.GetAxis("Player" + numberOfControllerString + "ChangeCameraKey"));
		}
		//falls Controller benutzt wird
		else
		{
			car.setThrottle(Input.GetAxis("Player" + numberOfControllerString + "Throttle"));
			car.setSteer(Input.GetAxis("Player" + numberOfControllerString + "Steer"));
			car.resetCar(Input.GetButtonDown("Player" + numberOfControllerString + "ResetCar"));
			car.setHandbrake(Input.GetButton("Player" + numberOfControllerString + "Handbrake"));

			inv.setFiring(Input.GetButton("Player" + numberOfControllerString + "Fire"));
			inv.prevWeapon(Input.GetButtonDown("Player" + numberOfControllerString + "CycleWeaponDown"));
			inv.nextWeapon(Input.GetButtonDown("Player" + numberOfControllerString + "CycleWeaponUp"));

			cameraCtrl.lookBack(Input.GetButton("Player" + numberOfControllerString + "LookBack"));
			cameraCtrl.cycleCamera(Input.GetAxis("Player" + numberOfControllerString + "ChangeCamera"));
		}
	}
}
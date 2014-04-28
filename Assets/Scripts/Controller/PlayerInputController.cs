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
	//Name des Spielers
	public string playerName = "";
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
	//hat dieser Controller schon das Rennen beendet? Man soll nach dem Rennen das TargetCar für den CameraConntroller ändern können
	private bool raceEnded = false;
	//referenz auf den CircuitMode
	private CircuitRaceMode circuitMode;
	//wurde nach dem Beendeten Rennen das Zielauto geweschslt?
	private bool changedTargetCar = false;

	public GameObject hudPrefab;
	public HUD hud;

	void Awake()
	{
		car = GetComponent<Car>();
		inv = GetComponentInChildren<CarInventory>();
		
		//neuen CameraController aufsetzen
		if(this.networkView.isMine || Network.connections.Length == 0){
			GameObject camObj;
			camObj = (GameObject)GameObject.Instantiate(cameraCtrlPrefab, car.transform.position, car.transform.rotation);

			cameraCtrl = camObj.GetComponent<CameraController>();
			
			cameraCtrl.targetCar = car;
			//erstes Element ist Motorhaubenkamera
			cameraCtrl.hoodCamera = additionalCameraPositions[0];
			//zweites Element ist Kofferraumkamera
			cameraCtrl.hoodCameraLookBack = additionalCameraPositions[1];
			
			
			GameObject hudObj;
			hudObj = (GameObject) GameObject.Instantiate(hudPrefab);
			
			hud = hudObj.GetComponent<HUD>();
			hud.cameraObject = camObj;
			setupHUD();
		}
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
		//falls der Name des Spielers Player ist, soll die Nummer seines Controllers angehängt werden
		if(playerName.Equals(""))
		{
			playerName = "Player" + numberOfControllerString;
		}
	}

	public void setupHUD(){
		if(numberOfControllerString != "One"){
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
		if(!this.networkView.isMine && Network.connections.Length > 0)
			return;
		//falls das Rennen noch nicht beendet wurde, soll man das AUto normal steuern können
		if(raceEnded == false)
		{
			//falls für diesen Spieler kein Controller angeschloßen ist, benutze die Tastatur
			if(controllerAttached == false)
			{
				car.setThrottle(Input.GetAxis("Player" + numberOfControllerString + "ThrottleKey"));
				car.setBrake(Input.GetAxis("Player" + numberOfControllerString + "BrakeKey"));
				car.setSteer(Input.GetAxis("Player" + numberOfControllerString + "SteerKey"));
				car.resetCar(Input.GetButton("Player" + numberOfControllerString + "ResetCarKey"));
				car.setHandbrake(Input.GetButton("Player" + numberOfControllerString + "HandbrakeKey"));
				
				inv.setFiring(Input.GetButton("Player" + numberOfControllerString + "FireKey"));
				inv.prevWeapon(Input.GetButtonDown("Player" + numberOfControllerString + "CycleWeaponDownKey"));
				inv.nextWeapon(Input.GetButtonDown("Player" + numberOfControllerString + "CycleWeaponUpKey"));

				if(cameraCtrl != null)
				{
					cameraCtrl.lookBack(Input.GetButton("Player" + numberOfControllerString + "LookBackKey"));
					cameraCtrl.cycleCamera(Input.GetAxis("Player" + numberOfControllerString + "ChangeCameraKey"));
				}
			}
			//falls Controller benutzt wird
			else
			{
				car.setThrottle(Input.GetAxis("Player" + numberOfControllerString + "Throttle"));
				car.setBrake(Input.GetAxis("Player" + numberOfControllerString + "Brake"));
				car.setSteer(Input.GetAxis("Player" + numberOfControllerString + "Steer"));
				car.resetCar(Input.GetButtonDown("Player" + numberOfControllerString + "ResetCar"));
				car.setHandbrake(Input.GetButton("Player" + numberOfControllerString + "Handbrake"));
				
				inv.setFiring(Input.GetButton("Player" + numberOfControllerString + "Fire"));
				inv.prevWeapon(Input.GetButtonDown("Player" + numberOfControllerString + "CycleWeaponDown"));
				inv.nextWeapon(Input.GetButtonDown("Player" + numberOfControllerString + "CycleWeaponUp"));

				
				if(cameraCtrl != null)
				{
					cameraCtrl.lookBack(Input.GetButton("Player" + numberOfControllerString + "LookBack"));
					cameraCtrl.cycleCamera(Input.GetAxis("Player" + numberOfControllerString + "ChangeCamera"));
				}
			}
		}
		//ansonsten soll man nur die Kamera ändern können
		else
		{
			if(circuitMode != null)
			{
				//falls der Spieler aufs Gas drückt und er noch keinen Kamerawechsel gemacht hat, soll das ZielAuto ein anderes sein
				if((Input.GetAxis("Player" + numberOfControllerString + "Throttle") > 0.5f || Input.GetAxis("Player" + numberOfControllerString + "ThrottleKey") > 0.5f) && changedTargetCar == false)
				{
					changedTargetCar = true;
					//Array mit der Spielerliste
					GameObject[] playerList = circuitMode.getCarPlayerList().ToArray();

					//wir gucken zunächst mal, wo das TargetCar der Kamera in der Liste der SPieler befindet
					int indexOfTargetCar = 0;
					//suche den Index des TargetCars des CameraControllers in der Liste der Spieler
					for(int i = 0; i < playerList.Length; i++)
					{
						//falls das aktuelle Objekt aus der Lsite das ist, breche die schleife ab
						if(playerList[i].Equals(cameraCtrl.targetCar.transform.gameObject))
						{
							indexOfTargetCar = i;
							break;
						}
					}

					//falls es das letzte Auto in der Liste ist, gehe wirder zum Anfang
					if(indexOfTargetCar == playerList.Length-1)
					{
						cameraCtrl.changeTargetCar(true, playerList[0].GetComponent<Car>());
					}
					//ansonsten ghe eines weiter
					else
					{
						cameraCtrl.changeTargetCar(true, playerList[indexOfTargetCar + 1].GetComponent<Car>());
					}
				}
				//falls der Spieler nun nichts mehr auf einen der Tasten drückt, darf er wieder das Ziel Auto wechseln
				else if((Input.GetAxis("Player" + numberOfControllerString + "Throttle") + Input.GetAxis("Player" + numberOfControllerString + "ThrottleKey") == 0.0f) && changedTargetCar == true)
				{
					changedTargetCar = false;
				}
			}
		}
	}

	public void endRace()
	{
		raceEnded = true;
		//suche den CircuitRaceMode
		circuitMode = GameObject.FindGameObjectWithTag("CircuitMode").GetComponent<CircuitRaceMode>();
	}
}
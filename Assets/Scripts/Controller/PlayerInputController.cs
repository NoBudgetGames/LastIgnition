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
	//refernz auf die Kamera Transforms
	public GameObject[] cameras;
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

			inv.setFiring(Input.GetButton("Player" + playerString + "FireKey"));
			inv.prevWeapon(Input.GetButton("Player" + playerString + "CycleWeaponDownKey"));
			inv.nextWeapon(Input.GetButton("Player" + playerString + "CycleWeaponUpKey"));

			car.setHandbrake(Input.GetButton("Player" + playerString + "HandbrakeKey"));
			cycleCamera(Input.GetButtonDown("Player" + playerString + "ChangeCameraKey"));

		}
		//falls Controller benutzt wird
		else
		{
			car.setThrottle(Input.GetAxis("Player" + playerString + "Throttle"));
			car.setSteer(Input.GetAxis("Player" + playerString + "Steer"));
			car.resetCar(Input.GetAxis("Player" + playerString + "ResetCar"));
			car.setHandbrake(Input.GetButton("Player" + playerString + "Handbrake"));	
			cycleCamera(Input.GetButtonDown("Player" + playerString + "ChangeCamera"));
		}
	}

	//diese Methode geht durch die enzelnen Kameras durch
	private void cycleCamera(bool cam)
	{
		if(cam)
		{
			//gehe durch das Array durch und aktiviere die richtige Kamera
			for (int i = 0; i < cameras.Length; i++)
			{
				//suche die momentan verwendetet Kamera und aktiviere die nächste
				if(cameras[i].activeSelf == true)
				{
					//falls momentane Kamera nicht das letzte Element im Array ist
					if(i+1 < cameras.Length)
					{
						cameras[i].SetActive(false);
						cameras[i+1].SetActive(true);
						return;
					}
					//ansonsten gehe zum Anfang zurück
					else
					{
						cameras[i].SetActive(false);
						cameras[0].SetActive(true);
						return;
					}
				}
			}
		}
	}
/*
	//fügt eine weitere Camera der Liste hinzu
	public void addCamera(GameObject cam)
	{
		GameObject[] tempCam = new GameObject[cameras.Length + 1];
		for(int i = 0; i < cameras.Length; i++)
		{
			tempCam[i] = cameras[i];
		}
		tempCam[cameras.Length + 1] = cam;
		cameras = tempCam;
	}
*/	
}

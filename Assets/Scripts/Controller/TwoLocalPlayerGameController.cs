using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Diese Klasse initialisiert die Fahrzeuge für den lokalen 2 Spieler "CooP"
 */

public class TwoLocalPlayerGameController : MonoBehaviour 
{
	//Startpunkte
	public Transform[] spawnPoints;
	//Referenz auf die Auto Prefab
	public GameObject carPrefab;
	//referenz auf die RearFloatingCamLow Prefab
	public GameObject camPrefab;
	//Liste mit Spielern
	private List<GameObject> playerList;

	// Use this for initialization
	void Start()
	{
		playerList = new List<GameObject>();
		instaciateNewPlayer(spawnPoints[0], "One");
		instaciateNewPlayer(spawnPoints[1], "Two");
	}

	//
	void Update()
	{
		foreach(GameObject obj in playerList)
		{
			if(obj.GetComponent<Car>().getHealth() <= 0.0f)
			{
				Debug.Log ("PLAYER " + obj.GetComponent<PlayerInputController>().playerString + " HAD HIS LAST IGNITION!");
			}
		}
		if(Input.GetKey(KeyCode.O))
		{
			reInstanciatePlayer("One");
		}
		if(Input.GetKey(KeyCode.P))
		{
			reInstanciatePlayer("Two");
		}
	}

	//resete das Auto des SPielers (FULL HEALTH)
	private void reInstanciatePlayer(string playerName)
	{
		GameObject player = new GameObject();
		//finde den richtigen Spieler aus der Liste
		foreach(GameObject obj in playerList)
		{
			if(obj.GetComponent<PlayerInputController>().playerString.Equals(playerName))
			{
				player = obj;
				break;
			}
		}
		if(player != null)
		{
			//gehe jede Kamera durch und zerstöre sie
			GameObject.Destroy(player.GetComponent<PlayerInputController>().cameraCtrl);
			
			//instanszere einen neuen Spieler
			instaciateNewPlayer(spawnPoints[0], playerName);

			playerList.Remove(player);
			//zerstöre den Spieler
			GameObject.Destroy(player);
		}
	}

	//diese Methode instanziert einen neuen Spieler und setzt die richtigen Referezen
	private void instaciateNewPlayer(Transform trans, string playerName)
	{
		//neues Auto
		GameObject player = (GameObject)GameObject.Instantiate(carPrefab, trans.position, trans.rotation);

		GameObject camObj = (GameObject)GameObject.Instantiate(camPrefab, trans.position, trans.rotation);
		CameraController cam = camObj.GetComponent<CameraController>();

		//der InputController muss wissen, welchen Spieler er gerade ist
		PlayerInputController input = player.GetComponent<PlayerInputController>();
		input.playerString = playerName;
		//Kamera muss aufgesetzt werden
		player.GetComponent<PlayerInputController>().cameraCtrl = cam;
		cam.targetCar = player.GetComponent<Car>();
		//erstes Element ist Motorhaubenkamera
		cam.hoodCamera = input.additionalCameras[0];
		//zweites Element ist Kofferraumkamera
		cam.hoodCameraLookBack = input.additionalCameras[1];
		setCamera(cam.GetComponent<Camera>(), playerName);
		playerList.Add(player);

		//Spieler 2 hat keine Tasten auf der Tastatur, nur solange noch Tastatur benötigt wird
		if(playerName.Equals("Two"))
		{
			input.usingController = true;
		}
	}

	private void setCamera(Camera cam, string playerName)
	{
		if(cam != null)
		{
			if(playerName.Equals("One"))
			{
				//(left, top, width, height)
				cam.rect = new Rect(0, 0, 1.0f, 0.5f);
			}
			else if(playerName.Equals("Two"))
			{
				cam.rect = new Rect(0, 0.5f, 1.0f, 0.5f);
			}

		}
	}

}
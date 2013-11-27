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
	public GameObject rearCamLowPrefab;
	//referenz auf die RearFloatingCamHIgh Prefab
	public GameObject rearCamHighPrefab;
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
	}

	//diese Methode instanziert einen neuen Spieler
	private void instaciateNewPlayer(Transform trans, string playerName)
	{
		//neues Auto
		GameObject player = (GameObject)GameObject.Instantiate(carPrefab, trans.position, trans.rotation);
		//niedrige Kamera
		GameObject camLow = (GameObject)GameObject.Instantiate(rearCamLowPrefab);
		//hohe Kamera
		GameObject camHigh = (GameObject)GameObject.Instantiate(rearCamHighPrefab);

		//die targets der Kameras müssen referenziert werden
		camLow.GetComponent<RearFlotingCameraController>().targetCar = player.GetComponent<Car>();
		camHigh.GetComponent<RearFlotingCameraController>().targetCar = player.GetComponent<Car>();

		//der InputController muss wissen, welchen Spieler er gerade ist
		PlayerInputController input = player.GetComponent<PlayerInputController>();
		input.playerString = playerName;

		//nur zum testen
		if(playerName.Equals("Two"))
		{
			input.usingController = true;
		}

		input.cameras[0] = camLow;
		input.cameras[1] = camHigh;

		foreach(GameObject obj in input.cameras)
		{
			Camera cam = obj.GetComponent<Camera>();
			setCamera(cam, playerName);
		}
		input.cameras[2].SetActive(false);
		camHigh.SetActive(false);
		playerList.Add(player);
	}

	private void setCamera(Camera cam, string playerName)
	{
		if(cam != null)
		{
			if(playerName.Equals("One"))
			{
				//(left: float, top: float, width: float, height: float)
				cam.rect = new Rect(0, 0, 1.0f, 0.5f);
			}
			else if(playerName.Equals("Two"))
			{
				cam.rect = new Rect(0, 0.5f, 1.0f, 0.5f);
			}

		}
	}

}
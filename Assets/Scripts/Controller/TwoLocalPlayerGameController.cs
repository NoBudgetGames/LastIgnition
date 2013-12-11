using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Diese Klasse initialisiert die Fahrzeuge für den lokalen 2 Spieler "CooP"
 * Die Autos werden werden an ihren Startpositionen gespawnt
 * Falls ein Auto kaputt ist, kann man es wieder neu  spawnen (llerdings erstmal nur zu testzwecken)
 */

public class TwoLocalPlayerGameController : MonoBehaviour 
{
	//Startpunkte
	public Transform[] spawnPoints;
	//Referenz auf die Auto Prefab
	public GameObject carPrefab;
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
		if(Input.GetKeyDown(KeyCode.O))
		{
			reInstanciatePlayer("One");
		}
		if(Input.GetKeyDown(KeyCode.P))
		{
			reInstanciatePlayer("Two");
		}
	}

	//diese Methode instanziert einen neuen Spieler und setzt die richtigen Referezen
	//bei playerName wird nur zwischen One und Two unterschieden, es ist nicht dre richtige Name des Spielers!
	private void instaciateNewPlayer(Transform trans, string playerName)
	{
		//neues Auto
		GameObject player = (GameObject)GameObject.Instantiate(carPrefab, trans.position, trans.rotation);

		//der InputController muss wissen, welcher Spieler er gerade ist
		PlayerInputController input = player.GetComponent<PlayerInputController>();
		input.playerString = playerName;
		input.setupHUD();
		//Kamera muss aufgesetzt werden
		setCamera(input.cameraCtrl.GetComponent<Camera>(), playerName);
		playerList.Add(player);

		//Spieler 2 hat keine Tasten auf der Tastatur, nur solange noch Tastatur benötigt wird
		if(playerName.Equals("Two"))
		{
			input.usingController = true;
		}
	}

	//resete das Auto des SPielers (FULL HEALTH)
	//bei playerName wird nur zwischen One und Two unterschieden, es ist nicht der richtige Name des Spielers!
	private void reInstanciatePlayer(string playerName)
	{
		GameObject player = new GameObject("PLayerToDestroy");
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
			//zerstöre die Kamera
			GameObject.Destroy(player.GetComponent<PlayerInputController>().cameraCtrl.gameObject);
			//lösche den Spieler aus der Liste
			playerList.Remove(player);
			//zerstöre den Spieler
			//Anmerkung: die Transform Komponente lässt sich nicht zerstören und bleibt daher noch in der der 
			//Szene übrig
			GameObject.Destroy(player.gameObject);
			
			//instanszere einen neuen Spieler
			if(playerName.Equals("One"))
			{
				//spieler 1 an ersten Spawnpunkt
				instaciateNewPlayer(spawnPoints[0], playerName);
			}
			else
			{
				//spieler 2 an zweiten Spawnpunkt
				instaciateNewPlayer(spawnPoints[1], playerName);
			}
		}
	}

	//diese Methode setzt die Kamerahöhe auf die richtigen Werte, abhängig vom Spieler
	//bei playerName wird nur zwischen One und Two unterschieden, es ist nicht dre richtige Name des Spielers!
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
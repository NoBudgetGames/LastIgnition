using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Diese Klasse initialisiert die Fahrzeuge für den lokalen 2 Spieler "CooP"
 * Die Autos werden werden an ihren Startpositionen gespawnt
 * Falls ein Auto kaputt ist, kann man es wieder neu  spawnen (allerdings erstmal nur zu testzwecken)
 */

public class TwoLocalPlayerGameController : MonoBehaviour 
{
	//Referenz auf die Start-/Spawnpunkte
	public GameObject[] spawnPoints;
	//Referenz auf die Auto Prefabs
	//MUSS MIT DER LISTE IM CAR SELECTOR ÜBEREINSTIMMEN!!
	public GameObject[] carPrefabs;
	//Liste mit Spielern, wird vom Script selber gefüllt,
	public List<GameObject> playerList;
	
	// Use this for initialization
	void Awake()
	{
		playerList = new List<GameObject>();
		if(GameObject.FindGameObjectsWithTag("SpawnPoint") != null)
		{
			spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		}
		if(PlayerPrefs.GetInt("LocalPlayers") == 1)
		{
			instaciateNewPlayer(spawnPoints[0], "One");
			this.enabled = false;
		}
		else
		{
			instaciateNewPlayer(spawnPoints[0], "One");
			instaciateNewPlayer(spawnPoints[1], "Two");
		}
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.O))
		{
			reInstanciatePlayer("One", true);
		}
		if(Input.GetKeyDown(KeyCode.P))
		{
			reInstanciatePlayer("Two", true);
		}
	}
	
	//diese Methode instanziert einen neuen Spieler und setzt die richtigen Referezen
	//bei playerName wird nur zwischen One und Two unterschieden, es ist nicht dre richtige Name des Spielers!
	private void instaciateNewPlayer(GameObject trans, string playerName)
	{
		int carIndex = PlayerPrefs.GetInt(playerName);
		//neues Auto
		GameObject player = (GameObject)GameObject.Instantiate(carPrefabs[carIndex], trans.transform.position, trans.transform.rotation);
		
		//der InputController muss wissen, welcher Spieler er gerade ist
		PlayerInputController input = player.GetComponent<PlayerInputController>();
		input.numberOfControllerString = playerName;
		input.setupHUD();
		//Kamera muss aufgesetzt werden
		setCamera(input.cameraCtrl.GetComponent<Camera>(), playerName);
		playerList.Add(player);
	}
	
	//resete das Auto des SPielers (FULL HEALTH)
	//bei playerName wird nur zwischen One und Two unterschieden, es ist nicht der richtige Name des Spielers!
	public void reInstanciatePlayer(string playerName, bool toBeDestroyed)
	{
		GameObject player = new GameObject("PLayerToDestroy");
		//finde den richtigen Spieler aus der Liste
		foreach(GameObject obj in playerList)
		{
			if(obj.GetComponent<PlayerInputController>().numberOfControllerString.Equals(playerName))
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
			//zertöre den HUD
			GameObject.Destroy(player.GetComponent<PlayerInputController>().hud.gameObject);

			//falls das Auto nicht mehr in der Szene sein soll, lösche das Auto
			if(toBeDestroyed == true)
			{
				//zerstöre den Spieler
				//Anmerkung: die Transform Komponente lässt sich nicht zerstören und bleibt daher noch in der der 
				//Szene übrig
				GameObject.Destroy(player.gameObject);
			}
			//ansonsten lasse das Auto in der Szene und lösche alle nicht mehr relevanten/benötigten Komponenten
			else
			{
				//lösche den Minimal Icon GameObject
				GameObject.Destroy(player.GetComponentInChildren<MiniMapElement>().gameObject);
				//lösche den inputController
				GameObject.Destroy(player.GetComponent<PlayerInputController>());

			}
			
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
	
	//diese Methode setzt die Kameraposition/-höhe (für den SliptScreen am Bildschirm) auf die richtigen Werte,
	//abhängig vom Spieler. Bei playerName wird nur zwischen One und Two unterschieden, es ist nicht der richtige Name des Spielers!
	private void setCamera(Camera cam, string playerName)
	{
		if(cam != null)
		{
			if(PlayerPrefs.GetInt("LocalPlayers") == 1){
				cam.clearFlags = CameraClearFlags.Skybox;
				cam.rect = new Rect(0, 0, 1.0f, 1.0f);
			} else {
				cam.clearFlags = CameraClearFlags.Skybox;
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
}
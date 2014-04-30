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
	public SpawnZone[] spawnPoints;
	//Referenz auf die Auto Prefabs
	//MUSS MIT DER LISTE IM CAR SELECTOR ÜBEREINSTIMMEN!!
	public GameObject[] carPrefabs;
	//Liste mit Spielern, wird vom Script selber gefüllt,
	public List<GameObject> playerList;

	
	// Use this for initialization
	void Awake()
	{
		//this.networkView.viewID = Network.AllocateViewID();
		playerList = new List<GameObject>();
		if(PlayerPrefs.GetInt("LocalPlayers") == 1)
		{
			if(Network.isServer || Network.connections.Length == 0){
				playerList.Add(instaciateNewPlayer(getFreeSpawnPoint(), "One"));
				//this.enabled = false;
			} else {
				this.networkView.RPC("requestPlayerSpawn",RPCMode.Server,Network.player,"One",0);
			}
		}
		else
		{
			if(Network.isServer || Network.connections.Length == 0){
				playerList.Add(instaciateNewPlayer(getFreeSpawnPoint(), "One"));
				playerList.Add(instaciateNewPlayer(getFreeSpawnPoint(), "Two"));
			} else {
				this.networkView.RPC("requestPlayerSpawn",RPCMode.Server,Network.player,"One",0);
				this.networkView.RPC("requestPlayerSpawn",RPCMode.Server,Network.player,"Two",1);
			}
		}
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.I))
		{
			reInstanciatePlayer("One", true);
		}
		if(Input.GetKeyDown(KeyCode.O))
		{
			reInstanciatePlayer("Two", true);
		}
	}
	
	//diese Methode instanziert einen neuen Spieler und setzt die richtigen Referezen
	//bei playerInputString handelt es sich um den Einfüestring für die Steuerung
	//Die Methode liefert den Spieler wieder zurück
	private GameObject instaciateNewPlayer(GameObject trans, string playerInputString)
	{
		int carIndex = PlayerPrefs.GetInt(playerInputString);
		//neues Auto
		GameObject player;
		if(Network.connections.Length > 0){
			player = (GameObject)Network.Instantiate(carPrefabs[carIndex], trans.transform.position, trans.transform.rotation,0);
		} else {
			player = (GameObject)GameObject.Instantiate(carPrefabs[carIndex], trans.transform.position, trans.transform.rotation);
		}
		
		//der InputController muss wissen, welcher Spieler er gerade ist
		PlayerInputController input = player.GetComponent<PlayerInputController>();
		input.numberOfControllerString = playerInputString;
		input.setupHUD();
		//setze den Namen des Spielers
		if(playerInputString.Equals("One"))
		{
			input.playerName = PlayerPrefs.GetString("PlayerOneName");
		}
		else
		{
			input.playerName = PlayerPrefs.GetString("PlayerTwoName");
		}
		//Kamera muss aufgesetzt werden
		setCamera(input.cameraCtrl.GetComponent<Camera>(), playerInputString);
		//playerList.Add(player);
		return player;
	}

	//Anfrage an den Server um einen freien Spawn Punkt zu finden
	[RPC]
	void requestPlayerSpawn(NetworkPlayer client, string playerInputString,int listIndex){
		GameObject spawn = getFreeSpawnPoint();
		spawn.GetComponent<SpawnZone>().spawnIsNotFree();
		this.networkView.RPC("instantiateNetworkPlayer",client,spawn.transform.position,spawn.transform.rotation
		                     ,playerInputString,listIndex);
	}
	//Client spawnt einen neuen Spieler basierend auf den Koordinaten vom Server
	[RPC]
	void instantiateNetworkPlayer(Vector3 spawnPosition,Quaternion spawnRotation, string playerInputString,int listIndex){
		int carIndex = PlayerPrefs.GetInt(playerInputString);
		//neues Auto
		GameObject player;
		player = (GameObject)Network.Instantiate(carPrefabs[carIndex], spawnPosition, spawnRotation,0);
		
		
		//der InputController muss wissen, welcher Spieler er gerade ist
		PlayerInputController input = player.GetComponent<PlayerInputController>();
		input.numberOfControllerString = playerInputString;
		input.setupHUD();
		//setze den Namen des Spielers
		if(playerInputString.Equals("One"))
		{
			input.playerName = PlayerPrefs.GetString("PlayerOneName");
		}
		else
		{
			input.playerName = PlayerPrefs.GetString("PlayerTwoName");
		}
		//Kamera muss aufgesetzt werden
		setCamera(input.cameraCtrl.GetComponent<Camera>(), playerInputString);

		playerList.Insert(listIndex,player);

	}
	
	//resete das Auto des SPielers (FULL HEALTH)
	//bei playerInputString handelt es sich um den Einfüestring für die Steuerung
	public void reInstanciatePlayer(string playerInputString, bool toBeDestroyed)
	{
		//um ein neues Fahrzeug neu zu instanzieren, muss zunächst geschaut werden, wo sich das "alte" Auto in der Spielerliste befindet,
		//um genau dort wieder das neu instanzierte Fahreug einzufügen
		//index des Spielers aus der Liste
		int index = 0;
		GameObject player = new GameObject("PLayerToDestroy");
		//finde den richtigen Spieler aus der Liste
		foreach(GameObject obj in playerList)
		{
			//falls es der richtige ist
			if(obj.GetComponent<PlayerInputController>().numberOfControllerString.Equals(playerInputString))
			{
				player = obj;
				break;
			}
			index++;
		}
		if(player != null)
		{
			//zerstöre die Kamera
			GameObject.Destroy(player.GetComponent<PlayerInputController>().cameraCtrl.gameObject);
			//lösche den Spieler aus der Liste
			playerList.Remove(player);
			//zertöre den HUD
			GameObject.Destroy(player.GetComponent<PlayerInputController>().hud.gameObject);

			//falls das Auto(-wrack) nicht mehr in der Szene sein soll, lösche das Auto
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
				//GameObject.Destroy(player.GetComponentInChildren<MiniMapElement>().gameObject);
				//lösche den inputController
				GameObject.Destroy(player.GetComponent<PlayerInputController>());
			}
			//instanszere einen neuen Spieler und füge ihn in der List da ein, wo er vorher war
			if(Network.isServer || Network.connections.Length == 0){
				playerList.Insert(index, instaciateNewPlayer(getFreeSpawnPoint(), playerInputString));
			} else {
				this.networkView.RPC("requestPlayerSpawn",RPCMode.Server,Network.player,playerInputString,index);
			}
		}
	}
	
	//diese Methode setzt die Kameraposition/-höhe (für den SliptScreen am Bildschirm) auf die richtigen Werte,
	//abhängig vom Spieler. bei playerInputString handelt es sich um den Einfüestring für die Steuerung
	private void setCamera(Camera cam, string playerInputString)
	{
		if(cam != null)
		{
			//die Kamera soll Skyboxen darstellen
			cam.clearFlags = CameraClearFlags.Skybox;
			//falls wir nur einen Spieler haben, soll der Viewport den gesamtem Bildsichrm ausfüllen
			if(PlayerPrefs.GetInt("LocalPlayers") == 1)
			{
				cam.rect = new Rect(0, 0, 1.0f, 1.0f);
			}
			//ansonsten haben wir zwei Spieler und müssen den Bildschirm in zwei Teile aufteilen
			else 
			{
				if(playerInputString.Equals("Two"))
				{
					//(left, top, width, height)
					cam.rect = new Rect(0, 0, 1.0f, 0.5f);
				}
				else if(playerInputString.Equals("One"))
				{
					cam.rect = new Rect(0, 0.5f, 1.0f, 0.5f);
				}
			}
		}
	}

	//diese Methode leifert einen freien SpawnPunkt zurück
	private GameObject getFreeSpawnPoint()
	{
		GameObject obj = new GameObject();
		
		//gehe alle Spawnpunkte durch und schaue, ob einer frei ist
			foreach(SpawnZone spawnPoint in spawnPoints)
			{
				//falls einer frei ist, returne das GameObject zum Spawnpunkt
				if(spawnPoint.isSpawnZoneFree() == true)
				{
					obj = spawnPoint.gameObject;
					spawnPoint.spawnIsNotFree();
					break;
				}
			}
			return obj; 

	}

}
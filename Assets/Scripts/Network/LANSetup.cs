using UnityEngine;
using System.Collections;
using System;

/*
 * Diese Methode setzt einen Lokalen Server auf
 * Dabei sucht sie sich die IP Addresse des PCs im Netzwerk
 * Der Spieler, der zum Server verbinden möchte, muss allerdings die IP Addresse kennen
 */ 

public class LANSetup : MonoBehaviour 
{

	//IP Addresse des Servers / lokalen Spielers
	private string serverIP = "127.0.0.1";
	//Port des Servers / lokalen Spielers
	private int port = 25000;
	//wurde der lokale Server initialisiert?
	private bool initializedServer = false;
	//soll ein Server gejoined werden?
	private bool joinServer = false;
	//wurde das das Spiel schon gestartet? Wenn ja, sollen die Menüs nicht mehr dargestellt werden
	private bool gameRunning = false;
	//	
	private int levelPrefix = 0;
	//die Anzahl der Spieler, die momentan auf dem Server sind
	private int numberOfCurrentsPlayers = 0;
	
	// Use this for initialization
	void Start () 
	{
		DontDestroyOnLoad(this);
		Application.runInBackground = true;
		this.networkView.group = 1;
		if(PlayerPrefs.GetInt("LocalPlayers") == 1)
		{
			numberOfCurrentsPlayers = 1;
		}
		if(PlayerPrefs.GetInt("LocalPlayers") == 2)
		{
			numberOfCurrentsPlayers = 2;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		Application.runInBackground = true;
	}

	//Diese Methode startet den Server
	void startServer()
	{
		//hole die IPAddresse des PCs im Netzwerk
		serverIP = Network.player.ipAddress;
		//port = Network.player.port;
		//initialliziere den Server
		Network.InitializeServer(32, port, false);
	}

	//diese Methode verbindet einen Client mit dem Server
	void connectToServer()
	{
		Network.Connect(serverIP, port);
	}

	//Diese Methode wird beim Server aufgerufen, wenn der Server erfolgreich initializiert wurde
	void OnServerInitialized()
	{
		Debug.Log ("Server initialized with IP: " + serverIP + " Port: " + port);
		initializedServer = true;
	}

	//Diese Methode wird beim Client aufgerufen, wenn sich ein Client mit dem Server verbunden hat
	void OnConnectedToServer() 
	{
		Debug.Log("Connected to server");
	}

	//Diese Methode wird beim Client aufgerufen, wenn der Client keine Verbindug zum Server aufbauen konnte
	void OnFailedToConnect(NetworkConnectionError error)
	{
		Debug.Log("Could not connect to server: " + error);
	}

	[RPC]
	void loadLevel(string level, int newLevelPrefix)
	{
		StartCoroutine(loadLevelCoroutine(level,newLevelPrefix));
	}
	
	private IEnumerator loadLevelCoroutine(string level, int newLevelPrefix)
	{
		levelPrefix = newLevelPrefix;
		
		Network.SetSendingEnabled(0,false);
		
		Network.isMessageQueueRunning = false;
		
		//Network.SetLevelPrefix(levelPrefix);
		Application.LoadLevel(level);
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		
		Network.isMessageQueueRunning = true;
		
		Network.SetSendingEnabled(0, true);
		
		foreach( GameObject g in FindObjectsOfType(typeof(GameObject)))
		{
			g.SendMessage("OnNetworkLoadedLevel",SendMessageOptions.DontRequireReceiver);
		}
	}

	[RPC]
	void debug(string message)
	{
		Debug.Log(message);
	}

	void OnGUI()
	{
		if(gameRunning == false)
		{
			//Button für Server starten
			if(GUI.Button(new Rect(10, 10, 100, 20), "Server erstellen"))
			{
				//starte den Server
				startServer();
				joinServer = false;
			}
			//falls er gestartet wurde, zeige IPaddresse und Port an
			if(Network.isServer == true)
			{
				GUI.Label(new Rect(10, 30, 150, 100), "Server Running!");
				GUI.Label(new Rect(10, 45, 150, 100), "LAN IP: " + serverIP);
				GUI.Label(new Rect(10, 60, 150, 100), "LAN Port: " + port);
				//GUI.Label(new Rect(10, 90, 150, 100), "External IP: " + Network.player.externalIP);
				//GUI.Label(new Rect(10, 105, 150, 100), "External Port: " + Network.player.externalPort);
			}
			
			//Button um mit dem Server zu verbinden
			if(GUI.Button(new Rect(200, 10, 150, 20), "Connect To Server"))
			{
				joinServer = true;
			}
			
			//falls mit einen Server verbunden werden soll, zeige Eingabefelder für IPaddresse und Port an
			if(joinServer == true)
			{
				serverIP = GUI.TextField(new Rect(200, 35, 100, 20), serverIP, 15);
				port = Convert.ToInt32(GUI.TextField(new Rect(200, 55, 100, 20), "" + port, 5));

				if (GUI.Button(new Rect(200, 75, 100, 20), "Join Game"))
				{
					connectToServer();
					initializedServer = false;
				}

				if (GUI.Button(new Rect(200, 90, 100, 20), "Debug"))
				{
					this.networkView.RPC("debug",RPCMode.AllBuffered, "BLUB BLIB");
				}
			}
			
			if(Network.isServer && Network.connections.Length > 0)
			{
				GUI.Label(new Rect(10, 75, 100, 100), "Connections: " + Network.connections.Length);
				if(GUI.Button(new Rect(10, 100, 150, 20), "Start Game"))
				{
					gameRunning = true;
					Network.RemoveRPCsInGroup(0);
					Network.RemoveRPCsInGroup(1);
					this.networkView.RPC("loadLevel",RPCMode.AllBuffered,"ChooseCar", levelPrefix + 1);
				}
			}
		}
	}
}
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

	// Use this for initialization
	void Start () 
	{
		Application.runInBackground = true;
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
		initializedServer = true;
		Debug.Log ("Server initialized with IP: " + serverIP + " Port: " + port);
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

	void OnGUI()
	{
		//Button für Server starten
		if(GUI.Button(new Rect(10, 10, 100, 20), "Start Server"))
		{
			//starte den Server
			startServer();
			joinServer = false;
		}
		//falls er gestartet wurde, zeige IPaddresse und Port an
		if(initializedServer == true)
		{
			GUI.Label(new Rect(10, 30, 100, 100), "Sever Running!");
			GUI.Label(new Rect(10, 45, 100, 100), "IP: " + serverIP);
			GUI.Label(new Rect(10, 60, 100, 100), "Port: " + port);
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
		}
	}
}
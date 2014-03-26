using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Diese Klasse ist für das Multiplayerspiel zuständig.
 * Man kann dabei zwischen einen Online-Multiplayer basierend auf den Unity-Master Servern starten
 * oder einen LAN SPiel erstellen
 * 
 * Im Online Multiplayer wird ein Server/Host beim MasterServer regristiriert und alle anderen Clients
 * fragen den MasterServer nach Host für dieses Spiel
 * 
 * In LAN Modus sucht der Host/Server die IP Addresse des PCs im lokalen Netzwerk
 * Der Spieler, der zum Server verbinden möchte, muss allerdings die IP Addresse kennen 
 * (was in einen LAN nicht schwierig sein sollte, da die SPieler in der Regeln auch im selben Rau sind)
 * 
 * Wenn ein Server oder die Verbindung zu einen Server steht, wird die Lobby-Szene geladen, von wo man aus das SPiel starten kann
 */ 

public class NetworkSetup : MonoBehaviour
{
	//die Prefab für das NetworkPlayerData Objekt
	public GameObject playerDataPrefab;

	//die maximale Anzahl an Spielern
	private const int MAX_PLAYERS = 8;

	//String, der das aktuell gewählte Menü zeigen soll
	private string currentMenu = "NetworkMain";
	//soll ein Onlinespiel oder ein LAN Spiel erstellt werden?
	private bool isThisAOnlineGame = false;
	//liste mit den Infos der Spieler
	private List<NetworkPlayerData> playerInfos;
	//Scrollview für die Lobby
	private Vector2 lobbyScrollView = Vector2.zero;

	//Online
	//eine Liste der Hosts, die sich beim MasterServer angeldet haben
	private HostData[] hostList;
	//Scrollview für den ServerBrowser
	private Vector2 serverBrowserScrollView = Vector2.zero;

	//LAN
	//IP Addresse des Servers / lokalen Spielers
	private string LANIPAddress = "127.0.0.1";
	//Port des Servers / lokalen Spielers
	private int LANPort = 25000;
	//ist dieses Spiel ein LAN Spiel?
	private bool isLANGame = false;

	//soll ein Server aufgesetzt werden?
	private bool wantToStartServer = false;
	//wurde der Server initialisiert?
	private bool initializedServer = false;
	//soll ein Server gejoined werden?
	private bool wantToJoinServer = false;
	//wurde das das Spiel schon gestartet? Wenn ja, sollen die Menüs nicht mehr dargestellt werden
	private bool gameRunning = false;
	//
	private int levelPrefix = 0;
	//die Anzahl der Spieler, die momentan auf dem Server sind
	private int numberOfCurrentsPlayers = 0;
	//die Referenz auf das NetworkPlayerData Object für Spieler 1
	private GameObject playerDataOne;
	//die Referenz auf das NetworkPlayerData Object für Spieler 1
	private GameObject playerDataTwo;

	private bool startGameBool;
	private bool setup;
	private bool levelSelect;

	// Use this for initialization
	void Start ()
	{
		//dieses GameObject soll weiterhin existieren
		DontDestroyOnLoad(this);
		//maximal 8 Spieler
		playerInfos = new List<NetworkPlayerData>();
		levelPrefix = 0;
		this.networkView.group = 1;
		//Spiel soll auch im Hintergrund laufen
		Application.runInBackground = true;
	}

	// Update is called once per frame
	void Update ()
	{
		Application.runInBackground = true;
	}

	//Diese Methode startet den lokalen Server für einen LAN SPiel
	private void startLANServer()
	{
		//hole die IPAddresse des PCs im Netzwerk
		LANIPAddress = Network.player.ipAddress;
		//port = Network.player.port;
		//initialliziere den Server
		Network.InitializeServer(32, LANPort, false);
		isLANGame = true;
	}

	//diese Methode verbindet einen Client mit dem Server
	void connectToLANServer()
	{
		Network.Connect(LANIPAddress, LANPort);
		isLANGame = true;
	}

	//Diese Methode instanziert die NetworkPlayerData für die lokalen Spieler
	private void intanciateNetPlayerData()
	{
		//Anzahl der lokalen (an einen PC) Spieler
		if(PlayerPrefs.GetInt("LocalPlayers") == 1)
		{
			numberOfCurrentsPlayers = 1;
			//PlayerData für Spieler 1
			playerDataOne = (GameObject)Network.Instantiate(playerDataPrefab, this.transform.position, this.transform.rotation, 1);
			playerDataOne.name = "playerDataOne";
			playerInfos.Add(playerDataOne.GetComponent<NetworkPlayerData>());
		}
		if(PlayerPrefs.GetInt("LocalPlayers") == 2)
		{
			numberOfCurrentsPlayers = 2;
			//PlayerData für Spieler 1
			playerDataOne = (GameObject)Network.Instantiate(playerDataPrefab, this.transform.position, this.transform.rotation, 1);
			playerDataOne.name = "playerDataOne";
			playerInfos.Add(playerDataOne.GetComponent<NetworkPlayerData>());
			
			//PlayerData für Spieler 2
			playerDataTwo = (GameObject)Network.Instantiate(playerDataPrefab, this.transform.position, this.transform.rotation, 1);
			playerDataTwo.name = "playerDataTwo";
			//ConntrolerString muss noch gesetzr werden
			playerDataTwo.GetComponent<NetworkPlayerData>().setControllerString("Two");
			playerInfos.Add(playerDataTwo.GetComponent<NetworkPlayerData>());
		}
	}

	//diese Methode starte einen OnlineServer, in dem der MasterServer kontaktiert wird
	private void startOnlineServer()
	{
		Network.InitializeServer(4,25000,Network.HavePublicAddress());
		//Spielname, Lobbyname (in dem Fall der Name des Spieler 1), Anzahl der Verbunden Spieler (NICHT DER CLIENTS!!, da 2 Spieler
		//pro Client möglich ist)
		MasterServer.RegisterHost("FHTrierLastIgnition", PlayerPrefs.GetString("PlayerOneName"), "" + numberOfCurrentsPlayers);

		//instanziere die NetworkPlayerData
		intanciateNetPlayerData();
		
		//falls der der Server aufgesetzt wurde, gehe zur Lobby
		currentMenu = "Lobby";
		//die Lobby Szene ist dabei eine leere Szene mit einer Kamera, damit man nach einen Rennen wieder zur Lobby wechslen kann
		Application.LoadLevel("MultiplayerLobby");
	}

	private void startGame()
	{
		gameRunning = true;
		Network.RemoveRPCsInGroup(0);
		Network.RemoveRPCsInGroup(1);
		this.networkView.RPC("loadLevel",RPCMode.AllBuffered,"ChooseCar",levelPrefix+1);
	}

	//diese Methode aktuallisiert die verfügbaren Server
	void refreshHostList()
	{
		MasterServer.RequestHostList("FHTrierLastIgnition");
	}

	//diese Methode
	void joinOnlineServer(HostData hd)
	{
		Network.Connect(hd);
	}

//// EVENT METHODEN

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	//diese MEthode wird auf dem Server aufgerufen, wenn der Server gestartet wurde
	void OnServerInitialized()
	{
		Debug.Log ("Server initialized");
		currentMenu = "Lobby";
	}

	//diese Methode wird auf dem Server aufgerufen, wenn sich ein Spieler erfolgreich mit dem Server verbunden hat
	void OnPlayerConnected(NetworkPlayer player)
	{
		Debug.Log("Player " + " connected from " + player.ipAddress);
		this.networkView.RPC("receiveLevelName",RPCMode.AllBuffered,PlayerPrefs.GetString("Level"));
	}

	//Diese Methode wird beim Client aufgerufen, wenn sich ein Client mit dem Server verbunden hat
	void OnConnectedToServer() 
	{
		Debug.Log("Connected to server");
			
		//instanziere die NetworkPlayerData
		intanciateNetPlayerData();

		//sage dem Server bescheid, wie viele Spieler der Client hat
		this.networkView.RPC("updatePlayerNumber",RPCMode.AllBuffered, PlayerPrefs.GetInt("LocalPlayers"));

		if(PlayerPrefs.GetInt("LocalPlayers") == 1)
		{
			//sage dem Server bescheid, wie die playerData ausieht
			this.networkView.RPC("updatePlayerInfo",RPCMode.AllBuffered, playerDataOne);
		}
		else
		{
			//sage dem Server bescheid, wie die playerData ausieht
			this.networkView.RPC("updatePlayerInfo",RPCMode.AllBuffered, playerDataOne);
			//sage dem Server bescheid, wie die playerData ausieht
			this.networkView.RPC("updatePlayerInfo",RPCMode.AllBuffered, playerDataTwo);
		}
		currentMenu = "Lobby";
	}

	//Diese Methode wird beim CLient aufgerufen, wenn sich der CLient nicht mit dem Server verbindne konnte
	void OnFailedToConnect(NetworkConnectionError error)
	{
		//Debug.Log("Could not connect to server: " + error);
		GUI.Label(new Rect(Screen.width/2 - 250, Screen.height/2 + 150, 500, 30), "Konnte nicht mit dem Serer verbinden. Fehler: " + error);
	}

//// RPC METHODEN

	//nur um z gucken, ob sich nachrichten verschicken lassen
	[RPC]
	void debug(string message)
	{
		Debug.Log(message);
	}

	[RPC]
	void loadLevel(string level, int newLevelPrefix){
		StartCoroutine(loadLevelCoroutine(level,newLevelPrefix));
	}
	
	private IEnumerator loadLevelCoroutine(string level, int newLevelPrefix){
		
		levelPrefix = newLevelPrefix;
		
		Network.SetSendingEnabled(0,false);
		
		Network.isMessageQueueRunning = false;
		
		Network.SetLevelPrefix(levelPrefix);
		Application.LoadLevel(level);
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		
		Network.isMessageQueueRunning = true;
		
		Network.SetSendingEnabled(0, true);
		
		foreach( GameObject g in FindObjectsOfType(typeof(GameObject))){
			g.SendMessage("OnNetworkLoadedLevel",SendMessageOptions.DontRequireReceiver);
		}
	}

	[RPC]
	void receiveLevelName(string level){
		PlayerPrefs.SetString("Level",level);
		Debug.Log("RECEIVED");
	}

	//diese Methode aktuallisiert die Anzahl der Spieler,
	[RPC]
	private void updatePlayerNumber(int localPlayers)
	{
		numberOfCurrentsPlayers += localPlayers;
		if(isThisAOnlineGame == true)
		{
			//Update die HostData für den MasterServer
			MasterServer.RegisterHost("FHTrierLastIgnition", PlayerPrefs.GetString("PlayerOneName"), "" + numberOfCurrentsPlayers);
		}
	}

	//diese Methode aktuallisiert die Spieler Infos, sodass der Server weiss, wie viele tatsächliche Spieler am Renne teilnehmen
	[RPC]
	private void updatePlayerInfo(string[] playerData)
	{
		//index der Daten, falls ausgetauscht werden muss
		int index = 0;
		bool needToReplace = false;
		foreach(NetworkPlayerData player in playerInfos)
		{
			//falls die Network ID die selbe ist, ist der Spieler bereits in der Liste und wir müssen die Daten austauschen
			if(player.getPlayerData()[0] == playerData[0])
			{
				index = playerInfos.IndexOf(player);
				needToReplace = true;
			}
		}
		if(needToReplace == true)
		{
			playerInfos.RemoveAt(index);
			NetworkPlayerData data = new NetworkPlayerData();
			data.setAll(playerData);
			playerInfos.Insert(index, data);
		}
		//ansonsten befindet sich der Spieler nicht in der Liste, und wir fügen ihn hinzu
		else
		{
			NetworkPlayerData data = new NetworkPlayerData();
			data.setAll(playerData);
			playerInfos.Add(data);
		}		
	}

//// GUI METHODEN

	//Die GUI Methode
	void OnGUI()
	{
		if(gameRunning == false)
		{
			//falls aktuelles Menu das Hauptmenü ist
			if(currentMenu.Equals("NetworkMain"))
			{
				networkMainMenu();
			}
			//falls OnlineMultiplayer gewählt wurde
			if(currentMenu.Equals("Online"))
			{
				onlineMultiplayer();
			}
			//falls man ein Onlinespiel joinen will, muss zunächst der Serverbrowser angezeigt werden
			if(currentMenu.Equals("OnlineServerBrowser"))
			{
				onlineServerBrowser();
			}
			//falls LAN Multiplayer gewählt wurde
			if(currentMenu.Equals("LAN"))
			{
				LANMultiplayer();
			}
			//falls zu einen LAN Server verbunden werden soll
			if(currentMenu.Equals("LANConnect"))
			{
				LANConnectToServer();
			}
			//falls der Server gestartet wurde und die Lobby gezeigt werden soll
			if(currentMenu.Equals("Lobby"))
			{
				lobby();
			}
		}

		/*
		if (!Network.isClient && !Network.isServer && setup)
		{
			if(!levelSelect){
				if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
					levelSelect = true;

				if (GUI.Button(new Rect(400, 100, 250, 100), "Refresh Host List"))
					refreshHostList();

				if(hostList != null){
					for(int i = 0; i<hostList.Length; ++i){
						if (GUI.Button(new Rect(400, 300+i*200, 250, 100), hostList[i].gameName))
							joinOnlineServer(hostList[i]);
					}
				}
			} else {
				multiLevelSelection();
			}
		}
		if(Network.isServer && Network.connections.Length > 0 && setup ){
			if (GUI.Button(new Rect(100, 300, 250, 100), "Start Game")){
				setup = false;
				Network.RemoveRPCsInGroup(0);
				Network.RemoveRPCsInGroup(1);
				this.networkView.RPC("loadLevel",RPCMode.AllBuffered,"ChooseCar",levelPrefix+1);
			}
		}
		*/		
	}

	//"Hauptmenü" des NEtzwerksmenüs
	private void networkMainMenu()
	{
		isThisAOnlineGame = false;
		isLANGame = false;
		//kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 80, Screen.height/2 - 200, 160, 200), "Multiplayer");
		
		//Button für Online Multiplayer
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 150, 100, 20), "Online")) 
		{
			//falls man eine Verbindung zum Internet herstellen kann, gehe weiter
			if(Application.internetReachability != NetworkReachability.NotReachable)
			{
				currentMenu = "Online";
			}
			//Ansonsten zeige Fehler an 
			else
			{
				GUI.Label(new Rect(Screen.width/2 - 250, Screen.height/2 + 150, 500, 30), "Es wurde keine Internetverbindung gefunden! Bitte Verbindung überprüfen!");
			}
		}
		//button für LAN
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 100, 100, 20), "LAN")) 
		{
			currentMenu = "LAN";
		}
		//button um zum Hauptmenü zurückzukehren
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 50, 100, 20), "Hauptmenü")) 
		{
			Application.LoadLevel("MainMenuScene");
		}
	}

	//das Menü, das bei einen Onlinemultiplayer gezeigt werden soll
	private void onlineMultiplayer()
	{
		//kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 80, Screen.height/2 - 200, 160, 200), "Online");
		isThisAOnlineGame = true;

		//Button um Online Server zu erstellen
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 150, 100, 20), "Server erstellen"))
		{
			startOnlineServer();
		}
		//button um Online Server zu suchen
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 100, 100, 20), "Server finden")) 
		{
			currentMenu = "OnlineServerBrowser";
		}
		//button um zum Netzwerjmenü zurückzukehren
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 50, 100, 20), "zürück")) 
		{
			isThisAOnlineGame = false;
			currentMenu = "NetworkMain";
		}
	}

	//diese Methode zeigt die verfügbaren Server an
	private void onlineServerBrowser()
	{
		refreshHostList();

		//die Liste der regristrierten Host auf dem MasterServer
		hostList = MasterServer.PollHostList();
		//HintergrundBox
		GUI.Box(new Rect(10, 10, Screen.width - 20, Screen.height - 20), "Server Browser");
		if(hostList != null)
		{
			//Infos für die Spalten
			GUI.Label(new Rect(40, 30, 500, 25), "Servername");
			GUI.Label(new Rect(240, 30, 500, 25), "Anzahl Spieler");

			if(GUI.Button(new Rect(400, 30, 100, 25), "Aktuallisieren"))
			{
				refreshHostList();
				//die Liste der regristrierten Host auf dem MasterServer
				hostList = MasterServer.PollHostList();
			}
			
			GUI.Box(new Rect(30, 60, Screen.width - 60, Screen.height - 110), "");
			// Begin the ScrollView
			serverBrowserScrollView = GUI.BeginScrollView (new Rect(30, 60, Screen.width - 60, Screen.height - 110), serverBrowserScrollView, new Rect (0, 0, Screen.width - 90, 1000), false, true);
			//alle nachfolgenden Positionsangaben bis zu EnsScrolView sind relativ zur Scrollview

			//zum testen
			if(hostList.Length == 0)
			{
				for(int i = 0; i < 100; i++)
				{
					if(GUI.Button(new Rect(360, 0 + (30 * i), 100, 25), "Beitreten"))
					{
						joinOnlineServer(hostList[i]);
					}
					GUI.Label(new Rect(10, 0 + (30 * i), 500, 25), "Hier steht ein Name");
					GUI.Label(new Rect(210, 0 + (30 * i), 500, 25), "anzahl der Spieler/8");
				}
			}

			//Liste der gefundenen Servers
			for(int i = 0; i < hostList.Length; i++)
			{
				if(GUI.Button(new Rect(360, 0 + (30 * i), 100, 25), "Beitreten"))
				{
					joinOnlineServer(hostList[i]);
				}
				GUI.Label(new Rect(10, 0 + (30 * i), 500, 25), "" + hostList[i].gameName);
				//im Kommentafeld des Servers steht die Anzahl der Verbundenen Spieler, hostList[i].connections zeigt nur
				//die Zahl der verbundenen Clients an, nicht der eigentlichen SPieler
				GUI.Label(new Rect(210, 0 + (30 * i), 500, 25), hostList[i].comment + "/8");
			}
			GUI.EndScrollView();
		}
		//button um zum Netzwerkmenü zurückzukehren
		if(GUI.Button(new Rect(30, Screen.height - 40, 200, 20), "zürück zum Hauptmenü")) 
		{
			Application.LoadLevel("MainMenuScene");
		}
	}

	private void LANMultiplayer()
	{
		//kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 80, Screen.height/2 - 200, 160, 200), "LAN");
		
		//Button um Online Server zu erstellen
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 150, 100, 20), "Server erstellen"))
		{
			startLANServer();
		}
		//button um Online Server zu suchen
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 100, 100, 20), "Server finden")) 
		{
			currentMenu = "LANConnect";
		}
		//button um zum Netzwerkmenü zurückzukehren
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 50, 100, 20), "zürück")) 
		{
			currentMenu = "NetworkMain";
		}
	}

	private void LANConnectToServer()
	{
		//kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 80, Screen.height/2 - 200, 160, 270), "Verbinde zu LAN-Server");

		//Label für IP
		GUI.Label(new Rect(Screen.width/2 - 50, Screen.height/2 - 150, 100, 20), "IP Addresse");
		//Eingabefeld für IP
		LANIPAddress = GUI.TextField(new Rect(Screen.width/2 - 50, Screen.height/2 - 130, 100, 20), LANIPAddress, 15);

		//label für Port
		GUI.Label(new Rect(Screen.width/2 - 50, Screen.height/2 - 100, 100, 20), "Port Nummer:");
		//Eingabefeld für Port
		LANPort = Convert.ToInt32(GUI.TextField(new Rect(Screen.width/2 - 50, Screen.height/2 - 80, 100, 20), "" + LANPort, 5));

		//Button um Online Server zu erstellen
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 30, 100, 20), "Verbinde"))
		{
			connectToLANServer();
		}
		//button um zum Netzwerkmenü zurückzukehren
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 + 20, 100, 20), "zürück")) 
		{
			currentMenu = "NetworkMain";
		}
	}

	//das Menü, dass die Lobby darstellen soll. Hier sind Sachen drin wie gewählte Strecke, Namen der Spieler, 
	//gewähltes Auto
	private void lobby()
	{
		//HintergrundBox
		GUI.Box(new Rect(10, 10, Screen.width - 20, Screen.height - 20), "Lobby");

		GUI.Box(new Rect(30, 60, Screen.width/2, Screen.height - 110), "");
		//Infos für die Spalten
		GUI.Label(new Rect(40, 30, 500, 25), "Name");
		GUI.Label(new Rect(160, 30, 500, 25), "Gewähltes Auto");
		GUI.Label(new Rect(280, 30, 500, 25), "Bereit");

		//falls es sich um einen LAN SPiel handelt, stelle die IP Addresse dar
		if(Network.isServer == true && isLANGame == true)
		{
			GUI.Label(new Rect(360, 30, 500, 25), "LAN IP: " + LANIPAddress);
			GUI.Label(new Rect(520, 30, 500, 25), "LAN Port: " + LANPort);
			//GUI.Label(new Rect(660, 30, 500, 25), "External IP: " + Network.player.externalIP);
			//GUI.Label(new Rect(880, 30, 500, 250), "External Port: " + Network.player.externalPort);
		}

		//stelle die Infos dar
		int i = 0;
		foreach(NetworkPlayerData player in playerInfos)
		{
			GUI.Label(new Rect(40, 60 + (25 * i), 500, 25), player.GetComponent<NetworkPlayerData>().getPlayerData()[1]);
			GUI.Label(new Rect(160, 60 + (25 * i), 500, 25), player.GetComponent<NetworkPlayerData>().getPlayerData()[2]);
			GUI.Label(new Rect(280, 60 + (25 * i), 500, 25), player.GetComponent<NetworkPlayerData>().getPlayerData()[3]);
			i++;
		}

		//falls wir der Server sind, wähle die das Level aus
		if(Network.isServer == true)
		{
			// kleine Hintergrundbox erstellen
			GUI.Box(new Rect(Screen.width/2 + 40, 60, Screen.width/2 - 60, Screen.height - 110), "Levelauswahl");

			//default Strecke ist ArenaStadium
			PlayerPrefs.SetString("Level","ArenaStadium");

			//erster Button, falls gedrückt, wird das erste Level geladen
			if(GUI.Button(new Rect(Screen.width/2 + 60, 90, 160, 20), "Derby-Arena im Stadium")) 
			{
				PlayerPrefs.SetString("Level","ArenaStadium");
				networkView.RPC("receiveLevelName", RPCMode.Others, PlayerPrefs.GetString("Level"));
			}
			//zweites Level
			if(GUI.Button(new Rect(Screen.width/2 + 60, 120, 160, 20), "Wüsten-Arena")) 
			{
				PlayerPrefs.SetString("Level","DesertArena");
				networkView.RPC("receiveLevelName", RPCMode.Others, PlayerPrefs.GetString("Level"));
			}
			//drittes Level
			if(GUI.Button(new Rect(Screen.width/2 + 60, 150, 160, 20), "Wüsten-Strecke")) 
			{
				PlayerPrefs.SetString("Level","DesertCircuit");
				networkView.RPC("receiveLevelName", RPCMode.Others, PlayerPrefs.GetString("Level"));
			}
			//....
			if(GUI.Button(new Rect(Screen.width/2 + 60, 180, 160, 20), "Stefs Test-Strecke")) 
			{
				PlayerPrefs.SetString("Level","StefTestScene2");
				networkView.RPC("receiveLevelName", RPCMode.Others, PlayerPrefs.GetString("Level"));
			}
		}
		//falls wir Client sind
		if(Network.isClient == true)
		{
			// kleine Hintergrundbox erstellen
			GUI.Box(new Rect(Screen.width/2 + 40, 60, Screen.width/2 - 60, Screen.height - 110), "Level");
			//Levelname anzeigen
			GUI.Label(new Rect(Screen.width/2 + 60, 90, 160, 20), PlayerPrefs.GetString("Level"));
		}

		//button um das Spiel zu starten
		if(GUI.Button(new Rect(Screen.width - 130, Screen.height - 40, 100, 20), "Spiel starten")) 
		{
			startGame();
		}

		//DebugButton
		if(GUI.Button(new Rect(Screen.width - 250, Screen.height - 40, 100, 20), "Debug"))
		{
			this.networkView.RPC("debug",RPCMode.All, "BLUB BLIB");
		}

		//button um zum Netzwerkmenü zurückzukehren
		if(GUI.Button(new Rect(30, Screen.height - 40, 200, 20), "zürück zum Hauptmenü")) 
		{
			//alle Verbindungen schließen
			Network.Disconnect();
			Application.LoadLevel("MainMenuScene");
		}
	}
}
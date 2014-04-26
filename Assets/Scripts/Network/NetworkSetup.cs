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
	//liste mit den Infos der Spieler auf dem Client
	private List<NetworkPlayerData> localPlayerInfos;
	//liste mit den Infos der Spieler auf dem Server
	private List<NetworkPlayerData> serverPlayerInfos;

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

	// Use this for initialization
	void Start ()
	{
		//dieses GameObject soll weiterhin existieren
		DontDestroyOnLoad(this);
		//maximal 8 Spieler
		serverPlayerInfos = new List<NetworkPlayerData>();
		localPlayerInfos = new List<NetworkPlayerData>();
		levelPrefix = 0;
		this.networkView.group = 1;
		//Spiel soll auch im Hintergrund laufen
		Application.runInBackground = true;

		//default Strecke ist Arena
		PlayerPrefs.SetString("Level","ArenaStadium");
	}

	// Update is called once per frame
	void Update ()
	{
		Application.runInBackground = true;
	}

	//diese Method soll vom CarSelectionManager aufgerufen werden, wenn die jeweiligen SPieler ihre Autos gewählt haben
	public void loadLobby()
	{
		//falls der der Server aufgesetzt wurde, gehe zur Lobby
		currentMenu = "Lobby";
		//die Lobby Szene ist dabei eine leere Szene mit einer Kamera, damit man nach einen Rennen wieder zur Lobby wechslen kann
		Application.LoadLevel("MultiplayerLobby");
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
			localPlayerInfos.Add(playerDataOne.GetComponent<NetworkPlayerData>());
		}
		if(PlayerPrefs.GetInt("LocalPlayers") == 2)
		{
			numberOfCurrentsPlayers = 2;
			//PlayerData für Spieler 1
			playerDataOne = (GameObject)Network.Instantiate(playerDataPrefab, this.transform.position, this.transform.rotation, 1);
			playerDataOne.name = "playerDataOne";
			localPlayerInfos.Add(playerDataOne.GetComponent<NetworkPlayerData>());
			
			//PlayerData für Spieler 2
			playerDataTwo = (GameObject)Network.Instantiate(playerDataPrefab, this.transform.position, this.transform.rotation, 1);
			playerDataTwo.name = "playerDataTwo";
			//ConntrolerString muss noch gesetzr werden
			playerDataTwo.GetComponent<NetworkPlayerData>().setControllerString("Two");
			localPlayerInfos.Add(playerDataTwo.GetComponent<NetworkPlayerData>());
		}
	}

	//diese Methode starte einen OnlineServer, in dem der MasterServer kontaktiert wird
	private void startOnlineServer()
	{
		Network.InitializeServer(4,25000,Network.HavePublicAddress());
		//Spielname, Lobbyname (in dem Fall der Name des Spieler 1), Anzahl der Verbunden Spieler (NICHT DER CLIENTS!!, da 2 Spieler
		//pro Client möglich ist)
		MasterServer.RegisterHost("FHTrierLastIgnition", PlayerPrefs.GetString("PlayerOneName"), "" + numberOfCurrentsPlayers);
		isThisAOnlineGame = true;
	}

	private void startGame()
	{
		gameRunning = true;
		Network.RemoveRPCsInGroup(0);
		Network.RemoveRPCsInGroup(1);
		this.networkView.RPC("loadLevel", RPCMode.AllBuffered, PlayerPrefs.GetString("Level"), levelPrefix+1);
		this.networkView.RPC("leavingLobby", RPCMode.AllBuffered);
	}

	//diese Methode aktuallisiert die verfügbaren Server
	void refreshHostList()
	{
		MasterServer.RequestHostList("FHTrierLastIgnition");
	}

	//diese Methode
	private void joinOnlineServer(HostData hd)
	{
		Network.Connect(hd);
		isThisAOnlineGame = true;
	}

	//diese Methoder disconnected vomServer
	public void leaveServer()
	{
		//falls es sich um den Client handelt, sage dem Server bescheid, das wir ihn verlassen und gehe ins Hauptmenü
		if(Network.isClient == true)
		{
			//Anzahl der lokalen (an einen PC) Spieler
			if(PlayerPrefs.GetInt("LocalPlayers") == 1)
			{
				NetworkPlayerData dataOne = GameObject.Find("playerDataOne").GetComponent<NetworkPlayerData>();						
				networkView.RPC("removePlayerInfo",RPCMode.Server, dataOne.getPlayerData()[0], dataOne.getPlayerData()[1], dataOne.getPlayerData()[2], dataOne.getPlayerData()[3]);
			}
			if(PlayerPrefs.GetInt("LocalPlayers") == 2)
			{
				NetworkPlayerData dataOne = GameObject.Find("playerDataOne").GetComponent<NetworkPlayerData>();						
				networkView.RPC("removePlayerInfo",RPCMode.Server, dataOne.getPlayerData()[0], dataOne.getPlayerData()[1], dataOne.getPlayerData()[2], dataOne.getPlayerData()[3]);
				
				NetworkPlayerData dataTwo = GameObject.Find("playerDataTwo").GetComponent<NetworkPlayerData>();						
				networkView.RPC("removePlayerInfo",RPCMode.Server, dataTwo.getPlayerData()[0], dataTwo.getPlayerData()[1], dataTwo.getPlayerData()[2], dataTwo.getPlayerData()[3]);
			}
			//Server soll SpielerInfos aktualliseren
			networkView.RPC("updatePlayersForClients",RPCMode.Server);
		}
		//falls wir Server sind, disconnecte alle 
		if(Network.isServer == true)
		{
			networkView.RPC("clientsLeaveServer",RPCMode.Others);
			for(int i = 0; i < Network.connections.Length; i++)
			{
				Network.CloseConnection(Network.connections[i], true);
			}
		}		
		//schließe die Verbindung
		Network.Disconnect();
		//kehre zum Hauptmenü zurück
		Application.LoadLevel("MainMenuScene");
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

		//instanziere die lokale NetworkPlayerData
		intanciateNetPlayerData();
		//update die PlayerData auf dem Server
		foreach(NetworkPlayerData player in localPlayerInfos)
		{
			this.networkView.RPC("updatePlayerInfo",RPCMode.All, player.getPlayerData()[0], player.getPlayerData()[1], player.getPlayerData()[2], player.getPlayerData()[3]);
		}
		
		//falls der der Server aufgesetzt wurde, gehe zur Lobby
		currentMenu = "Lobby";
		//die Lobby Szene ist dabei eine leere Szene mit einer Kamera, damit man nach einen Rennen wieder zur Lobby wechslen kann
		Application.LoadLevel("MultiplayerLobby");
	}

	//diese Methode wird auf dem Server aufgerufen, wenn sich ein Spieler erfolgreich mit dem Server verbunden hat
	//Called on the server whenever a new player has successfully connected.
	void OnPlayerConnected(NetworkPlayer player)
	{
		this.networkView.RPC("receiveLevelName", RPCMode.AllBuffered, PlayerPrefs.GetString("Level"));
		//Debug.Log("Player " + " connected from " + player.ipAddress);
	}

	//diese Methode wird auf dem Server aufgerufen, wenn sich ein Spieler vom Server getrennt hat
	//Called on the server whenever a player is disconnected from the server.
	void OnPlayerDisconnected()
	{

	}

	//Diese Methode wird beim Client aufgerufen, wenn sich ein Client mit dem Server verbunden hat
	//Called on the client when you have successfully connected to a server.
	void OnConnectedToServer() 
	{
		//sage dem Server bescheid, wie viele Spieler der Client hat
		this.networkView.RPC("updatePlayerNumber",RPCMode.Server, PlayerPrefs.GetInt("LocalPlayers"));

		//instanziere die NetworkPlayerData
		intanciateNetPlayerData();
		//update die PlayerData auf dem Server
		foreach(NetworkPlayerData player in localPlayerInfos)
		{
			this.networkView.RPC("updatePlayerInfo",RPCMode.Server, player.getPlayerData()[0], player.getPlayerData()[1], player.getPlayerData()[2], player.getPlayerData()[3]);
		}
		currentMenu = "Lobby";
		//Server soll spielerinfos für alle aktuallisieren
		networkView.RPC("updatePlayersForClients",RPCMode.Server);
	}

	//diese Methode wird beim verlassen des Servers aufgerufen
	//Called on client during disconnection from server, but also on the server when the connection has disconnected.
	void OnDisconnectedFromServer()
	{

	}

	//Diese Methode wird beim CLient aufgerufen, wenn sich der CLient nicht mit dem Server verbindne konnte
	//Called on the client when a connection attempt fails for some reason.
	void OnFailedToConnect(NetworkConnectionError error)
	{
		//Debug.Log("Could not connect to server: " + error);
		GUI.Label(new Rect(Screen.width/2 - 250, Screen.height/2 + 150, 500, 30), "Konnte nicht mit dem Serer verbinden. Fehler: " + error);
	}

//// RPC METHODEN

	[RPC]
	void loadLevel(string level, int newLevelPrefix)
	{
		StartCoroutine(loadLevelCoroutine(level,newLevelPrefix));
	}
	
	private IEnumerator loadLevelCoroutine(string level, int newLevelPrefix){
		
		levelPrefix = newLevelPrefix;
		
		Network.SetSendingEnabled(0,false);
		
		Network.isMessageQueueRunning = false;
		
		Network.SetLevelPrefix(levelPrefix);
		Application.LoadLevel(level);

		while(Application.isLoadingLevel){
			yield return new WaitForEndOfFrame();
		}
	
		Network.isMessageQueueRunning = true;
		
		Network.SetSendingEnabled(0, true);

		
		foreach( GameObject g in FindObjectsOfType(typeof(GameObject))){
			g.SendMessage("OnNetworkLoadedLevel",SendMessageOptions.DontRequireReceiver);
		}
	}

	[RPC]
	void receiveLevelName(string level)
	{
		PlayerPrefs.SetString("Level",level);
		Debug.Log("RECEIVED");
	}

	//diese Method dient dazu, das die Lobby auf den Clients nicht mehr angezeigt wird
	[RPC]
	void leavingLobby()
	{
		//stelle kein Menü dar
		currentMenu ="Nothing";
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

	//diese Methode aktuallisiert die Spieler Infos, sodass der Server und Clients weiss, wie viele tatsächliche Spieler am Rennen teilnehmen
	[RPC]
	private void updatePlayerInfo(string dataID, string dataName, string dataCar, string dataReady)
	{
		string[] playerData = new string[]{dataID, dataName, dataCar, dataReady};
		foreach(NetworkPlayerData player in serverPlayerInfos)
		{
			//falls die Network ID die selbe ist, ist der Spieler bereits in der Liste und wir müssen die Daten austauschen
			if(player.getPlayerData()[0] == playerData[0])
			{
				player.setAll(playerData);
				//wir könne die suche abbrechen
				return;
			}
		}
		//ansonsten befindet sich der Spieler noch nicht in der Liste
		NetworkPlayerData data = new NetworkPlayerData();
		data.setAll(playerData);
		serverPlayerInfos.Add(data);
	}

	//diese Methode entfert den Spieler aus der Liste, z.B. wenn ein Client den Server verlässt, sollte nur beim Server aufgerufen werden
	[RPC]
	private void removePlayerInfo(string dataID, string dataName, string dataCar, string dataReady)
	{
		string[] playerData = new string[]{dataID, dataName, dataCar, dataReady};
		//index des zu löschenden Spielers
		int index = -1;
		foreach(NetworkPlayerData player in serverPlayerInfos)
		{
			//falls die Network ID die selbe ist, haben wir den Spieler zum löschen gefunden
			if(player.getPlayerData()[0] == playerData[0])
			{
				index = serverPlayerInfos.IndexOf(player);
				//wir könne die suche abbrechen
				break;
			}
		}
		//falls der zu löschende Spieler gefunden wurde
		if(index != -1)
		{
			serverPlayerInfos.RemoveAt(index);
		}
		//CLients sollen SpielerInfos synchronisieren
		networkView.RPC("synchronisePlayersForClients",RPCMode.OthersBuffered);
		//hier muss man nun alle mit dem SPieler verbundenen Objekte löschen
		//TO DO
	}

	//diese Methode sorgt dafür, das auf dem Server und allen Clients die SpielerInfos aktuallisiert werden
	[RPC]
	private void updatePlayersForClients()
	{
		foreach(NetworkPlayerData player in serverPlayerInfos)
		{
			networkView.RPC("updatePlayerInfo",RPCMode.AllBuffered, player.getPlayerData()[0], player.getPlayerData()[1], player.getPlayerData()[2], player.getPlayerData()[3]);
		}
	}

	//diese Methode sorgt dafür, das auf allen Clients die SpielerInfos synchroniosiert werden
	[RPC]
	private void synchronisePlayersForClients()
	{
		//fall es der CLient ist, müsst zunächst die Liste auf dem Client gelöscht werden
		if(Network.isClient == true)
		{
			serverPlayerInfos.Clear();
		}
		//Bitte an der Server, die aktuelle SpielerInfos zu schicken
		networkView.RPC("updatePlayersForClients",RPCMode.Server);
	}

	//diese Methode sorgt dafür, das allen Clients zum Hauptmenü zurückkehren, weil der Server beendet worden ist
	[RPC]
	private void clientsLeaveServer()
	{
		//schließe die Verbindung
		Network.Disconnect();
		//kehre zum Hauptmenü zurück
		Application.LoadLevel("MainMenuScene");
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
			//falls gerade die Fahrzeuge gewählt werden sollen, zeige kein Menü an
			if(currentMenu.Equals("Nothing"))
			{
				//tue nichts
			}
		}
	}

	//"Hauptmenü" des NEtzwerksmenüs
	private void networkMainMenu()
	{
		isThisAOnlineGame = false;
		isLANGame = false;
		//Verbindung schließen
		Network.Disconnect();
		//falls nötig, Host am Master Server abmelden
		MasterServer.UnregisterHost();

		//default Strecke ist ArenaStadium
		PlayerPrefs.SetString("Level","ArenaStadium");

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
		hostList = MasterServer.PollHostList();

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
			//verlasse den Server
			leaveServer();
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

		GUI.Box(new Rect(30, 60, Screen.width/2 + 60, Screen.height - 110), "");
		//Infos für die Spalten
		GUI.Label(new Rect(40, 30, 500, 25), "Name");
		GUI.Label(new Rect(160, 30, 500, 25), "Gewähltes Auto");
		GUI.Label(new Rect(320, 30, 500, 25), "Bereit");

		//falls es sich um einen LAN SPiel handelt, stelle die IP Addresse dar
		if(Network.isServer == true && isLANGame == true)
		{
			GUI.Label(new Rect(400, 30, 500, 25), "LAN IP: " + LANIPAddress);
			GUI.Label(new Rect(580, 30, 500, 25), "LAN Port: " + LANPort);
			//GUI.Label(new Rect(660, 30, 500, 25), "External IP: " + Network.player.externalIP);
			//GUI.Label(new Rect(880, 30, 500, 250), "External Port: " + Network.player.externalPort);
		}

		//stelle die Infos dar
		int i = 0;
		foreach(NetworkPlayerData player in serverPlayerInfos)
		{
			GUI.Label(new Rect(40, 60 + (25 * i), 500, 25), player.getPlayerData()[1]);
			GUI.Label(new Rect(160, 60 + (25 * i), 500, 25), player.getPlayerData()[2]);
			GUI.Label(new Rect(320, 60 + (25 * i), 500, 25), player.getPlayerData()[3]);
			i++;
		}

		//falls wir der Server sind, wähle die das Level aus
		if(Network.isServer == true)
		{
			// kleine Hintergrundbox erstellen
			GUI.Box(new Rect(Screen.width/2 + 100, 60, Screen.width/2 - 120, Screen.height - 110), "Levelauswahl");

			//zeige gewählte Strecke
			GUI.Label(new Rect(Screen.width/2 + 120, 90, 160, 20), "Level: " + PlayerPrefs.GetString("Level"));
			//Derby-Arena Asphalt
			if(GUI.Button(new Rect(Screen.width/2 + 120, 120, 160, 20), "Derby-Arena 1")) 
			{
				PlayerPrefs.SetString("Level","ArenaStadium");
				networkView.RPC("receiveLevelName", RPCMode.Others, PlayerPrefs.GetString("Level"));
			}
			//Derby-Arena Matsch
			if(GUI.Button(new Rect(Screen.width/2 + 120, 150, 160, 20), "Derby-Arena 2")) 
			{
				PlayerPrefs.SetString("Level","ArenaStadium02");
				networkView.RPC("receiveLevelName", RPCMode.Others, PlayerPrefs.GetString("Level"));
			}
			//Wüstensterke 1
			if(GUI.Button(new Rect(Screen.width/2 + 120, 180, 160, 20), "Wüsten-Strecke 1")) 
			{
				PlayerPrefs.SetString("Level","DesertCircuit");
				networkView.RPC("receiveLevelName", RPCMode.Others, PlayerPrefs.GetString("Level"));
			}
			//Wüstenstreke 2
			if(GUI.Button(new Rect(Screen.width/2 + 120, 210, 160, 20), "Wüsten-Strecke 2")) 
			{
				PlayerPrefs.SetString("Level","DesertCircuit02");
				networkView.RPC("receiveLevelName", RPCMode.Others, PlayerPrefs.GetString("Level"));
			}
			//Waldstrecke
			if(GUI.Button(new Rect(Screen.width/2 + 120, 240, 160, 20), "Wald-Strecke")) 
			{
				PlayerPrefs.SetString("Level","ForestCircuit");
				networkView.RPC("receiveLevelName", RPCMode.Others, PlayerPrefs.GetString("Level"));
			}

			//Überprüfe, ob die Spieler bereit sind
			bool playersReady = true;
			//gehe alle Spieler durch
			foreach(NetworkPlayerData player in serverPlayerInfos)
			{
				//getPlayerData leifert ein string Array zurück, an letzer Position steht, ob der spieler bereit ist
				if(player.getPlayerData()[3].Equals("nicht bereit"))
				{
					//falls einer nicht bereit ist, breche ab
					playersReady = false;
					break;
				}
			}

			//falls alle Spieler bereit sind, erlaube es, das Spiel zu starten
			if(playersReady == true)
			{
				//button um das Spiel zu starten
				if(GUI.Button(new Rect(Screen.width - 130, Screen.height - 40, 100, 20), "Spiel starten")) 
				{
					startGame();
				}
			}
			//ansonsten verbiete es
			else
			{
				GUI.Label(new Rect(Screen.width - 150, Screen.height - 40, 150, 20), "Nicht alle bereit");
			}
		}
		//falls wir Client sind, soll das gewählte LEvel angezeigt werden
		if(Network.isClient == true)
		{
			// kleine Hintergrundbox erstellen
			GUI.Box(new Rect(Screen.width/2 + 100, 60, Screen.width/2 - 120, Screen.height - 110), "Level");
			//Levelname anzeigen
			GUI.Label(new Rect(Screen.width/2 + 120, 90, 160, 20), PlayerPrefs.GetString("Level"));
		}

		//Button, um Auto zu wechslen
		//netView.RPC("loadLevel",RPCMode.All,PlayerPrefs.GetString("Level"),2);
		//Button, um bereit zu sein
		if(GUI.Button(new Rect(Screen.width - 350, Screen.height - 40, 100, 20), "Auto wählen"))
		{
			//momentanes "Menü" soll nichts sein
			currentMenu ="Nothing";

			//Server bescheid sagen, dass man nicht bereit ist
			foreach(NetworkPlayerData localPlayer in localPlayerInfos)
			{
				localPlayer.setReady(false);
				this.networkView.RPC("updatePlayerInfo",RPCMode.All, localPlayer.getPlayerData()[0], localPlayer.getPlayerData()[1], localPlayer.getPlayerData()[2], localPlayer.getPlayerData()[3]);
			}
			//lade den CarChooser
			Application.LoadLevel("ChooseCar");
		}

		//überprüfe, ob die lokalen SPieler schon ein Autogewählt haben
		bool localReady = true;
		foreach(NetworkPlayerData localPlayer in localPlayerInfos)
		{
			if(localPlayer.getPlayerData()[2].Equals("nicht gewählt"))
			{
				localReady = false;
			}
		}

		//Spieler sollen nur bereit sein, wen sie ein AUto gewählt haben
		if(localReady == true)
		{
			//Button, um bereit zu sein
			if(GUI.Button(new Rect(Screen.width - 250, Screen.height - 40, 100, 20), "Bereit!"))
			{
				foreach(NetworkPlayerData localPlayer in localPlayerInfos)
				{
					localPlayer.setReady(true);
					this.networkView.RPC("updatePlayerInfo",RPCMode.All, localPlayer.getPlayerData()[0], localPlayer.getPlayerData()[1], localPlayer.getPlayerData()[2], localPlayer.getPlayerData()[3]);
				}
			}
		}
		//button um zum Netzwerkmenü zurückzukehren
		if(GUI.Button(new Rect(30, Screen.height - 40, 200, 20), "zürück zum Hauptmenü")) 
		{
			leaveServer();
		}
	}
}
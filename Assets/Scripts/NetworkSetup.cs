using UnityEngine;
using System.Collections;

public class NetworkSetup : MonoBehaviour
{
	private HostData[] hostList;
	private bool startGame;
	private bool setup;
	private bool levelSelect;

	private int levelPrefix;
	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad(this);
		setup = true;
		startGame = false;
		levelSelect = false;
		levelPrefix = 0;
		this.networkView.group = 1;
		Application.runInBackground = true;
	}

	// Update is called once per frame
	void Update ()
	{
		Application.runInBackground = true;
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


	void testNat(){
		ConnectionTesterStatus status = Network.TestConnectionNAT(true);
		switch(status){
		case ConnectionTesterStatus.Error: 
			Debug.Log("Error");
			break;
		case ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted:
			Debug.Log("Limited NAT Punchthrough Port restricted");
			break;
		case ConnectionTesterStatus.LimitedNATPunchthroughSymmetric:
			Debug.Log("Limited NAT Punchthrough Port Symmetric");
			break;
		case ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone:
			Debug.Log("NAT Punchthrough Adress Restricted Cone");
			break;
		case ConnectionTesterStatus.NATpunchthroughFullCone:
			Debug.Log("NAT Punchthrough Full Cone");
			break;
		case ConnectionTesterStatus.PrivateIPHasNATPunchThrough:
			Debug.Log("Private Ip has NAT Punch Through");
			break;
		case ConnectionTesterStatus.PrivateIPNoNATPunchthrough:
			Debug.Log("Private Ip has NO NAT Punch Through");
			break;
		case ConnectionTesterStatus.PublicIPIsConnectable:
			Debug.Log("Public IP Connectable");
			break;
		case ConnectionTesterStatus.PublicIPNoServerStarted:
			Debug.Log("Public Ip no server started");
			break;
		case ConnectionTesterStatus.PublicIPPortBlocked:
			Debug.Log("Public IP port blocked");
			break;
		case ConnectionTesterStatus.Undetermined:
			Debug.Log("Undetermined");
			break;
		}
	}
	void startServer(){
		testNat();
		Network.InitializeServer(4,25000,Network.HavePublicAddress());
		MasterServer.RegisterHost("TestType","Testgame");
		testNat();
	}

	void refreshHostList(){
		MasterServer.RequestHostList("TestType");
	}

	void joinServer(HostData hd){
		Network.Connect(hd);
		Debug.Log ("Connected");
	}

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	void OnServerInitialized(){
		Debug.Log ("Server initialized");
	}

	void OnPlayerConnected(NetworkPlayer player) {
		Debug.Log("Player " + " connected from " + player.ipAddress);
		this.networkView.RPC("receiveLevelName",RPCMode.AllBuffered,PlayerPrefs.GetString("Level"));
	}

	[RPC]
	void receiveLevelName(string level){
		PlayerPrefs.SetString("Level",level);
		Debug.Log("RECEIVED");
	}

	void OnGUI(){
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
							joinServer(hostList[i]);
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
	}

	private void multiLevelSelection(){
		// kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 100, Screen.height/2 - 200, 200, 300), "Levelauswahl");
		
		//erster Button, falls gedrückt, wird das erste Level geladen
		if(GUI.Button(new Rect(Screen.width/2 - 80, Screen.height/2 - 150, 160, 20), "Derby-Arena im Stadium")) 
		{
			PlayerPrefs.SetString("Level","ArenaStadium");
			startServer();
		}
		//zweites Level
		if(GUI.Button(new Rect(Screen.width/2 - 80, Screen.height/2 - 100, 160, 20), "Wüsten-Arena")) 
		{
			PlayerPrefs.SetString("Level","DesertArena");
			startServer();
		}
		//drittes Level
		if(GUI.Button(new Rect(Screen.width/2 - 80, Screen.height/2 - 50, 160, 20), "Wüsten-Strecke")) 
		{
			PlayerPrefs.SetString("Level","DesertCircuit");
			startServer();
		}
		//....
		if(GUI.Button(new Rect(Screen.width/2 - 80, Screen.height/2, 160, 20), "Stefs Test-Strecke")) 
		{
			PlayerPrefs.SetString("Level","StefTestScene2");
			startServer();
		}
		//button, um zurück zum Hauptmenü zu gehen
		if(GUI.Button(new Rect(Screen.width/2 - 80, Screen.height/2 + 50, 160, 20), "Zurück")) 
		{
			levelSelect = false;
		}
	}
}


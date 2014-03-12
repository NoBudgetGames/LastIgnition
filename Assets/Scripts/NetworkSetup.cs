using UnityEngine;
using System.Collections;

public class NetworkSetup : MonoBehaviour
{
	HostData[] hostList;
	bool startGame;
	bool setup;
	NetworkView netView;
	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad(this);
		setup = true;
		startGame = false;
		netView = this.GetComponent<NetworkView>();
	}

	// Update is called once per frame
	void Update ()
	{

	}

	[RPC]
	void loadLevel(string level){
		Application.LoadLevel(level);
	}

	void startServer(){
		Network.InitializeServer(4,25000,!Network.HavePublicAddress());
		MasterServer.RegisterHost("TestType","Testgame");
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
	}

	void OnGUI(){
		if (!Network.isClient && !Network.isServer && setup)
		{
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				startServer();

			if (GUI.Button(new Rect(400, 100, 250, 100), "Refresh Host List"))
				refreshHostList();

			if(hostList != null){
				for(int i = 0; i<hostList.Length; ++i){
					if (GUI.Button(new Rect(400, 300+i*200, 250, 100), hostList[i].gameName))
						joinServer(hostList[i]);
				}
			}
		}
		if(Network.isServer && Network.connections.Length > 0){
			if (GUI.Button(new Rect(100, 300, 250, 100), "Start Game")){
				netView.RPC("loadLevel",RPCMode.All,"ChooseCar");
			}
		}
	}
}


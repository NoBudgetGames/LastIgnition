using UnityEngine;
using System.Collections;

public class CarSelectionManager : MonoBehaviour
{
	public CarSelection[] selectors;
	public GameObject[] presentators;
	bool[] networkFinished;
	bool rpcSent = false;

	// Use this for initialization
	void Start ()
	{
		if(PlayerPrefs.GetInt("LocalPlayers") == 1){
			GameObject.Destroy(selectors[1].getSelectedCarObject());
			GameObject.Destroy(selectors[1].gameObject);
			CarSelection[] tmp = new CarSelection[1];
			tmp[0] = selectors[0];
			selectors = tmp;
		}

		networkFinished = new bool[Network.connections.Length+1];
		for(int i = 0; i< networkFinished.Length; ++i){
			networkFinished[i] = false;
		}

		this.networkView.group = 0;
	}

	// Update is called once per frame
	void Update ()
	{
		bool allSelected = true;
		for(int i = 0; i<selectors.Length; ++i){
			if(!selectors[i].playerReady){
				allSelected = false;
				break;
			}
		}

		if(allSelected){
			for(int i = 0; i<selectors.Length; ++i){
				PlayerPrefs.SetInt(selectors[i].playerName,selectors[i].getCarTypeIndex());
			}
			//falls es eine Netztwerkverbindung gibt, also um ein Netzwerkspiel handelt
			if(Network.isServer == true || Network.isClient == true)
			{
				if(!rpcSent){
					this.networkView.RPC("finishedSelection",RPCMode.All);
					rpcSent = true;
				}
			}
			//ansonsten ist es ein lokales (SIngleplayer-)Spiel
			else
			{
				Application.LoadLevel(PlayerPrefs.GetString("Level"));
			}
		}

		float axis = 5.0f*Time.deltaTime;
		for(int i = 0; i<selectors.Length; ++i){
			presentators[i].transform.Rotate(presentators[i].transform.up,axis);
			selectors[i].getSelectedCarObject().transform.Rotate(selectors[i].getSelectedCarObject().transform.up,axis);
			axis*=-1;
		}

		//falls Netzwerkspiel und die jeweiligen Spieler ihre Autos gewählt haben
		if(allDoneNetwork() == true)// && (Network.connections.Length > 0))
		{
			//NetworkView netView = GameObject.Find("Network").networkView;
			//netView.RPC("loadLevel",RPCMode.All,PlayerPrefs.GetString("Level"),2);
			NetworkSetup netSet = GameObject.Find("Network").GetComponent<NetworkSetup>();
			netSet.setLobby();
		}
	}

	bool allDoneNetwork(){
		for(int i = 0; i < networkFinished.Length; ++i){
			if(!networkFinished[i])
				return false;
		} 
		return true;
	}

	[RPC]
	void finishedSelection(){
		for(int i = 0; i < networkFinished.Length; ++i){
			if(networkFinished[i] == false){
				networkFinished[i] = true;
				return;
			}
		}
	}
}
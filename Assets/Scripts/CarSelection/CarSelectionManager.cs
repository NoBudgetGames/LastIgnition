using UnityEngine;
using System.Collections;

public class CarSelectionManager : MonoBehaviour
{
	public CarSelection[] selectors;
	public GameObject[] presentators;
	bool[] networkFinished;

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
			//falls es keine Netztwerkverbindung gibt, also um ein lokales (Singleplayer-)Spiel handelt, lade das gewählte Level
			if(Network.isServer == false && Network.isClient == false)
			{
				Application.LoadLevel(PlayerPrefs.GetString("Level"));
			}
			//ansonsten ist es ein Netzwerkspiel, und kehre zur Lobby zurück
			else
			{
				NetworkSetup netSet = GameObject.Find("Network").GetComponent<NetworkSetup>();
				netSet.loadLobby();
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
			//gehe zur Lobby zurück
			NetworkSetup netSet = GameObject.Find("Network").GetComponent<NetworkSetup>();
			if(netSet != null)
			{
				netSet.loadLobby();
			}
		}
	}

	bool allDoneNetwork(){
		for(int i = 0; i < networkFinished.Length; ++i){
			if(!networkFinished[i])
				return false;
		} 
		return true;
	}
}
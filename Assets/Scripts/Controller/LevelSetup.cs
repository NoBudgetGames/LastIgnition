using UnityEngine;
using System.Collections;


/*
 * Das Skript wird für den Netzwerkmodus benötigt um ein reibungsloses Laden des Levels zu gewähren
 * und abzusichern, dass sich alle Spieler im Level befinden bevor wieder Nachrichten gesendet werden.
 * Die beiden übergebenen Gamobjekte sollten möglichst deaktiviert werden, um zu verhindern, dass sie vor
 * dem Skript aufgerufen werden.
 * 
 *
 * 
 * */
public class LevelSetup : MonoBehaviour
{
	public GameObject twoLocalPlayerObject;  //Referenz auf den Controller, der das Spawnen der Spieler übernimmt
	public GameObject modeObject;  //Referenz auf das Objekt, welches die Logik des Spielmodus enthält


	bool[] loadedPlayers; //Spieler die bereits das Level geladen haben (ohne den lokalen Spieler)

	// Use this for initialization
	void Start ()
	{
		loadedPlayers = new bool[Network.connections.Length];
		twoLocalPlayerObject.SetActive(false);
		modeObject.SetActive(false);

	}

	// Update is called once per frame
	void Update ()
	{
		//Wenn alle geladen haben wird der Modus und der Spieler Spawner aktiviert
		if(allPlayersLoaded()){ 
			twoLocalPlayerObject.SetActive(true);
			modeObject.SetActive(true);
			GameObject.Destroy(this.gameObject);
		}
		
	}

	//Wird von einem Spieler zur Benachrichtigung wenn er das Level geladen hat an andere gesendet
	[RPC]
	void notifyLevelLoad(){
		for(int i = 0; i<loadedPlayers.Length;++i){
			if(!loadedPlayers[i]){
				loadedPlayers[i] = true;
				return;
			}
		}
	}

	//Überprüft ob alle Spieler den Level geladen haben
	bool allPlayersLoaded(){
		for(int i = 0; i<loadedPlayers.Length;++i){
			if(!loadedPlayers[i]){
				return false;
			}
		}

		return true;
	}

	//Wird aufgerufen sobald das Level geladen wurde und der Nachrichten Stream wieder aktiv ist
	void OnNetworkLoadedLevel(){
		
		this.networkView.RPC("notifyLevelLoad",RPCMode.OthersBuffered);
	}
	/*
 	void OnGUI(){
		if(GUI.Button(new Rect(Screen.width/2,Screen.height/2,100,100),"Bereit")){
			this.networkView.RPC("notifyLevelLoad",RPCMode.OthersBuffered);
		}
	}
	*/
}


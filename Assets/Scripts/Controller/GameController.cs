using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Diese Klasse initialisiert die Fahrzeuge für den lokalen 2 Spieler "CooP"
 */

public class GameController : MonoBehaviour 
{
	//Startpunkte
	public Transform[] spawnPoints;
	//Referenz auf die Auto Prefab
	public GameObject carPrefab;
	//referenz auf die RearFloatingCamLow Prefab
	public GameObject rearCamLowPrefab;
	//referenz auf die RearFloatingCamHIgh Prefab
	public GameObject rearCamHighPrefab;


	//Liste mit Spielern
	private List<GameObject> playerList;

	//diese Methode instanziert 
	void Start()
	{
		instaciateNewPlayer(spawnPoints[0], "One");
		instaciateNewPlayer(spawnPoints[1]), "Two");
	}

	private void instaciateNewPlayer(Vector3 position, string playerName)
	{
		GameObject player = GameObject.Instantiate(carPrefab);
		GameObject camLow = GameObject.Instantiate(rearCamLowPrefab);
		GameObject camHigh = GameObject.Instantiate(rearCamHighPrefab);
		PlayerInputController input = player.GetComponent<PlayerInputController>();
		if(input != null)
		{
			input.
		}


	}

	private void setCamera(Camera cam, string playerName)
	{
		if(cam != null)
		{
			if(playerName.Equals("One"))
			{
				//(left: float, top: float, width: float, height: float)
				Rect rect = new Rect(0, 0, 1.0f, 0.5f);
			}
			else if(playerName.Equals("Two"))
			{
				Rect rect = new Rect(0, 0.5f, 1.0f, 0.5f);
			}

		}
	}

}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaMode : MonoBehaviour
{
	//Referenz innerhalb der Szene aud den TwoLocalPlayerCOntrolle
	public TwoLocalPlayerGameController control;
	//wurde das Match schon gestartet?
	private bool hasMatchStarted = false;
	//wurde das Match beendet?
	private bool hasMatchFinished = false;
	//Countdown für den Anfang des Matches
	private float countDown = 4.0f;

	List<GameObject> players;
	List<int> lives;
	List<int> ranks;
	bool initialised;
	const int MAX_LIVES = 3;

	// Use this for initialization
	void Start ()
	{
		initialised = false;
		lives = new List<int>();
		ranks = new List<int>();
	}

	// Update is called once per frame
	void Update ()
	{
		if(!initialised){
			players = control.playerList;
			for(int i = 0; i < players.Count; ++i){
				lives.Add(MAX_LIVES);
				ranks.Add(1);
			}	
			updateRanks();
		}

		//falls Match noch nicht gestartet, zähle Countdown runter und freeze Spieler
		if(hasMatchStarted == false)
		{
			//gehe jedes Auto durch
			foreach(GameObject player in players)
			{
				//blokiere alle Bewegungen
				player.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			}
			//zähle den Counter runter
			countDown -= Time.deltaTime;

			//wenn countDown abgelaufen, starte das Match
			if(countDown <= 0.0f)
			{
				hasMatchStarted = true;
				//gehe jedes Auto durch
				foreach(GameObject player in players)
				{
					//blokiere nicht mehr alle Bewegungen
					player.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
				}
			}
		}
					
		if(players.Count == 1){
			PlayerInputController p = players[0].GetComponent<PlayerInputController>();
			Debug.Log("Player " + p.numberOfControllerString + " won the Battle!");

			//deaktiviere den InputController, damit das Auto nicht meh weiterfahren kann
			p.enabled = false;
			//Setze den Throttle vom Auto auf 0 und bremse mit der Handbremse
			Car car = p.gameObject.GetComponent<Car>();
			car.setThrottle(0.0f);
			car.setHandbrake(true);
			hasMatchFinished = true;
		}
			
		for(int i = 0; i<players.Count; ++i){
			Car car = players[i].GetComponent<Car>();
			if(lives[i] > 0 && car.getHealth()<=0.0f){
				lives[i]--;
				PlayerInputController p = players[i].GetComponent<PlayerInputController>();
				if(lives[i] == 0){
					Debug.Log("Player " + p.numberOfControllerString + " eliminated!");
					players.RemoveAt(i);
					lives.RemoveAt(i);
					ranks.RemoveAt(i);
				} else {
					control.reInstanciatePlayer(p.numberOfControllerString);
				}
				updateRanks();
			}
		}
	}

	void updateRanks(){
		int currentLives = MAX_LIVES;
		int rank = 1;
		bool rankChanged = false;
		while(currentLives > 0){
		for(int i = 0; i < players.Count; ++i){
				if(lives[i] == currentLives){
					ranks[i] = rank;
					players[i].GetComponent<PlayerInputController>().hud.rank.text = ""+rank + " place";
					rankChanged = true;
				}
			}
			currentLives--;
			if(rankChanged)
				rank++;

			rankChanged = false;
		}
	}
}
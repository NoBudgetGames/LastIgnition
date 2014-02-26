using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaMode : MonoBehaviour
{
	//Referenz innerhalb der Szene aud den TwoLocalPlayerCOntrolle
	public TwoLocalPlayerGameController control;
	//Referenz innerhald der Szene auf die FinishedCam
	public FinishedRaceCamera finishCam;

	//wurde das Match schon gestartet?
	private bool hasMatchStarted = false;
	//wurde das Match beendet?
	private bool hasMatchFinished = false;
	//Countdown für den Anfang des Matches
	private float countDown = 4.0f;
	//wurden die Cameras schon zerstört?
	private bool camerasDestroyed;
	//countDown zum anzeigen der Ergebnisse
private float finishCountdown = 2.0f;

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
		camerasDestroyed = false;
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

		//falls nur noch ein Spieler übrig ist
		if(players.Count == 1)
		{
			PlayerInputController p = players[0].GetComponent<PlayerInputController>();
			Debug.Log("Player " + p.numberOfControllerString + " won the Battle!");

			//deaktiviere den InputController, damit das Auto nicht meh weiterfahren kann
			p.enabled = false;
			//Setze den Throttle vom Auto auf 0 und bremse mit der Handbremse
			Car car = p.gameObject.GetComponent<Car>();
			car.setThrottle(0.0f);
			car.setHandbrake(true);
			//das Match wurde beendet
			hasMatchFinished = true;
		}

		//falls Match beendet wurde, zähle den finishCountdown runter
		if(hasMatchFinished == true)
		{
			finishCountdown -= Time.deltaTime;
		}
		
		//hier werden die Kameras, die die Autos verfolgen, gelöscht, damit die Ergebnisse dargestellt werden können
		if(camerasDestroyed == false && finishCountdown <0.0f)
		{
			//aktiviere die finish Kamera
			finishCam.activateCamera();
			finishCam.setArenaMode(true);

			//gehe durch alle Spieler durch und zerstöre die überflüssigen Sachen
			foreach(GameObject player in control.playerList)
			{
				//zerstöre die Kamera und das HUD
				GameObject.Destroy(player.GetComponent<PlayerInputController>().cameraCtrl.gameObject);
				GameObject.Destroy(player.GetComponent<PlayerInputController>().hud.gameObject);
			}
			//gehe alle Objekte mit dem Tag MiniMap durch und lösche sie
			GameObject[] minimaps = GameObject.FindGameObjectsWithTag("MiniMap");
			foreach(GameObject obj in minimaps)
			{
				GameObject.Destroy(obj);
			}
			camerasDestroyed = true;
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
					if(players[i].GetComponent<PlayerInputController>().hud != null)
					{
						players[i].GetComponent<PlayerInputController>().hud.rank.text = ""+rank + " place";
					}
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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Diese Klasse ist für den Rundkurs Spielmodus zuständig.
 * Es gibt dabei eine Liste der mit zu durchfahrenden Checkpoints auf der Strecke
 * Sie instanziert auch die Fahrzeuge über den LoacalPlayerController
 * Wenn das Rennen vorbei ist, wird das HUD, der CameraController und die Minimap zerstört und
 * stadtdessen die Ergebnisse dargestellt
 */

public class CircuitRaceMode : MonoBehaviour 
{
	//Anzahl der zu fahrenden Runden
	public int lapsToDrive;
	//Referenz in der Szene auf anfangs Checkpoint, wird nicht durchfahren, dient hautpsächlich zur Positionsbestimmung, solange der 0-te 
	//Checkpoint noch nicht durchfahren wurde
	public Checkpoint firstCheckpoint;
	//Liste mit Referenzen innerhald der Szene auf die CheckPoints, muss in zu fahrender Reihenfolge sein!
	public Checkpoint[] checkpoints;
	//ZweiSpieler Controller, wichtig für die Liste der Spieler, ist eine Referenz innerhalb der Szene
	public TwoLocalPlayerGameController playerCtrl;
	//Referenz innerhald der Szene auf die FinishedRaceCam
	public FinishedRaceCamera finishedCam;
	//Textur für falsche Richtung
	public Texture wrongWayTexture;

	//Liste der Spieler mit dem CircuitModePlayerStats
	private List<CircuitModePlayerStats> playerList;
	//int List mit der Wagenummer des Autos, an erster Stelle steht das Auto, das momentan führt, usw...
	private List<int> playerPosition;
	//Lite mit String Array der vorher ausgeschiedenen (vor der Zieleinfahrt explodierten) Autos
	private List<string[]> explodedPlayerData;
	//timer fürs update der Leaderboard Methode, siehe Update()-Methode
	private float leaderboardTimer = 0.0f;
	//haben alle das Rennen Beendet?
	private bool haveAllFinishedTheRace = false;
	//wurde die Kameras zerstört?
	private bool camerasDestroyed = false;
	//Countdown für start des Spiels
	private float countDown = 4.0f;
	//wurd das Rennen gestartet?
	private bool hasRaceStarted = false;
	//countDown zum anzeigen der SpielerInfos nachdem das Rennen vorbei ist
	private float finishedRaceCountdown = 4.0f;


	// Use this for initialization
	void Start () 
	{
		playerList = new List<CircuitModePlayerStats>();
		explodedPlayerData = new List<string[]>();
		playerPosition = new List<int>();
		
		//gehe alle Checkpoints durch und numeriere sie
		for(int i = 0; i < checkpoints.Length; i++)
		{
			//setzte die Nummer des Checkpoints
			checkpoints[i].setNumber(i);
			//zerstöre die MeshRenderer Komponente, da sie nur im Editor sichtbar ein soll
			GameObject.Destroy(checkpoints[i].GetComponent<MeshRenderer>());
			//zerstöre die Richtungs-Pyramide, da sie nur im Editor sichtbar sein soll
			//es gibt nur ein Child (die Pyramide), daher GetChild(0)
			GameObject.Destroy(checkpoints[i].transform.GetChild(0).gameObject);
		}
		
		//gehe die Playerlist durch und initialisiere die CircuitModePlayerStats
		for(int i = 0; i < playerCtrl.playerList.Count; i++)
		{
			//füge an jeden Player einen CircuitModePlayerStats hinzu
			CircuitModePlayerStats player = playerCtrl.playerList[i].AddComponent("CircuitModePlayerStats") as CircuitModePlayerStats;
			player.circuitMode = this;
			player.setNumberOfCheckpoints(checkpoints.Length);
			player.carNumber = i;
			player.setFirstCheckpoint(firstCheckpoint);
			playerList.Add(player);
			//erstes Auto ist erstmal erster
			playerPosition.Add(i);
		}
	}
	
	void Update()
	{
		//falls das Rennen nicht gestartet wurde, verhindere, das die AUtos sich bewegen
		if(hasRaceStarted == false)
		{
			//gehe jedes Auto durch
			foreach(CircuitModePlayerStats player in playerList)
			{
				//blokiere alle Bewegungen
				player.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			}
			//zähle Timer runter
			countDown -= Time.deltaTime;
			if(countDown <= 0.0f)
			{
				hasRaceStarted = true;
				//gehe jedes Auto durch
				foreach(CircuitModePlayerStats player in playerList)
				{
					//blokiere nicht mehr alle Bewegungen
					player.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
					//starte das Rennen
					player.startRace();
				}
			}
		}

		//da die updateLeaderboard Methode nur alle x Sekunden aufgerufen werden soll, Platzierung muss nicht ständig aktuallisert werden
		//wird hier ein "sleep"Timer verwendet
		leaderboardTimer += Time.deltaTime;
		if(leaderboardTimer >= 3.0f)
		{
			updateLeaderboard();
			leaderboardTimer = 0.0f;
		}
		
		//gehe durch alle Spieler durch und schaue, ob sie das rennen beendet haben
		if(haveAllFinishedTheRace == false)
		{
			foreach(CircuitModePlayerStats player in playerList)
			{
				//falls der Spieler das Rennen nicht beendet hat, stoppe die schleife
				if(player.getHasFinishedRace() == false)
				{
					haveAllFinishedTheRace = false;
					break;
				}
				//ansonsten gehe weiter
				else
				{
					//spieler hat das Rennen beendet, schaue dann weiter zum nächsten
					haveAllFinishedTheRace = true;
				}
			}
		}
		//falls das Rennen beendet wurde, zähle den Countdown runter
		else
		{
			finishedRaceCountdown -= Time.deltaTime;
		}

		//hier werden die Kameras, die die Autos verfolgen, gelöscht, damit die Ergebnisse dargestellt werden können
		//nur wenn das Rennen beendet wurde und der Countdown runterlief
		if(camerasDestroyed == false && finishedRaceCountdown <0.0f)
		{
			//drehe die explodierten Infos um, damit der zuerst explodierte auf dem letzten Platz landet 
			explodedPlayerData.Reverse();
			//übergebe diese Infos der FinishCamera
			foreach(string[] explodedPlayer in explodedPlayerData)
			{
				playerHasFinishedRace(explodedPlayer);
			}

			//aktiviere die finish Kamera
			finishedCam.activateCamera();
			finishedCam.setArenaMode(false);
			//gehe durch alle Spieler durch
			foreach(CircuitModePlayerStats player in playerList)
			{
				//zerstöre die Kamera, das HUD und die CircuitModePlayerStats
				GameObject.Destroy(player.GetComponent<PlayerInputController>().cameraCtrl.gameObject);
				GameObject.Destroy(player.GetComponent<PlayerInputController>().hud.gameObject);
				//circuitModePlayerStats deswegen, weil sonst ein großer WRONG WAY erscheint
				GameObject.Destroy(player.GetComponent<CircuitModePlayerStats>());
			}
			//gehe alle Objekte mit dem Tag MiniMap durch und lösche sie
			GameObject[] minimaps = GameObject.FindGameObjectsWithTag("MiniMap");
			foreach(GameObject obj in minimaps)
			{
				GameObject.Destroy(obj);
			}
			camerasDestroyed = true;
		}
	}

	//diese Methode wird von den CircuitModePlayerStas aufgerufen, sobald das Auto das Rennen beendet hat
	//Dabei werden seine Stats der Liste der FinishCam hinzugefügt
	public void playerHasFinishedRace(string[] data)
	{
		finishedCam.addPlayerData(data);
	}

	public void addExplodedPlayerData(string[] data)
	{
		explodedPlayerData.Add(data);
	}


	//diese Methode aktuallisiert die Positionsanzeige (wer grad erster ist)
	private void updateLeaderboard()
	{
		//falls das Rennen bereits beendet wurde, ist es nicht meht nötig den erster usw. zu finden
		if(haveAllFinishedTheRace == true)
		{
			return;
		}
		//sortieren die Liste nach meiner eigenen sortierfunktion
		playerPosition.Sort(comparePlayerCheckpoints);
		
		//hier müsste man dem HUD bescheidsagen, das er sich aktuallisieren soll
		//eventuell könnte man das als StringArray mit den Spielernamen machen
		for(int i = 0; i < playerPosition.Count; i++)
		{
			Debug.Log (i + ". Pos: " + playerPosition[i]);	
			HUD hud = playerCtrl.playerList[i].GetComponent<PlayerInputController>().hud;
			string postfix;
			switch(playerPosition[i]){
				case 0: postfix="st";
					break;
				case 1: postfix="nd";
					break;
				case 2: postfix="rd";
					break;
				default: postfix="th";
					break;
			}
			hud.rank.text = ""+ (playerPosition[i]+1)+ postfix +" Place";
		}
	}
	
	//diese Methode sortiert die PlayerPosition Liste nach folgenden Kriterien:
	// 1. überprüfe, welches Auto die höhere durchgefahrenen Checkpoints hat
	// 2. falls gleiche CheckpointNummer: welcher davon am nahesten zum nächsten Checkpoint ist
	// liefert -1 falls A vor B ist, 0 falls beide gleichauf sind (sollte nicht passierten), und 1 falls B vor A ist
	private int comparePlayerCheckpoints(int carNumberA, int carNumberB)
	{
		//Auto A aus der Liste
		CircuitModePlayerStats carA = playerList[carNumberA];
		//Auto B aus der Liste
		CircuitModePlayerStats carB = playerList[carNumberB];
		
		//falls A mehr Checkpoints als B hat
		if(carA.getNumberOfDrivenCheckpoints() > carB.getNumberOfDrivenCheckpoints())
		{
			return -1;
		}
		//falls B mehr Checkpoints als A hat
		else if(carA.getNumberOfDrivenCheckpoints() < carB.getNumberOfDrivenCheckpoints())
		{
			return 1;
		}
		//falls beide gleich sind, muss geschaut werden, welcher am nächsten dran zum nächsten Checkpoint ist
		else
		{
			//die nummer des aktuellen Checkpoints. Da beide Autos den selben Checkpoint haben, reicht es beim CarA nachugucken
			int chkNum = carA.getCurrentCheckpointNumber();
			//falls es der letzte Checkpoint ist, muss der 0-ten Checkpoint geprüft werden
			if(chkNum == checkpoints.Length - 1)
			{
				chkNum = 0;
			}
			//ansonsten zähle eins hoch
			else
			{
				chkNum++;
			}
			
			//der nächste CHeckpoint
			Checkpoint chk = checkpoints[chkNum];
			//Abstände zum nächsten Checkpoint
			float distA = chk.distanceToCheckpoint(carA.transform.position);
			float distB = chk.distanceToCheckpoint(carB.transform.position);
			
			//falls A näher am CHeckpoint ist als B
			if(distA < distB)
			{
				return -1;
			}
			//falls B näher am Checkpoint ist
			else if(distA > distB)
			{
				return 1;
			}
			//falls sie den gleichen Abstand haben (sollte hoffentlich nicht passieren)
			else
			{
				return 0;
			}
		}
	}
}
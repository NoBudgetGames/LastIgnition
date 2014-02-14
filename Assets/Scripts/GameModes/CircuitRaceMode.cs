using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Diese Klasse ist für den Rundkurs Spielmodus zuständig.
 * Es gibt dabei eine Liste der mit zu durchfahrenden Checkpoints auf der Strecke
 * Sie instanziert auch die Fahrzeuge über den LoacalPlayerController
 */

public class CircuitRaceMode : MonoBehaviour 
{
	//Anzahl der zu fahrenden Runden
	public int lapsToDrive;
	//Referenz in der Szene auf anfangs Checkpoint, wird nicht durchfahren, dient hautpsächlich zur Positionsbestimmung, solange der 0-te 
	//Checkpoint noch nicht durchfahren wurde
	public Checkpoint firstCheckpoint;
	//Liste mit Referenzen auf die CheckPoints, muss in zu fahrender Reihenfolge sein!
	public Checkpoint[] checkpoints;
	//ZweiSpieler Controller, wichtig für die Liste der Spieler, ist eine Referenz innerhalb der Szene
	public TwoLocalPlayerGameController playerCtrl;

	//Liste der Spieler mit dem CircuitModePlayerStats
	private List<CircuitModePlayerStats> playerList;
	//int List mit der Wagenummer des Autos, an erster Stelle steht das Auto, das momentan führt, usw...
	private List<int> playerPosition;
	//timer fürs update der Leaderboard Methode, siehr Update()-Methode
	private float leaderboardTimer;

	// Use this for initialization
	void Start () 
	{
		playerList = new List<CircuitModePlayerStats>();
		playerPosition = new List<int>();
		leaderboardTimer = 0.0f;

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
		//da die updateLeaderboard Methode nur alle x Sekunden aufgerufen werden soll (wegen Performance)
		//wird hier ein "sleep"Timer verwendet
		leaderboardTimer += Time.deltaTime;
		if(leaderboardTimer >= 3.0f)
		{
			updateLeaderboard();
			leaderboardTimer = 0.0f;
		}

	}

	//diese Methode aktuallisiert die Positionsanzeige (wer grad erster ist)
	private void updateLeaderboard()
	{
		//sortieren die Liste nach meiner eigenen sortierfunktion
		playerPosition.Sort(comparePlayerCheckpoints);

		//hier müsste man dem HUD bescheidsagen, das er sich aktuallisieren soll
		//eventuell könnte man das als StringArray mit den Spielernamen machen
		for(int i = 0; i < playerPosition.Count; i++)
		{
			Debug.Log (i + ". Pos: " + playerPosition[i]);	
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
			//die nummer des aktuellen Checkpoints
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
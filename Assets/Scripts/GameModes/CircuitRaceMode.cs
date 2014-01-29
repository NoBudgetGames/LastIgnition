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
	//Liste der CheckPoints, muss in zu fahrender Reihenfolge sein!
	public Checkpoint[] checkpoints;

	//ZweiSpieler Controller, wichtig für die Lieste der Spieler
	private TwoLocalPlayerGameController playerCtrl;
	//Liste der CheckpointCounter, je eins pro Fahrzeug
	private List<CheckpointCounter> checkpointCounters;
	//int Array mit den momentanen Runde der Fahrzeuge, ist so lang wie die Anzahl der Autos
	//jeder Eintrag ist dabei die Zahl in welcher RUnde sich das i-te Auto befindet (wird auf die PlayerList gemappt)
	//STARTET BEI 1!!
	private int[] lapCounter;

	// Use this for initialization
	void Start () 
	{
		playerCtrl = GetComponent<TwoLocalPlayerGameController>();
		checkpointCounters = new List<CheckpointCounter>();
		lapCounter = new int[playerCtrl.playerList.Count];

		//gehe alle Checkpoints durch und numeriere sie
		for(int i = 0; i < checkpoints.Length; i++)
		{
			//setzte die Nummer des Checkpoints
			checkpoints[i].setNumber(i);
			//zerstöre die MeshRenderer Komponente, da sie nur im Editor sichtbar ein soll
			GameObject.Destroy(checkpoints[i].GetComponent<MeshRenderer>());
			//zerstöre die Richtungs-Pyramide, da sie nur im Editor sichtbar sein soll
			GameObject.Destroy(checkpoints[i].transform.GetChild(0).gameObject);
		}

		//gehe die Playerliste durch und initialisiere die CheckpointCounter
		for(int i = 0; i < playerCtrl.playerList.Count; i++)
		{
			//füge an jeden Player einen CheckpointCounter hinzu
			CheckpointCounter chkCount = playerCtrl.playerList[i].AddComponent("CheckpointCounter") as CheckpointCounter;
			chkCount.circuitMode = this;
			chkCount.numberOfCheckpoints = checkpoints.Length;
			chkCount.carNumber = i;
			chkCount.setFirstCheckpoint(checkpoints[0]);
			checkpointCounters.Add(chkCount);
			lapCounter[i] = 1;
		}
	}

	//diese Methode wird ausgeführt, wenn eins der Autos eine komplette Runde gefahren hat
	public void finishedLap(int numberOfCar)
	{
		//zahle die RUnde für dieses AUto hoch
		lapCounter[numberOfCar] = lapCounter[numberOfCar] + 1;

		//falls es die letzt Runde war
		if(lapCounter[numberOfCar] == lapsToDrive +1)
		{
			playerCtrl.playerList[numberOfCar].GetComponent<PlayerInputController>().enabled = false;

			//Setze den Throttle vom Auto auf 0 und bremse mit der Handbremse
			Car car = playerCtrl.playerList[numberOfCar].GetComponent<Car>();
			car.setThrottle(0.0f);
			car.setHandbrake(true);

			Debug.Log ("NumOfCar " + numberOfCar + " has won the RACE!");
		}
	}
}
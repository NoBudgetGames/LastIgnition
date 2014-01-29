using UnityEngine;
using System.Collections;

/*
 * Diese KLasse "zählt" die durchgefahrenen Checkpoints hoch
 * Sie speichert dabei welchen Checkpoint das Auto zuletzt durchfahren hat,
 * SIe schaut auch, ob das Auto in die falsche Richtung fährt
 * Sie wird für jedes Fahrzeug benötigt
 */

public class CheckpointCounter : MonoBehaviour 
{
	//die Anzahl der Checkpoints
	public int numberOfCheckpoints;
	//Referenz auf den LapRaceMode
	public CircuitRaceMode circuitMode;
	//nummer der Autos (wichtig bei mehreren Spielern)
	public int carNumber;

	//aktueller (zuletzt erfolgreich durchgefahrener) CheckPoint, in Fahrtrichtung
	private Checkpoint currentCheckpoint;
	//timer, der anspringen soll, wenn sich das Auto nach x sekunden in die falsche richtung fährt
	private float directionTimer = 0.0f;

	// Update is called once per frame
	//hier wird überprüft, ob sich das AUto in der falschen Richtung bewegt
	//dazu wird geschaut, ob der Geschwindindigkeitsvektor mit der (Fahrt-)Richtung des Checkpoints übereinstimmt
	void Update () 
	{	
		//überprüfe, ob das AUto in die falsche RIchtung fährt
		bool rightDirection = currentCheckpoint.isDrivingInRightDirection(transform.GetComponent<Rigidbody>().velocity);
		//falls wir in die richtige Richtung fahren, resete den Timer
		if (rightDirection == true)
		{
			directionTimer = 0.0f;
		}
		else
		{
			directionTimer += Time.deltaTime;
		}

		//gucke, ob wir lange genug in die falsche Richtung fahren, um den HUD bescheid zu sagen
		if(directionTimer > 2.0f)
		{
			Debug.Log ("WRONG DIRECTION DUDE! TURN AROUND!!!!");
		}
	}

	//der erste Checkpoint wird gesetzt
	public void setFirstCheckpoint(Checkpoint chk)
	{
		currentCheckpoint = chk;
	}

	//diese Methode überprüft, ob das Auto den richtigen Checkpoint abgefahren hat
	public void updateCheckpoint(Checkpoint chkPoint)
	{
		//falls wir den richtigen Checkpoint abgefahren haben, setze den aktuellen Checkpoint
		if(currentCheckpoint.checkpointNumber + 1 == chkPoint.checkpointNumber)
		{
			currentCheckpoint = chkPoint;
		}
		//falls es die Checkpointmummer 0 ist (Startcheckpoint) und wir vom letzten gekommen sind
		else if(chkPoint.checkpointNumber == 0 && currentCheckpoint.checkpointNumber == numberOfCheckpoints - 1)
		{
			circuitMode.finishedLap(carNumber);
			currentCheckpoint = chkPoint;
		}
	}
}
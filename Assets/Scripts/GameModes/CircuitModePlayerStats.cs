using UnityEngine;
using System.Collections;

/*
 * Diese KLasse enthält Infos über den spieler, wie z.B. die Rundenzeit oder die anzahl an durchgefahrenen
 * Checkpoints. Sie speichert welchen Checkpoint das Auto zuletzt durchfahren hat.
 * Sie schaut auch, ob das Auto in die falsche Richtung fährt
 * Sie wird für jedes Fahrzeug benötigt, aber nur, wenn es sich um einen Rundkurs handelt (das macht der 
 * CircuitRaceMode).
 * Ursprünglich war sie nur ein Checkpoint Zähler.
 */

public class CircuitModePlayerStats : MonoBehaviour 
{
	//Referenz auf den LapRaceMode
	public CircuitRaceMode circuitMode;
	//nummer der Autos (wichtig bei mehreren Spielern)
	public int carNumber;
	
	//aktueller (zuletzt erfolgreich durchgefahrener) CheckPoint, in Fahrtrichtung
	private Checkpoint currentCheckpoint;
	//die Anzahl der Checkpoints
	private int numberOfCheckpoints;
	//timer, der anspringen soll, wenn sich das Auto nach x sekunden in die falsche richtung fährt
	private float directionTimer = 0.0f;
	//wurde das rennen gestertet?
	private bool hasRaceStarted = false;
	//hat das Auto das Rennen beendet?
	private bool hasFinishedRace = false;
	//aktuelle Rundenzeit
	private float lapTime;
	//gesamtzeit
	private float totalTime;
	//schnellste Runde
	private float fastestLap;
	//anzahl durchgefahrerer Checkpoints
	private int numberOfDrivenCheckpoints = 0;
	//aktuell zu durchfahrende RUnde
	private int currentLapToDrive = 1;
	
	void Start()
	{
		hasRaceStarted = true;
		lapTime = 0.0f;
		totalTime = 0.0f;
		fastestLap = -1.0f;
	}
	
	// Update is called once per frame
	//hier wird überprüft, ob sich das AUto in der falschen Richtung bewegt
	//dazu wird geschaut, ob der Geschwindindigkeitsvektor mit der (Fahrt-)Richtung des Checkpoints übereinstimmt
	void Update () 
	{	
		//zähle die Rundenzeit hoch, nur, wenn das Rennen noch nicht beendet wurde und es gestarte wurde
		if(hasRaceStarted == true && hasFinishedRace ==  false)
		{
			lapTime += Time.deltaTime;
			totalTime += Time.deltaTime;
		}
		
		//Debug.Log("LatTime " + lapTime + " TotalTime " + totalTime + " FastestLap " + fastestLap);
		
		//HUD bescheidsagen, das er die Rundenzeit aktuellisieren soll
		
		//überprüfe, ob das AUto in die falsche RIchtung fährt
		//falls wir in die richtige Richtung fahren, resete den Timer
		if (currentCheckpoint.isDrivingInRightDirection(transform.GetComponent<Rigidbody>().velocity))
		{
			directionTimer = 0.0f;
		}
		//ansonsten zähle den Timer hoch
		else
		{
			directionTimer += Time.deltaTime;
		}
		
		//gucke, ob wir lange genug in die falsche Richtung fahren, um den HUD bescheid zu sagen
		if(directionTimer > 2.0f)
		{
			Debug.Log ("WRONG DIRECTION DUDE! TURN AROUND!1!!");
		}
	}
	
	//die Methode sagt dem Auto quasi Bescheid, dass das Rennen jetzt startet
	public void startRace()
	{
		hasRaceStarted = true;
	}
	
	//der erste Checkpoint wird gesetzt
	public void setFirstCheckpoint(Checkpoint chk)
	{
		currentCheckpoint = chk;
	}
	
	//die Anzahl der CHeckpoiints wird gesetzt
	public void setNumberOfCheckpoints(int chkNum)
	{
		numberOfCheckpoints = chkNum;
	}
	
	//die Methode gibt die Anzahl der durchgefahrenen Checkpoints zurück
	public int getNumberOfDrivenCheckpoints()
	{
		return numberOfDrivenCheckpoints;
	}
	
	//gibt die Nummer des momentanens Checkpoint zurück
	public int getCurrentCheckpointNumber()
	{
		return currentCheckpoint.checkpointNumber;
	}
	
	//liefert den aktuellen Checkpoint zurück
	public Checkpoint getCurrentCheckpoint()
	{
		return currentCheckpoint;
	}

	//liefert einen bool zurück, ob das Auto das Rennen bereits beendet hat oder nicht
	public bool getHasFinishedRace()
	{
		return hasFinishedRace;
	}

	//diese Methode wird aufgerufen, sobald eine Runde zu ende gefahren wurde
	private void finishedLap()
	{
		//zahle die Runde für dieses Auto hoch
		currentLapToDrive++;
		
		//aktuellisiere die schnellste Runde
		if(lapTime < fastestLap)
		{
			fastestLap = lapTime;
		}
		//fall es die erste Runde ist
		if(fastestLap == -1.0f)
		{
			fastestLap = lapTime;
		}
		
		//resete denRundenzähler
		lapTime = 0.0f;
		
		//falls es die letzt Runde war
		if(currentLapToDrive == circuitMode.lapsToDrive +1)
		{
			//deaktiviere den InputController
			transform.GetComponent<PlayerInputController>().enabled = false;
			
			//Setze den Throttle vom Auto auf 0 und bremse mit der Handbremse
			Car car = transform.GetComponent<Car>();
			car.setThrottle(0.0f);
			car.setHandbrake(true);
			
			//das Rennden wurde beendet
			hasFinishedRace = true;
			//übergebe die PlayerStats
			string[] str = new string[]{transform.GetComponent<PlayerInputController>().numberOfControllerString,"" + totalTime, "" + fastestLap};
			circuitMode.playerHasFinishedRace(str);
		}
	}
	
	//diese Methode überprüft, ob das Auto den richtigen Checkpoint abgefahren hat
	public void updateCheckpoint(Checkpoint chkPoint)
	{
		//falls wir den richtigen Checkpoint abgefahren haben, setze den aktuellen Checkpoint
		if(currentCheckpoint.checkpointNumber + 1 == chkPoint.checkpointNumber)
		{
			currentCheckpoint = chkPoint;
			numberOfDrivenCheckpoints++;
		}
		//falls es die Checkpointmummer 0 ist (Startcheckpoint) und wir vom letzten gekommen sind
		else if(chkPoint.checkpointNumber == 0 && currentCheckpoint.checkpointNumber == numberOfCheckpoints - 1)
		{
			finishedLap();
			currentCheckpoint = chkPoint;
		}
	}
}
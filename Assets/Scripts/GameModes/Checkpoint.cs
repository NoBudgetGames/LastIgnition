using UnityEngine;
using System.Collections;

/*
 * Diese Klasse stellt einen Checkpoint dar
 * Sie hat einen Trigger, der der den CheckpointCounter hochzählt
 * Außrderm besitzt der Checkpoint an sich auch noch eine Richtung
 * um festzustellen, ob das Auto in die Falsche RIchtung fährt.
 * DIe RIchtung ist dabei immer die y-Komponente im LocalSpace (die Prefab ist 
 * enstprechend mit einer Pyramide gekennzeichnet)
 * BITTE BEIM SETZTEN IN DIE SZENE DIE CHEKPOINTNUMMER IM NAMEN ÄNDERN
 */

public class Checkpoint : MonoBehaviour 
{
	//die Nummer des Checkpoints, wird durch den CircuitRaceMode Script gesetzt
	public int checkpointNumber;

	//setzt den erten Checkpoint
	public void setNumber(int num)
	{
		checkpointNumber = num;
	}

	//wird ausgeführt, sobald ein Collider durchfährt,
	//hier muss geschaut werden, ob der Collider einen Checkpointcouter hat und ihn wenn ja,
	//ihn "hochzählen"
	void OnTriggerEnter(Collider other)
	{
		//gucke auf die Wurzel des other, da sich dort der CheckpointCounter befindet, das das Auto keinen Parent hat
		//wird allerdings für jeden DestructibleCarPart aufgerufen....
		CircuitModePlayerStats chkPointCount = other.gameObject.transform.root.GetComponent<CircuitModePlayerStats>();
		if(chkPointCount != null)
		{
			chkPointCount.updateCheckpoint(this);
		}
	}

	//diese Methode überprüft, ob sich das Auto in die falsche Richtung bewegt (vom checkpoint weg)
	public bool isDrivingInRightDirection(Vector3 carVelocity)
	{
		//falls die Geschwindigkeit des Autos mit der RIchtung des Checkpoints übereinstimmt
		if(transform.InverseTransformDirection(carVelocity).y >= 0.0f)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	//diese Methode errechnet den Abstand vom Auto zum Checkpoint (parallel zur Straße)
	public float distanceToCheckpoint(Vector3 carPos)
	{
		//erreche position des Autos zum Checkpoint in localspace des CHeckpoints um,
		//hier ist nur der y-Wert interresant, da er den Abstand zum Checkpoint parallel zur Straße darstellt
		//da die Autos aus der negative Achse kommen, muss hier ,it - 1 multipliziert werden, damit der Abstand auch korrekt ist
		return -transform.InverseTransformDirection(carPos).y;
	}
}
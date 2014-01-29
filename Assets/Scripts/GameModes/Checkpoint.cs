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
		CheckpointCounter chkPointCount = other.gameObject.transform.root.GetComponent<CheckpointCounter>();
		if(chkPointCount != null)
		{
			chkPointCount.updateCheckpoint(this);
		}
	}

	public bool isDrivingInRightDirection(Vector3 carVelocity)
	{
		//falls die Geschwindigkeit des Autos mit der RIchtung des Checkpoints übereinstimmt
		if(transform.InverseTransformDirection(carVelocity).y > 0.0f)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
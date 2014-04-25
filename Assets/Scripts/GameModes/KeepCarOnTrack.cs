using UnityEngine;
using System.Collections;

/*
 * Diese Klasse verhindert, dass AUto sich zu weit von der Straße entfernt
 * Sollte es von der Straße abgekommen sein, so wird es an seinen zuletzt durchgefahrenen Checkpoint zurückgesetzt
 */

public class KeepCarOnTrack : MonoBehaviour 
{
	//solange der Spieler sich im Trigger befindet, wird der Timer hochgezählt
	void OnTriggerEnter(Collider other)
	{
		//falls es sich um einen Auto handelt, CircuitModePlayerStats befinden sich unterhalb dem Root-Object
		CircuitModePlayerStats player = other.gameObject.transform.root.GetComponentInChildren<CircuitModePlayerStats>();
		if(player != null)
		{
			//neue resetPosition soll der zuletzt durchgefahrene Checkpoitn sein
			player.transform.root.gameObject.GetComponent<Car>().setResetPosition(player.getCurrentCheckpoint().gameObject);
			//resete das Auto
			player.transform.root.gameObject.GetComponent<Car>().resetCar(true);
			//setzte die resetPositionzurück
			player.transform.root.gameObject.GetComponent<Car>().setResetPosition(player.gameObject);
		}
	}
}
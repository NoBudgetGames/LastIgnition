using UnityEngine;
using System.Collections;

/*
 * Diese Klasse stellt eine Spawnzone dar. Sie besitzt einen Trigger um festzustellen, ob ein Fahrzeug
 * gerade dort spawnen kann.
 */

public class SpawnZone : MonoBehaviour 
{
	//kann man diese Zone zum Spawnen benutzen?
	private bool canSpawn;
	//Anzahl der Fahrzeuge, die sich in der Zone befinden
	int numberOfCars = 0;

	void Awake() 
	{
		canSpawn = true;
	}
	
	public bool isSpawnZoneFree()
	{
		return canSpawn;
	}

	//setzt canSpaen immer auf false, sodass andere Autos nicht mehr Spawnen können
	public void spawnIsNotFree()
	{
		canSpawn = false;
	}

	void OnTriggerEnter(Collider other)
	{
		Car car = other.gameObject.transform.root.GetComponent<Car>();
		//falls other eine Car Komponente besitzt, ist dass Spawnen in dieser Zone nicht erlaubt
		if(car != null)
		{
			canSpawn = false;
			numberOfCars++;
		}
	}

	void OnTriggerExit(Collider other)
	{
		Car car = other.gameObject.transform.root.GetComponent<Car>();
		//falls other eine Car Komponente besitzt, ist dass Spawnen in dieser Zone wieder erlaubt
		if(car != null)
		{
			numberOfCars--;
			if(numberOfCars == 0)
			{
				canSpawn = true;
			}
		}
	}

	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info){
		bool canSpawnSerial = false;
		if(stream.isWriting){
			canSpawnSerial = canSpawn;
			stream.Serialize(ref canSpawnSerial);
		} else {
			stream.Serialize(ref canSpawnSerial);
			canSpawn = canSpawnSerial;
		}
	}
}
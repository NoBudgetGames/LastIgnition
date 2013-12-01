using UnityEngine;
using System.Collections;

public class CarCollision : MonoBehaviour
{
	//referenz auf dieses Auto
	private Car car;

	void Start()
	{
		car = transform.parent.gameObject.GetComponent<Car>();
	}

	//Diese Methode soll nur aufgerufen werden, wenn das Auto mit anderen Objecten Kollidiert, 
	//Waffen werden von diesen Trigger nicht erkannt, da sie in einer anderen Layer liegen (layer-base collision detection)
	void OnTriggerEnter(Collider other)
	{
		//falls other ein DestructibleCarPart hat
		if(other.GetComponent<DestructibleCarPart>())
		{
			Vector3 relVelocityVec = car.GetComponent<Rigidbody>().velocity - other.GetComponent<DestructibleCarPart>().car.GetComponent<Rigidbody>().velocity;
			float relVelocity = relVelocityVec.magnitude;

			if(relVelocity >10.0f)
			{
				//schaden wird mit relativer Geschwindigkeit zum anderen Objekt multipliziert
				AbstractDestructibleObject destr = other.GetComponent<AbstractDestructibleObject>();
				if(destr != null)
				{
					destr.receiveDamage(car.crashDamage * relVelocity);
				}
			}
		}
	}
}


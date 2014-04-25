using UnityEngine;
using System.Collections;

/*
 * Diese Klasse dient um bei einer Kollision zwischen zwei Autos Schaden zuzufügen.
 * Dabei wird die die Geschwindigkeit des Autos als Schadensmultiplikator benutzt.
 * Sie wird als Script-Komponente an ein Car-Objekt hinzugefügt.
 * Im GameObject selber besitzt das GameObject noch einen MeshCollider - der als Trigger
 * in der CarCollision-Layer liegt - verwendet, um nicht mit dem richtigen Kollisionmehs des
 * Autos und den Waffen zu kollidieren
 */

public class CarCollision : MonoBehaviour
{
	//referenz auf dieses Auto
	private Car car;
	public GameObject virtualCrashPrefab;

	void Start()
	{
		//Car Komponente ist in der Hierachie eins weiter oben
		car = transform.parent.gameObject.GetComponent<Car>();

	}

	//Diese Methode soll nur aufgerufen werden, wenn das Auto mit anderen Objecten Kollidiert, 
	//Waffen werden von diesen Trigger nicht erkannt, da sie in einer anderen Layer liegen (layer-base collision detection)
	void OnTriggerEnter(Collider other)
	{
		if(car.GetComponent<Rigidbody>().velocity.magnitude > 10.0f){
			if(Network.connections.Length == 0){
				GameObject.Instantiate(virtualCrashPrefab,this.transform.position,this.transform.rotation);
			} else {
				Network.Instantiate(virtualCrashPrefab,this.transform.position,this.transform.rotation,0);
			}
		}
		//falls other ein DestructibleCarPart hat
		if(other.GetComponent<DestructibleCarPart>())
		{
			if(car.GetComponent<Rigidbody>().velocity.magnitude > 10.0f)
			{
				//schaden wird mit aktueller Geschwindigkeit multipliziert
				AbstractDestructibleObject destr = other.GetComponent<AbstractDestructibleObject>();
				if(destr != null)
				{
					//relative Geschwindigkeit der beiden
					Vector3 relVel = car.GetComponent<Rigidbody>().velocity - other.transform.root.GetComponent<Rigidbody>().velocity;
					destr.receiveDamage(car.crashDamage * relVel.magnitude);
				}
			}
		}
	}
}
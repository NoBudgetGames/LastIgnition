using UnityEngine;
using System.Collections;

/*
 * Diese KLasse ist für die Straßenschilder gedacht, damit sie, sobald etwas sie berüht, die Gravitation auch aktiviert wird.
 * Das hat den Hintergrund, das die Straßenschilder sonst sofeort umfallen, wenn sie an einer schrägen Fläche stehen.
 * Besonderheit beim setzten der Schilder: die Colider dürfen niemals des Boden berühren, da die Schilder sonst mit dem Boden 
 * kollidieren und schweben bzw. "wegfliegen"
 */

public class ActivateGravityOnHit : MonoBehaviour
{
	void OnCollisionEnter(Collision collision)
	{
		//die Schilder sollen nur umfallen, wenn sie ein Auto getroffen hat
		Car car = collision.gameObject.transform.root.GetComponent<Car>();
		if(car != null)
		{
			//Rigibdody ist auf der selben Ebene wie das Script
			Rigidbody rigid = transform.GetComponent<Rigidbody>();
			if(rigid != null)
			{
				rigid.useGravity = true;
			}
		}
	}
}
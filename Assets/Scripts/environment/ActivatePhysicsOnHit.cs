using UnityEngine;
using System.Collections;

/*
 * Diese KLasse ist für die Katken gedacht, damit sie, sobald etwas sie berüht, die Physik auch "aktiviert wird"
 * Das hat den Hintergrund, das der Kaktus aus mehreren Einzeiteilen besteht ud damit der Kaktus nicht gleich am Anfang kaputt
 * geht, wird im Rigidbody der Einzelteil alle Constrains aktiviert.
 * Sie wird am Collider der Einzelteile angehängt
 */

public class ActivatePhysicsOnHit : MonoBehaviour 
{
	void OnCollisionEnter(Collision collision)
	{
		//das Rigisbody der Einzelteile ist immer in der Hierachier eins weiter oben
		Rigidbody rigid = transform.parent.gameObject.AddComponent<Rigidbody>();
		rigid.mass = 10.0f;
		rigid.drag = 0.5f;
	}
}
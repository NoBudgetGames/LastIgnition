using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour
{
	Vector3 forwardVector; //Richtung in der die Rakete fliegen soll
	public GameObject parent; //Gameobjekt von dem die Rakete ausgeht
	
	float damage; //Schaden der durch sie verursacht wird
	float radius; //Radius in dem Schadn verursacht wird
	float force; //Die Kraft mit der Objekte zurückgestoßen werden
	float speed; //Die Geschwindigkeit mit der die Rakete sich fortbewegt
	// Use this for initialization
	void Start ()
	{
		forwardVector = parent.transform.forward;
		
		damage = 40.0f;
		radius = 10.0f;
		force = 7000.0f;
		speed = 200.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Simpler gerader Bahnverlauf
		transform.position += forwardVector*Time.deltaTime*speed;
	}
	
	void OnTriggerEnter(Collider other){
		//Rakete zündet nicht beim Parent Objekt
		if(other.gameObject == parent)
			return;
		//Von der Explosionsposition der Rakete aus werden mittels einer Sphere alle Objekte
		//im Explosionsradius erfasst. Objekte die zerstörbar sind erhalten Schaden, Objekte 
		//mit einem Rigidbody werden zurückgeworfen (abhängig von der Masse)
		Vector3 explosionPosition = transform.position;
		Collider[] affectedObjects = Physics.OverlapSphere(explosionPosition,radius);
		foreach(Collider col in affectedObjects){
			if(col.rigidbody)
				col.rigidbody.AddExplosionForce(force,explosionPosition,radius);
			DestructibleObject destr = col.GetComponent<DestructibleObject>();
			if(destr != null)
				destr.receiveDamage(damage);
		}
		GameObject.Destroy(this.gameObject);
		
	}
}


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

	public GameObject explosionPrefab;

	//timer, um die Rakete, falls sie nichts treffen sollte, zu zerstören
	private float destroyTimer = 10.0f;

	// Use this for initialization
	void Start ()
	{
		forwardVector = parent.transform.forward;
		
		damage = 7.5f;
		radius = 20.0f;
		force = 500000.0f;
		speed = 250.0f;
	}

	void FixedUpdate ()
	{
		//Simpler gerader Bahnverlauf
		transform.position += forwardVector*Time.deltaTime*speed;
		//this.rigidbody.AddForce(forwardVector*speed,ForceMode.Acceleration);

		destroyTimer -= Time.deltaTime;
		//falls der Timer abgelaufen ist, zerstöre die Rakete
		if(destroyTimer < 0.0f)
		{
			GameObject.Destroy(gameObject);
		}
	}
	/*
	void OnTriggerEnter(Collider other){
		if(other.GetComponent<Checkpoint>())
			return;
		//falls wir eine SpawnZone getroffen haben, mache nichts
		if(other.GetComponent<SpawnZone>())
		{
			return;
		}
		if(parent.transform.root == other.transform.root){
			return;
		}
	
		GameObject.Instantiate(explosionPrefab,this.transform.position,this.transform.rotation);
		//Von der Explosionsposition der Rakete aus werden mittels einer Sphere alle Objekte
		//im Explosionsradius erfasst. Objekte die zerstörbar sind erhalten Schaden, Objekte 
		//mit einem Rigidbody werden zurückgeworfen (abhängig von der Masse)
		Vector3 explosionPosition = transform.position;
		Collider[] affectedObjects = Physics.OverlapSphere(explosionPosition,radius);
		foreach(Collider col in affectedObjects){
			if(col.rigidbody)
				col.rigidbody.AddExplosionForce(force,explosionPosition,radius);
			AbstractDestructibleObject destr = col.GetComponent<AbstractDestructibleObject>();
			if(destr != null)
			{
				destr.receiveDamage(damage);
				if(Network.connections.Length > 0){
					col.networkView.RPC("receiveDamage",col.networkView.owner,damage);
				}
				if(col.GetComponent<DestructibleCarPart>())
				{
					col.GetComponent<DestructibleCarPart>().car.rigidbody.AddExplosionForce(force * 2,explosionPosition,radius);
				}
			}				
		}
		GameObject.Destroy(this.gameObject);
	}
*/
	void OnCollisionEnter(Collision collision) {
		if(collision.collider.GetComponent<Checkpoint>())
			return;
		//falls wir eine SpawnZone getroffen haben, mache nichts
		if(collision.collider.GetComponent<SpawnZone>())
		{
			return;
		}
		if(parent.transform.root == collision.collider.transform.root){
			return;
		}
		
		GameObject.Instantiate(explosionPrefab,this.transform.position,this.transform.rotation);
		//Von der Explosionsposition der Rakete aus werden mittels einer Sphere alle Objekte
		//im Explosionsradius erfasst. Objekte die zerstörbar sind erhalten Schaden, Objekte 
		//mit einem Rigidbody werden zurückgeworfen (abhängig von der Masse)
		Vector3 explosionPosition = transform.position;
		Collider[] affectedObjects = Physics.OverlapSphere(explosionPosition,radius);
		foreach(Collider col in affectedObjects){
			if(col.rigidbody)
				col.rigidbody.AddExplosionForce(force,explosionPosition,radius);
			AbstractDestructibleObject destr = col.GetComponent<AbstractDestructibleObject>();
			if(destr != null)
			{
				destr.receiveDamage(damage);
				if(Network.connections.Length > 0){
					col.networkView.RPC("receiveDamage",col.networkView.owner,damage);
				}
				if(col.GetComponent<DestructibleCarPart>())
				{
					col.GetComponent<DestructibleCarPart>().car.rigidbody.AddExplosionForce(force * 2,explosionPosition,radius);
				}
			}				
		}
		GameObject.Destroy(this.gameObject);
	}
}
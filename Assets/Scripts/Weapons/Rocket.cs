using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour
{
	Vector3 forwardVector;
	public GameObject parent;
	
	float damage;
	float radius;
	float force;
	float speed;
	// Use this for initialization
	void Start ()
	{
		forwardVector = GameObject.FindGameObjectWithTag("Player").transform.forward;
		
		damage = 40.0f;
		radius = 10.0f;
		force = 700.0f;
		speed = 200.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.position += forwardVector*Time.deltaTime*speed;
	}
	
	void OnTriggerEnter(Collider other){
		if(other.gameObject == parent)
			return;
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


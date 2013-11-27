using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour
{

		float damage; //Schaden der durch sie verursacht wird
		float radius; //Radius in dem Schadn verursacht wird
		float force; //Die Kraft mit der Objekte zurückgestoßen werden

		// Use this for initialization
		void Start ()
		{
			damage = 40.0f;
			radius = 10.0f;
			force = 7000.0f;
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		void OnTriggerEnter(Collider other){
			
			Vector3 explosionPosition = transform.position;
			Collider[] affectedObjects = Physics.OverlapSphere(explosionPosition,radius);
			foreach(Collider col in affectedObjects){
				if(col.rigidbody)
					col.rigidbody.AddExplosionForce(force,explosionPosition,radius);
				AbstractDestructibleObject destr = col.GetComponent<AbstractDestructibleObject>();
				if(destr != null)
				{
					destr.receiveDamage(damage);
					if(col.GetComponent<DestructibleCarPart>())
					{
						col.GetComponent<DestructibleCarPart>().car.rigidbody.AddExplosionForce(force * 2,explosionPosition,radius);
					}
					GameObject.Destroy(this.gameObject);
				}	
			}
		}
}

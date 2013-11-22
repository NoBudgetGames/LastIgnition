using UnityEngine;
using System.Collections;

public class Spike : MonoBehaviour
{
	float damage;
	float speed;
	float force;
	float maxDistance;
	public Vector3 startingPosition;
	public GameObject parent;
	public int spikeNumber;
	
	float elapsedTime;
	Quaternion startingRotation;
	// Use this for initialization
	void Start ()
	{
		damage = 20.0f;
		speed = 10.0f;
		force = 1500.0f;
		maxDistance = 10.0f;
		elapsedTime = 0.0f;
		startingRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update ()
	{
		elapsedTime += Time.deltaTime;
		if(elapsedTime < 5.0f){
			this.transform.position = parent.transform.position +this.transform.up * maxDistance;
			//this.transform.rotation = startingRotation;
			if(spikeNumber == 1){
				this.transform.Rotate(parent.transform.up,+10.0f,Space.World);
			} else {
				this.transform.Rotate(parent.transform.up,+10.0f,Space.World);
			}
			//this.transform.position += (this.transform.up * speed * elapsedTime);
		} else {
			GameObject.Destroy(this.gameObject);
		}
	}
	
	void OnTriggerEnter(Collider other){
		AbstractDestructibleObject  obj = other.GetComponent<AbstractDestructibleObject >();
		if(obj != null && other.gameObject != parent){
			obj.receiveDamage(damage);
			//falls es ein Auto ist muss über die DestructibleCarPart auf den Rigidbody zugegriffen werden
			if(obj.GetComponent<DestructibleCarPart>())
			{
				obj.GetComponent<DestructibleCarPart>().car.rigidbody.AddForce(this.transform.up * force);
			}
			//ansonsten ganz normal
			else
			{
				obj.rigidbody.AddForce(this.transform.up*force);	
			}

			//GameObject.Destroy(this.gameObject);
		}
	}
}


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
		force = 500.0f;
		maxDistance = 20.0f;
		elapsedTime = 0.0f;
		startingRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update ()
	{
		elapsedTime += Time.deltaTime;
		float travelledDistance = (startingPosition-transform.position).magnitude;
		if(travelledDistance <= maxDistance){
			this.transform.position = parent.transform.position;
			this.transform.rotation = startingRotation;
			if(spikeNumber == 1){
				this.transform.Rotate(this.transform.forward,-parent.transform.eulerAngles.y-90.0f,Space.Self);
			} else {
				this.transform.Rotate(this.transform.forward,-parent.transform.eulerAngles.y+90.0f,Space.Self);
			}
			this.transform.position += (this.transform.up * speed * elapsedTime);
		} else {
			GameObject.Destroy(this.gameObject);
		}
	}
	
	void OnTriggerEnter(Collider other){
		DestructibleObject obj = other.GetComponent<DestructibleObject>();
		if(obj != null && other.gameObject != parent){
			obj.receiveDamage(damage);
			obj.rigidbody.AddForce(this.transform.up*force);
			//GameObject.Destroy(this.gameObject);
		}
	}
}


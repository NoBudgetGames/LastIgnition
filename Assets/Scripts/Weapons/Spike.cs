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
	float angle;
	Quaternion startingRotation;
	// Use this for initialization
	void Start ()
	{
		angle = 10.0f;
		damage = 20.0f;
		speed = 10.0f;
		force = 15000.0f;
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
			this.transform.rotation = parent.transform.rotation;
			this.transform.Rotate(this.transform.forward,-parent.transform.eulerAngles.x+90.0f,Space.Self);
			
			if(spikeNumber == 1){
				this.transform.Rotate(parent.transform.up,+angle+180.0f,Space.World);
			} else {
				this.transform.Rotate(parent.transform.up,+angle,Space.World);
			}
			angle += 10.0f;
			if(angle >= 360.0f){
				angle = 0.0f;
			}
			//this.transform.position += (this.transform.up * speed * elapsedTime);
		} else {
			GameObject.Destroy(this.gameObject);
		}
	}
	
	void OnTriggerEnter(Collider other){
		AbstractDestructibleObject  obj = other.GetComponent<AbstractDestructibleObject >();
		if(obj != null && other.transform.root.gameObject != parent){
			obj.receiveDamage(damage);
			//falls es ein Auto ist muss über die DestructibleCarPart auf den Rigidbody zugegriffen werden
			if(other.GetComponent<DestructibleCarPart>())
			{
				other.GetComponent<DestructibleCarPart>().car.rigidbody.AddForce(this.transform.up * force, ForceMode.Impulse);
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


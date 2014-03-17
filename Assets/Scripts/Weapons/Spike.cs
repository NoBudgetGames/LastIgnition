using UnityEngine;
using System.Collections;

public class Spike : MonoBehaviour
{
	float damage;
	float force;
	
	float elapsedTime;
	Animation anim;
	public GameObject parent;
	// Use this for initialization
	void Start ()
	{
		damage = 1.0f;
		force = 15000.0f;
		elapsedTime = 0.0f;

		anim = this.GetComponent<Animation>();
		anim.Play("MoveOut");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!anim.isPlaying){
			anim.Play("Spin");
		}
		elapsedTime += Time.deltaTime;
		if(elapsedTime > 5.0f){
			GameObject.Destroy(this.transform.parent.gameObject);
		}
	}
	
	void OnTriggerEnter(Collider other){
		AbstractDestructibleObject  obj = other.GetComponent<AbstractDestructibleObject >();
		if(obj != null && other.transform.root.gameObject != parent){
			obj.receiveDamage(damage);
			if(Network.connections.Length > 0){
				other.networkView.RPC("receiveDamage",other.networkView.owner,damage);
			}
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


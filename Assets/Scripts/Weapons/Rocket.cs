using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour
{
	Vector3 forwardVector;
	// Use this for initialization
	void Start ()
	{
		forwardVector = GameObject.FindGameObjectWithTag("Player").transform.forward;
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.position += forwardVector*Time.deltaTime*50;
	}
	
	void OnTriggerEnter(Collider other){
		if(other.tag != "Player"){
			GameObject.Destroy(this.gameObject);
		}
	}
}


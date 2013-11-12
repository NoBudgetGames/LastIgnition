using UnityEngine;
using System.Collections;

public class CarColission : MonoBehaviour
{
	
	float baseDamage;
	// Use this for initialization
	void Start ()
	{
		baseDamage = 0.10f;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	void OnCollisionEnter(Collision colission){
		if(colission.relativeVelocity.magnitude >10.0f){
			this.GetComponent<DestructibleObject>().receiveDamage(baseDamage*colission.relativeVelocity.magnitude);
		}
			rigidbody.AddForce((colission.relativeVelocity*0.5f),ForceMode.VelocityChange);
		
	}
}


using UnityEngine;
using System.Collections;

public class DestructibleObject : MonoBehaviour
{
	public float health = 100.0f;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(health<=0.0f)
			GameObject.Destroy(this.gameObject);
	}
	
	public void receiveDamage(float dmg){
		health -= dmg;	
	}
}


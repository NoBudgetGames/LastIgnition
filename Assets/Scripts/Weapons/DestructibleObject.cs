using UnityEngine;
using System.Collections;

public class DestructibleObject : MonoBehaviour
{
	public float health = 100.0f; //Lebenspunkte eines zerstörbaren Objekts

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Wenn die Lebenspunkte 0 erreichen wird das Objekt zerstört
		if(health<=0.0f)
			GameObject.Destroy(this.gameObject);
	}
	
	//Zieht schaden von Lebenspunkten ab
	public void receiveDamage(float dmg){
		health -= dmg;	
	}
}


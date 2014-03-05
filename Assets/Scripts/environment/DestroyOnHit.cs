using UnityEngine;
using System.Collections;

public class DestroyOnHit : MonoBehaviour {

	public float delay;

	void OnTriggerEnter(Collider collider){
		Debug.Log(collider.transform.root.tag);
		if(collider.transform.root.tag == "Player"){
			Destroy (gameObject.transform.parent.gameObject,delay);
		}
	}
}

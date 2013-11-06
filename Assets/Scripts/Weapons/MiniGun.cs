using UnityEngine;
using System.Collections;

public class MiniGun : MonoBehaviour {
	
	float timer;
	bool firing;
	// Use this for initialization
	void Start () {
		timer = 0.0f;
		firing = false;
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetMouseButtonDown(0))
			firing = true;
		
		if(firing){
			timer+=Time.deltaTime;
			if(timer>=0.1f){
				timer = 0.0f;
				RaycastHit hit;
				if(Physics.Raycast(transform.position,transform.forward,out hit,30)){
					if(hit.collider.gameObject.tag == "destructible"){
						DestructibleObject obj = hit.collider.GetComponent<DestructibleObject>();
						obj.receiveDamage(5.0f);
						Debug.Log(obj.health);
					} else {
						Debug.Log("Fail!");	
					}
				}
			}
		}
		if(Input.GetMouseButtonUp(0)){
			timer = 0.0f;
			firing = false;
		}
	}
}

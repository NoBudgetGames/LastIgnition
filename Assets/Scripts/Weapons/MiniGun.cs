using UnityEngine;
using System.Collections;

public class MiniGun : MonoBehaviour {
	
	float timer; //Zeit zwichen den Schüssen
	bool firing; //Gibt an ob die Minigun bereits schießt
	// Use this for initialization
	void Start () {
		timer = 0.0f;
		firing = false;
	}
	
	// Update is called once per frame
	void Update () {
	
		//Sobald der Button gedrückt wird schießt die Minigun
		//EVTL DELAY EINFÜGEN, WEGEN ANLAUFEN
		if(Input.GetMouseButtonDown(0))
			firing = true;
		
		//Wenn der Timer sein Limit erreicht hat wird ein Raycast ausgeführt der in gerader Richtung nach
		//Vorne verläuft, das erste Objekt das getroffen wird erhält schaden
		//EVTL SPRAY EINFÜGEN
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
		
		//Wenn der Button losgelassen wird hört die Minigun auf zu feuern
		if(Input.GetMouseButtonUp(0)){
			timer = 0.0f;
			firing = false;
		}
	}
}

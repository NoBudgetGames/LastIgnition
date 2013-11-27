using UnityEngine;
using System.Collections;

public class MiniGun : Weapon {

	bool firing; //Gibt an ob die Minigun bereits schießt
	// Use this for initialization
	void Start () {
		timer = 0.0f;
		maxTime = 0.1f;
		minAmmo = 64;
		maxAmmo = 256;
		incAmmo = 64;

		_weaponType = WeaponType.MINIGUN;
	}
	
	// Update is called once per frame
	void Update () {
	
		//Sobald der Button gedrückt wird schießt die Minigun
		//EVTL DELAY EINFÜGEN, WEGEN ANLAUFEN
		if(buttonPressed)
			firing = true;
		
		//Wenn der Timer sein Limit erreicht hat wird ein Raycast ausgeführt der in gerader Richtung nach
		//Vorne verläuft, das erste Objekt das getroffen wird erhält schaden
		//EVTL SPRAY EINFÜGEN

		if(ammo > 0){
			if(firing){
				timer+=Time.deltaTime;
				if(timer>=maxTime){
					timer = 0.0f;
					ammo--;
					RaycastHit hit;
					if(Physics.Raycast(transform.position,transform.forward,out hit,30)){
						Debug.DrawLine (transform.position, hit.point);
						if(hit.collider.GetComponent<AbstractDestructibleObject>())
						{
							hit.collider.GetComponent<AbstractDestructibleObject>().receiveDamage(5.0f);
						} 
						else 
						{
							Debug.Log("Minigun hit something, but could't apply damage!");	
						}
					}
				}
			}
		}else {
			timer = 0.0f;
		}
	}

}

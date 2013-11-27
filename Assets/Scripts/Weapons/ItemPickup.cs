using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour
{
	public WeaponType optainableWeapon = WeaponType.NONE; //Waffentyp des Pickups
	private float timer; //Zeit bis zum Respawn
	private const float MAX_TIME = 10.0f; //Zeit bis zum Respawn
	private bool objectUsed; //Gibt an ob das Objekt benutzt wurde
	// Use this for initialization
	void Start ()
	{
		timer = 0.0f;
		objectUsed = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Zählt den Timer hoch bis die Maximal Zeit erreicht wurde und aktiviet dann wieder das Objekt
		if(objectUsed){
			timer+=Time.deltaTime;
			if(timer >= MAX_TIME){
				this.renderer.enabled = true;
				this.collider.enabled = true;
				timer = 0.0f;
				objectUsed = false;
			}
		}
	}
	//Wenn das Item berührt wurde wird die Waffe des Fahrers erneuert und das Item deaktiviert
	void OnTriggerEnter(Collider other){
		if(!objectUsed){
			CarInventory inv = other.transform.root.GetComponentInChildren<CarInventory>();
			if(inv != null){
				if(inv.equippedWeapon != null){
					if(inv.equippedWeapon.weaponType != optainableWeapon){
						inv.activateWeapon(optainableWeapon);
					} else {
						inv.increaseAmmo();
					}
				} else {
					inv.activateWeapon(optainableWeapon);
				}

				objectUsed = true;
				this.renderer.enabled = false;
				this.collider.enabled = false;
			}
		}
	}
}


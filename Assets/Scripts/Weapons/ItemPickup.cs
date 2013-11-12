using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour
{
	public WeaponType optainableWeapon = WeaponType.NONE;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	void OnTriggerEnter(Collider other){
		CarInventory inv = other.GetComponent<CarInventory>();
		if(inv != null){
			inv.activateWeapon(optainableWeapon);
			this.gameObject.SetActive(false);
		}
	}
}


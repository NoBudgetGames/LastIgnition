using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour
{
	public WeaponType optainableWeapon = WeaponType.NONE; //Waffentyp des Pickups
	private float timer; //Zeit bis zum Respawn
	private const float MAX_TIME = 10.0f; //Zeit bis zum Respawn
	private bool objectUsed; //Gibt an ob das Objekt benutzt wurde

	//die Positionen, zwischen dem das GameObject hin und her schwingen soll
	Vector3 bottomPos;
	Vector3 topPos;
	//bool um festzustellen, ob es gerad nach oben geht oder nicht
	private bool goingDown = false;
	//der floatwert, der hoch und runter gezählt wird
	private float t = 0.0f;

	// Use this for initialization
	void Start ()
	{
		//untere Position soll die im Editor platirte Position sein
		bottomPos = transform.position;
		//obere Position soll etwas darüber liegen
		topPos = transform.position + new Vector3(0.0f, 3.0f, 0.0f);
		timer = 0.0f;
		objectUsed = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//falls wir nach oben gehen, zähle t hoch
		if(goingDown == false)
		{
			if(t < 0.9f)
			{
				t += Time.deltaTime;
			}
			//fals wir oben sind, stoppe die hochwärtsbewegung
			else
			{
				goingDown = true;
			}
		}
		//falls wir nach unten gehen, zähle t runter
		else
		{
			if(t > 0.1f)
			{
				t -= Time.deltaTime;
			}
			//falls wir unten sind, zähle nicht mehr runter
			else
			{
				goingDown = false;
			}
		}

		//die Position soll immer zwischen den beiden Positonen hin und her gehen
		transform.position =  Vector3.Lerp(bottomPos, topPos, t);

		Vector3 tempAngles = transform.eulerAngles;
		tempAngles.y += Time.deltaTime * 200.0f;
		transform.eulerAngles = tempAngles;

		//Zählt den Timer hoch bis die Maximal Zeit erreicht wurde und aktiviet dann wieder das Objekt
		if(objectUsed){
			timer+=Time.deltaTime;
			if(timer >= MAX_TIME){
				this.GetComponentInChildren<Renderer>().enabled = true;
				this.GetComponent<Collider>().enabled = true;
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
					//if(inv.equippedWeapon.weaponType != optainableWeapon){
						inv.increaseAmmo(optainableWeapon);
						if(inv.equippedWeapon.remainingAmmo() <= 0)
							inv.activateWeapon(optainableWeapon);
					//} 
				} else {
					inv.activateWeapon(optainableWeapon);
					inv.increaseAmmo(optainableWeapon);
				}

				objectUsed = true;
				this.GetComponentInChildren<Renderer>().enabled = false;
				this.GetComponent<Collider>().enabled = false;
			}
		}
	}
}
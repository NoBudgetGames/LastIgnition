using UnityEngine;
using System.Collections;

public class MineThrower : Weapon
{

	public GameObject minePrefab;
		// Use this for initialization
		void Start ()
		{
			timer = 0.0f;
			maxTime = 0.5f;
			minAmmo = 1;
			maxAmmo = 6;
			incAmmo = 2;

			_weaponType = WeaponType.MINE_THROWER;
		}
	
		// Update is called once per frame
	void Update ()
	{
		if(ammo > 0){
			if(timer == 0.0f){
				if(buttonPressed){
					GameObject mine;
					if(Network.connections.Length > 0){
						mine = (GameObject) Network.Instantiate(minePrefab,minePrefab.transform.position,minePrefab.transform.rotation,0);
					} else {
						mine = (GameObject) GameObject.Instantiate(minePrefab);
					}
					mine.transform.position = this.transform.position - this.transform.forward * 2.0f;
					mine.rigidbody.AddForce(-this.transform.forward * 500.0f);
					//mine.GetComponent<Mine>().parent = transform.root.gameObject;

					ammo--;
					timer+=Time.deltaTime;
				}
			} else {
				timer+=Time.deltaTime;
				if(timer>=maxTime)
					timer = 0.0f;
			}
		}else {
			timer = 0.0f;
		}
	}
}


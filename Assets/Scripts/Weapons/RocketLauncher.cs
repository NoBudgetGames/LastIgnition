using UnityEngine;
using System.Collections;

public class RocketLauncher : Weapon
{
	public GameObject rocketPrefab; //Prefab der Rakete
	// Use this for initialization
	void Start ()
	{
		timer = 0.0f;
		maxTime = 0.5f;
		minAmmo = 2;
		maxAmmo = 6;
		incAmmo = 2;

		_weaponType = WeaponType.ROCKET_LAUNCHER;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Eine Raketenobjekt wird aus dem Prefab erstellt und die Rotation und Position werden abhängig vom Auto
		//angepasst.
		if(ammo > 0){
			if(timer == 0.0f){
				if(buttonPressed){
					buttonPressed = false;
					GameObject rocket = (GameObject) GameObject.Instantiate(rocketPrefab);
					rocket.transform.position = this.transform.position;
					rocket.transform.Rotate(rocket.transform.up,this.transform.parent.transform.eulerAngles.y,Space.Self);
					rocket.GetComponent<Rocket>().parent = transform.root.gameObject;
					timer+=Time.deltaTime;
					ammo--;
				}
			} else {
				timer+=Time.deltaTime;
				if(timer >= maxTime)
					timer = 0.0f;
			}
		}else {
			timer = 0.0f;
		}
	}
}


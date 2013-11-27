using UnityEngine;
using System.Collections;

public class CarInventory : MonoBehaviour
{
	
	public Weapon equippedWeapon;
	
	Weapon minigunComp;
	Weapon rocketLauncherComp;
	Weapon spikeHandleComp;
	Weapon mineThrowerComp;

	bool firing;
	float lastInput;
	// Use this for initialization
	void Start ()
	{
		lastInput = 0.0f;
		firing = false;
		equippedWeapon = null;
		minigunComp = this.GetComponentInChildren<MiniGun>();
		rocketLauncherComp = this.GetComponentInChildren<RocketLauncher>();
		spikeHandleComp = this.GetComponentInChildren<SpikeHandle>();
		mineThrowerComp = this.GetComponentInChildren<MineThrower>();
		activateWeapon(WeaponType.NONE);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(equippedWeapon != null){
			equippedWeapon.setFiring(firing);
		}
		firing = false;

	}

	public void setFiring(float input){
		if(lastInput != input){
			if(input > 0.9f)
				firing = true;
			else 
				firing = false;

			lastInput = input;
		}
	}

	public void activateWeapon(WeaponType weapon){

		switch(weapon){
			case WeaponType.NONE:
				equippedWeapon = null;
				break;
			case WeaponType.MINIGUN:
				equippedWeapon = minigunComp;
				break;
			case WeaponType.ROCKET_LAUNCHER:
				equippedWeapon = rocketLauncherComp;
				break;
			case WeaponType.SPIKES:
				equippedWeapon = spikeHandleComp;
				break;
			case WeaponType.MINE_THROWER:
				equippedWeapon = mineThrowerComp;
			break;
		}

		if(equippedWeapon != null)
			equippedWeapon.reset();
	}

	public void increaseAmmo(){
		equippedWeapon.increase();
	}
}


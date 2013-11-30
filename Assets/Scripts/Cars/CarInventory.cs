using UnityEngine;
using System.Collections;

public class CarInventory : MonoBehaviour
{
	
	public Weapon equippedWeapon;
	int equippedWeaponNumber;
	
	Weapon minigunComp;
	Weapon rocketLauncherComp;
	Weapon spikeHandleComp;
	Weapon mineThrowerComp;

	Weapon[] allWeapons;

	bool firing;
	bool lastInput;

	bool lastNextPress;
	bool lastPrevPress;
	// Use this for initialization
	void Start ()
	{
		lastInput = false;
		firing = false;
		lastNextPress = false;
		lastPrevPress = false;
		equippedWeapon = null;
		minigunComp = this.GetComponentInChildren<MiniGun>();
		rocketLauncherComp = this.GetComponentInChildren<RocketLauncher>();
		spikeHandleComp = this.GetComponentInChildren<SpikeHandle>();
		mineThrowerComp = this.GetComponentInChildren<MineThrower>();

		allWeapons = new Weapon[(int)WeaponType.NUMBER_OF_WEAPONS];
		allWeapons[(int)WeaponType.MINIGUN] = minigunComp;
		allWeapons[(int)WeaponType.ROCKET_LAUNCHER] = rocketLauncherComp;
		allWeapons[(int)WeaponType.MINE_THROWER] = mineThrowerComp;
		allWeapons[(int)WeaponType.SPIKES] = spikeHandleComp;

		activateWeapon(WeaponType.NONE);
		equippedWeaponNumber = (int)WeaponType.NONE;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(equippedWeapon != null){
			equippedWeapon.setFiring(firing);
		}
		firing = false;

	}

	public void setFiring(bool input){
		if(lastInput != input){
			firing = input;
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
			equippedWeaponNumber = (int)equippedWeapon.weaponType;
		/*
		if(equippedWeapon != null)
			equippedWeapon.reset();
		*/
	}

	public void increaseAmmo(){
		equippedWeapon.increase();
	}

	public void increaseAmmo(WeaponType weapon){
		allWeapons[(int)weapon].increase();
	}

	public void nextWeapon(bool pressed){
		if(lastNextPress != pressed){
			if(pressed){
				if(equippedWeaponNumber+1>(int)WeaponType.NUMBER_OF_WEAPONS-1){
					equippedWeaponNumber = 0;
				} else {
					equippedWeaponNumber++;
				}
				equippedWeapon = allWeapons[(int)equippedWeaponNumber];
			}
		}
		lastNextPress = pressed;
	}

	public void prevWeapon(bool pressed){
		if(lastPrevPress != pressed){
			if(pressed){
				if(equippedWeaponNumber-1<0){
					equippedWeaponNumber = (int)WeaponType.NUMBER_OF_WEAPONS-1;
				} else {
					equippedWeaponNumber--;
				}
				equippedWeapon = allWeapons[(int)equippedWeaponNumber];
			}
		}
		lastPrevPress = pressed;
	}
}


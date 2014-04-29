
using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
	protected bool buttonPressed = false;
	protected int ammo = 0;
	protected int minAmmo;
	protected int maxAmmo;
	protected int incAmmo;

	protected float timer;
	protected float maxTime;

	public Texture hudIcon;

	protected WeaponType _weaponType;
	public WeaponType weaponType {
		get{return _weaponType;}
	}

	public void setFiring(bool input){
		buttonPressed = input;
	}

	public void reset(){
		ammo = minAmmo;
		timer = 0.0f;
	}
	public void increase(){
		if(maxAmmo >= ammo+incAmmo)
			ammo+=incAmmo;
		else{
			if(ammo < maxAmmo)
				ammo = maxAmmo;
		}
	}

	public int remainingAmmo(){
		return ammo;
	}
}



using UnityEngine;
using System.Collections;

public class CarInventory : MonoBehaviour
{
	
	public WeaponType equippedWeapon;
	
	MiniGun minigunComp;
	RocketLauncher rocketLauncherComp;
	SpikeHandle spikeHandleComp;
	// Use this for initialization
	void Start ()
	{
		equippedWeapon = WeaponType.NONE;
		minigunComp = this.GetComponentInChildren<MiniGun>();
		rocketLauncherComp = this.GetComponentInChildren<RocketLauncher>();
		spikeHandleComp = this.GetComponentInChildren<SpikeHandle>();
		activateWeapon(WeaponType.NONE);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	public void activateWeapon(WeaponType weapon){
		switch(equippedWeapon){
			case WeaponType.NONE:
				break;
			case WeaponType.MINIGUN:
				minigunComp.enabled = false;
				break;
			case WeaponType.ROCKET_LAUNCHER:
				rocketLauncherComp.enabled = false;
				break;
			case WeaponType.SPIKES:
				spikeHandleComp.enabled = false;
				break;
		}
		equippedWeapon = weapon;
		switch(weapon){
			case WeaponType.NONE:
				minigunComp.enabled = false;
				rocketLauncherComp.enabled = false;
				spikeHandleComp.enabled = false;
				break;
			case WeaponType.MINIGUN:
				minigunComp.enabled = true;
				break;
			case WeaponType.ROCKET_LAUNCHER:
				rocketLauncherComp.enabled = true;
				break;
			case WeaponType.SPIKES:
				spikeHandleComp.enabled = true;
				break;
		}
	}
}


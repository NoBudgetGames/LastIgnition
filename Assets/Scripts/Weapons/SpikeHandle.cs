using UnityEngine;
using System.Collections;

public class SpikeHandle : Weapon
{
	public GameObject spikePrefab;
	// Use this for initialization
	void Start ()
	{
		timer = 0.0f;
		maxTime = 5.0f;
		minAmmo = 1;
		maxAmmo = 3;
		incAmmo = 1;

		_weaponType = WeaponType.SPIKES;
	}

	// Update is called once per frame
	void Update ()
	{
		if(ammo>0){
			if(timer==0.0f){
				if(buttonPressed){
					buttonPressed = false;
					GameObject spike = (GameObject) GameObject.Instantiate(spikePrefab);
					spike.transform.position = this.transform.position;
					spike.transform.rotation = this.transform.rotation;
					spike.transform.Rotate(spike.transform.forward,-this.transform.parent.transform.eulerAngles.x+90.0f,Space.Self);
					spike.transform.Rotate(spike.transform.up,-this.transform.parent.transform.eulerAngles.y-90.0f,Space.Self);
					spike.GetComponent<Spike>().parent = transform.root.gameObject;
					spike.GetComponent<Spike>().startingPosition = this.transform.position;
					spike.GetComponent<Spike>().spikeNumber = 1;
				
					GameObject spike2 = (GameObject) GameObject.Instantiate(spikePrefab);
					spike2.transform.position = this.transform.position;
					spike2.transform.rotation = this.transform.rotation;
					spike2.transform.Rotate(spike2.transform.forward,-this.transform.parent.transform.eulerAngles.y+90.0f,Space.Self);
					spike2.transform.Rotate(spike2.transform.up,-this.transform.parent.transform.eulerAngles.y+90.0f,Space.Self);
					spike2.GetComponent<Spike>().parent = transform.root.gameObject;
					spike2.GetComponent<Spike>().startingPosition = this.transform.position;
					spike2.GetComponent<Spike>().spikeNumber = 2;

					ammo--;	
					timer+=Time.deltaTime;
				}
			} else {
				timer+=Time.deltaTime;
				if(timer>=maxTime)
					timer = 0.0f;
			}
		} else {
			timer = 0.0f;
		}
	}
	
}


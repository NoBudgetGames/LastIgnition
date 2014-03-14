using UnityEngine;
using System.Collections;

public class SpikeHandle : Weapon
{
	//Das Prefab zum erstellen der Spikes
	public GameObject spikePrefab;
	//Die Reifen, die die Position angeben
	//Es muss zwichen rechts und links unterschieden werden, da die Spikes
	//für die Linke Seite gedreht werden müssen
	public GameObject[] spikePositionLeft = new GameObject[2];
	public GameObject[] spikePositionRight = new GameObject[2];

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
				//Erzeugt Spike Objekte in den Reifen mittels des übergebenen Prefabs
				if(buttonPressed){
					buttonPressed = false;
					for(int i = 0; i<2; ++i){
					GameObject spike;
					if(Network.connections.Length > 0){
						spike = (GameObject) Network.Instantiate(spikePrefab,spikePrefab.transform.position,spikePrefab.transform.rotation,0);
					} else {
						spike = (GameObject) GameObject.Instantiate(spikePrefab);
					}
					spike.transform.position = spikePositionLeft[i].transform.position;
					spike.transform.rotation = spikePositionLeft[i].transform.rotation;
					spike.transform.parent = spikePositionLeft[i].transform;
					spike.transform.Rotate(this.transform.up,180.0f,Space.Self);
					spike.GetComponentInChildren<Spike>().parent = spikePositionLeft[i].transform.root.gameObject;


			
					GameObject spike2 = (GameObject) GameObject.Instantiate(spikePrefab);
					spike2.transform.position = spikePositionRight[i].transform.position;
					spike2.transform.rotation = spikePositionRight[i].transform.rotation;
					spike2.transform.parent = spikePositionRight[i].transform;
					spike2.GetComponentInChildren<Spike>().parent = spikePositionRight[i].transform.transform.root.gameObject;
					}
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


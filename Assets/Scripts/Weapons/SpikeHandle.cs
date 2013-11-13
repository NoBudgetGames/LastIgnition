using UnityEngine;
using System.Collections;

public class SpikeHandle : MonoBehaviour
{
	public GameObject spikePrefab;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetMouseButtonDown(0)){
				GameObject spike = (GameObject) GameObject.Instantiate(spikePrefab);
				spike.transform.position = this.transform.position;
				spike.transform.Rotate(spike.transform.up,-this.transform.parent.transform.eulerAngles.y-90.0f,Space.Self);
				spike.GetComponent<Spike>().parent = transform.parent.gameObject;
				spike.GetComponent<Spike>().startingPosition = this.transform.position;
				spike.GetComponent<Spike>().spikeNumber = 1;
			
				GameObject spike2 = (GameObject) GameObject.Instantiate(spikePrefab);
				spike2.transform.position = this.transform.position;
				spike2.transform.Rotate(spike2.transform.up,-this.transform.parent.transform.eulerAngles.y+90.0f,Space.Self);
				spike2.GetComponent<Spike>().parent = transform.parent.gameObject;
				spike2.GetComponent<Spike>().startingPosition = this.transform.position;
				spike2.GetComponent<Spike>().spikeNumber = 2;
			}
	}
}


using UnityEngine;
using System.Collections;

public class EngineSound : MonoBehaviour
{
	public Car car;
	AudioSource audio;
	float pitch;
	// Use this for initialization
	void Start ()
	{
		audio = this.GetComponent<AudioSource>();
		pitch = audio.pitch;
	}

	// Update is called once per frame
	void Update ()
	{
		if(Network.connections.Length == 0 || this.transform.root.networkView.owner == Network.player){
			this.GetComponent<RealAudioSource>().setPosition(new Vector3(0,0,1));
		} else {
			this.GetComponent<RealAudioSource>().setPosition(this.transform.root.position);
		}
		pitch = 0 + 1 * car.getRPM()/3000;
		audio.pitch = pitch;
	}
}

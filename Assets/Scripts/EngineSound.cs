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
		pitch = 0 + 1 * car.getRPM()/3000;
		audio.pitch = pitch;
	}
}

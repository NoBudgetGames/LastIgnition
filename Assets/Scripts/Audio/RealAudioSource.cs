using UnityEngine;
using System.Collections;

/*
 * Diese Klasse enthält die Soundfiles. 
 * Pro VirtualAudioListener (nur ein Tag) muss je einmal ein Object dieser KLasse instanziert werden
 */

public class RealAudioSource: MonoBehaviour 
{
	//Referenz auf den richtigen AudioListener
	public GameObject audioListener;

	// Use this for initialization
	void Awake() 
	{
		audioListener = GameObject.FindGameObjectWithTag("AudioListener");
	}

	//diese Methode aktuallisert die Position der echten AudioQuelle zur echten AudioListener
	public void setPosition(Vector3 relativePosition)
	{	
		//verschiebe die Position zum AudioListener
		transform.position = audioListener.transform.position + relativePosition;
	}
}
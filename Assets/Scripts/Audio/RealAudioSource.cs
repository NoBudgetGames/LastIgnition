using UnityEngine;
using System.Collections;

/*
 * Diese Klasse enthält die Soundfiles. 
 * Pro VirtualAudioListener (ist nur ein Tag an einen GameObject in der Hierachie des KameraControllers) muss je einmal ein Object dieser KLasse instanziert werden
 */

public class RealAudioSource: MonoBehaviour 
{
	//soll das GameObject gelöscht werden, nachdem es die AudioDatei abgespielt hat?
	public bool destroyAfterPlayed = false;

	//Referenz auf den richtigen AudioListener
	private GameObject audioListener;
	//die Audioquelle
	private AudioSource source;

	// Use this for initialization
	void Awake() 
	{
		audioListener = GameObject.FindGameObjectWithTag("AudioListener");
		source = gameObject.GetComponent<AudioSource>();
	}

	void Update()
	{
		//falls die Audiosource aufgehört hat zu spielen, lösche das GameObejct
		if(source.isPlaying == false && destroyAfterPlayed == true)
		{
			GameObject.Destroy(gameObject);//, 5.0f);
		}
	}

	//diese Methode aktuallisert die Position der echten AudioQuelle zur echten AudioListener
	public void setPosition(Vector3 relativePosition)
	{	
		//verschiebe die Position zum AudioListener
		transform.position = audioListener.transform.position + relativePosition;
	}
}
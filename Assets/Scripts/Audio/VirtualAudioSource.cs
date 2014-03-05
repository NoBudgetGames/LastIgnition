using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Dise Klasse stellt eine "virtuellen" AudioSource dar. Da Unity nur einen AudioListener pro Szene erlaubt, 
 * müssen alle erzeugten Sounds relativ zum "virtuellen" Listener um den AuioListener positioniert werden
 * SIe enthält eine Liste mit allen virtualAudioListeners, 
 * außerdem enthält sie eine Referenz auf die RealAudioSourcePrefab mit der richtige Soundfile für dieses Objekt
 * 
 * Dieses Script wird z.B. an die Prefab für die Explosion angehängt, welche keine SoundFiles enthält
 * Die Soundfiles befinden sich in dem RealAUdioGameObject
 */

public class VirtualAudioSource : MonoBehaviour 
{
	//referenz auf die realAudioSource
	public GameObject realAudioPrefab;

	//Liste mit Referenzen auf die VirtuellenAudioListener (der sich am Auto befidnet)
	private List<GameObject> virtAudioListener;
	//Liste mit den RealAudioSources
	private List<GameObject> realAudioList;

	void Start()
	{
		virtAudioListener = new List<GameObject>();
		realAudioList = new List<GameObject>();
		//finde alle VirtualAudioListener
		GameObject[] objs = GameObject.FindGameObjectsWithTag("VirtAudioListener");
		foreach(GameObject obj in objs)
		{
			//füge sie der Liste hinzu
			virtAudioListener.Add(obj.transform.root.gameObject);
			//instanziere die echte Soundquelle
			GameObject realAudioSrc = (GameObject)GameObject.Instantiate(realAudioPrefab);
			//füge sie der Liste hinzu
			realAudioList.Add(realAudioSrc);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//aktualliesiere für jede RealAudioSource die Position
		for(int i = 0; i < realAudioList.Count; i++)
		{
			//die relative Position der VirtuelAudioSource zum VirtualAudioListener
			Vector3 relativePosition = virtAudioListener[i].transform.InverseTransformPoint(transform.position);
			realAudioList[i].GetComponent<RealAudioSource>().setPosition(relativePosition);
		}
	}
}
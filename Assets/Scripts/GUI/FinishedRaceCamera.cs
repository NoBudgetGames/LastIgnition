using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Diese Klasse stellt die Übersicht nach einen beendeten Rennen/Match dar
 * Sie besitzt eine Kamera sowie eine GUI
 * Sie besitzt dabei eine Liste mit einen String-Array, dass es darstellt
 */

public class FinishedRaceCamera : MonoBehaviour 
{
	//Liste String-Array mit Spielerinfos, z.B. Spielername und Rundezeit
	private List<string[]> playerData;
	//ist die Kamera aktiviert?
	private bool camActive = false;

	// Use this for initialization
	void Start () 
	{
		playerData = new List<string[]>();
		gameObject.GetComponent<Camera>().enabled = false;
	}

	public void addPlayerData(string[] data)
	{
		playerData.Add(data);
	}

	public void activateCamera()
	{
		camActive = true;
	}

	//GUI, diese Methode stellt die Renninfos dar
	void OnGUI()
	{
		if(camActive == true)
		{
			gameObject.GetComponent<Camera>().enabled = true;
			//zeichne eine Box
			GUI.Box(new Rect(10,10,600,500), "");
			GUI.Label(new Rect(50,15,400,100), "Player       Total time      Fastest lap");

			int i = 0;
			//gehe jedes Auto durch
			foreach(string[] str in playerData)
			{
				//gebe die Infos aus
				for(int j = 0; j < str.Length; j++)
				{
					GUI.Label(new Rect(50 + (j*70),30 + (i*15),400,100), str[j]);
				}
				i++;
			}
		}
	}
}
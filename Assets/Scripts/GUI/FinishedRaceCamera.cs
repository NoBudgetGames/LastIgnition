using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Diese Klasse stellt die Übersicht nach einen beendeten Rennen/Match dar
 * Sie besitzt eine Kamera sowie eine GUI
 * Sie besitzt dabei eine Liste mit einen String-Array, dass es darstellt
 * Die Liste enthält dabei Infos zum Spieler, z.B. Name, Rundenzeit oder Leben
 */

public class FinishedRaceCamera : MonoBehaviour 
{
	//Liste String-Array mit Spielerinfos, z.B. Spielername und Rundezeit
	private List<string[]> playerData;
	//ist die Kamera aktiviert?
	private bool camActive = false;
	//Arenamodus oder Rundkurdmodus?
	private bool arenaMode = false;

	// Use this for initialization
	void Start () 
	{
		playerData = new List<string[]>();
		gameObject.GetComponent<Camera>().enabled = false;
		gameObject.GetComponent<AudioListener>().enabled = false;
	}

	public void addPlayerData(string[] data)
	{
		playerData.Add(data);
	}

	public void activateCamera()
	{
		camActive = true;
	}

	public void setArenaMode(bool mode)
	{
		arenaMode = mode;
	}

	//GUI, diese Methode stellt die Renninfos dar
	void OnGUI()
	{
		if(camActive == true)
		{
			gameObject.GetComponent<Camera>().enabled = true;
			gameObject.GetComponent<AudioListener>().enabled = true;
			//zeichne eine Box
			GUI.Box(new Rect(10,10,500,300), "");
			//fall es ein Rundkursrennen ist, stelle Rundkursinfos dar
			if(arenaMode == false)
			{
				GUI.Label(new Rect(45,15,400,100), "Player       Total time      Fastest lap");
			}
			//ansonsten Arenainfos
			else
			{
				GUI.Label(new Rect(45,15,400,100), "Player       Survival Time   Lives");
			}

			int i = 0;
			//gehe jedes Auto durch
			foreach(string[] str in playerData)
			{
				GUI.Label(new Rect(45, 30 + (i*15),400,100), i + ".");
				//gebe die Infos aus
				for(int j = 0; j < str.Length; j++)
				{
					GUI.Label(new Rect(60 + (j*70),30 + (i*15),400,100), str[j]);
				}
				i++;
			}

			//was soll gemacht werden, wenn das Rennen vorbei ist?
			GUI.Label(new Rect(60, 30 + (i*20),400,100), "Zurück zum Hauptmenü: [ESC], Rennen wiederholen: [ENTER]");
			//Falls ESC Taste, kehre zum Hauptmenü zurück
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				Application.LoadLevel("MainMenuScene");
			}
			//ansonsten wiederhole das Rennen
			if(Input.GetKeyDown(KeyCode.Return))
			{
				Application.LoadLevel(PlayerPrefs.GetString("Level"));
			}
		}
	}
}
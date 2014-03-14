using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Diese Klasse ist ein stellt ein Pausenmenü dar, wo man das laufende Rennen pausieren kann
 */

public class PauseMenu : MonoBehaviour 
{
	//ist das Spiel gerade pausiert?
	private bool paused = false;
	//timer, wann man wieder pause drücken darf, sont passiert es, dass man aus dem Pausenmenü nicht mehr rauskommt, weil man 
	//dann immer noch auf P drückt (wenn auch nur für ne millionsten Sekunde), was dazu führt, dass das Pausenmenü wieder aufgerufen wird
	float pauseTimer = 0.0f;
	//liste mit HUDs
	private GameObject[] hudList;

	// Use this for initialization
	void Start () 
	{
		//suche alle HUDs, diese zeichnen noch das Tacho
		hudList = GameObject.FindGameObjectsWithTag("HUD");
		gameObject.GetComponent<Camera>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//zähle den PauseTimer runter
		pauseTimer -= Time.deltaTime;
		//falls nicht pauseirt wurde, und wir auf P drücken, pausiere
		if(Input.GetKeyDown(KeyCode.P) && paused == false && pauseTimer < 0.0f)
		{
			paused = true;
			//pausiere, indem keine Zeit vergeht
			Time.timeScale = 0.0f;
		}
	}

	//GUI, diese Methode stellt die Renninfos dar
	void OnGUI()
	{
		//falls das Rennen pausiert wurde
		if(paused == true)
		{
			//aktiviere die Kamera
			gameObject.GetComponent<Camera>().enabled = true;

			//deaktiviere die HUDs
			foreach(GameObject obj in hudList)
			{
				obj.SetActive(false);
			}

			int width = Screen.width;
			int height = Screen.height;

			int offsetWidth = 120;
			int offsetHeight = 70;

			//zeichne eine Box
			GUI.Box(new Rect(width/2 - offsetWidth, height/2 - offsetHeight, offsetWidth * 2, offsetHeight * 2), "Pause");
			
			//was soll gemacht werden, wenn das Rennen pausiert wurde?
			GUI.Label(new Rect(width/2 - 80, height/2 - 20, offsetWidth * 2, offsetHeight*2), "[P] Rennen fortsetzen");
			GUI.Label(new Rect(width/2 - 80, height/2, offsetWidth * 2, offsetHeight*2), "[ESC] Zurück zum Hauptmenü");
			//GUI.Label(new Rect(width/2 - 80, height/2 + 20, offsetWidth * 2, offsetHeight*2), "[ENTER] Rennen wiederholen");

			//falls wieder Pause gedrückt, 
			if(Input.GetKeyDown(KeyCode.P) && paused == true)
			{
				paused = false;
				gameObject.GetComponent<Camera>().enabled = false;
				//zeit läuft wieder normal ab
				Time.timeScale = 1.0f;
				pauseTimer = 0.5f;
				//aktiviere wieder die HUDs
				foreach(GameObject obj in hudList)
				{
					obj.SetActive(true);
				}
			}
			//Falls ESC Taste, kehre zum Hauptmenü zurück
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				Application.LoadLevel("MainMenuScene");
			}
			//ansonsten wiederhole das Rennen
			if(Input.GetKeyDown(KeyCode.Return))
			{
				//Application.LoadLevel(Application.loadedLevel); //PlayerPrefs.GetString("Level"));
			}
		}
	}
}
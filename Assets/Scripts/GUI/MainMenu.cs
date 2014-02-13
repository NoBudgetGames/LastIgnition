using UnityEngine;
using System.Collections;

/*
 * Diese KLasse stellt das Hauptmenü dar.
 */

public class MainMenu : MonoBehaviour {

	bool levelSelected = false;

	void OnGUI () 
	{
		if(!levelSelected){
			// kleine Hintergrundbox erstellen
			GUI.Box(new Rect(10,10,300,170), "Level Auswahl");
			
			//erster Button, falls gedrückt, wird das erste Level geladen
			if(GUI.Button(new Rect(20,40,280,20), "Derby-Arena im Stadium")) 
			{
				//Application.LoadLevel("ArenaStadium");
				PlayerPrefs.DeleteAll();
				PlayerPrefs.SetString("Level","ArenaStadium");
				levelSelected = true;
			}
			//zweites Level
			if(GUI.Button(new Rect(20,70,280,20), "Wüsten-Arena")) 
			{
				//Application.LoadLevel("DesertArena");
				PlayerPrefs.DeleteAll();
				PlayerPrefs.SetString("Level","DesertArena");
				levelSelected = true;
			}
			//drittes Level
			if(GUI.Button(new Rect(20,100,280,20), "Wüsten-Strecke")) 
			{
				//Application.LoadLevel("DesertCircuit");
				PlayerPrefs.DeleteAll();
				PlayerPrefs.SetString("Level","DesertCircuit");
				levelSelected = true;
			}
			//....
			if(GUI.Button(new Rect(20,130,280,20), "Stefs Test-Strecke")) 
			{
				//Application.LoadLevel("StefTestScene2");
				PlayerPrefs.DeleteAll();
				PlayerPrefs.SetString("Level","StefTestScene2");
				levelSelected = true;
			}
		} else {
			GUI.Box(new Rect(10,10,300,170), "Anzahl der Spieler");

			if(GUI.Button(new Rect(20,40,280,20), "1 Spieler")) 
			{
				PlayerPrefs.SetInt("LocalPlayers",1);
				Application.LoadLevel("ChooseCar");
			}

			if(GUI.Button(new Rect(20,70,280,20), "2 Spieler")) 
			{
				PlayerPrefs.SetInt("LocalPlayers",2);
				Application.LoadLevel("ChooseCar");
			}
		}
	}
}

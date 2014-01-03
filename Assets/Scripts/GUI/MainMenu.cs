using UnityEngine;
using System.Collections;

/*
 * Diese KLasse stellt das Hauptmenü dar.
 */

public class MainMenu : MonoBehaviour {

	void OnGUI () 
	{
		// kleine Hintergrundbox erstellen
		GUI.Box(new Rect(10,10,300,170), "Level Auswahl");
		
		//erster Button, falls gedrückt, wird das erste Level geladen
		if(GUI.Button(new Rect(20,40,280,20), "Derby-Arena im Stadium")) 
		{
			Application.LoadLevel("ArenaStadium");
		}
		//zweites Level
		if(GUI.Button(new Rect(20,70,280,20), "Wüsten-Arena")) 
		{
			Application.LoadLevel("DesertArena");
		}
		//drittes Level
		if(GUI.Button(new Rect(20,100,280,20), "Wüsten-Strecke")) 
		{
			Application.LoadLevel("DesertCircuit");
		}
		//....
		if(GUI.Button(new Rect(20,130,280,20), "Stefs Test-Strecke")) 
		{
			Application.LoadLevel("StefTestScene2");
		}
	}
}

using UnityEngine;
using System.Collections;

/*
 * Diese KLasse stellt das Hauptmenü dar.
 */

public class MainMenu : MonoBehaviour {

	void OnGUI () 
	{
		// kleine Hintergrundbox erstellen
		GUI.Box(new Rect(10,10,300,130), "Level Auswahl");
		
		//erster Button, falls gedrückt, wird das erste Level geladen
		if(GUI.Button(new Rect(20,40,280,20), "Zwei Spieler Test Level")) 
		{
			Application.LoadLevel("StefTestScene2");
		}
		//zweites Level
		if(GUI.Button(new Rect(20,70,280,20), "Ein Spieler Test Level")) 
		{
			Application.LoadLevel("StefTestScene");
		}
		//drittes Level
		if(GUI.Button(new Rect(20,100,280,20), "Waffen Test Level")) 
		{
			Application.LoadLevel("CTestScene");
		}
	}
}

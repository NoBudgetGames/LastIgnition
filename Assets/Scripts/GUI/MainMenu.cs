using UnityEngine;
using System.Collections;

/*
 * Diese KLasse stellt das Hauptmenü dar.
 */

public class MainMenu : MonoBehaviour 
{
	//String, der das aktuell gewählte Menü zeigen soll
	private string currentMenu = "Main";

	void OnGUI () 
	{
		//falls aktuelles Menu das Hauptmenü ist
		if(currentMenu.Equals("Main"))
		{
			mainMenu();
		}
		//falls Singelplayer gewählt wurde
		if(currentMenu.Equals("Singleplayer"))
		{
			localSinglePlayer();
		}
		//falls Slitscreen gewählt wurde
		if(currentMenu.Equals("Splitscreen"))
		{
			localSplitScreen();
		}
		//falls Multiplayer gewählt wurde
		if(currentMenu.Equals("Multiplayer"))
		{
			multiplayer();
		}
		//falls die Levelauswahl für ein lokales Spiel gewählt wurde
		if(currentMenu.Equals("SelectLevel"))
		{
			localLevelSelection();
		}
		//falls die Levelauswahl für ein lokales Spiel gewählt wurde
		if(currentMenu.Equals("multiSingleplayer"))
		{
			multiSinglePlayer();
		}
		//falls die Levelauswahl für ein lokales Spiel gewählt wurde
		if(currentMenu.Equals("multiSplitscreen"))
		{
			multiSplitScreen();
		}
	}

	//das Hauptmenü
	private void mainMenu()
	{
		//aller bisherigen PlayerPrefs löschen
		PlayerPrefs.DeleteAll();

		//kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 80, Screen.height/2 - 200, 160, 250), "Hauptmenü");

		//Button für Singleplayer
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 150, 100, 20), "Einzelspieler")) 
		{
			currentMenu = "Singleplayer";
		}
		//button für splitscreen
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 100, 100, 20), "Splitscreen")) 
		{
			currentMenu = "Splitscreen";
		}
		//button für Multiplayer
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 50, 100, 20), "Multiplayer")) 
		{
			currentMenu = "Multiplayer";
		}
		//button, um das Spiel zu beenden
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2, 100, 20), "Spiel beenden")) 
		{
			Application.Quit();
		}
	}

	//das Menü, für den lokalen einzelspieler
	private void localSinglePlayer()
	{
		PlayerPrefs.SetInt("LocalPlayers", 1);

		//kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 80, Screen.height/2 - 200, 160, 250), "Einzelspieler");

		//name des Spielers
		GUI.Label(new Rect(Screen.width/2 - 55, Screen.height/2 - 150, 160, 20), "Name von Spieler 1");
		//Eingabefeld
		string playerName = PlayerPrefs.GetString("PlayerOneName");
		playerName = GUI.TextField(new Rect(Screen.width/2 - 50, Screen.height/2 - 125, 100, 20), playerName, 15);
		PlayerPrefs.SetString("PlayerOneName", playerName);

		//Button für Levelauswahl
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 50, 100, 20), "Levelauswahl")) 
		{
			currentMenu = "SelectLevel";
		}
		//button, um zurück zum Hauptmenü zu gehen
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2, 100, 20), "Hauptmenü")) 
		{
			currentMenu = "Main";
		}
	}

	//das Menü für den lokalen Splitscreen
	private void localSplitScreen()
	{
		PlayerPrefs.SetInt("LocalPlayers", 2);
		
		//kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 80, Screen.height/2 - 200, 160, 300), "Splitscreen");
		
		//name des Spielers
		GUI.Label(new Rect(Screen.width/2 - 55, Screen.height/2 - 150, 160, 20), "Name von Spieler 1");
		//Eingabefeld
		string playerOneName = PlayerPrefs.GetString("PlayerOneName");
		playerOneName = GUI.TextField(new Rect(Screen.width/2 - 50, Screen.height/2 - 125, 100, 20), playerOneName, 15);
		PlayerPrefs.SetString("PlayerOneName", playerOneName);

		//name des Spielers
		GUI.Label(new Rect(Screen.width/2 - 55, Screen.height/2 - 100, 160, 20), "Name von Spieler 2");
		//Eingabefeld
		string playerTwoName = PlayerPrefs.GetString("PlayerTwoName");
		playerTwoName = GUI.TextField(new Rect(Screen.width/2 - 50, Screen.height/2 - 75, 100, 20), playerTwoName, 15);
		PlayerPrefs.SetString("PlayerTwoName", playerTwoName);
		
		//Button für Levelauswahl
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2, 100, 20), "Levelauswahl")) 
		{
			currentMenu = "SelectLevel";
		}
		//button, um zurück zum Hauptmenü zu gehen
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 + 50, 100, 20), "Hauptmenü")) 
		{
			currentMenu = "Main";
		}
	}

	//das Menü, das angezeigt werden soll, wenn Multiplayer gewählt wurde
	private void multiplayer()
	{
		//kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 80, Screen.height/2 - 200, 160, 200), "Multiplayer");
		
		//Button für Singleplayer
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 150, 100, 20), "Einzelspieler")) 
		{
			currentMenu = "multiSingleplayer";
		}
		//button für splitscreen
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 100, 100, 20), "Splitscreen")) 
		{
			currentMenu = "multiSplitscreen";
		}
		//button, um zurück zum Hauptmenü zu gehen
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 50, 100, 20), "Hauptmenü")) 
		{
			currentMenu = "Main";
		}
	}


	//das Menü, für den multiplayer einzelspieler
	private void multiSinglePlayer()
	{
		PlayerPrefs.SetInt("LocalPlayers", 1);
		
		//kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 80, Screen.height/2 - 200, 160, 300), "Einzelspieler");
		
		//name des Spielers
		GUI.Label(new Rect(Screen.width/2 - 55, Screen.height/2 - 150, 160, 20), "Name von Spieler 1");
		//Eingabefeld
		string playerOneName = PlayerPrefs.GetString("PlayerOneName");
		playerOneName = GUI.TextField(new Rect(Screen.width/2 - 50, Screen.height/2 - 125, 100, 20), playerOneName, 15);
		PlayerPrefs.SetString("PlayerOneName", playerOneName);

		//Button für LAN Multiplayer
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 50, 100, 20), "LAN (Test)")) 
		{
			Application.LoadLevel("LANNetworkTest");
		}
		//Button für Online Multiplayer
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2, 100, 20), "Online (Test)")) 
		{
			Application.LoadLevel("NetworkTest");
		}
		//button, um zurück zum Hauptmenü zu gehen
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 + 50, 100, 20), "Hauptmenü")) 
		{
			currentMenu = "Main";
		}
	}
	
	//das Menü für den multiplayer Splitscreen
	private void multiSplitScreen()
	{
		PlayerPrefs.SetInt("LocalPlayers", 2);
		
		//kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 80, Screen.height/2 - 200, 160, 350), "Splitscreen");
		
		//name des Spielers
		GUI.Label(new Rect(Screen.width/2 - 55, Screen.height/2 - 150, 160, 20), "Name von Spieler 1");
		//Eingabefeld
		string playerOneName = PlayerPrefs.GetString("PlayerOneName");
		playerOneName = GUI.TextField(new Rect(Screen.width/2 - 50, Screen.height/2 - 125, 100, 20), playerOneName, 15);
		PlayerPrefs.SetString("PlayerOneName", playerOneName);
		
		//name des Spielers
		GUI.Label(new Rect(Screen.width/2 - 55, Screen.height/2 - 100, 160, 20), "Name von Spieler 2");
		//Eingabefeld
		string playerTwoName = PlayerPrefs.GetString("PlayerTwoName");
		playerTwoName = GUI.TextField(new Rect(Screen.width/2 - 50, Screen.height/2 - 75, 100, 20), playerTwoName, 15);
		PlayerPrefs.SetString("PlayerTwoName", playerTwoName);
		
		//Button für LAN Multiplayer
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2, 100, 20), "LAN (Test)")) 
		{
			Application.LoadLevel("LANNetworkTest");
		}
		//Button für Online Multiplayer
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 + 50, 100, 20), "Online (Test)")) 
		{
			Application.LoadLevel("NetworkTest");
		}
		//button, um zurück zum Hauptmenü zu gehen
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 + 100, 100, 20), "Hauptmenü")) 
		{
			currentMenu = "Main";
		}
	}

	private void localLevelSelection()
	{
		// kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 100, Screen.height/2 - 200, 200, 300), "Levelauswahl");
		
		//erster Button, falls gedrückt, wird das erste Level geladen
		if(GUI.Button(new Rect(Screen.width/2 - 80, Screen.height/2 - 150, 160, 20), "Derby-Arena im Stadium")) 
		{
			PlayerPrefs.SetString("Level","ArenaStadium");
			Application.LoadLevel("ChooseCar");
		}
		//zweites Level
		if(GUI.Button(new Rect(Screen.width/2 - 80, Screen.height/2 - 100, 160, 20), "Wüsten-Arena")) 
		{
			PlayerPrefs.SetString("Level","DesertArena");
			Application.LoadLevel("ChooseCar");
		}
		//drittes Level
		if(GUI.Button(new Rect(Screen.width/2 - 80, Screen.height/2 - 50, 160, 20), "Wüsten-Strecke")) 
		{
			PlayerPrefs.SetString("Level","DesertCircuit");
			Application.LoadLevel("ChooseCar");
		}
		//....
		if(GUI.Button(new Rect(Screen.width/2 - 80, Screen.height/2, 160, 20), "Stefs Test-Strecke")) 
		{
			PlayerPrefs.SetString("Level","StefTestScene2");
			Application.LoadLevel("ChooseCar");
		}
		//button, um zurück zum Hauptmenü zu gehen
		if(GUI.Button(new Rect(Screen.width/2 - 80, Screen.height/2 + 50, 160, 20), "Hauptmenü")) 
		{
			currentMenu = "Main";
		}
	}



}

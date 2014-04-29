using UnityEngine;
using System.Collections;

/*
 * Diese KLasse stellt das Hauptmenü dar.
 */

public class MainMenu : MonoBehaviour 
{
	//String, der das aktuell gewählte Menü zeigen soll
	private string currentMenu = "Main";

	//styles for customParts
	public GUIStyle customBox;
	public GUIStyle customButton;
	public GUIStyle customLabel;
	//main menu styles
	public GUIStyle singleplayerButton;
	public GUIStyle multiplayerButton;
	public GUIStyle exit;
	public GUIStyle splitscreen;
	//The Logo
	public GUIStyle logo;

	void Start()
	{
		//falls noch ein Netzwerk GameObject vorhanden ist (wenn man z.B. vom Multiplayermenü zurückgeht), lösche es
		if(GameObject.Find("Network") != null)
		{
			GameObject.Destroy(GameObject.Find("Network").gameObject);
		}
		//lösche ebenfalls die eventuell noch vorhandenen NetworkPlayerDatas
		if(GameObject.Find("playerDataOne") != null)
		{
			GameObject.Destroy(GameObject.Find("playerDataOne"));
		}
		if(GameObject.Find("playerDataTwo") != null)
		{
			GameObject.Destroy(GameObject.Find("playerDataTwo"));
		}
	}

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
		//falls ein Multiplayer mit einen lokalen Spieler gewählt wurde
		if(currentMenu.Equals("multiSingleplayer"))
		{
			multiSinglePlayer();
		}
		//falls ein Multiplayer mit zwei lokalen Spielern gewählt wurde
		if(currentMenu.Equals("multiSplitscreen"))
		{
			multiSplitScreen();
		}
	}

	//das Hauptmenü
	private void mainMenu()
	{
		//Namen der vorherigen Spieler suchen
		string tmpOne = PlayerPrefs.GetString("PlayerOneName");
		string tmpTwo = PlayerPrefs.GetString("PlayerTwoName");
		//falls noch keine Namen gesetzt wurden, wähle Standardnamen
		if(tmpOne.Equals(""))
		{
			tmpOne = "Spieler 1";
		}
		if(tmpTwo.Equals(""))
		{
			tmpTwo = "Spieler 2";
		}
		//aller bisherigen PlayerPrefs löschen
		PlayerPrefs.DeleteAll();
		//Spieleramen setzen
		PlayerPrefs.SetString("PlayerOneName", tmpOne);
		PlayerPrefs.SetString("PlayerTwoName", tmpTwo);

		//kleine Hintergrundbox erstellen
		//GUI.Box(new Rect((Screen.width/3)*2 - 80, Screen.height/2 - 200, 160, 250), "Hauptmenü", customBox);

		//the logo - box
		GUI.Box(new Rect(0,0, 425, 310), "", logo);


		//Button für Singleplayer
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 - 200, 455, 90), "", singleplayerButton)) 
		{
			currentMenu = "Singleplayer";
		}
		//button für splitscreen
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 - 100, 455, 90), "", splitscreen)) 
		{
			currentMenu = "Splitscreen";
		}
		//button für Multiplayer
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2, 455, 90), "", multiplayerButton)) 
		{
			currentMenu = "Multiplayer";
		}
		//button, um das Spiel zu beenden
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 + 100, 455, 90), "", exit)) 
		{
			Application.Quit();
		}
	}

	//das Menü, für den lokalen einzelspieler
	private void localSinglePlayer()
	{
		PlayerPrefs.SetInt("LocalPlayers", 1);

		//the logo - box
		GUI.Box(new Rect(0,0, 425, 310), "", logo);

		//kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 235, Screen.height/2 - 265, 470, 530), "Einzelspieler", customBox);
		
		//name des Spielers
		GUI.Label(new Rect(Screen.width/2 - 65, Screen.height/2 - 150, 160, 20), "Name von Spieler 1", customLabel);
		//Eingabefeld
		string playerName = PlayerPrefs.GetString("PlayerOneName");
		playerName = GUI.TextField(new Rect(Screen.width/2 - 160, Screen.height/2 - 125, 320, 55), playerName, 15);
		PlayerPrefs.SetString("PlayerOneName", playerName);
		
		//Button für Levelauswahl
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 + 75, 350, 65), "Levelauswahl", customButton)) 
		{
			currentMenu = "SelectLevel";
		}
		//button, um zurück zum Hauptmenü zu gehen
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 + 135, 350, 65), "Hauptmenü", customButton)) 
		{
			currentMenu = "Main";
		}

		//Button für Singleplayer
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 - 200, 455, 90), "", singleplayerButton)) 
		{
			currentMenu = "Singleplayer";
		}
		//button für splitscreen
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 - 100, 455, 90), "", splitscreen)) 
		{
			currentMenu = "Splitscreen";
		}
		//button für Multiplayer
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2, 455, 90), "", multiplayerButton)) 
		{
			currentMenu = "Multiplayer";
		}
		//button, um das Spiel zu beenden
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 + 100, 455, 90), "", exit)) 
		{
			Application.Quit();
		}

	}

	//das Menü für den lokalen Splitscreen
	private void localSplitScreen()
	{
		PlayerPrefs.SetInt("LocalPlayers", 2);

		//the logo - box
		GUI.Box(new Rect(0,0, 425, 310), "", logo);
		
		//kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 235, Screen.height/2 - 265, 470, 530), "Splitscreen", customBox);
		
		//name des Spielers
		GUI.Label(new Rect(Screen.width/2 - 65, Screen.height/2 - 150, 160, 20), "Name von Spieler 1", customLabel);
		//Eingabefeld
		string playerOneName = PlayerPrefs.GetString("PlayerOneName");
		playerOneName = GUI.TextField(new Rect(Screen.width/2 - 160, Screen.height/2 - 125, 320, 55), playerOneName, 15);
		PlayerPrefs.SetString("PlayerOneName", playerOneName);
		
		//name des Spielers
		GUI.Label(new Rect(Screen.width/2 - 65, Screen.height/2 - 65, 160, 20), "Name von Spieler 2", customLabel);
		//Eingabefeld
		string playerTwoName = PlayerPrefs.GetString("PlayerTwoName");
		playerTwoName = GUI.TextField(new Rect(Screen.width/2 - 160, Screen.height/2 - 35, 320, 55), playerTwoName, 15);
		PlayerPrefs.SetString("PlayerTwoName", playerTwoName);
		
		//Button für Levelauswahl
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 + 75, 350, 65), "Levelauswahl", customButton)) 
		{
			currentMenu = "SelectLevel";
		}
		//button, um zurück zum Hauptmenü zu gehen
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 + 135, 350, 65), "Hauptmenü", customButton)) 
		{
			currentMenu = "Main";
		}

		//Button für Singleplayer
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 - 200, 455, 90), "", singleplayerButton)) 
		{
			currentMenu = "Singleplayer";
		}
		//button für splitscreen
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 - 100, 455, 90), "", splitscreen)) 
		{
			currentMenu = "Splitscreen";
		}
		//button für Multiplayer
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2, 455, 90), "", multiplayerButton)) 
		{
			currentMenu = "Multiplayer";
		}
		//button, um das Spiel zu beenden
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 + 100, 455, 90), "", exit)) 
		{
			Application.Quit();
		}

	}

	//das Menü, das angezeigt werden soll, wenn Multiplayer gewählt wurde
	private void multiplayer()
	{
		//the logo - box
		GUI.Box(new Rect(0,0, 425, 310), "", logo);

		//kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 235, Screen.height/2 - 265, 470, 530), "Multiplayer", customBox);
		
		//Button für Singleplayer
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 - 150, 350, 65), "Einzelspieler", customButton)) 
		{
			currentMenu = "multiSingleplayer";
		}
		//button für splitscreen
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 - 90, 350, 65), "Splitscreen", customButton)) 
		{
			currentMenu = "multiSplitscreen";
		}
		//button, um zurück zum Hauptmenü zu gehen
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 + 135, 350, 65), "Hauptmenü", customButton)) 
		{
			currentMenu = "Main";
		}

		//Button für Singleplayer
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 - 200, 455, 90), "", singleplayerButton)) 
		{
			currentMenu = "Singleplayer";
		}
		//button für splitscreen
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 - 100, 455, 90), "", splitscreen)) 
		{
			currentMenu = "Splitscreen";
		}
		//button für Multiplayer
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2, 455, 90), "", multiplayerButton)) 
		{
			currentMenu = "Multiplayer";
		}
		//button, um das Spiel zu beenden
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 + 100, 455, 90), "", exit)) 
		{
			Application.Quit();
		}

	}


	//das Menü, für den multiplayer einzelspieler
	private void multiSinglePlayer()
	{
		//the logo - box
		GUI.Box(new Rect(0,0, 425, 310), "", logo);

		PlayerPrefs.SetInt("LocalPlayers", 1);
		
		//kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 235, Screen.height/2 - 265, 470, 530), "Netzwerk - Einzelspieler", customBox);
		
		//name des Spielers
		GUI.Label(new Rect(Screen.width/2 - 65, Screen.height/2 - 150, 160, 20), "Name von Spieler 1", customLabel);
		//Eingabefeld
		string playerOneName = PlayerPrefs.GetString("PlayerOneName");
		playerOneName = GUI.TextField(new Rect(Screen.width/2 - 160, Screen.height/2 - 125, 320, 55), playerOneName, 15);
		PlayerPrefs.SetString("PlayerOneName", playerOneName);
		
		//Button für Multiplayer
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 + 75, 350, 65), "Weiter", customButton)) 
		{
			Application.LoadLevel("MultiplayerSetup");
		}
		//button, um zurück zum Hauptmenü zu gehen
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 + 135, 350, 65), "Hauptmenü", customButton)) 
		{
			currentMenu = "Main";
		}

		//Button für Singleplayer
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 - 200, 455, 90), "", singleplayerButton)) 
		{
			currentMenu = "Singleplayer";
		}
		//button für splitscreen
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 - 100, 455, 90), "", splitscreen)) 
		{
			currentMenu = "Splitscreen";
		}
		//button für Multiplayer
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2, 455, 90), "", multiplayerButton)) 
		{
			currentMenu = "Multiplayer";
		}
		//button, um das Spiel zu beenden
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 + 100, 455, 90), "", exit)) 
		{
			Application.Quit();
		}

	}
	
	//das Menü für den multiplayer Splitscreen
	private void multiSplitScreen()
	{
		PlayerPrefs.SetInt("LocalPlayers", 2);

		//the logo - box
		GUI.Box(new Rect(0,0, 425, 310), "", logo);
		
		//kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 235, Screen.height/2 - 265, 470, 530), "Netzwerk - Splitscreen", customBox);
		
		//name des Spielers
		GUI.Label(new Rect(Screen.width/2 - 65, Screen.height/2 - 150, 160, 20), "Name von Spieler 1", customLabel);
		//Eingabefeld
		string playerOneName = PlayerPrefs.GetString("PlayerOneName");
		playerOneName = GUI.TextField(new Rect(Screen.width/2 - 160, Screen.height/2 - 125, 320, 55), playerOneName, 15);
		PlayerPrefs.SetString("PlayerOneName", playerOneName);
		
		//name des Spielers
		GUI.Label(new Rect(Screen.width/2 - 65, Screen.height/2 - 65, 160, 20), "Name von Spieler 2", customLabel);
		//Eingabefeld
		string playerTwoName = PlayerPrefs.GetString("PlayerTwoName");
		playerTwoName = GUI.TextField(new Rect(Screen.width/2 - 160, Screen.height/2 - 35, 320, 55), playerTwoName, 15);
		PlayerPrefs.SetString("PlayerTwoName", playerTwoName);
		
		//Button für Multiplayer
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 + 75, 350, 65), "Weiter", customButton)) 
		{
			Application.LoadLevel("MultiplayerSetup");
		}
		//button, um zurück zum Hauptmenü zu gehen
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 + 135, 350, 65), "Hauptmenü", customButton)) 
		{
			currentMenu = "Main";
		}

		//Button für Singleplayer
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 - 200, 455, 90), "", singleplayerButton)) 
		{
			currentMenu = "Singleplayer";
		}
		//button für splitscreen
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 - 100, 455, 90), "", splitscreen)) 
		{
			currentMenu = "Splitscreen";
		}
		//button für Multiplayer
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2, 455, 90), "", multiplayerButton)) 
		{
			currentMenu = "Multiplayer";
		}
		//button, um das Spiel zu beenden
		if(GUI.Button(new Rect(Screen.width - 455, Screen.height/2 + 100, 455, 90), "", exit)) 
		{
			Application.Quit();
		}
	}

	private void localLevelSelection()
	{
		//the logo - box
		GUI.Box(new Rect(0,0, 425, 310), "", logo);

		// kleine Hintergrundbox erstellen
		GUI.Box(new Rect(Screen.width/2 - 235, Screen.height/2 - 265, 470, 530), "Levelauswahl", customBox);
		
		//erster Button, falls gedrückt, wird das erste Level geladen
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 - 220, 350, 65), "Derby-Arena 1", customButton)) 
		{
			PlayerPrefs.SetString("Level","ArenaStadium");
			Application.LoadLevel("ChooseCar");
		}
		//Derby 2
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 - 150, 350, 65), "Derby-Arena 2", customButton)) 
		{
			PlayerPrefs.SetString("Level","ArenaStadium02");
			Application.LoadLevel("ChooseCar");
		}
		//drittes Level
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 - 80, 350, 65), "Wüsten-Strecke 1", customButton)) 
		{
			PlayerPrefs.SetString("Level","DesertCircuit");
			Application.LoadLevel("ChooseCar");
		}
		//....
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 - 10, 350, 65), "Wüsten-Strecke 2", customButton)) 
		{
			PlayerPrefs.SetString("Level","DesertCircuit02");
			Application.LoadLevel("ChooseCar");
		}
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 + 60, 350, 65), "Wald-Strecke", customButton)) 
		{
			PlayerPrefs.SetString("Level","ForestCircuit");
			Application.LoadLevel("ChooseCar");
		}

		//button, um zurück zum Hauptmenü zu gehen
		if(GUI.Button(new Rect(Screen.width/2 - 175, Screen.height/2 + 135, 350, 65), "Hauptmenü", customButton))
		{
			currentMenu = "Main";
		}
	}
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Diese Klasse initialisiert die Fahrzeuge
 */

public class GameController : MonoBehaviour 
{
	//Anzahl der Fahrzeug auf der Strecke
	public int numberOfCars;
	//Anzhal der Spieler am lokaler Rechner
	public int numberOfLocalPlayers;
	//Startpunkte
	public Transform[] spawnPoints;
	//Liste mit Spielern
	private List<GameObject> playerList;

	//diese Methode instanziert 
	void Start()
	{

	}

	private void instaciateNewPlayer(Vector3 position, string playerName)
	{

	}
}
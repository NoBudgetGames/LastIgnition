using UnityEngine;
using System.Collections;

/*
 * Diese Klasse stellt die Spieler Infos eines einzelnen Spielers dar. Dieses Objekt bleibt über
 * den Verlauf des Multiplayerspierl bestehen
 * Sie enthält im wesentlichen Spielername, dessen gewähltes Auto und ob er bereit ist
 */ 

public class NetworkPlayerData : MonoBehaviour 
{
	//die Infos über diesen Spieler, netID, Name, Auto, bereit
	private string[] playerData;
	//die NetworkView, die an diesen Objekt dranhängt
	private NetworkView netView;

	// Use this for initialization
	void Awake() 
	{
		//Objekt soll bestehen bleiben
		DontDestroyOnLoad(this);
		netView = gameObject.GetComponent<NetworkView>();
		// NetworkView ID | Spielername | gewähltes Autos (Default: Dodge Charger Lee), abhängig vom ersten Auto in Carselection | ist Spieler bereit?
		playerData = new string[]{netView.viewID.ToString(), PlayerPrefs.GetString("PlayerOneName"), "Dodge Charger Lee", "nicht bereit"};
	}

	void Update()
	{
		//Objekt soll bestehen bleiben
		DontDestroyOnLoad(this);
	}

	public void setAll(string[] data)
	{
		playerData = data;
	}

	//setzt die NetworkID, bzw. den string davon
	public void setID(string ID)
	{
		playerData[0] = ID;
	}

	//Diese Methode setzt den Spielernamen in abhängigkeit vom ControllerString
	public void setControllerString(string controller)
	{
		if(controller.Equals("One"))
		{
			playerData[1] = PlayerPrefs.GetString("PlayerOneName");
		}
		else
		{
			playerData[1] = PlayerPrefs.GetString("PlayerTwoName");
		}
	}

	//Diese Methode aktuallisiert den Namen des gewählten Autos
	public void setChoosenCar(string carName)
	{
		playerData[2] = carName;
	}

	//diese Methode macht den Spieler bereit
	public void setReady(bool ready)
	{
		if(ready == true)
		{
			playerData[3] = "bereit";
		}
		else
		{
			playerData[3] = "nicht bereit";
		}
	}

	public string[] getPlayerData()
	{
		return playerData;
	}
}
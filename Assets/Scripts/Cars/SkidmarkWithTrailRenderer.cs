using UnityEngine;
using System.Collections;

/*
 * Diese Klasse zeichnet rutschspuren auf dem Boden
 * Sie benutzt dazu einen TrailRenderer, der die SKidmarks zeichnet. Der TrailRenderer zieht quasi eine
 * Spur hinter sich her, die eine betimmte Maximallänge hat.
 */

public class SkidmarkWithTrailRenderer : MonoBehaviour 
{

//// FARBEN DER VERSCHIEDENEN UNTERGRÜNDE

	//fester Sand, SandNormal
	public Color colorSandNormal;
	//loser Sand, SandLose
	public Color colorSandLose;
	//Schotter, Rubble
	public Color colorRubble;
	//Erdweg, Dirt
	public Color colorDirt;
	//Grass, Grass
	public Color colorGrass;

	//TrailRender Komponente, um die Skidmarks zu rendern
	private TrailRenderer trailRend;
	//Layer des Untergrundes, sobald die Layer geändert wird, wird auch die Farbe des MAterials des Trailrenderes geändert
	private int groundLayer;
	//Zeit seit dem remove Befehl
	private float timeSinceRemove = -1.0f;

//// START UND UPDATEMETHODEN

	// Use this for initialization
	void Awake () 
	{
		trailRend = GetComponent<TrailRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//falls der Timer größer als die zulässige Zeit für den TrailRenderer ist, zerstöre das Object
		if(timeSinceRemove > trailRend.time)
		{
			GameObject.Destroy(transform.gameObject);
			//Material wird nicht gelöscht, daher muss man die nicht benutzten Assets wieder "entladen" bzw. den Speicher freimachen
			Resources.UnloadUnusedAssets();
		}
		//falls der Zähler gestartet wurde, zähle ihn hoch
		if(timeSinceRemove >= 0.0f)
		{
			timeSinceRemove += Time.deltaTime;
		}
	}

//// GET UND SET METHODEN

	//diese Methode gibt den int Wert der GroundLayer zurürk
	public int getGroundLayer()
	{
		return groundLayer;
	}

	//diese Methode setzt die GrpundLayer, damit die Komonente weiss, auf welchen Untergrund sie sich gerade befindet
	//entprechend wird dann die Farbe des Materials geändert
	public void setGroundLayer(int layer)
	{
		groundLayer = layer;
		//schau auf welcher Layer sich der Reifen befindet uhdändere entsprechend die Farbe des Materials
		//switch erlaubt keine abfragen wie "case LayerMask.NameToLayer("Default"):", daher feste Werte
		//Compiler Error CS0150: A constant value is expected
		switch(layer)
		{
		case 9:
			//fester Sand, SandNormal
			trailRend.material.color = colorSandNormal;
			break;
		case 10:
			//loser Sand, SandLose
			trailRend.material.color = colorSandLose;
			break;
		case 11:
			//Schotter, Rubble
			trailRend.material.color = colorRubble;
			break;
		case 12:
			//Erdweg, Dirt
			trailRend.material.color = colorDirt;
			break;
		case 13:
			//Grass, Grass
			trailRend.material.color = colorGrass;
			break;
		default:
			//asphalt, Default Layer (mach nichts)
			break;
		}
	}

//// SONSTIGES

	//sobald das Auto nicht mehr rutscht oder die Layer auf dem er fährt gewechslent hat, wird die Skidmark
	//nicht mehr erzeugt und wird an der letzten Position nach unten verschoben um sie zu verstecken
	public void removeSkidmark()
	{
		//Skidmark unter die Straße verschieben, da der TrailRenderer noch weiterarbeitet
		transform.Translate(-Vector3.up);
		//Timer reseten
		timeSinceRemove = 0.0f;
	}
}
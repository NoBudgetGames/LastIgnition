
using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour
{
	public GameObject cameraObject;
	CarInventory inventory;
	Car car;
	CameraController camControl;
	public LayerMask layer;
	public string player;

	public GUITexture weapon;
	public GUIText ammo;
	public Texture2D speedo;
	public Texture2D speedoArrow;
	public GUIText rank;
	public GUIText modeInfo;
	public GUITexture health;
	public GUITexture healthFrame;

	//wurde das Rennen für dieses HUD schon beendet? Wenn ja, soll er nur den Namen des Autos anzeigen, das er gerade verfolgt
	public bool raceEnded = false;

	int numberOfHuds = -1;

	float healthMaxBorderValue;
	float maxHealth;

	float speedoSizeX, speedoSizeY, speedoArrowSizeX, speedoArrowSizeY, offset, initialScreenWidth;
	// Use this for initialization
	void Start ()
	{
		camControl = cameraObject.GetComponent<CameraController>();
		car = camControl.targetCar;
		inventory = car.GetComponent<CarInventory>(); 

		weapon.gameObject.layer = layer;
		ammo.gameObject.layer = layer;
		rank.gameObject.layer = layer;
		modeInfo.gameObject.layer = layer;
		health.gameObject.layer = layer;
		healthFrame.gameObject.layer = layer;
		this.gameObject.layer = layer;

		healthMaxBorderValue = health.transform.localScale.x;
		maxHealth = car.getHealth();

		speedoSizeY = Screen.height/4f;
		speedoSizeX = Screen.height/4f;
		initialScreenWidth = Screen.width;
		speedoArrowSizeX = speedoSizeX;
		speedoArrowSizeY = speedoSizeY;
		offset = 15.0f;
	}

	// Update is called once per frame
	void Update ()
	{
		//falls das Rennen für diesen HUD noch läuft, stelle die Infos dar
		if(raceEnded == false)
		{
			speedoSizeY = Screen.height/4f* Screen.width/initialScreenWidth;
			speedoSizeX = Screen.height/4f * Screen.width/initialScreenWidth;

			speedoArrowSizeX = Screen.height/4f * Screen.width/initialScreenWidth;
			speedoArrowSizeY = Screen.height/4f * Screen.width/initialScreenWidth;

			weapon.texture = inventory.equippedWeapon.hudIcon;
			ammo.text = ""+inventory.equippedWeapon.remainingAmmo();
			if(numberOfHuds == -1){
				if(PlayerPrefs.GetInt("LocalPlayers") != 0){
					numberOfHuds = PlayerPrefs.GetInt("LocalPlayers");
				} else{
					numberOfHuds = GameObject.FindGameObjectsWithTag("HUD").Length;
				}
			}

			if(numberOfHuds == 1){
				offset=30.0f;
			}
			float w = car.getHealth()/maxHealth * healthMaxBorderValue;
			if(w >=0){
				Vector3 newVec = new Vector3(w,health.transform.localScale.y,health.transform.localScale.z);
				health.transform.localScale = newVec;
				//health.pixelInset = new Rect(health.pixelInset.x,health.pixelInset.y,w,health.pixelInset.height);
			} else {
				Vector3 newVec = new Vector3(0,health.transform.localScale.y,health.transform.localScale.z);
				health.transform.localScale = newVec;
			}
		}
		//ansonsten muss es nicht mehr dargestellt werden
		else
		{
			ammo.text = "";
			rank.text = "";
			modeInfo.text = "";
		}
	}

	void OnGUI(){
		//falls das Rennen für diesen HUD noch läuft, stelle die Infos dar
		if(raceEnded == false)
		{
			float angle = car.getVelocityInKmPerHour();
			if(numberOfHuds > 1){
				if(player == "One"){
					
					GUI.DrawTexture(new Rect(60, Screen.height/2-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedo);
					GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));
					GUIUtility.RotateAroundPivot(-125.0f,new Vector2(speedoSizeX/2+60,Screen.height/2-speedoSizeY/2-offset+4f));
					GUIUtility.RotateAroundPivot(angle,new Vector2(speedoSizeX/2+60,Screen.height/2-speedoSizeY/2-offset+4f));
					GUI.DrawTexture(new Rect(60,Screen.height/2-speedoSizeY-offset,speedoArrowSizeX,speedoArrowSizeY),speedoArrow);
					GUI.EndGroup();
					
				} else {
					GUI.DrawTexture(new Rect(60, Screen.height-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedo);
					GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));
					GUIUtility.RotateAroundPivot(-125.0f,new Vector2(speedoSizeX/2+60,Screen.height-speedoSizeY/2-offset+4f));
					GUIUtility.RotateAroundPivot(angle,new Vector2(speedoSizeX/2+60,Screen.height-speedoSizeY/2-offset+4f));
					GUI.DrawTexture(new Rect(60,Screen.height-speedoSizeY-offset,speedoArrowSizeX,speedoArrowSizeY),speedoArrow);
					GUI.EndGroup();
				}
			} else {
				GUI.DrawTexture(new Rect(60, Screen.height-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedo);
				GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));
				GUIUtility.RotateAroundPivot(-125.0f,new Vector2(speedoSizeX/2+60,Screen.height-speedoSizeY/2-offset+4f));
				GUIUtility.RotateAroundPivot(angle,new Vector2(speedoSizeX/2+60,Screen.height-speedoSizeY/2-offset+4f));
				GUI.DrawTexture(new Rect(60,Screen.height-speedoSizeY-offset,speedoArrowSizeX,speedoArrowSizeY),speedoArrow);
				GUI.EndGroup();
			}
		}
		//ansonsten zeige an, welchen SPieler man gerade verfolgt
		else
		{
			//"deaktiviere" die restlichen Texturen, die noch gezeignet werden 
			health.texture = null;
			healthFrame.texture = null;
			weapon.texture = null;
			//falls mehrere HUDs vorhanden sind
			if(numberOfHuds > 1)
			{
				if(player != "One")
				{
					GUI.Box(new Rect(Screen.width/2 - 100, 50, 200, 25), "Spectating: " + camControl.targetCar.GetComponent<PlayerInputController>().playerName);
				} 
				else
				{
					GUI.Box(new Rect(Screen.width/2 - 100, Screen.height/2 + 50, 200, 25), "Spectating: " + camControl.targetCar.GetComponent<PlayerInputController>().playerName);
				}
			}
			//ansonsten ist es nur ein HUD
			else
			{
				GUI.Box(new Rect(Screen.width/2 - 100, 50, 200, 25), "Spectating: " + camControl.targetCar.GetComponent<PlayerInputController>().playerName);
			}
		}


	}

}
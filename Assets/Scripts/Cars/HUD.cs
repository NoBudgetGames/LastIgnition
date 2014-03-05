
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
		}
	}

	void OnGUI(){
		float angle = car.getVelocityInKmPerHour();
		float x = Screen.width/initialScreenWidth;
		if(numberOfHuds > 1){
			if(player != "One"){

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

}
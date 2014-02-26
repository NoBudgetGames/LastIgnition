
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

	int numberOfHuds = -1;

	float healthMaxBorderValue;
	float maxHealth;

	float speedoSizeX, speedoSizeY, offset;
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
		this.gameObject.layer = layer;
		
		speedoSizeY = Screen.height/3f;
		speedoSizeX = speedoSizeY;
		offset = 0.0f;

		healthMaxBorderValue = health.pixelInset.width;
		maxHealth = car.getHealth();

	}

	// Update is called once per frame
	void Update ()
	{
		weapon.texture = inventory.equippedWeapon.hudIcon;
		ammo.text = ""+inventory.equippedWeapon.remainingAmmo();
		if(numberOfHuds == -1){
			if(PlayerPrefs.GetInt("LocalPlayers") != 0){
				numberOfHuds = PlayerPrefs.GetInt("LocalPlayers");
			} else{
				numberOfHuds = GameObject.FindGameObjectsWithTag("HUD").Length;
			}
		}

		if(numberOfHuds == 2){
			
		}
		float w = car.getHealth()/maxHealth * healthMaxBorderValue;
		if(w >=0)
			health.pixelInset = new Rect(health.pixelInset.x,health.pixelInset.y,w,health.pixelInset.height);
	}

	void OnGUI(){
		float angle = car.getVelocityInKmPerHour();
		if(numberOfHuds > 1){
			if(player != "One"){
				GUI.DrawTexture(new Rect(0, Screen.height/2-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedo);
				GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));
				GUIUtility.RotateAroundPivot(-125.0f,new Vector2(speedoSizeX/2,Screen.height/2-speedoSizeY/2-offset+4f));
				GUIUtility.RotateAroundPivot(angle,new Vector2(speedoSizeX/2,Screen.height/2-speedoSizeY/2-offset+4f));
				GUI.DrawTexture(new Rect(0,Screen.height/2-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedoArrow);
				GUI.EndGroup();
				
			} else {
				GUI.DrawTexture(new Rect(0, Screen.height-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedo);
				GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));
				GUIUtility.RotateAroundPivot(-125.0f,new Vector2(speedoSizeX/2,Screen.height-speedoSizeY/2-offset+4f));
				GUIUtility.RotateAroundPivot(angle,new Vector2(speedoSizeX/2,Screen.height-speedoSizeY/2-offset+4f));
				GUI.DrawTexture(new Rect(0,Screen.height-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedoArrow);
				GUI.EndGroup();
			}
		} else {
			GUI.DrawTexture(new Rect(0, Screen.height-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedo);
			GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));
			GUIUtility.RotateAroundPivot(-125.0f,new Vector2(speedoSizeX/2,Screen.height-speedoSizeY/2-offset+4f));
			GUIUtility.RotateAroundPivot(angle,new Vector2(speedoSizeX/2,Screen.height-speedoSizeY/2-offset+4f));
			GUI.DrawTexture(new Rect(0,Screen.height-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedoArrow);
			GUI.EndGroup();
		}
	}

}
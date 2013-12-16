
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
	int numberOfHuds;

	float speedoSizeX, speedoSizeY, offset;
	// Use this for initialization
	void Start ()
	{
		camControl = cameraObject.GetComponent<CameraController>();
		car = camControl.targetCar;
		inventory = car.GetComponent<CarInventory>(); 

		weapon.gameObject.layer = layer;
		ammo.gameObject.layer = layer;
		this.gameObject.layer = layer;

		speedoSizeX = Screen.width/5;
		speedoSizeY = Screen.height/5;
		offset = 50.0f;
	}

	// Update is called once per frame
	void Update ()
	{
		weapon.texture = inventory.equippedWeapon.hudIcon;
		ammo.text = ""+inventory.equippedWeapon.remainingAmmo();
		numberOfHuds = GameObject.FindGameObjectsWithTag("HUD").Length;

	}

	void OnGUI(){
		float angle = car.getVelocity();
		if(numberOfHuds > 1){
			if(player != "One"){
				GUI.DrawTexture(new Rect(0, Screen.height/2-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedo);
				GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));
				GUIUtility.RotateAroundPivot(-135.0f,new Vector2(speedoSizeX/2,Screen.height/2-speedoSizeY/2-offset));
				GUIUtility.RotateAroundPivot(angle,new Vector2(speedoSizeX/2,Screen.height/2-speedoSizeY/2-offset));
				GUI.DrawTexture(new Rect(0,Screen.height/2-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedoArrow);
				GUI.EndGroup();
				
			} else {
				GUI.DrawTexture(new Rect(0, Screen.height-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedo);
				GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));
				GUIUtility.RotateAroundPivot(-135.0f,new Vector2(speedoSizeX/2,Screen.height-speedoSizeY/2-offset));
				GUIUtility.RotateAroundPivot(angle,new Vector2(speedoSizeX/2,Screen.height-speedoSizeY/2+-offset));
				GUI.DrawTexture(new Rect(0,Screen.height-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedoArrow);
				GUI.EndGroup();
			}
		} else {
			GUI.DrawTexture(new Rect(0, Screen.height-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedo);
			GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));
			GUIUtility.RotateAroundPivot(-135.0f,new Vector2(speedoSizeX/2,Screen.height-speedoSizeY/2-offset));
			GUIUtility.RotateAroundPivot(angle,new Vector2(speedoSizeX/2,Screen.height-speedoSizeY/2-offset));
			GUI.DrawTexture(new Rect(0,Screen.height-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedoArrow);
			GUI.EndGroup();
		}
	}

}
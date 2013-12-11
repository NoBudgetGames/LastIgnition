
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
	// Use this for initialization
	void Start ()
	{
		camControl = cameraObject.GetComponent<CameraController>();
		car = camControl.targetCar;
		inventory = car.GetComponent<CarInventory>(); 

		weapon.gameObject.layer = layer;
		ammo.gameObject.layer = layer;
		this.gameObject.layer = layer;
	}

	// Update is called once per frame
	void Update ()
	{
		weapon.texture = inventory.equippedWeapon.hudIcon;
		ammo.text = ""+inventory.equippedWeapon.remainingAmmo();

	}

	void OnGUI(){
		float angle = car.getVelocity();
		if(player != "One"){
			GUI.DrawTexture(new Rect(0, Screen.height/2-140,140,140),speedo);
			GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));
			GUIUtility.RotateAroundPivot(-135.0f,new Vector2(70,Screen.height/2-70));
			GUIUtility.RotateAroundPivot(angle,new Vector2(70,Screen.height/2-70));
			GUI.DrawTexture(new Rect(0,Screen.height/2-140,140,140),speedoArrow);
			GUI.EndGroup();
			
		} else {
			GUI.DrawTexture(new Rect(0, Screen.height-140,140,140),speedo);
			GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));
			GUIUtility.RotateAroundPivot(-135.0f,new Vector2(70,Screen.height-70));
			GUIUtility.RotateAroundPivot(angle,new Vector2(70,Screen.height-70));
			GUI.DrawTexture(new Rect(0,Screen.height-140,140,140),speedoArrow);
			GUI.EndGroup();
		}
	}

}
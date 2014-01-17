
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
	protected Camera miniMap;

	int numberOfHuds = -1;

	bool miniMapOneSet = false ,miniMapTwoSet = false, miniMapTwoInstantiated = false;

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

		speedoSizeX = Screen.width/5;
		speedoSizeY = Screen.height/5;
		offset = 50.0f;

		if(GameObject.FindGameObjectWithTag("MiniMap") != null)
			miniMap = GameObject.FindGameObjectWithTag("MiniMap").camera;
		else
			Debug.LogError("Please place and adjust the minimap prefab");

		healthMaxBorderValue = health.pixelInset.width;
		maxHealth = car.getHealth();

	}

	// Update is called once per frame
	void Update ()
	{
		weapon.texture = inventory.equippedWeapon.hudIcon;
		ammo.text = ""+inventory.equippedWeapon.remainingAmmo();
		if(numberOfHuds == -1){
			numberOfHuds = GameObject.FindGameObjectsWithTag("HUD").Length;
		}
		if(player == "Two" && !miniMapTwoInstantiated && miniMap != null){
			GameObject newMap = (GameObject)GameObject.Instantiate(GameObject.FindGameObjectWithTag("MiniMap"));
			miniMap = newMap.camera;
			miniMapTwoInstantiated = true;
		}
		float w = car.getHealth()/maxHealth * healthMaxBorderValue;
		if(w >=0)
			health.pixelInset = new Rect(health.pixelInset.x,health.pixelInset.y,w,health.pixelInset.height);
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

				if(!miniMapTwoSet && miniMap !=null){
					Rect viewRect = miniMap.rect;
					viewRect.y = 0.18f;
					miniMap.rect = viewRect;
					miniMapTwoSet = true;
				}
				
			} else {
				GUI.DrawTexture(new Rect(0, Screen.height-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedo);
				GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));
				GUIUtility.RotateAroundPivot(-135.0f,new Vector2(speedoSizeX/2,Screen.height-speedoSizeY/2-offset));
				GUIUtility.RotateAroundPivot(angle,new Vector2(speedoSizeX/2,Screen.height-speedoSizeY/2+-offset));
				GUI.DrawTexture(new Rect(0,Screen.height-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedoArrow);
				GUI.EndGroup();

				if(!miniMapOneSet && miniMap !=null){
					Rect viewRect = miniMap.rect;
					viewRect.y = 0.71f;
					miniMap.rect = viewRect;
					miniMapOneSet = true;
				}
			}
		} else {
			GUI.DrawTexture(new Rect(0, Screen.height-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedo);
			GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));
			GUIUtility.RotateAroundPivot(-135.0f,new Vector2(speedoSizeX/2,Screen.height-speedoSizeY/2-offset));
			GUIUtility.RotateAroundPivot(angle,new Vector2(speedoSizeX/2,Screen.height-speedoSizeY/2-offset));
			GUI.DrawTexture(new Rect(0,Screen.height-speedoSizeY-offset,speedoSizeX,speedoSizeY),speedoArrow);
			GUI.EndGroup();

			if(!miniMapOneSet && miniMap !=null){
				Rect viewRect = miniMap.rect;
				viewRect.y = 0.71f - viewRect.height;
				viewRect.x = viewRect.x - viewRect.width;
				viewRect.width  = viewRect.width * 2;
				viewRect.height = viewRect.height * 2;
				miniMap.rect = viewRect;
				miniMapOneSet = true;
			}
		}
	}

	void OnDestroy(){
		if(player == "Two"){
			GameObject.Destroy(miniMap.gameObject);
		}
	}

}
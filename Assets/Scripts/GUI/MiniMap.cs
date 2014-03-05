using UnityEngine;
using System.Collections;

public class MiniMap : MonoBehaviour
{
	int localPlayers;
	
	GameObject secondMap;

	public GUITexture frame;
	// Use this for initialization
	void Start ()
	{
		localPlayers = PlayerPrefs.GetInt("LocalPlayers");
		
		if(localPlayers == 1){

			Rect viewRect = this.camera.rect;


			viewRect.x = viewRect.x -viewRect.width/1.5f;
			viewRect.width  = viewRect.width * 1.5f;
			viewRect.height = viewRect.height * 1.5f;
			viewRect.y = 0.84f - viewRect.height/1.5f;
			this.camera.rect = viewRect;

		} else {
			secondMap = (GameObject)GameObject.Instantiate(this.gameObject);
			secondMap.GetComponent<MiniMap>().enabled = false;

			
			Rect viewRect = this.camera.rect;
			viewRect.y = 0.84f;
			this.camera.rect = viewRect;
			
			Rect viewRect2 = this.camera.rect;
			viewRect2.y = 0.34f;
			secondMap.camera.rect = viewRect2;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}


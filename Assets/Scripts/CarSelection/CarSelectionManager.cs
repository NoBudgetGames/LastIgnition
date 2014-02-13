using UnityEngine;
using System.Collections;

public class CarSelectionManager : MonoBehaviour
{
	public CarSelection[] selectors;
	public GameObject[] presentators;

	// Use this for initialization
	void Start ()
	{
		if(PlayerPrefs.GetInt("LocalPlayers") == 1){
			GameObject.Destroy(selectors[1].getSelectedCarObject());
			GameObject.Destroy(selectors[1].gameObject);
			CarSelection[] tmp = new CarSelection[1];
			tmp[0] = selectors[0];
			selectors = tmp;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		bool allSelected = true;
		for(int i = 0; i<selectors.Length; ++i){
			if(!selectors[i].playerReady){
				allSelected = false;
				break;
			}
		}

		if(allSelected){
			for(int i = 0; i<selectors.Length; ++i){
				PlayerPrefs.SetInt(selectors[i].playerName,selectors[i].getCarTypeIndex());
			}
			Application.LoadLevel(PlayerPrefs.GetString("Level"));
		}

		float axis = 5.0f*Time.deltaTime;
		for(int i = 0; i<selectors.Length; ++i){
			presentators[i].transform.Rotate(presentators[i].transform.up,axis);
			selectors[i].getSelectedCarObject().transform.Rotate(selectors[i].getSelectedCarObject().transform.up,axis);
			axis*=-1;
		}
	}
}


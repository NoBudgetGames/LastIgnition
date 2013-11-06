using UnityEngine;
using System.Collections;

public class RocketLauncher : MonoBehaviour
{
	public GameObject rocketPrefab;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetMouseButtonDown(0)){
			GameObject rocket = (GameObject) GameObject.Instantiate(rocketPrefab);
			rocket.transform.position = this.transform.position;
		}
	}
}


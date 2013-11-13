using UnityEngine;
using System.Collections;

public class RocketLauncher : MonoBehaviour
{
	public GameObject rocketPrefab; //Prefab der Rakete
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Eine Raketenobjekt wird aus dem Prefab erstellt und die Rotation und Position werden abhängig vom Auto
		//angepasst.
		if(Input.GetMouseButtonDown(0)){
			GameObject rocket = (GameObject) GameObject.Instantiate(rocketPrefab);
			rocket.transform.position = this.transform.position;
			rocket.transform.Rotate(rocket.transform.up,-this.transform.parent.transform.eulerAngles.y,Space.Self);
			rocket.GetComponent<Rocket>().parent = transform.parent.gameObject;
		}
	}
}


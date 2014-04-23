using UnityEngine;
using System.Collections;

public class MiniMapElement : MonoBehaviour
{

	public GameObject parent;
	public Color color;
	Quaternion initialRotation;
	// Use this for initialization
	void Start ()
	{
		initialRotation = this.transform.rotation;
		this.GetComponent<SpriteRenderer>().color = color;
	}

	// Update is called once per frame
	void Update ()
	{
		if(parent != null){
			Vector3 newPos = new Vector3(parent.transform.position.x,this.transform.position.y,parent.transform.position.z);
			this.transform.position = newPos;
			this.transform.rotation = Quaternion.identity;
			this.transform.Rotate(90.0f,parent.transform.rotation.eulerAngles.y,0.0f,Space.Self);
		}
	}

}


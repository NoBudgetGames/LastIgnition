using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {
	
	
	public float AccelarationForce = 100;
	public float TurnForce = 100;
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		getInput();
		
	
	}
	
	private void getInput()
	{
		if(Input.GetKey(KeyCode.UpArrow))
		{
			rigidbody.AddRelativeForce(Vector3.forward * AccelarationForce, ForceMode.Acceleration);
		}
		
		if(Input.GetKey(KeyCode.DownArrow))
		{
			rigidbody.AddRelativeForce(Vector3.forward * AccelarationForce * (-1), ForceMode.Acceleration);
		}
		
		if(Input.GetKey(KeyCode.RightArrow))
		{
			rigidbody.AddRelativeTorque(Vector3.up * TurnForce, ForceMode.Acceleration);
		}
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			rigidbody.AddRelativeTorque(Vector3.up * TurnForce * (-1), ForceMode.Acceleration);
		}
	}
	
}

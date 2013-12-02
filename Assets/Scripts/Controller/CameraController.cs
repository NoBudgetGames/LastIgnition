using UnityEngine;
using System.Collections;
/*
 * diese Klasse ist für die verschiedenen Kamerapositionen am AUto zuständig. Die Position wird dabei in jeden FixedUpdate
 * neu errechnet
 * sie verarbeite auch den Input vom Spieler
 */

public class CameraController : MonoBehaviour 
{
	//referenz zum Auto, für Verfolgerkamera
	public Car targetCar;
	//referenz auf Transform für Kamera auf der Motothaube
	public Transform hoodCamera;
	//referenz auf Transform für Kamera auf kofferraum, um nach hinten zu gucken
	public Transform hoodCameraLookBack;
	//die Dämpfung, mit dem die Kamera an die position gehalten werden soll in Z Richtung (längsachse)
	public int floatingCamDampingZ = 30;
	//die Dämpfung, mit dem die Kamera an die position gehalten werden soll in X Richtung (seitliche Achse)
	public int floatingCamDampingX = 20;
	//Entfernung zum Ziel
	public float distanceToTarget = 15.0f;
	//mindestabstand zum Boden
	public float heightOnGround = 10.0f;
	//Höhe über dem Auto, wo die Kamera hingucken soll
	public float viewHeight = 2.0f;
	//multiplikator für die Hohe Verfolgerkamera
	public float highCamMultiplier = 1.5f;

	//an welcher Position befindet sich die Kamera?
	private CameraPosition camPos = CameraPosition.REAR_FLOATING_LOW;
	//schauen wir gerade nacht hinten?
	private bool lookingBack = false;

	//da der D-Pad auf dem 360 Controller eine Achse darstellt und nicht mit GetButtonDown angesprochen werden kann, 
	//stellt changed eine Hilfvariable dar um festzustellen, ob die Kamera schon geändert wurde, solange der Spieler
	//den D-Pad noch nach unten drückt
	private bool changed = false;

	void Start()
	{
		//chasingCam(false, lookingBack);
		//transform.parent = null;
	}

	//in dieser Methode wird die Kamera position errechnet
	//FixedUpdate da die Kameraposition sonst leicht "ruckelt"
	void FixedUpdate()
	{
		//schaue an welcher Position sich die Kamera befinden soll
		switch(camPos)
		{
			case CameraPosition.REAR_FLOATING_LOW:
				chasingCam(false, lookingBack);
				transform.parent = null;
				break;
			case CameraPosition.REAR_FLOATING_HIGH:
				chasingCam(true, lookingBack);
				transform.parent = null;
				break;
			case CameraPosition.HOOD:
				hoodCam(lookingBack);
				//damit die Kamera auf der Motorhaube festsitzt, ist das Zielauto kurzerhand der Parent der Kamera
				transform.parent = targetCar.transform;
				break;
		}
	}

	//wird vom InputController aufgerufen
	public void lookBack(bool look)
	{
		lookingBack = look;
	}

	//diese Methode wählt die nächste Kamera aus
	public void cycleCamera(float cycle)
	{
		if(cycle < -0.8f && changed == false)
		{
			//Kameraposition hochzählen
			camPos++;
			//falls letzte Kameraposition, gehe zum Anfang zurück
			if(camPos == CameraPosition.NUM_OF_CAMS)
			{
				camPos = CameraPosition.REAR_FLOATING_LOW;
			}
			//Kamera wurde geändert
			changed = true;
		}
		else if(cycle == 0.0f && changed == true)
		{
			//Spieler drückt nicht mehr auf den D-Pad, d.h. wir erlauben ihm die Kamera wieder zu ändern
			changed = false;
		}
	}

	//diese Methode setzt die Kamera auf die Motorhaube bzw. auf den Kofferraum
	private void hoodCam(bool lookingBack)
	{
		//wenn wir nicht nach hinten gucken, soll die Kamera Position auf der Motorhaube sein
		if(lookingBack == false)
		{
			transform.position = hoodCamera.transform.position;
			transform.rotation = hoodCamera.transform.rotation;
		}
		//ansonsten auf dem Kofferraum
		else 
		{
			transform.position = hoodCameraLookBack.transform.position;
			transform.rotation = hoodCameraLookBack.transform.rotation;
		}
	}

	//diese Methode berechnet die Position einer Verfolgerkamera
	private void chasingCam(bool highCam, bool lookingBack)
	{
		//Multiplikator um nach hinten oder nach vorne zu gucken
		int lookingBackInt = 1;
		if(lookingBack == true)
		{
			//mal 2 da die Kamera zu nach am Auto ist
			lookingBackInt = -2;
		}

		//Multiplikator für die hohe Verfolgerkamera
		float highCamDistanceFloat = 1.0f;
		float highCamHeightFloat = 1.0f;
		if(highCam)
		{
			highCamDistanceFloat = highCamMultiplier;
			highCamHeightFloat = highCamMultiplier * 0.8f;
		}

		//Zielposition ist hinterm Auto bzw. vorm AUto beim Rückwärtsfahren
		Vector3 targetPosition = targetCar.transform.position;

		//nach unten raycasten um zu guckenn, ob die Kamera leicht über dem Auto ist,nur wen Fahrzeuf nahe am Boden ist
		RaycastHit hit;
		if(Physics.Raycast(transform.position, -transform.up, out hit, heightOnGround * highCamHeightFloat))
		{
			//falls wir ganz langsam sind, soll die Kamera hinterm Auto sein
			if(targetCar.getVelocity() < 1.0f)
			{
				targetPosition = targetCar.transform.TransformPoint(0.0f, 0.0f, -distanceToTarget * highCamDistanceFloat);
			}
			//falls am rückwärtsfahren ist Kamera vor dem Auto
			else if(targetCar.isCarReversing())
			{
				targetPosition = targetCar.transform.TransformPoint(0.0f, 0.0f, distanceToTarget * highCamDistanceFloat * lookingBackInt);
			}
			//ansonsten hinterm Auto
			else
			{
				targetPosition = targetCar.transform.TransformPoint(0.0f, 0.0f, -distanceToTarget * highCamDistanceFloat * lookingBackInt);
			}
			//y Position ist um height nach oben verschoben (überm Boden), 
			targetPosition.y = Mathf.Lerp(targetPosition.y, hit.point.y + heightOnGround * highCamHeightFloat, 0.9f);
		}
		//falls sich das Auto in der Luft befindet
		else
		{
			//ziel Position soll in negativer Geschwindigkeitsrichtung sein
			Vector3 negVelocity = Vector3.Normalize(new Vector3 (targetCar.rigidbody.velocity.x, 0.0f, targetCar.rigidbody.velocity.z));
			//richtiger Abstadd zum Auto
			negVelocity *= distanceToTarget * lookingBackInt;
			//richtige Höhe zum Auto
			negVelocity.y = -heightOnGround * highCamDistanceFloat;
			targetPosition = targetCar.transform.position - negVelocity;
		}

		//momentane Position soll langsam der Zielposition angepasst werden
		//nur wenn die Geschwindigkeit nicht sehr klein ist
		Vector3 tmpPos = transform.position;
		tmpPos.x = Mathf.Lerp(tmpPos.x, targetPosition.x, Time.deltaTime * floatingCamDampingX);
		tmpPos.y = targetPosition.y; //Mathf.Lerp(tmpPos.y, targetPosition.y, 0.7f);
		tmpPos.z = Mathf.Lerp(tmpPos.z, targetPosition.z, Time.deltaTime * floatingCamDampingZ);
		transform.position = tmpPos;

		//aktuallisiere die rotation
		//die Kamera soll nicht direkt auf das AUto gucken, sondern ein bischen darüber
		Vector3 tmpLookPos = new Vector3(targetCar.transform.position.x, targetCar.transform.position.y, targetCar.transform.position.z);
		tmpLookPos.y += viewHeight * highCamHeightFloat;
		transform.LookAt(tmpLookPos);
	}
}

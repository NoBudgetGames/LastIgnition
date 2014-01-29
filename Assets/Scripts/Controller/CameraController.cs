using UnityEngine;
using System.Collections;
/*
 * diese Klasse ist für die verschiedenen Kamerapositionen am Auto zuständig. Die Position wird dabei in jeden FixedUpdate
 * neu errechnet. Sie verarbeite auch den Input vom Spieler.
 * Die Klasse wird als Script-Komponente an einer Kamera angehängt.
 * (Eine Prefab existiert hierfür schon)
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
	//Entfernung zum Ziel für ferne Kamera
	public float distanceToTargetHigh = 15.0f;
	//mindestabstand zum Boden  für ferne Kamera
	public float heightOnGroundHigh = 12.0f;
	//Höhe über dem Auto, wo die Kamera hingucken soll, für ferne Kamera
	public float viewHeightHigh = 3.0f;

	//an welcher Position befindet sich die Kamera?
	private CameraPosition camPos = CameraPosition.REAR_FLOATING_LOW;
	//schauen wir gerade nacht hinten?
	private bool lookingBack = false;
	//Dampfüngswert für X Richtung
	private float dampX;

	//da der D-Pad auf dem 360 Controller eine Achse darstellt und nicht mit GetButtonDown angesprochen werden kann, 
	//stellt changed eine Hilfvariable dar um festzustellen, ob die Kamera schon geändert wurde, solange der Spieler
	//den D-Pad noch nach unten drückt
	private bool changed = false;

	void Start()
	{
		dampX = floatingCamDampingX;
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
			lookingBackInt = -1;
		}

		//Hilfsvariablen
		float distance = distanceToTarget;
		float height = heightOnGround;
		float targetHeight = viewHeight;
		int dampZ = floatingCamDampingZ;
		//falls die hohe Kamera benutzt werden soll, solen andere Werte verwendet werden
		if(highCam)
		{
			distance = distanceToTargetHigh;
			height = heightOnGroundHigh;
			targetHeight = viewHeightHigh;
		}

		//Zielposition ist erstmal hinter dem Auto
		Vector3 targetPosition = targetCar.transform.TransformPoint(0.0f, 0.0f, -distance * lookingBackInt);

		//nach unten raycasten um zu guckenn, ob die Kamera zu nah am Boden ist
		RaycastHit hit;
		if(Physics.Raycast(targetPosition, -Vector3.up, out hit, height))
		{
			//y Position ist um height nach oben verschoben (überm Boden), 
			targetPosition.y = Mathf.Lerp(targetPosition.y, hit.point.y + height, 0.9f);

			//falls der Abstand der Kamera zum Auto zu gross ist, wird der Dämpfungsfaktor in Z (Fahrrichtung) erhöht,
			//damit die Kamera nicht zu weit vom Auto weg ist
			if((this.transform.position - targetCar.transform.position).magnitude > (distance * 1.1f))
			{
				//dampZ *= 3;
			}
			
			//falls das Auto nicht am Lenken ist, soll die Kamera direkt hinter dem AUto sein
			if(targetCar.isSteering() == false)
			{
				if(dampX < (floatingCamDampingX * 3))
				{
					//dampX *= 1.05f;
				}
			}
			else
			{
				dampX = floatingCamDampingX;
			}
		}
		//falls die geschwindigkeit nicht sehr niedrig ist und die Kamera zu weit überm Boden ist
		else if(targetCar.getVelocityInKmPerHour() > 0.5f) 
		{
			//ziel Position soll in negativer Geschwindigkeitsrichtung sein
			Vector3 negVelocity = Vector3.Normalize(new Vector3 (targetCar.rigidbody.velocity.x, 0.0f, targetCar.rigidbody.velocity.z));
			//richtiger Abstand zum Auto
			negVelocity *= distance * lookingBackInt;
			//richtige Höhe zum Auto
			negVelocity.y = -height/2;
			targetPosition = targetCar.transform.position - negVelocity;
		}

		//momentane Position soll langsam der Zielposition angepasst werden
		Vector3 tmpPos = transform.position;
		tmpPos.x = Mathf.Lerp(tmpPos.x, targetPosition.x, Time.deltaTime * dampX);
		tmpPos.y = targetPosition.y; //Mathf.Lerp(tmpPos.y, targetPosition.y, 0.9f);
		tmpPos.z = Mathf.Lerp(tmpPos.z, targetPosition.z, Time.deltaTime * dampZ);
		transform.position = tmpPos;

		//aktuallisiere die rotation
		//die Kamera soll nicht direkt auf das AUto gucken, sondern ein bischen darüber
		Vector3 tmpLookPos = new Vector3(targetCar.transform.position.x, targetCar.transform.position.y, targetCar.transform.position.z);
		tmpLookPos.y += targetHeight;
		transform.LookAt(tmpLookPos);
	}
}
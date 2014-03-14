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
	//die Dämpfung, mit dem die Kamera an die position gehalten werden soll
	public int chaseCamDamping = 30;
	//Entfernung zum Ziel
	public float distanceToTarget = 15.0f;
	//die Höhe der Kamera relativ zum Auto
	public float heightOverCar = 10.0f;
	//Höhe über dem Auto, wo die Kamera hingucken soll
	public float viewHeight = 2.0f;
	//Entfernung zum Ziel für ferne Kamera
	public float distanceToTargetHigh = 15.0f;
	//die Höhe der fernen Kamera relativ zum Auto
	public float heightOverCarHigh = 12.0f;
	//Höhe über dem Auto, wo die Kamera hingucken soll, für ferne Kamera
	public float viewHeightHigh = 3.0f;

	//an welcher Position befindet sich die Kamera?
	private CameraPosition camPos = CameraPosition.REAR_FLOATING_LOW;
	//schauen wir gerade nacht hinten?
	private bool lookingBack = false;
	//wurde das Rennen für diesen CameraController schon beendet?
	private bool raceEnded = false;

	//da der D-Pad auf dem 360 Controller eine Achse darstellt und nicht mit GetButtonDown angesprochen werden kann, 
	//stellt changed eine Hilfvariable dar um festzustellen, ob die Kamera schon geändert wurde, solange der Spieler
	//den D-Pad noch nach unten drückt
	//dadurch wird die Kamere beim drücken des D-Pads nur einmal geändert 
	private bool changed = false;

	//in dieser Methode wird die Kamera position errechnet
	//FixedUpdate da die Kameraposition sonst leicht "ruckelt"
	void FixedUpdate()
	{
		//falls das Rennen für diese Kamera nicht beendet wurde, soll die Kamera für das eigene Auto gelten
		if(raceEnded == false)
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
		//ansonsten soll möglich sein, ein anderes Auto zu verfolgen
		else
		{
			chasingCam(false, false);
			transform.parent = null;
		}
		//gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(45,90, (targetCar.getVelocityInKmPerHour()-60)/100);
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
		float height = heightOverCar;
		float targetHeight = viewHeight;
		//falls die hohe Kamera benutzt werden soll, solen andere Werte verwendet werden
		if(highCam)
		{
			distance = distanceToTargetHigh;
			height = heightOverCarHigh;
			targetHeight = viewHeightHigh;
		}

		//Zielposition ist erstmal hinter dem Auto
		Vector3 targetPosition = targetCar.transform.TransformPoint(0.0f, height, -distance * lookingBackInt);

		//nach hinten raycasten um zu guckenn, ob die Kamera durch irgendwelche Objekte durchgeht
		RaycastHit hit;
		if(Physics.Raycast(targetPosition, -Vector3.forward, out hit, distance))
		{
			//falls was getroffen wurde, setze die Kamera Position an den Hitpoint
			targetPosition.x = hit.point.x;
			targetPosition.z = hit.point.z;
			//verschiede die Kamerapositon ein kleines bischen nach oben
			targetPosition.y = Mathf.Lerp(hit.point.y + 1.0f, targetPosition.y, Time.deltaTime * chaseCamDamping);
		}

		//momentane Position soll langsam der Zielposition angepasst werden
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * chaseCamDamping);

		//aktuallisiere die rotation
		//die Kamera soll nicht direkt auf das AUto gucken, sondern ein bischen darüber
		transform.LookAt(new Vector3(targetCar.transform.position.x, targetCar.transform.position.y + targetHeight, targetCar.transform.position.z));
	}

	public void changeTargetCar(bool finishedRace, Car newCar)
	{
		raceEnded = finishedRace;
		targetCar = newCar;
	}
}
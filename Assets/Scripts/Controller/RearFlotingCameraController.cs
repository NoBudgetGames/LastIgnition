using UnityEngine;
using System.Collections;
/*
 * diese Klasse berechnet die Position der Kamera die hinter dem Auto ist. Die Kamera folgt dem auto
 * und schwebt in einer kleinen Höhe. Sollte das Aut in der Luft sein, ist die Kamera direkt hinter dem Auto
 */

public class RearFlotingCameraController : MonoBehaviour 
{
	//referenz zum Auto, 
	public Car targetCar;
	//die Dämpfung, mit dem die Kamera an die position gehalten werden soll in Z Richtung (längsachse)
	public float dampingZ;
	//die Dämpfung, mit dem die Kamera an die position gehalten werden soll in X Richtung (seitliche Achse)
	public float dampingX;
	//Entfernung zum Ziel
	public float distance;
	//mindestabstand zum Boden
	public float height;
	//Höhe über dem Auto, wo die Kamera hingucken soll
	public float viewHeight;

	//in dieser Methode wird die Kamera position errechnet
	void FixedUpdate()
	{
		//Zielposition ist hinterm Auto bzw. vorm AUto beim Rückwärtsfahren
		Vector3 targetPosition = targetCar.transform.position;

		//nach unten raycasten um zu guckenn, ob die Kamera leicht über dem Auto ist,nur wen Fahrzeuf nahe am Boden ist
		RaycastHit hit;
		if(Physics.Raycast(transform.position, -transform.up, out hit, height))
		{
			//falls am rückwärtsfahren ist Kamera vor dem Auto
			if(targetCar.isCarReversing())
			{
				targetPosition = targetCar.transform.TransformPoint(0, Vector3.up.y + height, distance);
			}
			//ansonsten hinterm Auto
			else
			{
				targetPosition = targetCar.transform.TransformPoint(0, 0, -distance);
			}
			//y Position ist um height nach oben verschoben (überm Boden), Lerp von 0.7 da sonst die Kamera hin und her springt
			targetPosition.y = Mathf.Lerp(targetPosition.y, hit.point.y + height, 0.7f);
		}
		//falls sich das Auto in der Luft befindet
		else
		{
			//ziel Position soll in negativer Geschwindigkeitsrichtung sein
			Vector3 negVelocity = Vector3.Normalize(new Vector3 (targetCar.rigidbody.velocity.x, 0.0f, targetCar.rigidbody.velocity.z));
			//richtiger Abstadd zum Auto
			negVelocity *= distance;
			//richtige Höhe zum Auto
			negVelocity.y = -height;
			targetPosition = targetCar.transform.position - negVelocity;
		}
	
		//momentane Position soll langsam der Zielposition angepasst werden
		Vector3 tmpPos = transform.position;
		tmpPos.x = Mathf.Lerp(tmpPos.x, targetPosition.x, Time.deltaTime * dampingX);
		tmpPos.y = Mathf.Lerp(tmpPos.y, targetPosition.y, 0.7f);
		tmpPos.z = Mathf.Lerp(tmpPos.z, targetPosition.z, Time.deltaTime * dampingZ);
		transform.position = tmpPos;
		
		//aktuallisiere die rotation
		//die Kamera soll nicht direkt auf das AUto gucken, sondern ein bischen darüber
		Vector3 tmpLookPos = new Vector3(targetCar.transform.position.x, targetCar.transform.position.y, targetCar.transform.position.z);
		tmpLookPos.y += viewHeight;
		//tmpLookPos += Vector3.Normalize(targetCar.getVelocityVector()) * 3;
		transform.LookAt(tmpLookPos);
	}
}

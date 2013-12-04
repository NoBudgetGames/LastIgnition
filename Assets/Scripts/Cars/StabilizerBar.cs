using UnityEngine;
using System.Collections;

/*
 * Diese Klasse simuliert einen Stabilizator innerhalb eines Autos, um ihm vom Kippen/Rollen während dem Kurvenfahren anzuhalten
 * Für einen Stabilizator wird eine Achse mit einen Radpaar benötigt, nämlich rechtes Rad und linkes Rad. Da dies für jeder Achse gemacht werden muss, 
 * wurde die Funktionalität von der Car-Klasse getrennt. Dadurch ist es einfacher Fahrzeuge zu erstellen, die mehr als 4 Räder haben,
 * auch wenn wir das vermutlich nich benötigen. Sonst wäre das ein bischen eklig geworden, das im Car-Script abzuprüfen
 */

public class StabilizerBar: MonoBehaviour 
{
	//referenz auf linkes Rad
	public Wheel leftWheel;
	//referenz auf rechts Rad
	public Wheel rightWheel;
	//Stärke der Stabilizierkraft
	private float stabilizierMultiplier;
	//referenz auf rigidbody
	private Rigidbody thisRigidBody;
	
	// Use this for initialization
	void Start () 
	{
		thisRigidBody = rigidbody;
		//die maximal übertragene Kraft soll etwas niedriger sein als die Stärke (Kraft) der Feder
		stabilizierMultiplier = leftWheel.wheelCol.suspensionSpring.spring * 0.8f;
	}
	
	void FixedUpdate()
	{
		// nur ausführen falls eines der Räder nicht zerstört wurde
		if(leftWheel != null && rightWheel != null)
		{
			//wie weit würde die linke Feder der Aufhängung zusammengedrückt?
			float suspensionCompressionLeft = leftWheel.getSuspensionFactor();
			//wie weit wurde die rechte Feder der Aufhängung zusammengedrückt?
			float suspensionCompressionRight = rightWheel.getSuspensionFactor();
	
			//die maximale Kraft, die übertragen werden kann ist abhängig von der Differenz der beiden Federn
			//falls die (nicht absolute) Differenz > 0, dann fährt das AUto nach links (rechte Feder ist mehr zusammengedrückt als linke)
			//falls < 0 genau umgekehrt
			//die Kraft ist also vorzeichenbehaftet, je nach dem in welcher Schräglage sich das Auto befindet
			float stabilizierForce = (suspensionCompressionLeft - suspensionCompressionRight) * stabilizierMultiplier;
	
			//die Kraft soll nur aufgewendet werden, wenn beide Reifen den Boden berührt. Falls der Reifen den Boden nicht berührt, ist es eh schon zu spät ;)
			if(leftWheel.wheelCol.isGrounded)
			{
				//füge eine Kraft auf den Reifen hinzu, der ihm nach unten drückt, - da vorzeichenbehaftet für linke Seite
				thisRigidBody.AddForceAtPosition(leftWheel.transform.up * -stabilizierForce, leftWheel.transform.position);
			}
			if(rightWheel.wheelCol.isGrounded)
			{
				//hier kein -, da rechte Seite
				thisRigidBody.AddForceAtPosition(rightWheel.transform.up * stabilizierForce, rightWheel.transform.position);
			}
		}
	}
}

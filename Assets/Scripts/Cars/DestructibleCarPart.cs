using UnityEngine;
using System.Collections;

/* 
 * Diese KLasse it eine Implementierung der abstrackten KLasse AbstractDestructableObject
 * Sie stellt eine einzelne Schadeszone am Auto dar, sie ist ein Trigger, der den ankommenden 
 * Schaden auffängt und an die Car Klasse weiterleitet, welche auch die Lebenspunkte enthält.
 * das zugehörige GameObject enthält auch noch einen MeshCollider (Trigger).
 */

public class DestructibleCarPart : AbstractDestructibleObject
{
	//An welcher Stelle steht der Trigger im Auto?
	public DamageZone damageZone;
	//referent auf car
	public Car car;

	//diese Methode verarbeitet den eingegangenen Schaden
	[RPC]
	public override void receiveDamage(float damage)
	{
		car.applyDamage(damageZone, damage);
	}
}
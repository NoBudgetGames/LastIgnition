using UnityEngine;
using System.Collections;

/* 
 * Diese KLasse it eine Implementierung der abstrackten KLasse AbstractDestructableObject
 * Sie ist für die einzelnen Schadeszonen des Autos dar, sie ist ein trigger, der den ankommenden 
 * Schaden auffängt und an die Car Klasse weiterleitet, welche auch die Lebenspunkt enthält
 */

public class DestructibleCarPart : AbstractDestructibleObject
{
	//An welcher Stelle steht der Trigger im Auto?
	public DamageDirection direction;
	//referent auf car
	public Car car;

	//diese Methode verarbeitet den eingegangenen Schaden
	public override void receiveDamage(float damage)
	{
		car.applyDamage(direction, damage);
	}
}
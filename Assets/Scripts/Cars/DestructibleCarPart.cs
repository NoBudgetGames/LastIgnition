using UnityEngine;
using System.Collections;

/* Diese Klasse ist ein trigger, der den ankommenden Schaden auffängt und an die Car Klasse weiterleitet
 * An jeden Auto gib es mehrere davon
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
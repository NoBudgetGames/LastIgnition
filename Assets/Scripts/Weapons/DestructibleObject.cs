using UnityEngine;
using System.Collections;

/* diese KLasse it eine Implementierung ds Interfaces IDestructableObject
 * sie ist für alle Standard Objekte gedacht
 */

public class DestructibleObject : AbstractDestructibleObject
{	
	//die Lebenspunkte des Objects
	public float health = 100f;

	//Zieht schaden von Lebenspunkten ab
	public override void receiveDamage(float dmg)
	{
		health -= dmg;	
		//Wenn die Lebenspunkte 0 erreichen wird das Objekt zerstört
		if(health<=0.0f)
		{
			GameObject.Destroy(this.gameObject);
		}
	}
}


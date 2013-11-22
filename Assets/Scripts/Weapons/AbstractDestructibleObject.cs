using UnityEngine;
using System.Collections;

/*
 * das Interface das Schaden verarbeitet
 * */

public abstract class AbstractDestructibleObject : MonoBehaviour
{
	//diese Methode verarbeitet den schaden
	public abstract void receiveDamage(float damage);
}

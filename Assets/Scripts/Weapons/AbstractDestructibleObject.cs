using UnityEngine;
using System.Collections;

/*
 * die abstrakte KLasse die den Schaden verarbeitet
 * Alle Waffen rufen die receiveDamage MEthode beim getroffenen Objekt auf
 * Je nachdem ob es sich um einen Auto oder ein normales Objekt handelt werden
 * zwei verschiedene KLassen benötigt
 * 
 * Klassen die von dieser erben: DestructibleCarPart, DestructlibleObject
 */

public abstract class AbstractDestructibleObject : MonoBehaviour
{
	//diese Methode verarbeitet den schaden
	public abstract void receiveDamage(float damage);
}

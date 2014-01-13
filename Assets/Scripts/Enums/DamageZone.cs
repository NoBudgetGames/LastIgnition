using System;

/* 
 * Dieser Enum enthält die verschiedenen Schadenszonen am Auto
 * Der Wert selber wird vom Trigger am Fahrzeug benutzt, um im Car Script die applyDamage Methode aufzurufen
 */ 

public enum DamageZone
{
	NONE = -1,
	FRONT, 
	REAR, 
	LEFT, 
	RIGHT,
	FRONT_LEFT,
	FRONT_RIGHT,
	REAR_LEFT,
	REAR_RIGHT
};
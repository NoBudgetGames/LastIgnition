using UnityEngine;
using System.Collections;

/*
 * Diese statische Klasse ist für die Umwandlung von Zeitwerten in den entsprechenden Format zuständig
 */

public static class TimeConverter
{
	//diese Methode wandelt einen float-Wert ins folgendes Format: 09:04:05
	public static string floatToString(float time)
	{
		int minutes = (int)(time/60.0f);
		string minutesStr = "";
		if(minutes < 10)
		{
			minutesStr = "0" + minutes;
		}
		else
		{
			minutesStr = "" + minutes;
		}

		int seconds = (int)(time%60.0f);
		string secondsStr = "";
		if(seconds < 10)
		{
			secondsStr = "0" + seconds;
		}
		else
		{
			secondsStr = "" + seconds;
		}

		int milliseconds = (int)((time*1000.0f)%1000);
		//es soll nur auf 2 nachkomastellen genau angezeigt werden
		milliseconds /= 10;
		string millisecondsStr = "";
		if(milliseconds < 10)
		{
			millisecondsStr = "0" + milliseconds;
		}
		else
		{
			millisecondsStr = "" + milliseconds;
		}

		return minutesStr + ":" + secondsStr + ":" + millisecondsStr;
	}
}
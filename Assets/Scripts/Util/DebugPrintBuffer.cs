using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;



//------------------------------------------------------
/*
 				Debug Print Buffer
*/
//------------------------------------------------------

public class DebugPrintBuffer : MonoBehaviour 
{
	public static DebugPrintBuffer Instance;

	void Awake () 
	{
		Instance = this;
	}


	List<string> mdDebugPrintList = new List<string>();
	public void addToDPrintBuffer(string info)
	{
		mdDebugPrintList.Add(info);
	}

	public void flushDPrintBuffer()
	{
		string outputString = "debug log:\n";
		foreach (string line in mdDebugPrintList) {
			outputString += line + "\n";
		}
		Debug.Log (outputString);
		mdDebugPrintList = new List<string>();
	}


}
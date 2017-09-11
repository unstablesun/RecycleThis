using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UTIL
public partial class HexManager : MonoBehaviour 
{
	private GameObject QueryFindObjectByID(int id) 
	{
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript.ID == id && objectScript._Type == HexObject.eType.Main) {

				return tObj;
			}
		}
		return null;
	}

	public void QueryClearMarked() 
	{
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main) {
				objectScript.MarkedColor = (int)GemObject.eColorType.Black;
			}
		}
	}

	public void QueryClearScan() 
	{
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main) {
				objectScript.ScanColor = (int)GemObject.eColorType.Black;
			}
		}
	}


}

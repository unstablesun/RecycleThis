using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DEBUG
public partial class HexManager : MonoBehaviour 
{

	public void QueryShowScannedHexes(GemObject.eColorType eColorType, byte r, byte g, byte b) 
	{
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main) {
				if (objectScript.ScanColor == (int)eColorType) {

					objectScript.SetObjectColor (r, g, b, 255);
				}
			}
		}
	}


	public void QueryShowMarkedHexes() 
	{
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main) {
				if (objectScript.MarkedColor != (int)GemObject.eColorType.Black) {

					objectScript.SetObjectColor (255, 0, 0, 255);
				}
			}
		}
	}

}

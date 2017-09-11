using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//REMOVAL
public partial class HexManager : MonoBehaviour 
{

	//-------------------------------------------------
	//				Activate for Removal
	//-------------------------------------------------

	public bool QueryScanForRemoval() 
	{
		bool active = false;
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main) {

				if (objectScript.MarkedColor != (int)GemObject.eColorType.Black) {

					objectScript.RemoveGem(0);

					active = true;

				}
			}
		}

		return active;
	}

	public bool QueryRemovalAnimationStillActive() 
	{
		bool active = false;
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main) {

				if (objectScript.MarkedColor != (int)GemObject.eColorType.Black) {

					if(objectScript.IsGemAnimating(0)) {
						active = true;
					} else {
						objectScript.ClearGem();
						objectScript.MarkedColor = (int)GemObject.eColorType.Black;
					}
				}
			}
		}

		return active;
	}

}

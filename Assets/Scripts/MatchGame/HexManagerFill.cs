using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FILL
public partial class HexManager : MonoBehaviour 
{
	//-------------------------------------------------
	//				Activate for Fill
	//-------------------------------------------------

	public bool QueryScanForFill() 
	{
		QueryClearScan ();


		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main && objectScript.SpecialType == HexObject.eSpecialType.BottomRow) {

				ScanColumnForFill(objectScript);
			}
		}

		return false;
	}


	private bool ScanColumnForFill(HexObject objectScript) 
	{

		bool columnDone = false;
		HexObject objScript = objectScript;
		while (objScript != null) {

			if(objScript.NoGemAttached() == true) {

				//get a new gem and attach

				Debug.Log("No Gem ID = " + objScript.ID);


				int count = GridLogic.Instance.GemObjectList.Count;

				GameObject gem = GemManager.Instance.QueryGetAvailableObject();

				if(gem != null) {

					int colorType = (int)Random.Range (0, count);

					GemObject gemScript = gem.GetComponent<GemObject> ();
					gemScript.SetGemSprite(GridLogic.Instance.GemObjectList[colorType], (GemObject.eColorType) colorType);

					objScript.AttachGem(gem);
				}


			} else {

				Debug.Log("Gem Attached ID = " + objScript.ID);

			}

			objScript = getNextInColumn(objScript);

		}			

		return columnDone;
	}


}

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

					Debug.LogError("New Gem");

					int colorType = (int)Random.Range (0, count);

					//we have a new gem for the empty cell but need to animate it in from the top

					GemObject gemScript = gem.GetComponent<GemObject> ();
					gemScript.SetGemSprite(GridLogic.Instance.GemObjectList[colorType], (GemObject.eColorType) colorType);

					//objScript.AttachGem(gem);
					objScript.AttachGemNoPos (gem);

					//set top row hex
					HexObject topScript = ScanColumnForTopHex (objScript);
					Debug.LogWarning ("ScanColumnForTopHex ID = " + topScript.ID);
					if (topScript != null) {
						objScript.AttachOffsetHexRef (topScript.gameObject);
					}



					objScript.MoveGemToThisHex ();

				}


			} else {

				Debug.Log("Gem Attached ID = " + objScript.ID);

			}

			objScript = getNextInColumn(objScript);

		}			

		return columnDone;
	}

	public bool QueryFillAnimationStillActive() 
	{
		bool active = false;
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main) {

				if(objectScript.IsGemAnimating(0)) {
					active = true;
				}
			}
		}

		return active;
	}

	//debug
	public bool QueryFillAnimationStillActiveDebug() 
	{
		bool active = false;
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main) {

				if(objectScript.IsGemAnimating(0)) {
					active = true;

					objectScript.DebugPrintGemID ();
				}
			}
		}

		return active;
	}

}

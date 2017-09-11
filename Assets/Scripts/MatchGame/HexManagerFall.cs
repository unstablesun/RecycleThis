using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FALL
public partial class HexManager : MonoBehaviour 
{

	//-------------------------------------------------
	//				Activate for Fall
	//-------------------------------------------------

	public bool QueryScanForFall() 
	{
		QueryClearScan ();
		QueryMarkFallScan();

		//debug
		//QueryShowScannedHexes(GemObject.eColorType.Green, 0, 255, 0);

		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main && objectScript.SpecialType == HexObject.eSpecialType.BottomRow) {

				ScanColumnForFall(objectScript);
			}
		}

		//debug
		//QueryShowScannedHexes(GemObject.eColorType.Green, 0, 255, 0);
		//QueryShowScannedHexes(GemObject.eColorType.Blue, 0, 0, 255);


		return false;
	}

	private bool ScanColumnForFall(HexObject objectScript) 
	{
		//This function scans the column several times to re-org the cells
		//so they fall to there new locations.
		//The parameter objectScript is the bottom cell of the column

		bool restart = false;
		bool columnDone = false;
		HexObject objScript = objectScript;
		while (objScript != null) {

			//process - start a scan if cell is empty
			if(objScript.ScanColor == (int)GemObject.eColorType.Green) {

				//Debug.Log("...Found Empty cell obj ID = " + objScript.ID);

				//this cell is empty so we need to look above
				if(ScanColumnForFullCell(objScript) == false){
					columnDone = true;
					break;
				} else {
					//start over from bottom
					objScript = objectScript;
					restart = true;
				}
			}

			if(restart == true) {
				restart = false;
			}else{
				objScript = getNextInColumn(objScript);
			}
		}			

		return columnDone;
	}



	private bool ScanColumnForFullCell(HexObject objectScript) 
	{
		//This function starts from objectScript which should be empty comming in
		//and finds the first full obj then exits

		//Debug.Log("...ScanColumnForFullCell");
		bool cellFound = false;
		HexObject objScript = getNextInColumn(objectScript);

		bool evalNext = true;
		while (evalNext == true) {

			//Debug.Log("...evalNext While Loop");

			if(objScript != null) {

				if(objScript.ScanColor == (int)GemObject.eColorType.Green) {

					//empty - keep going
					//Debug.Log("...ScanColumnForFullCell - Green");

				} else {

					//Found - here's the core of the logic
					//full - found a cell to fall
					//this cell needs to move to the obj passed in

					//Debug.Log("...Set obj ID = " + objScript.ID + " to position of obj ID = " + objectScript.ID);

					//note: a cell can only be marked once to fall
					//this full cell gets marked to empty 
					//and the empty cell gets marked to full... so
					objScript.ScanColor = (int)GemObject.eColorType.Green;
					objectScript.ScanColor = (int)GemObject.eColorType.Blue;
					objScript.AttachNewHexRef(objectScript.gameObject);



					//debug
					//objScript.SetObjectColor (255, 0, 255, 255);
					//objectScript.SetObjectColor (255, 255, 0, 255);

					evalNext = false;
					cellFound = true;
					break;

				}

			} else { //reached end of column top + 1

				//Debug.Log("...objScript = null");
				evalNext = false;
			}

			if(objScript != null) {
				objScript = getNextInColumn(objScript);

			} else {
				evalNext = false;
				objScript = null;
			}

		}

		return cellFound;
	}


	public void QueryMarkFallScan() 
	{
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main) {
				if(objectScript.NoGemAttached() == false) {
					objectScript.ScanColor = (int)GemObject.eColorType.Blue; //FULL
				} else {
					objectScript.ScanColor = (int)GemObject.eColorType.Green; //EMPTY
				}
			}
		}
	}

	private HexObject getNextInColumn(HexObject objectScript) 
	{
		if(objectScript.HexLinkList != null) {

			//Debug.Log("...getNextInColumn");

			List <GameObject> sublinkList = objectScript.HexLinkList;
			HexObject objScript = sublinkList[0].GetComponent<HexObject>();
			if(objScript.isNullObject == false && objScript._Type == HexObject.eType.Main) {

				//Debug.Log("RETURN ID = " + objScript.ID);
				return objScript;
			}
		}

		return null;
	}

	public bool QueryFallAnimationStillActive() 
	{
		bool active = false;
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main) {

				//if (objectScript.MarkedColor != (int)GemObject.eColorType.Black) {

					if(objectScript.IsGemAnimating(0)) {
						active = true;
					}
				//}
			}
		}

		return active;
	}


	/*
	public void QueryMoveGemsToNewHexes() 
	{
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main) {
				objectScript.MoveGemToNewHex ();
			}
		}
	}


	public void QueryResolveGemsToNewHexes() 
	{
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main) {
				objectScript.ResolveGemToNewHex ();
			}
		}
	}
	*/

	public void QueryResolveGemsToNewHexes() 
	{
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main && objectScript.SpecialType == HexObject.eSpecialType.BottomRow) {
				ScanColumnForGemResove(objectScript);
			}
		}
	}

	private void ScanColumnForGemResove(HexObject objectScript) 
	{
		HexObject objScript = objectScript;
		while (objScript != null) {
			objScript.ResolveGemToNewHex ();
			objScript = getNextInColumn(objScript);
		}			
	}




	public void QueryMoveGemsToNewHexes() 
	{
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main && objectScript.SpecialType == HexObject.eSpecialType.BottomRow) {
				ScanColumnForGemMove(objectScript);
			}
		}
	}

	private void ScanColumnForGemMove(HexObject objectScript) 
	{
		HexObject objScript = objectScript;
		while (objScript != null) {
			objScript.MoveGemToNewHex ();
			objScript = getNextInColumn(objScript);
		}			
	}



}

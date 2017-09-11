using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//MATCH
public partial class HexManager : MonoBehaviour 
{


	//-------------------------------------------------
	//				Scan for Matches
	//-------------------------------------------------


	public void QueryScanToMarked() 
	{
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main) {

				if (objectScript.MarkedColor == (int)GemObject.eColorType.Black) {
					objectScript.MarkedColor = objectScript.ScanColor;
				}
			}
		}
	}

	public int QueryCountScanColors(int scanColor) 
	{
		int count = 0;
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main) {
				if (objectScript.ScanColor == scanColor) {

					count++;
				}
			}
		}

		return count;
	}


	public void QueryScanAndMark() 
	{
		QueryClearMarked ();

		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main && objectScript.MarkedColor == (int)GemObject.eColorType.Black) {

				//new target so clear scan colors
				QueryClearScan ();
				ScannedLinkedList.Clear ();

				//do link walk
				GemObject.eColorType colorType = objectScript.GetGemRefColorType ();
				objectScript.ScanColor = (int)colorType;

				//get link list for this target object
				List <GameObject> linkList = objectScript.HexLinkList;

				bool eval = EvaluateLinkedList (linkList, colorType);

				while (eval == true) {
					List<GameObject> evalList = new List<GameObject>(ScannedLinkedList);
					ScannedLinkedList.Clear ();

					if (evalList.Count == 0) {
						eval = false;
					}

					bool innerEval = false;
					foreach (GameObject linkObj in evalList) {
						HexObject objScript = linkObj.GetComponent<HexObject> ();
						List <GameObject> sublinkList = objScript.HexLinkList;
						if(EvaluateLinkedList (sublinkList, colorType)) {
							innerEval = true;
						}
					}
					eval = innerEval;

				}

				int count = QueryCountScanColors ((int)colorType);
				if (count >= 4) {

					//if >= 5 -> add powerup
					QueryScanToMarked ();

					Debug.Log ("########### QueryScanToMarked for color " + colorType.ToString() + "  count = " + count);

				}
			}
		}
	}

	private bool EvaluateLinkedList(List <GameObject> linkList, GemObject.eColorType colorType)
	{
		bool eval = false;
		foreach (GameObject linkObj in linkList) {

			HexObject objectScript = linkObj.GetComponent<HexObject> ();

			if (objectScript._Type == HexObject.eType.Main) {

				if (objectScript.ScanColor == (int)GemObject.eColorType.Black && objectScript.MarkedColor == (int)GemObject.eColorType.Black) {

					GemObject.eColorType cType = objectScript.GetGemRefColorType ();

					if (cType == colorType) {
						objectScript.ScanColor = (int)colorType;
						ScannedLinkedList.Add (linkObj);
						eval = true;
					}

				}
			}
		}

		return eval;
	}



}

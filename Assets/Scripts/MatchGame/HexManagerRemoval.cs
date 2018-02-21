using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//REMOVAL
public partial class HexManager : MonoBehaviour 
{

	public List <GameObject> RemovalRedList = null;
	public List <GameObject> RemovalGreenList = null;
	public List <GameObject> RemovalBlueList = null;
	public List <GameObject> RemovalYellowList = null;
	public List <GameObject> RemovalCyanList = null;
	public List <GameObject> RemovalPurpleList = null;
	public List <GameObject> RemovalOrangeList = null;

	public void ClearRemovalLists() 
	{
		RemovalRedList = new List<GameObject>();
		RemovalGreenList = new List<GameObject>();
		RemovalBlueList = new List<GameObject>();
		RemovalYellowList = new List<GameObject>();
		RemovalCyanList = new List<GameObject>();


	}

	public void AddRemovalObject(GameObject tObj) 
	{
		HexObject objectScript = tObj.GetComponent<HexObject> ();

		if (objectScript.MarkedColor == (int)GemObject.eColorType.Red) {
			if (RemovalRedList != null) {
				RemovalRedList.Add (tObj);
			}
		}if (objectScript.MarkedColor == (int)GemObject.eColorType.Green) {
			if (RemovalGreenList != null) {
				RemovalGreenList.Add (tObj);
			}
		}if (objectScript.MarkedColor == (int)GemObject.eColorType.Blue) {
			if (RemovalBlueList != null) {
				RemovalBlueList.Add (tObj);
			}
		}if (objectScript.MarkedColor == (int)GemObject.eColorType.Yellow) {
			if (RemovalYellowList != null) {
				RemovalYellowList.Add (tObj);
			}
		}if (objectScript.MarkedColor == (int)GemObject.eColorType.Cyan) {
			if (RemovalCyanList != null) {
				RemovalCyanList.Add (tObj);
			}
		}


	}


	//-------------------------------------------------
	//				Collect for Removal
	//-------------------------------------------------

	public bool QueryScanForRemoval() 
	{
		ClearRemovalLists();

		bool active = false;
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main) {

				if (objectScript.MarkedColor != (int)GemObject.eColorType.Black) {

					//add to removal list, remove different colors separately

					AddRemovalObject(tObj);

					active = true;

				}
			}
		}

		//DebugRemovalObject();

		return active;
	}



	//-------------------------------------------------
	//				Process for Removal
	//-------------------------------------------------

	public void QueryProcessForRemoval() 
	{
		_processForRemoval(RemovalRedList);
		_processForRemoval(RemovalGreenList);
		_processForRemoval(RemovalBlueList);
		_processForRemoval(RemovalYellowList);
		_processForRemoval(RemovalCyanList);
	}

	public void _processForRemoval(List<GameObject> _colorlist) 
	{
		if (_colorlist != null) {

			Vector3 averageVector = Vector3.zero;
			List <Vector3> vList = new List<Vector3>();

			foreach(GameObject tObj in _colorlist) {
				HexObject objectScript = tObj.GetComponent<HexObject> ();
				Vector3 v3 = objectScript.transform.position;
				vList.Add(v3);
			}

			//average vectors
 			for (int i = 0; i < vList.Count; i++) {
     			averageVector += vList[i];
 			}
			averageVector /= vList.Count;

			foreach(GameObject tObj in _colorlist) {

				HexObject objectScript = tObj.GetComponent<HexObject> ();
				objectScript.SetRemovalDestinationV3(averageVector);
				objectScript.RemoveGem(0);
			}
		}
	}



/* 
	-----------------------------------------------------------------
	Poll to see if the animations for removals are still in progress
	-----------------------------------------------------------------	
*/
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



	public void DebugRemovalObject() 
	{
		if (RemovalRedList != null) {
			foreach(GameObject tObj in RemovalRedList) {
				Debug.Log("red");
			}
		}

		if (RemovalGreenList != null) {
			foreach(GameObject tObj in RemovalGreenList) {
				Debug.Log("green");
			}
		}

		if (RemovalBlueList != null) {
			foreach(GameObject tObj in RemovalBlueList) {
				Debug.Log("blue");
			}
		}

		if (RemovalYellowList != null) {
			foreach(GameObject tObj in RemovalYellowList) {
				Debug.Log("Yellow");
			}
		}

		if (RemovalCyanList != null) {
			foreach(GameObject tObj in RemovalCyanList) {
				Debug.Log("cyan");
			}
		}
	}


}

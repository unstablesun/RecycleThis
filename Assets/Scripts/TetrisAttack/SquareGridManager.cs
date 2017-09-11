using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareGridManager : MonoBehaviour 
{
	public int objectPoolSize = 128;

	public float SquareGridWidth = 8f;
	public float SquareGridHeight = 8f;
	public float SquareGridDX = 1f;
	public float SquareGridDY = 1f;

	public GameObject StoragePosition;
	public GameObject StartGridPosition;


	[HideInInspector]
	public List <GameObject> SquareGridObjectList = null;

	private GameObject SquareGridObjectContainer;

	public static SquareGridManager Instance;

	private GameObject _nullObj = null;

	private float gridStartX, gridStartY;

	public List <GameObject> ScannedLinkedList = null;



	private int _scanType = 0;
	public int ScanType {
		get {return _scanType; } 
		set {_scanType = value; }
	}

	private int _runningScanIndex;


	void Awake () 
	{
		Instance = this;

		SquareGridObjectList = new List<GameObject>();

		ScannedLinkedList = new List<GameObject>();
	}

	void Start () 
	{
		SquareGridObjectContainer = GameObject.Find ("SquareGridObjectContainer");

		gridStartX = StartGridPosition.transform.position.x;
		gridStartY = StartGridPosition.transform.position.y;

		LoadHexObjects ();

		QuerySetObjectsLoaded ();

		QuerySetObjectsPosition();

		QueryLinkHexObjects ();

	}


	void Update () 
	{

	}


	private void LoadHexObjects()
	{

		for (int t = 0; t < objectPoolSize; t++) {

			GameObject _sfObj = Instantiate (Resources.Load ("Prefabs/SquareGridObject", typeof(GameObject))) as GameObject;

			if (_sfObj != null) {

				if (SquareGridObjectContainer != null) {
					_sfObj.transform.parent = SquareGridObjectContainer.transform;
				}
				_sfObj.name = "squareObj" + t.ToString ();

				//default storage location
				_sfObj.transform.position = new Vector2 (StoragePosition.transform.position.x, StoragePosition.transform.position.y);

				SquareGridObject objectScript = _sfObj.GetComponent<SquareGridObject> ();
				objectScript.ID = t;

				SquareGridObjectList.Add (_sfObj);

			} else {

				Debug.Log ("Couldn't load hex object prefab");
			}
		}

		_nullObj = Instantiate (Resources.Load ("Prefabs/SquareGridObject", typeof(GameObject))) as GameObject;
		_nullObj.transform.position = new Vector2 (StoragePosition.transform.position.x, StoragePosition.transform.position.y);
		SquareGridObject nullObjectScript = _nullObj.GetComponent<SquareGridObject> ();
		nullObjectScript.ID = -1;
		nullObjectScript.isNullObject = true;

	}


	void QuerySetObjectsLoaded() 
	{
		foreach(GameObject tObj in SquareGridObjectList)
		{
			SquareGridObject objectScript = tObj.GetComponent<SquareGridObject> ();
			objectScript._State = SquareGridObject.eState.Loaded;

			objectScript.InitHexLinkList ();
		}
	}

	void QuerySetObjectsPosition() 
	{
		float xOffset = 0f;
		float yOffset = 0f;
		int lineCount = 0;
		int rowCount = 0;

		foreach(GameObject tObj in SquareGridObjectList)
		{
			SquareGridObject objectScript = tObj.GetComponent<SquareGridObject> ();

			float x = gridStartX + xOffset;
			float y = gridStartY + yOffset;

			objectScript.SetGridPosition (new Vector3(x,y,1));


			if(lineCount == 0 || lineCount == SquareGridWidth-1 || rowCount == 0 || rowCount == SquareGridHeight-1) {
				objectScript.SetToNullObject();
				objectScript.SetObjectColor(128, 128, 128, 200);

				objectScript._Type = SquareGridObject.eType.Edge;

			} else {

				objectScript._Type = SquareGridObject.eType.Main;
			}

			xOffset += SquareGridDX;
			lineCount++;
			if(lineCount >= SquareGridWidth) {
				lineCount = 0;
				xOffset = 0f;
				yOffset += SquareGridDY;
				rowCount++;
			}

			if(rowCount >= SquareGridHeight) {
				break;
			}
		}
	}


	private GameObject QueryFindObjectByID(int id) 
	{
		foreach(GameObject tObj in SquareGridObjectList)
		{
			SquareGridObject objectScript = tObj.GetComponent<SquareGridObject> ();
			if (objectScript.ID == id && objectScript._Type == SquareGridObject.eType.Main) {

				return tObj;
			}
		}
		return null;
	}


	void QueryLinkHexObjects() 
	{
		float w = SquareGridWidth;
		float h = SquareGridHeight;
		float max = w * h;

		foreach(GameObject tObj in SquareGridObjectList)
		{
			SquareGridObject objectScript = tObj.GetComponent<SquareGridObject> ();

			int id = objectScript.ID;

			float _id = (float)id;

			//Link Main Sequence

			//0
			float lookupID = _id - w;
			if (lookupID >= 0 && lookupID < max)
				SetLink (lookupID, objectScript);

			//1
			lookupID = _id - +1;
			if (lookupID >= 0 && lookupID < max)
				SetLink (lookupID, objectScript);

			//2
			lookupID = _id + w;
			if (lookupID >= 0 && lookupID < max)
				SetLink (lookupID, objectScript);

			//3
			lookupID = _id - 1;
			if (lookupID >= 0 && lookupID < max)
				SetLink (lookupID, objectScript);

		}
	}

	private void SetLink(float lookupID, SquareGridObject objectScript)
	{
		GameObject go = QueryFindObjectByID ((int)lookupID);
		if(go != null) {
			SquareGridObject objScript = go.GetComponent<SquareGridObject> ();
			if (objScript._Type == SquareGridObject.eType.Main) {
				objectScript.AddLinkedObject (go);
			} else {
				objectScript.AddLinkedObject (_nullObj);
			}
		} else {
			objectScript.AddLinkedObject (_nullObj);
		}

	}


	public void QueryAttachGemToHex(GameObject go) 
	{
		foreach(GameObject tObj in SquareGridObjectList)
		{
			SquareGridObject objectScript = tObj.GetComponent<SquareGridObject> ();
			if (objectScript._Type == SquareGridObject.eType.Main) {



			}
		}
	}

	public void SetScanSetting(int t)
	{
		_runningScanIndex = 0;
	}

	//this scans and returns hexs on the main sequence left to right top to bottom
	public GameObject QueryScanNextSquare() 
	{
		int count = SquareGridObjectList.Count;
		GameObject hexObj = null;
		while(_runningScanIndex < count)
		{
			hexObj = SquareGridObjectList[_runningScanIndex++];
			if(hexObj != null) {
				SquareGridObject objectScript = hexObj.GetComponent<SquareGridObject> ();
				if (objectScript._Type == SquareGridObject.eType.Main) {

					break;
				}
			}
		}

		return hexObj;
	}




	//-------------------------------------------------
	//				Scan for Matches
	//-------------------------------------------------

	public void QueryClearMarked() 
	{
		foreach(GameObject tObj in SquareGridObjectList)
		{
			SquareGridObject objectScript = tObj.GetComponent<SquareGridObject> ();
			if (objectScript._Type == SquareGridObject.eType.Main) {
				objectScript.MarkedColor = (int)GemObject.eColorType.Black;
			}
		}
	}

	public void QueryClearScan() 
	{
		foreach(GameObject tObj in SquareGridObjectList)
		{
			SquareGridObject objectScript = tObj.GetComponent<SquareGridObject> ();
			if (objectScript._Type == SquareGridObject.eType.Main) {
				objectScript.ScanColor = (int)GemObject.eColorType.Black;
			}
		}
	}

	public void QueryScanToMarked() 
	{
		foreach(GameObject tObj in SquareGridObjectList)
		{
			SquareGridObject objectScript = tObj.GetComponent<SquareGridObject> ();
			if (objectScript._Type == SquareGridObject.eType.Main) {

				if (objectScript.MarkedColor == (int)GemObject.eColorType.Black) {
					objectScript.MarkedColor = objectScript.ScanColor;
				}
			}
		}
	}

	public int QueryCountScanColors(int scanColor) 
	{
		int count = 0;
		foreach(GameObject tObj in SquareGridObjectList)
		{
			SquareGridObject objectScript = tObj.GetComponent<SquareGridObject> ();
			if (objectScript._Type == SquareGridObject.eType.Main) {
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

		foreach(GameObject tObj in SquareGridObjectList)
		{
			SquareGridObject objectScript = tObj.GetComponent<SquareGridObject> ();
			if (objectScript._Type == SquareGridObject.eType.Main && objectScript.MarkedColor == (int)GemObject.eColorType.Black) {

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
						SquareGridObject objScript = linkObj.GetComponent<SquareGridObject> ();
						List <GameObject> sublinkList = objScript.HexLinkList;
						if(EvaluateLinkedList (sublinkList, colorType)) {
							innerEval = true;
						}
					}
					eval = innerEval;

				}

				int count = QueryCountScanColors ((int)colorType);
				if (count >= 3) {

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

			SquareGridObject objectScript = linkObj.GetComponent<SquareGridObject> ();

			if (objectScript._Type == SquareGridObject.eType.Main) {

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


	public void QueryShowMarkedSquares() 
	{
		foreach(GameObject tObj in SquareGridObjectList)
		{
			SquareGridObject objectScript = tObj.GetComponent<SquareGridObject> ();
			if (objectScript._Type == SquareGridObject.eType.Main) {
				if (objectScript.MarkedColor != (int)GemObject.eColorType.Black) {

					objectScript.SetObjectColor (255, 0, 64, 128);
				}
			}
		}
	}

















	public void QueryScanForLinesAndMark() 
	{
		QueryClearMarked ();

		//Horizontal
		foreach(GameObject tObj in SquareGridObjectList)
		{
			SquareGridObject objectScript = tObj.GetComponent<SquareGridObject> ();
			if (objectScript._Type == SquareGridObject.eType.Main && objectScript.MarkedColor == (int)GemObject.eColorType.Black) {

				//new target so clear scan colors
				QueryClearScan ();
				ScannedLinkedList.Clear ();

				//do link walk
				GemObject.eColorType colorType = objectScript.GetGemRefColorType ();
				objectScript.ScanColor = (int)colorType;

				//get link list for this target object
				List <GameObject> linkList = objectScript.HexLinkList;

				bool eval = EvaluateLinkedListForIndex (linkList, colorType, 1);

				Debug.Log ("eval == true");

				while (eval == true) {
					
					List<GameObject> evalList = new List<GameObject>(ScannedLinkedList);
					ScannedLinkedList.Clear ();

					if (evalList.Count == 0) {
						eval = false;
					}

					bool innerEval = false;
					GameObject linkObj = evalList [0]; 
					SquareGridObject objScript = linkObj.GetComponent<SquareGridObject> ();
					List <GameObject> sublinkList = objScript.HexLinkList;
					if(EvaluateLinkedListForIndex (sublinkList, colorType, 1)) {
						innerEval = true;
					}

					eval = innerEval;

				}

				int count = QueryCountScanColors ((int)colorType);
				if (count >= 3) {

					//if >= 5 -> add powerup
					QueryScanToMarked ();

					Debug.Log ("########### QueryScanToMarked for color " + colorType.ToString() + "  count = " + count);

				}
			}
		}

		//Vertical
		foreach(GameObject tObj in SquareGridObjectList)
		{
			SquareGridObject objectScript = tObj.GetComponent<SquareGridObject> ();
			if (objectScript._Type == SquareGridObject.eType.Main && objectScript.MarkedColor == (int)GemObject.eColorType.Black) {

				//new target so clear scan colors
				QueryClearScan ();
				ScannedLinkedList.Clear ();

				//do link walk
				GemObject.eColorType colorType = objectScript.GetGemRefColorType ();
				objectScript.ScanColor = (int)colorType;

				//get link list for this target object
				List <GameObject> linkList = objectScript.HexLinkList;

				bool eval = EvaluateLinkedListForIndex (linkList, colorType, 2);

				Debug.Log ("eval == true");

				while (eval == true) {

					List<GameObject> evalList = new List<GameObject>(ScannedLinkedList);
					ScannedLinkedList.Clear ();

					if (evalList.Count == 0) {
						eval = false;
					}

					bool innerEval = false;
					GameObject linkObj = evalList [0]; 
					SquareGridObject objScript = linkObj.GetComponent<SquareGridObject> ();
					List <GameObject> sublinkList = objScript.HexLinkList;
					if(EvaluateLinkedListForIndex (sublinkList, colorType, 2)) {
						innerEval = true;
					}

					eval = innerEval;

				}

				int count = QueryCountScanColors ((int)colorType);
				if (count >= 3) {

					//if >= 5 -> add powerup
					QueryScanToMarked ();

					Debug.Log ("########### QueryScanToMarked for color " + colorType.ToString() + "  count = " + count);

				}
			}
		}

	}

	private bool EvaluateLinkedListForIndex(List <GameObject> linkList, GemObject.eColorType colorType, int index)
	{
		bool eval = false;

		if (linkList == null)
			return false;

		if (linkList.Count == 0)
			return false;
			
		GameObject linkObj = linkList [index];

		SquareGridObject objectScript = linkObj.GetComponent<SquareGridObject> ();

		if (objectScript._Type == SquareGridObject.eType.Main) {

			if (objectScript.ScanColor == (int)GemObject.eColorType.Black && objectScript.MarkedColor == (int)GemObject.eColorType.Black) {

				GemObject.eColorType cType = objectScript.GetGemRefColorType ();

				if (cType == colorType) {
					objectScript.ScanColor = (int)colorType;
					ScannedLinkedList.Add (linkObj);
					eval = true;
				}

			}
		}


		return eval;
	}



}


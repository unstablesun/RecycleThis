using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public partial class HexManager : MonoBehaviour 
{
	
	public int objectPoolSize = 128;

	public float HexGridWidth = 4f;
	public float HexGridHeight = 4f;
	public float HexGridDX = 1f;
	public float HexGridDY = 1f;
	public float HexSkipDY = 0.5f;

	public float HexBackingScaleX = 0.5f;
	public float HexBackingScaleY = 0.5f;

	public float TapPadScaleX = 1.0f;
	public float TapPadScaleY = 1.6f;

	public float SwapSpeed = 0.3f;
	public float FallSpeed = 0.3f;
	public float RemovalSpeed = 0.3f;

	public GameObject StoragePosition;
	public GameObject StartGridPosition;


	[HideInInspector]
	public List <GameObject> HexObjectList = null;

	private GameObject HexObjectContainer;

	public static HexManager Instance;

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

		HexObjectList = new List<GameObject>();

		ScannedLinkedList = new List<GameObject>();
	}

	void Start () 
	{
		HexObjectContainer = GameObject.Find ("HexObjectContainer");

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

	public bool SwapHexGems (int hexAID, int hexBID)
	{
		GameObject hexA = QueryFindObjectByID(hexAID);
		GameObject hexB = QueryFindObjectByID(hexBID);

		HexObject objectScriptA = hexA.GetComponent<HexObject> ();
		HexObject objectScriptB = hexB.GetComponent<HexObject> ();

		GameObject GemRefA = objectScriptA.GemRef;
		GameObject GemRefB = objectScriptB.GemRef;

		Vector3 newposA = objectScriptA.transform.position;
		Vector3 newposB = objectScriptB.transform.position;

		if(GemRefA != null) {
			GemObject gemObjectScript = GemRefA.GetComponent<GemObject> ();
			gemObjectScript.StartFallAnim(newposB.x, newposB.y, -3f, SwapSpeed);
		} else {
			Debug.Log("ASSERT MoveGemToNewHex GemRef == null");
		}

		if(GemRefB != null) {
			GemObject gemObjectScript = GemRefB.GetComponent<GemObject> ();
			gemObjectScript.StartFallAnim(newposA.x, newposA.y, -3f, SwapSpeed);
		} else {
			Debug.Log("ASSERT MoveGemToNewHex GemRef == null");
		}

		objectScriptA.AttachGemNoPos(GemRefB);
		objectScriptB.AttachGemNoPos(GemRefA);



		return true;
	}



	private void LoadHexObjects()
	{

		for (int t = 0; t < objectPoolSize; t++) {

			GameObject _sfObj = Instantiate (Resources.Load ("Prefabs/HexObject", typeof(GameObject))) as GameObject;

			if (_sfObj != null) {

				if (HexObjectContainer != null) {
					_sfObj.transform.parent = HexObjectContainer.transform;
				}
				_sfObj.name = "hexObj" + t.ToString ();

				//default storage location
				_sfObj.transform.position = new Vector2 (StoragePosition.transform.position.x, StoragePosition.transform.position.y);

				HexObject objectScript = _sfObj.GetComponent<HexObject> ();
				objectScript.ID = t;
				objectScript.SetBackingSpriteScale(HexBackingScaleX, HexBackingScaleY);
				objectScript.SetTapPadSpriteScale(TapPadScaleX, TapPadScaleY);

				HexObjectList.Add (_sfObj);

			} else {

				Debug.Log ("Couldn't load hex object prefab");
			}
		}

		_nullObj = Instantiate (Resources.Load ("Prefabs/HexObject", typeof(GameObject))) as GameObject;
		_nullObj.transform.position = new Vector2 (StoragePosition.transform.position.x, StoragePosition.transform.position.y);
		HexObject nullObjectScript = _nullObj.GetComponent<HexObject> ();
		nullObjectScript.ID = -1;
		nullObjectScript.isNullObject = true;

	}


	void QuerySetObjectsLoaded() 
	{
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			objectScript._State = HexObject.eState.Loaded;

			objectScript.InitHexLinkList ();
		}
	}

	void QuerySetObjectsPosition() 
	{
		float xOffset = 0f;
		float yOffset = 0f;
		int lineCount = 0;
		int rowCount = 0;

		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();

			float x = gridStartX + xOffset;
			float y = gridStartY + yOffset;
			if(lineCount % 2 == 1) {
				
				y += HexSkipDY;

				objectScript.isSkipHex = true;
			}

			objectScript.SetHexPosition (new Vector3(x,y,1));


			if(lineCount == 0 || lineCount == HexGridWidth-1 || rowCount == 0 || rowCount == HexGridHeight-1) {
				
				objectScript.SetToNullObject();
				objectScript.SetObjectColor(128, 128, 128, 200);

				objectScript._Type = HexObject.eType.Edge;

				if(rowCount == 0 && lineCount > 0 && lineCount < HexGridWidth-1) {
					objectScript.SpecialType = HexObject.eSpecialType.TopRow;
				}

			} else {
			
				if(rowCount == HexGridHeight-2) {
					objectScript.SpecialType = HexObject.eSpecialType.BottomRow;
				}

				objectScript._Type = HexObject.eType.Main;
			}
				
			xOffset += HexGridDX;
			lineCount++;
			if(lineCount >= HexGridWidth) {
				lineCount = 0;
				xOffset = 0f;
				yOffset += HexGridDY;
				rowCount++;
			}

			if(rowCount >= HexGridHeight) {
				break;
			}
		}
	}




	void QueryLinkHexObjects() 
	{
		float w = HexGridWidth;
		float h = HexGridHeight;
		float max = w * h;

		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();

			int id = objectScript.ID;
			bool isSkip = objectScript.isSkipHex;

			float _id = (float)id;

			//Link Main Sequence
			if (isSkip) {
				
				//0
				float lookupID = _id - w;
				if (lookupID >= 0 && lookupID < max)
					SetLink (lookupID, objectScript);

				//1
				lookupID = _id - (w - 1);
				if (lookupID >= 0 && lookupID < max)
					SetLink (lookupID, objectScript);

				//2
				lookupID = _id + 1;
				if (lookupID >= 0 && lookupID < max)
					SetLink (lookupID, objectScript);

				//3
				lookupID = _id + w;
				if (lookupID >= 0 && lookupID < max)
					SetLink (lookupID, objectScript);

				//4
				lookupID = _id - 1;
				if (lookupID >= 0 && lookupID < max)
					SetLink (lookupID, objectScript);

				//5
				lookupID = _id - (w + 1);
				if (lookupID >= 0 && lookupID < max)
					SetLink (lookupID, objectScript);
				
			} else { //non skip
			
				//0
				float lookupID = _id - w;
				if (lookupID >= 0 && lookupID < max)
					SetLink (lookupID, objectScript);

				//1
				lookupID = _id + 1;
				if (lookupID >= 0 && lookupID < max)
					SetLink (lookupID, objectScript);

				//2
				lookupID = _id + (w + 1);
				if (lookupID >= 0 && lookupID < max)
					SetLink (lookupID, objectScript);

				//3
				lookupID = _id + w;
				if (lookupID >= 0 && lookupID < max)
					SetLink (lookupID, objectScript);

				//4
				lookupID = _id + (w - 1);
				if (lookupID >= 0 && lookupID < max)
					SetLink (lookupID, objectScript);

				//5
				lookupID = _id - 1;
				if (lookupID >= 0 && lookupID < max)
					SetLink (lookupID, objectScript);
				
			}

		}
	}
		
	private void SetLink(float lookupID, HexObject objectScript)
	{
		GameObject go = QueryFindObjectByID ((int)lookupID);
		if(go != null) {
			HexObject objScript = go.GetComponent<HexObject> ();
			if (objScript._Type == HexObject.eType.Main) {
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
		foreach(GameObject tObj in HexObjectList)
		{
			HexObject objectScript = tObj.GetComponent<HexObject> ();
			if (objectScript._Type == HexObject.eType.Main) {



			}
		}
	}

	public void SetScanSetting(int t)
	{
		_runningScanIndex = 0;
	}

	//this scans and returns hexs on the main sequence left to right top to bottom
	public GameObject QueryScanNextHex() 
	{
		int count = HexObjectList.Count;
		GameObject hexObj = null;
		while(_runningScanIndex < count)
		{
			hexObj = HexObjectList[_runningScanIndex++];
			if(hexObj != null) {
				HexObject objectScript = hexObj.GetComponent<HexObject> ();
				if (objectScript._Type == HexObject.eType.Main) {

					break;
				}
			}
		}

		return hexObj;
	}

}

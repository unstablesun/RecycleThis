using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SquareGridObject : MonoBehaviour 
{
	public enum eState 
	{
		NoOp,
		Loaded,
		Empty,
		Full
	};
	public eState _State = eState.NoOp;

	public enum eType 
	{
		Null,
		Main,
		Edge,
		Top,
		Side,
		Meter
	};
	public eType _Type = eType.Null;

	public enum eLinkDirection 
	{
		North,
		NorthEast,
		SouthEast,
		South,
		SouthWest,
		NorthWest
	};
	public eLinkDirection _LinkDirection = eLinkDirection.North;

	public GameObject backingSprite;
	public GameObject tapPadSprite;
	public GameObject textObj;

	public int Debug_ID_;
	public int Debug_ScanColor_;
	public int Debug_MarkedColor_;

	private int _id = 0;
	public int ID {
		get {return _id; } 
		set {_id = value; }
	}


	public List <GameObject> HexLinkList = null;

	public bool isNullObject = false;
	public bool isSkipHex = false;


	//[HideInInspector]
	public GameObject GemRef = null;

	private int _scanColor = 0;
	public int ScanColor {
		get {return _scanColor; } 
		set {_scanColor = value; }
	}

	private int _markedColor = 0;
	public int MarkedColor {
		get {return _markedColor; } 
		set {_markedColor = value; }
	}

	void Start () 
	{}

	public void SetToNullObject () 
	{
		isNullObject = true;

		tapPadSprite.SetActive(false);
	}


	public void InitHexLinkList () 
	{
		HexLinkList = new List<GameObject>();

	}

	public void AttachGem (GameObject go) 
	{
		GemRef = go;
		GemRef.transform.position = new Vector3( transform.position.x, transform.position.y, -1);
	}

	public bool NoGemAttached()
	{
		if(GemRef == null)
			return true;
		else
			return false;
	}

	public GemObject.eColorType GetGemRefColorType()
	{
		if (GemRef != null) {
			GemObject objectScript = GemRef.GetComponent<GemObject> ();
			return objectScript.ColorType;
		}

		return GemObject.eColorType.Black;//not set
	}


	public void AddLinkedObject(GameObject go)
	{
		if (HexLinkList != null) {

			HexLinkList.Add (go);
		}
	}

	public void SetGridPosition(Vector3 vec)
	{
		transform.position = new Vector3(vec.x, vec.y, vec.z);
	}

	void Update () 
	{
		//debug--------
		Debug_ID_ = ID;
		Debug_ScanColor_ = ScanColor;
		Debug_MarkedColor_ = MarkedColor;

		TMP_Text m_text = textObj.GetComponent<TextMeshPro>();
		m_text.text = ID.ToString();
		//-------------
	}






	public void SetObjectColor(float red, float green, float blue, float alpha) 
	{
		if (backingSprite != null) {
			backingSprite.GetComponent<Renderer> ().material.color = new Color32 ((byte)red, (byte)green, (byte)blue, (byte)alpha);
		}
	}		

}

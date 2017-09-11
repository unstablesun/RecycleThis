using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;

public class HexObject : MonoBehaviour 
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
		Edge
	};
	public eType _Type = eType.Null;

	public enum eSpecialType 
	{
		None,
		BottomRow,
		TopRow
	};
	public eSpecialType SpecialType = eSpecialType.None;


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

	//[HideInInspector]
	public GameObject NewHexRef = null;//this is a ref to the hexobject the gem will be falling to

	//working scan
	private int _scanColor = 0;
	public int ScanColor {
		get {return _scanColor; } 
		set {_scanColor = value; }
	}

	//final mark for removal
	private int _markedColor = 0;
	public int MarkedColor {
		get {return _markedColor; } 
		set {_markedColor = value; }
	}

	void Start () 
	{
		GemRef = null;
		NewHexRef = null;
	}

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
		Assert.IsNotNull(go);
		Assert.IsNull(GemRef);

		if(GemRef == null) {
			
			GemRef = go;
			GemRef.transform.position = new Vector3( transform.position.x, transform.position.y, -1);
	
		}
	}
	public void AttachGemNoPos (GameObject go) 
	{
		
		GemRef = go;
	}

	public void ClearGem () 
	{
		Assert.IsNotNull(GemRef);

		if(GemRef != null) {
			
			GemObject objectScript = GemRef.GetComponent<GemObject> ();
			objectScript.Reset();

			GemRef = null;

		}
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

	public void AttachNewHexRef (GameObject go) 
	{
		Assert.IsNotNull(go);
		//Assert.IsNull(NewHexRef);

		if(NewHexRef == null) {
			NewHexRef = go;
		} else {

			HexObject hexObjectScript = NewHexRef.GetComponent<HexObject> ();
			HexObject hexObjectScript2 = go.GetComponent<HexObject> ();

			Debug.LogError("ERROR: AttachNewHexRef - ID = " + hexObjectScript.ID + "  ...trying to attach second NewHexRef with ID = " + hexObjectScript2.ID);
		}

	}

		
	public void AddLinkedObject(GameObject go)
	{
		if (HexLinkList != null) {
		
			HexLinkList.Add (go);
		}
	}

	public void SetHexPosition(Vector3 vec)
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


	public bool RemoveGem(int type)
	{
		if (GemRef != null) {
			GemObject objectScript = GemRef.GetComponent<GemObject> ();
		
			objectScript.StartRemovalAnim(0f, 2f, -3f, 1f);

		} else {
			return false;
		}

		return true;
	}

	public bool MoveGemToNewHex()
	{
		//Assert.IsNotNull(NewHexRef);
		//NewHexRef can be null or notnull coming in here

		if (NewHexRef != null) {

			HexObject hexObjectScript = NewHexRef.GetComponent<HexObject> ();
			Vector3 newpos = hexObjectScript.transform.position;

			if(GemRef != null) {
				GemObject gemObjectScript = GemRef.GetComponent<GemObject> ();
				gemObjectScript.StartFallAnim(newpos.x, newpos.y, -3f, 0.5f);
			} else {
				Debug.Log("ASSERT MoveGemToNewHex GemRef == null");
			}
				
			//Debug.Log("MoveGemToNewHex This ID = " + ID + " to ID = " + hexObjectScript.ID + "   ... moving from x = " + transform.position.x + " y = " + transform.position.y + " :: to x = " + newpos.x + " y = " + newpos.y);

			//gemObjectScript.transform.position = new Vector3(newpos.x, newpos.y, -3f);

		} else {
			return false;
		}

		return true;
	}

	public bool ResolveGemToNewHex()
	{
		//Assert.IsNotNull(NewHexRef);
		//NewHexRef can be null or notnull coming in here

		if (NewHexRef != null) {

			HexObject hexObjectScript = NewHexRef.GetComponent<HexObject> ();

			if(GemRef != null) {
				hexObjectScript.AttachGemNoPos(GemRef);
			} else {
				Debug.Log("ASSERT ResolveGemToNewHex GemRef == null");
			}

			//GemObject objectScript = GemRef.GetComponent<GemObject> ();
			//Debug.Log("AttachGemNoPos ID = " + ID + "  NewHexRef ID = " + hexObjectScript.ID);
			GemRef = null;//this is ok we've passed it off
			NewHexRef = null;//this needs to be cleared

		} else {
			return false;
		}

		return true;
	}

	public bool IsGemAnimating(int type)
	{
		bool aState = true;
		if (GemRef != null) {
			GemObject objectScript = GemRef.GetComponent<GemObject> ();
			if(objectScript.AnimState == GemObject.eAnimState.Ready) {
				aState = false;
			}
		}else{
			aState = false;
		}

		return aState;
	}


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GridLogic : MonoBehaviour 
{
	public enum eGridState 
	{
		NoOp,
		Wait,
		Diagnostic,
		EmptyScan,
		Calc,

		//core loop
		AllowInput,
		InitFill,
		ScanMatches,
		Removal,
		RemovalAnimation,
		Falling,
		FallingAnimation,
		Fill,
		FillAnimation,

		TrySwap,
		SwapAnimation,

		Dead
	};
	public eGridState _State = eGridState.NoOp;

	public List <GameObject> GemObjectList = null;

	public static GridLogic Instance;

	void Awake () 
	{
		Instance = this;

		Assert.raiseExceptions = true;


	}

	void Start () 
	{
		_State = eGridState.InitFill;

	}

	public void SwapHexGems (int hexA, int hexB)
	{
		if(HexManager.Instance.SwapHexGems (hexA, hexB) == true) {
			_State = eGridState.SwapAnimation;
		}
	}

	public void DebugPrintGridState () 
	{
		Debug.LogError ("Grid Logic state = " + _State.ToString());

	}

	void Update () 
	{
		switch( _State )
		{
			case eGridState.NoOp:
				break;
			case eGridState.Wait:
				_State = eGridState.EmptyScan;
				break;
			case eGridState.Calc:
			break;
			case eGridState.Dead:
			break;


			case eGridState.Diagnostic:
				
				FillEmptyDiagnostic ();

				//FillPreconfigDiagnostic ();

				TryMatch ();

				_State = eGridState.Removal;

				break;
			case eGridState.EmptyScan:
				TryMatch ();
				_State = eGridState.Removal;
				break;


			case eGridState.InitFill:
				FillEmptyDiagnostic ();
				_State = eGridState.ScanMatches;
			break;

			//CORE LOOP--------------------------------------------------------
			case eGridState.AllowInput:

			break;

			case eGridState.ScanMatches:

				Debug.Log("GridLogic Update ScanMatches (TryMatch)");

				TryMatch ();
				_State = eGridState.Removal;
			break;

			case eGridState.Removal:
				if(TryRemoval()) {
					_State = eGridState.RemovalAnimation;
				} else {
					_State = eGridState.AllowInput;
				}

			break;

			case eGridState.RemovalAnimation:
				if(IsRemovalAnimating() == false) {
					_State = eGridState.Falling;
				}

			break;

			case eGridState.Falling:
				TryScanForFall();
				_State = eGridState.FallingAnimation;

			break;

			case eGridState.FallingAnimation:
				if(IsFallAnimating() == false) {
					HexManager.Instance.QueryResolveGemsToNewHexes();
					_State = eGridState.Fill;
				}

			break;

			case eGridState.Fill:
				BackFill();
				_State = eGridState.FillAnimation;

			break;

			case eGridState.FillAnimation:

				//is animation still active

				if(IsFillAnimating() == false) {
					_State = eGridState.ScanMatches;
				}

			break;

			case eGridState.SwapAnimation:
				if (IsFallAnimating () == false) {

					Debug.Log ("SwapAnimation DONE Changing STATE");

					_State = eGridState.ScanMatches;
				} else {
				
					Debug.LogError ("SwapAnimation IsFallAnimating = true");

				}

			break;
		}

		
	}


	private void FillEmptyDiagnostic () 
	{
		int count = GemObjectList.Count;

		Debug.Log("FillEmptyDiagnostic");
		HexManager.Instance.SetScanSetting(0);

		GameObject go = HexManager.Instance.QueryScanNextHex();

		while(go != null)
		{

			HexObject objectScript = go.GetComponent<HexObject> ();

			if(objectScript.NoGemAttached()) {
			
				GameObject gem = GemManager.Instance.QueryGetAvailableObject();

				if(gem != null) {

					int colorType = (int)Random.Range (0, count);

					GemObject gemScript = gem.GetComponent<GemObject> ();
					gemScript.SetGemSprite(GemObjectList[colorType], (GemObject.eColorType) colorType);

					objectScript.AttachGem(gem);
				}
			}
				
			go = HexManager.Instance.QueryScanNextHex();
		}

	}



	int[] PreConfigBoard1 = 
	{ 
		0, 1, 2, 3, 4, 
		4, 3, 2, 1, 0, 
		0, 1, 2, 3, 4, 
		4, 3, 2, 1, 0, 
		0, 1, 2, 3, 4 
	};

	int[] PreConfigBoard2 = 
	{ 
		4, 0, 2, 0, 0, 
		0, 3, 0, 4, 3, 
		1, 2, 1, 3, 1, 
		0, 2, 2, 0, 0, 
		1, 1, 0, 0, 1 
	};

	int[] PreConfigBoard3 = 
	{ 
		1, 0, 0, 0, 0, 
		2, 0, 2, 4, 1, 
		2, 1, 2, 3, 4, 
		4, 3, 3, 1, 2, 
		2, 1, 2, 3, 4 
	};


	private void FillPreconfigDiagnostic () 
	{
		int count = GemObjectList.Count;

		Debug.Log("FillEmptyDiagnostic");
		HexManager.Instance.SetScanSetting(0);

		GameObject go = HexManager.Instance.QueryScanNextHex();

		int index = 0;
		while(go != null)
		{

			HexObject objectScript = go.GetComponent<HexObject> ();

			if(objectScript._Type == HexObject.eType.Main && objectScript.NoGemAttached()) {

				GameObject gem = GemManager.Instance.QueryGetAvailableObject();

				if(gem != null) {

					int colorType = PreConfigBoard3[index++];

					GemObject gemScript = gem.GetComponent<GemObject> ();
					gemScript.SetGemSprite(GemObjectList[colorType], (GemObject.eColorType) colorType);

					objectScript.AttachGem(gem);
				}
			}

			go = HexManager.Instance.QueryScanNextHex();
		}

	}



	private void TryMatch ()
	{
		HexManager.Instance.QueryScanAndMark ();

		//HexManager.Instance.QueryShowMarkedHexes ();
	}
		
	private bool TryRemoval ()
	{
		return (HexManager.Instance.QueryScanForRemoval () );
	}

	private bool IsRemovalAnimating ()
	{
		return (HexManager.Instance.QueryRemovalAnimationStillActive () );
	}
	private bool IsFallAnimating ()
	{
		return (HexManager.Instance.QueryFallAnimationStillActive () );
	}

	private bool IsFillAnimating ()
	{
		return (HexManager.Instance.QueryFillAnimationStillActive () );
	}



	private void TryScanForFall ()
	{
		HexManager.Instance.QueryScanForFall ();
		HexManager.Instance.QueryMoveGemsToNewHexes();

	}

	private void BackFill ()
	{
		HexManager.Instance.QueryScanForFill ();
	}




}

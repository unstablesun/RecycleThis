using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareGridLogic : MonoBehaviour 
{
	public enum eGridState 
	{
		NoOp,
		Init,
		Wait,
		Diagnostic,
		EmptyScan,
		Calc,
		Dead
	};
	public eGridState _State = eGridState.NoOp;

	public List <GameObject> GemObjectList = null;

	public static SquareGridLogic Instance;

	void Awake () 
	{
		Instance = this;

	}

	void Start () 
	{
		_State = eGridState.Init;

	}

	void Update () 
	{
		switch( _State )
		{
		case eGridState.NoOp:
			break;
		case eGridState.Init:
			_State = eGridState.Diagnostic;
			break;
		case eGridState.Wait:
			break;
		case eGridState.Diagnostic:

			FillEmptyDiagnostic ();

			//FillPreconfigDiagnostic ();

			TryMatchDiagnostic ();

			_State = eGridState.Wait;

			break;
		case eGridState.EmptyScan:
			break;
		case eGridState.Calc:
			break;
		case eGridState.Dead:
			break;
		}

	}


	private void FillEmptyDiagnostic () 
	{
		int count = GemObjectList.Count;

		Debug.Log("FillEmptyDiagnostic");
		SquareGridManager.Instance.SetScanSetting(0);

		GameObject go = SquareGridManager.Instance.QueryScanNextSquare();

		while(go != null)
		{

			SquareGridObject objectScript = go.GetComponent<SquareGridObject> ();

			if(objectScript.NoGemAttached()) {

				GameObject gem = GemManager.Instance.QueryGetAvailableObject();

				if(gem != null) {

					int colorType = (int)Random.Range (0, count);

					GemObject gemScript = gem.GetComponent<GemObject> ();
					gemScript.SetGemSprite(GemObjectList[colorType], (GemObject.eColorType) colorType);

					objectScript.AttachGem(gem);
				}
			}

			go = SquareGridManager.Instance.QueryScanNextSquare();
		}

	}


/*
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
		SquareGridManager.Instance.SetScanSetting(0);

		GameObject go = HexManager.Instance.QueryScanNextHex();

		int index = 0;
		while(go != null)
		{

			SquareGridObject objectScript = go.GetComponent<SquareGridObject> ();

			if(objectScript._Type == SquareGridObject.eType.Main && objectScript.NoGemAttached()) {

				GameObject gem = GemManager.Instance.QueryGetAvailableObject();

				if(gem != null) {

					int colorType = PreConfigBoard3[index++];

					GemObject gemScript = gem.GetComponent<GemObject> ();
					gemScript.SetGemSprite(GemObjectList[colorType], (GemObject.eColorType) colorType);

					objectScript.AttachGem(gem);
				}
			}

			go = SquareGridManager.Instance.QueryScanNextSquare();
		}

	}

*/

	private void TryMatchDiagnostic ()
	{
		SquareGridManager.Instance.QueryScanForLinesAndMark ();

		SquareGridManager.Instance.QueryShowMarkedSquares ();
	}


}

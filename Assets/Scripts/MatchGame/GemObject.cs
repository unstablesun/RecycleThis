using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;

public class GemObject : MonoBehaviour 
{
	public enum eState 
	{
		NoOp,
		Loaded,
		Prepare,
		Active,
		Exploding,
		Dead
	};
	public eState _State = eState.NoOp;

	//these colors are symbolic and used for matching only
	public enum eColorType 
	{
		Red,
		Green,
		Blue,
		Yellow,
		Cyan,
		Purple,
		Orange,
		White,
		Black
	};
	public eColorType ColorType = eColorType.White;

	public enum eAnimState 
	{
		Ready,
		Active,
	};
	public eAnimState AnimState = eAnimState.Ready;



	public GameObject gemSprite;

	private int _id = 0;
	public int ID {
		get {return _id; } 
		set {_id = value; }
	}

	private Sequence gemAnimSequence = null;

	private Vector3 startingScale;

	void Start () 
	{
		startingScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
	}
	
	//void Update () 
	//{}


	public void Reset () 
	{
		_State = eState.Loaded;

		transform.localScale = new Vector3(startingScale.x, startingScale.y, startingScale.z);
	}

	public void SetGemSprite (GameObject go, eColorType ctype) 
	{
		gemSprite.GetComponent<SpriteRenderer>().sprite= go.GetComponent<SpriteRenderer>().sprite;

		ColorType = ctype;
	}
		

	public void StartAnim (float endx, float endy, float z, float offsetx, float speed) 
	{
		transform.DOMove(new Vector3(endx + offsetx, endy, z), speed).SetLoops(1, LoopType.Restart).OnComplete(GemDone);
	}

	void GemDone () 
	{
		//Debug.Log ("CLOUD DONE");
		gameObject.SetActive (false);
	}


	public void StartFallAnim (float endx, float endy, float z, float speed) 
	{
		//slowest----- each of these has an Out as well.
		//InSine
		//InQuad
		//InCubic
		//InQuint
		//InExpo
		//fastest-----

		gemAnimSequence = DOTween.Sequence ().SetEase (Ease.InQuart);
		gemAnimSequence.Append(transform.DOMove(new Vector3(endx, endy, z), speed));
		gemAnimSequence.Append(transform.DOPunchPosition( new Vector3(0.0f, 0.1f, 0.0f), 1f, 4, 0.75f ).OnComplete(FallSequenceComplete));;
		AnimState = eAnimState.Active;

	}

	void FallSequenceComplete () 
	{
		//wait = 0;
		//transform.position = new Vector3 (sx, sy, sz);
		//StartAnim (sx, sy - 7f, sz, 0.25f);
		AnimState = eAnimState.Ready;
	}


	public void StartRemovalAnim (float endx, float endy, float z, float speed) 
	{
		gemAnimSequence = DOTween.Sequence ().SetEase (Ease.InQuart);
		gemAnimSequence.Append(transform.DOMove(new Vector3(endx, endy, z), speed));
		gemAnimSequence.Join(transform.DOScale(new Vector3(2f, 2f, 1f), speed));
		gemAnimSequence.Append(transform.DOPunchPosition( new Vector3(0.0f, 0.1f, 0.0f), 1f, 4, 0.75f ).OnComplete(RemovalSequenceComplete));;
		AnimState = eAnimState.Active;
	}

	void RemovalSequenceComplete () 
	{
		//reset position to storage
		transform.position = new Vector3 (-20, -20, -3);

		AnimState = eAnimState.Ready;
	}



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;

public class EaseObject : MonoBehaviour 
{
	int wait = 0;
	private Sequence easeSequence = null;


	float sx;
	float sy;
	float sz;


	void Start () 
	{
		sx = transform.position.x;
		sy = transform.position.y;
		sz = transform.position.z;

	}
	
	void Update () 
	{
		if (wait++ == 10) {
			

			StartAnim (sx, sy - 7f, sz, 0.25f);
		}
		
	}

	public void StartAnim (float endx, float endy, float z, float speed) 
	{
		//slowest----- each of these has an Out as well.
		//InSine
		//InQuad
		//InCubic
		//InQuint
		//InExpo
		//fastest-----


		easeSequence = DOTween.Sequence ().SetEase (Ease.InQuart);
		easeSequence.Append(transform.DOMove(new Vector3(endx, endy, z), speed));
		easeSequence.Append(transform.DOPunchPosition( new Vector3(0.0f, 0.1f, 0.0f), 1f, 4, 0.75f ).OnComplete(SequenceComplete));;

	}

	void SequenceComplete () 
	{
		wait = 0;
		transform.position = new Vector3 (sx, sy, sz);
		//StartAnim (sx, sy - 7f, sz, 0.25f);
	}

}

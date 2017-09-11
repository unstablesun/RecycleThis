using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGridPanel : MonoBehaviour 
{

	public GridLogic.eGridState GridState;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		GridState = GridLogic.Instance._State;
	}
}

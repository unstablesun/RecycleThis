using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GemManager : MonoBehaviour 
{
	public int objectPoolSize = 128;

	[HideInInspector]
	public List <GameObject> GemObjectList = null;
	private GameObject GemObjectContainer;

	public GameObject StoragePosition;

	public static GemManager Instance;

	void Awake () 
	{
		Instance = this;

		GemObjectList = new List<GameObject>();
	}

	void Start () 
	{
		GemObjectContainer = GameObject.Find ("GemObjectContainer");

		LoadGemObjects();

		QuerySetObjectsLoaded();
	}
	
	void Update () 
	{
		
	}

	private void LoadGemObjects()
	{

		for (int t = 0; t < objectPoolSize; t++) {

			GameObject _sfObj = Instantiate (Resources.Load ("Prefabs/GemObject", typeof(GameObject))) as GameObject;

			if (_sfObj != null) {

				if (GemObjectContainer != null) {
					_sfObj.transform.parent = GemObjectContainer.transform;
				}
				_sfObj.name = "gemObj" + t.ToString ();

				//default storage location
				_sfObj.transform.position = new Vector2 (StoragePosition.transform.position.x, StoragePosition.transform.position.y);

				GemObject objectScript = _sfObj.GetComponent<GemObject> ();
				objectScript.ID = t;

				GemObjectList.Add (_sfObj);

			} else {

				Debug.Log ("Couldn't load hex object prefab");
			}
		}


	}


	void QuerySetObjectsLoaded() 
	{
		foreach(GameObject tObj in GemObjectList)
		{
			GemObject objectScript = tObj.GetComponent<GemObject> ();
			objectScript._State = GemObject.eState.Loaded;

		}
	}

	private void InsertGem (Vector3 position) 
	{




	}

	public GameObject QueryGetAvailableObject() 
	{
		foreach(GameObject tObj in GemObjectList)
		{
			GemObject objectScript = tObj.GetComponent<GemObject> ();
			if(objectScript._State == GemObject.eState.Loaded) {
				objectScript._State = GemObject.eState.Prepare;
				return tObj;
			}
		}
		return null;
	}


}

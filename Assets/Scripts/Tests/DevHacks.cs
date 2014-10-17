using UnityEngine;
using System.Collections;

public class DevHacks : MonoBehaviour 
{
	public GameObject test;

	// Use this for initialization
	void Start () 
	{
		test = GameObject.Find("Hunter");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			GameManager.Instance.NextPlayerTurn();
		}

		if(Input.GetKey(KeyCode.W))
		{
			test.transform.Translate(0.1f, 0f, 0f);
		}
	}
}

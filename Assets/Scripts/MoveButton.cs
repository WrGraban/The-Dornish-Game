using UnityEngine;
using System.Collections;

public class MoveButton : MonoBehaviour 
{
	public BaseNGon owner;
	public Vector3 moveDirection;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnMouseDown()
	{
        owner.Move(moveDirection);
	}
}

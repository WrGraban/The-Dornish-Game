using UnityEngine;
using System.Collections;

public class CollisionDetector : MonoBehaviour 
{
	private Piece piece;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter(Collider other)
	{
		if(this.transform.parent != other.transform && other.tag != "Mover")
		{
			// Check to see if this piece and the other piece belong to different players
			if(piece.owner != other.GetComponent<Piece>().owner)
			{
				Destroy(other.gameObject);
			}
		}
	}
}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NGon3D), typeof(Blinker))]
public class Piece : MonoBehaviour 
{
	// Which player owns this?
	public GameObject owner;
	public GameObject moverPrefab;

	public enum MoveType { VertsOnly, EdgesOnly, EdgesAndVerts };
	public MoveType moveType;

	private NGon3D ngon;
	private Transform moverContainer;
	private Blinker blinker;

	// Use this for initialization
	void Start () 
	{
		ngon = this.GetComponent<NGon3D>();
		moverContainer = this.transform.FindChild("Mover Container");
		blinker = this.GetComponent<Blinker>();
		CreateMovers();
	}

	public void SelectPiece()
	{
		blinker.Enable();
		moverContainer.gameObject.SetActive(true);
	}

	public void UnselectPiece()
	{
		blinker.Disable();
		moverContainer.gameObject.SetActive(false);
	}

	private void SetMoversVisible(bool show)
	{
		
	}

	private void CreateMovers()
	{
		// First get the valid transforms from the N-Gon
		Transform[] validTransforms = GetValidTransforms();

		for(int i = 0; i < validTransforms.Length; ++i)
		{
			GameObject mover = Instantiate(moverPrefab, validTransforms[i].position + new Vector3(0f, ngon.vertOffset / 2f, 0f), validTransforms[i].transform.rotation) as GameObject;
			
			// I need each mover to have a link to the transform.  I had to move the movers up a bit
			//	to make them more visible.
			mover.GetComponent<Mover>().flipTransform = validTransforms[i];
			mover.transform.parent = moverContainer;
		}
	}

	private Transform[] GetValidTransforms()
	{
		// Ugh, I dislike having to create this array just to get the code to compile.  Meh, whatever
		Transform[] valids = new Transform[1];

		if(moveType == MoveType.VertsOnly)
		{
			valids = new Transform[ngon.flipTransforms.Length / 2];

			// First flipTransform is always a vert
			for (int i = 0; i < valids.Length; i++)
				valids[i] = ngon.flipTransforms[i * 2];
		}
		else if (moveType == MoveType.EdgesOnly)
		{
			valids = new Transform[ngon.flipTransforms.Length / 2];

			// Second flipTransform is always an edge
			for (int i = 0; i < valids.Length; i++)
				valids[i] = ngon.flipTransforms[i * 2 + 1];
		}
		else if(moveType == MoveType.EdgesAndVerts)
		{
			// All of them, beetches
			valids = new Transform[ngon.flipTransforms.Length];

			// Set 'em
			for(int i = 0; i < valids.Length; ++i)
				valids[i] = ngon.flipTransforms[i];
		}

		return valids;
	}

	public void FlipMovers()
	{
		for(int i = 0; i < moverContainer.childCount; ++i)
		{
			Vector3 moverPos = moverContainer.GetChild(i).transform.position;
			moverPos.y *= -1;
			moverContainer.GetChild(i).transform.position = moverPos;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void OnCollisionEnter(Collision other)
	{
		if(this.transform != other.transform.parent)
		{
			print(other.transform.name + " ouch");
			Destroy(other.gameObject);
		}
	}
}

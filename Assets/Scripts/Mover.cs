using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour 
{
	public Color highlighted;
	private Color oldColor;

	private bool isMoving = false;
	public float moveDuration;
	public Transform flipTransform;
	private float timer;

	private GameObject rotateCenter;
	private Vector3 rotateAxis;
	private Quaternion startRotation;
	private Quaternion endRotation;
	private NGon3D ngon;
	private Piece piece;

	// Use this for initialization
	void Start () 
	{
		ngon = this.transform.GetComponentInParent<NGon3D>();
		piece = this.transform.GetComponentInParent<Piece>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		/* Removing due to disgusting bugs
		if(isMoving == true)
		{
			timer += Time.deltaTime;

			if(timer < moveDuration)
			{
				float percentage = timer / moveDuration;

				// Rotate that sweet sweet object
				rotateCenter.transform.rotation = Quaternion.Slerp(startRotation, endRotation, percentage);
			}
			else
			{
				// Just kidding, it's still broken...
				//print("HOLY FUCK BOYS, IT WORKED");

				// Reset stuff
				timer = 0f;
				isMoving = false;

				// Make the rotation perfectly at the end
				rotateCenter.transform.rotation = endRotation;

				// Unparent the N-Gon
				this.transform.parent.parent = null;

				// Destroy the epic hero of the day, rotateCenter.  A statue shall be erected in his
				//	honor, and a day set aside to celebrate and meditate on his unrivaled greatness.
				Destroy(rotateCenter);
			}
		}
		*/ 
	}

	public void MouseEnter()
	{
		oldColor = this.renderer.material.color;
		this.renderer.material.color = highlighted;
	}

	public void MouseExit()
	{
		this.renderer.material.color = oldColor;
	}

	public void MouseDown()
	{
		BeginMove();
	}

	private void BeginMove()
	{
		// Don't let them try to do this twice.  It breaks.
		//if (isMoving == true)
		//	return;

		// Make an empty game object
		rotateCenter = new GameObject();
		rotateCenter.name = "Rotate Center";

		// Move it to the flipTransform associated with this mover
		rotateCenter.transform.position = flipTransform.position;

		// Parent the owner of this mover to the empty game object
		this.transform.parent.parent.parent = rotateCenter.transform;

		// Save the axis to rotate around
		rotateAxis = flipTransform.right;
		
		// Flip instantly for now.  My code for the slerp was bad...
		rotateCenter.transform.Rotate(rotateAxis, 180f);
		this.transform.parent.parent.parent = null;

		// Move the movers so that they are on the right side
		piece.FlipMovers();

		Destroy(rotateCenter);
		/* Something is broken here?
		// Calculate the starting and ending rotations
		startRotation = this.transform.rotation;
		
		// Oops, without the temp it snaps to the position immediately
		// This seems a bit silly...
		
		GameObject temp = new GameObject();
		temp.transform.rotation = rotateCenter.transform.rotation;
		temp.transform.Rotate(rotateAxis, 180f);
		endRotation = temp.transform.rotation;
		 */


		// Tell the object to start rotating
		//isMoving = true;
	}
}

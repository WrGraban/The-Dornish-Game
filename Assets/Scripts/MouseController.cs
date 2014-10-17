using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour
{
	private GameObject hoveredMover = null;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			HandleMouseClick();
		}
		else
		{
			HandleMouseHover();
		}
	}

	private void HandleMouseHover()
	{
		RaycastHit[] hits;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		// First I need to check for the mouse hovering over any objects in "Mouse Over Active"
		//	layer.
		hits = Physics.RaycastAll(ray, Mathf.Infinity, LayerMask.NameToLayer("Mouse Over Active"));

		bool found = false;
		// If an object is found, select it and set hoveredMover
		if(hits.Length != 0)
		{
			print(hits.Length);
			for(int i = 0; i < hits.Length; ++i)
			{
				if(hits[i].transform.tag == "Mover")
				{
					
					hoveredMover = hits[i].transform.gameObject;
					hoveredMover.GetComponent<Mover>().MouseEnter();
					found = true;
					break;
				}
			}
		}

		if(found)
		{
			// We're done with this function, return
			return;
		}

		// Now check for objects on the "Mouse Over Inactive" layer
		hits = Physics.RaycastAll(ray, Mathf.Infinity, LayerMask.NameToLayer("Mouse Over Inactive"));

		if(hoveredMover != null && hits.Length > 0)
		{
			hoveredMover.GetComponent<Mover>().MouseExit();
			hoveredMover = null;
		}
	}

	private void HandleMouseClick()
	{
		
	}

	private void MouseLogicOld()
	{
		// Use raycast because OnMouseDown works differently when a rigidbody is attached to the piece
		RaycastHit[] hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		

		if(Input.GetMouseButtonDown(0))
		{
			hit = Physics.RaycastAll(ray);

			if (hit.Length != 0)
			{
				for (int i = 0; i < hit.Length; ++i)
				{
					// Raycast for clicks!
					if (hit[i].transform.tag == "Mover")
					{
						hit[i].transform.GetComponent<Mover>().MouseDown();
					}
				}
			}
		}
		else // This is a bit wonky, but I don't want to raycast twice per frame when the mouse is down
		{
			hit = Physics.RaycastAll(ray);
			if (hit.Length != 0)
			{
				for (int i = 0; i < hit.Length; ++i)
				{
					if(hit.Length == 3)
					{
						print(hit[i].transform.name);
					}
					// Special case for hitting the floor and the floor only
					if(hit.Length == 1 && hit[0].transform.tag == "Floor" && hoveredMover != null)
					{
						hoveredMover.GetComponent<Mover>().MouseExit();
						hoveredMover = null;
						break;
					}

					if (hit[i].transform.tag == "Mover")
					{
						if (hoveredMover != hit[i].transform.gameObject)
						{
							// Leave the current mover
							if (hoveredMover != null)
								hoveredMover.GetComponent<Mover>().MouseExit();

							// We are hovering over a new object!
							hoveredMover = hit[i].transform.gameObject;
							hoveredMover.GetComponent<Mover>().MouseEnter();
							break;
						}
					}
					
					if(hit[i].transform.tag == "Piece Body" && hoveredMover != null && hoveredMover.transform.parent != hit[i].transform)
					{
						hoveredMover.GetComponent<Mover>().MouseExit();
						hoveredMover = null;
					}
				}
			}
		}
	}
}

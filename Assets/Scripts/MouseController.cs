using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour 
{
	private GameObject hoveredMover = null;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
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

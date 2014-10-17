using UnityEngine;
using System.Collections;

public class BaseNGon : MonoBehaviour 
{
    public Vector3[] verts;

    public int sides;
    public float size;

	private MeshFilter filter;
	private MeshRenderer mRenderer;

	public Material material;

	public Transform[] neighbors;
    public GameObject moveTransformPrefab;
    public GameObject moveButtonPrefab;

	// Use this for initialization
	void Awake ()
    {
        Create(sides, size, material);
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

	public void Create(int sides, float size, Material material)
	{
		this.sides = sides;
		this.size = size;
		this.material = material;

		verts = new Vector3[sides];
		neighbors = new Transform[sides];

		GenerateMesh();
        CreateEdgeTransforms();
        CreateVertTransforms();
        CreateMoveButtons();
	}

    private void CreateMoveButtons()
    {
        // Was encountering an infinite loop, this is my solution
        GameObject[] createdButtons = new GameObject[this.transform.childCount];

        // Loop through each of the children
		for(int i = 0; i < this.transform.childCount; ++i)
		{
            // Just do it once to save a little bit of time
			Transform child = this.transform.GetChild(i);

			// Create a moveButton for each child
			GameObject btn = Instantiate(moveButtonPrefab, child.position, Quaternion.identity) as GameObject;

            // Add that rude dude to the array
            createdButtons[i] = btn;
            
			// Let the button know some important data
			MoveButton mover = btn.GetComponent<MoveButton>();
			mover.moveDirection = btn.transform.position - this.transform.position;
			mover.owner = this;

            // Move the button closer to the camera
            btn.transform.Translate(0f, 0f, -0.5f);
            
            // Parent the button
            //btn.transform.parent = this.transform;  // LOL INFINITE LOOP
		}

        // Lastly parent the buttons to the N-Gon
        for(int i = 0; i < createdButtons.Length; ++i)
        {
            print("test");
            createdButtons[i].transform.parent = this.transform;
        }
    }

    private void CreateEdgeTransforms()
    {
        for(int i = 0; i < verts.Length; ++i)
        {
            // Find the midpoint of the edge
            Vector3 midPoint = (verts[i] + verts[(i + 1) % verts.Length] ) / 2f;

            // Draw a line from the position to the midpoint
            Vector3 line = midPoint - this.transform.position;

            // Make that B!
            GameObject xform = Instantiate(moveTransformPrefab, midPoint, Quaternion.LookRotation(line)) as GameObject;
            xform.transform.parent = this.transform;
        }
    }

    private void CreateVertTransforms()
    {
        for(int i = 0; i < verts.Length; ++i)
        {
            //  Draw a line from the center of the object to each vert
            Vector3 line = verts[i] - this.transform.position;

            // Add that line to the vert to find the direction the transform's forward should be facing
            GameObject xform = Instantiate(moveTransformPrefab, verts[i], Quaternion.LookRotation(line)) as GameObject;
            xform.transform.parent = this.transform;
        }
    }

    /*  BUILDING A 3D MESH
     * Sides * 2 number of transforms to define the edges and verts
     * Sides * 2 to determine the number of verts
     * First half of the verts will be some amount above the transforms
     * Seconds half of the verts will be the same amount below the transforms
     * Transforms used for movement calculation, not verts 
     */

    private void GenerateMesh()
    {
        CalculateVerts();

		mRenderer = this.GetComponent<MeshRenderer>();
		filter = this.GetComponent<MeshFilter>();

		filter.mesh.Clear();
		filter.mesh = BuildMesh();

		mRenderer.material = material;
    }

	private Mesh BuildMesh()
	{
		Mesh mesh = new Mesh();

		// We already have the verts
		// UVs
		Vector2[] uvs = new Vector2[verts.Length];

		for (int i = 0; i < verts.Length; ++i)
		{
			if (i % 2 == 0)
			{
				uvs[i] = new Vector2(0, 0);
			}
			else
			{
				uvs[i] = new Vector2(1, 1);
			}
		}

		// Triangles
		int[] tris = new int[3 * (verts.Length - 2)];
		int c1 = 0;
		int c2 = 2;
		int c3 = 1;

		for (int i = 0; i < tris.Length; i += 3)
		{
			tris[i] = c1;
			tris[i + 1] = c2;
			tris[i + 2] = c3;

			c2++;
			c3++;
		}

		// Assign mesh data
		mesh.vertices = verts;
		mesh.uv = uvs;
		mesh.triangles = tris;

		// Recalculations
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();

		return mesh;
	}

    public void Move(Vector3 direction)
    {
        // TODO: This only works for moving along verts, not edges!
        this.transform.Translate(direction * size * 2);
    }

    /// <summary>
    /// Decides where the vertices should be for the N-Gon
    /// </summary>
    /// <remarks>
    /// This function puts the first vertex at the top-most position and calculates
    /// the following vertices in a counter-clockwise motion.
    /// </remarks>
    private void CalculateVerts()
    {
        // Let's set each of the vert's positions to the position of the object
		for (int i = 0; i < verts.Length; ++i)
			verts[i] = this.transform.position;

		// Calculate the angle between the central intersecting lines
		float degrees = 360f / sides;
		float theta = degrees * (Mathf.PI / 180f);

		// First dude is at the top!
		verts[0].y += size;

		// Loop through the rest of the points and find their locations
		for(int i = 1; i < verts.Length; ++i)
		{
			verts[i].x = verts[i - 1].x * Mathf.Cos(theta) - verts[i - 1].y * Mathf.Sin(theta);
			verts[i].y = verts[i - 1].x * Mathf.Sin(theta) + verts[i - 1].y * Mathf.Cos(theta);
		}

		// TODO: Calculate the size of the collider
		CalculateCollider();
		
    }

	private void CalculateCollider()
	{
		Vector3 centerOfEdge = (verts[0] + verts[1]) / 2;

		this.gameObject.GetComponent<CircleCollider2D>().radius = Vector3.Distance(centerOfEdge, this.transform.position);
	}
}

using UnityEngine;
using System.Collections;

[RequireComponent(
	typeof(MeshFilter),
	typeof(MeshRenderer))]
public class NGon3D : MonoBehaviour 
{
    public Vector3[] verts;

    // Named because hopefully the piece will flip along that point to crush enemy pieces
    public Transform[] flipTransforms;
	private Transform flipTransformContainer;

    public int sides;
    public float size;
    public float vertOffset;

    private MeshFilter filter;
    private MeshRenderer mRenderer;

    public Material material;

    public GameObject flipTransformPrefab;
	public bool drawDebugLines = false;
    private Vector3 startPos;

    private int lastSides;
	public bool isMoving;

	// Use this for initialization
	void Awake ()
    {
		flipTransformContainer = this.transform.FindChild("Flip Transform Container");
        Create();
	}

	// TODO: If this function is necessary, it must be changed?
    private void KillChildren()
    {
        int count = this.transform.childCount;

        while(this.transform.childCount != 0)
        {
			Transform kid = this.transform.GetChild(0);
			kid.parent = null;
			Destroy(kid);
        }
    }

    public void Create()
    {
        // Clean up all of the children first!
        //KillChildren();

        // TODO: Discover why I have to build the object around the origin before I move it
        startPos = this.transform.position;
        this.transform.position = Vector3.zero;

        // Create arrays of the correct size
        verts = new Vector3[sides * 2];
        flipTransforms = new Transform[sides * 2];

        // Calculate the flip transforms using maths
        CalculateFlipTransforms();

        // Calculate the verts using the flip transforms and the offset
        CalculateVerts();

        // Generate the mesh
        GenerateMesh();

        // Finally, place the object back to it's starting position
        this.transform.position = startPos;
    }

	private void GenerateMesh()
	{
		mRenderer = this.GetComponent<MeshRenderer>();
		filter = this.GetComponent<MeshFilter>();

        if(filter.sharedMesh != null)
		    filter.sharedMesh.Clear();

		filter.sharedMesh = BuildMesh();

        // Modify the collider data
		// CHANGED: This collider is now on a child object because of how OnMouseDown reacts with rigidbodies
        this.transform.FindChild("Collision Object").GetComponent<MeshCollider>().sharedMesh = filter.sharedMesh;
		this.GetComponent<MeshCollider>().sharedMesh = filter.sharedMesh;

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
		// Longhand calculation of triangles for easier reading
		int trisOnTop = (sides - 2);
		int trisOnBottom = (sides - 2);
		int trisOnSides = 2 * sides;

		int[] tris = new int[3 * (trisOnSides + trisOnTop + trisOnBottom)];
		int indexCount = 0;

		// This is gonna get messy
		// ------- TOP TRIS -------- //
		int c1 = 0;
		int c2 = 2;
		int c3 = 1;
		
		for (int i = 0; i < trisOnTop; indexCount += 3, ++i)
		{
			tris[indexCount] = c1;
			tris[indexCount + 1] = c2;
			tris[indexCount + 2] = c3;

			c2++;
			c3++;
		}

		// ------- SIDE TRIS -------- //
		int topLeft = 0;
		int topRight = 1;
		int bottomLeft = sides;
		int bottomRight = sides + 1;

		for (int i = 0; i < sides; indexCount += 3, i++)
		{
			// Top tri on the strip
			tris[indexCount] = topLeft;
			tris[indexCount + 1] = topRight;
			tris[indexCount + 2] = bottomLeft;

			indexCount += 3;

			// Bottom tri on the strip
			tris[indexCount] = topRight;
			tris[indexCount + 1] = bottomRight;
			tris[indexCount + 2] = bottomLeft;

			// Increment!
			topLeft++;
			bottomLeft++;
			topRight = (topRight + 1) % sides;

            // Strange edge case for bottomRight
            if ((bottomRight + 1) % sides == 0)
                bottomRight = sides;
            else
			    bottomRight++;
		}
		// ------- BOTTOM TRIS -------- //
		c1 = sides;
		c2 = sides + 1;
		c3 = sides + 2;

		for (int i = 0; i < trisOnBottom; indexCount += 3, i++)
		{
			tris[indexCount] = c1;
			tris[indexCount + 1] = c2;
			tris[indexCount + 2] = c3;

			c2++;
			c3++;
		}
		print(indexCount + "\\" + tris.Length);

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

	private void CalculateVerts()
	{
		// Find the relevant flip transforms that are on the verts of the N-Gon
		Vector3[] flipVerts = new Vector3[sides];
		for (int i = 0; i < sides; ++i)
			flipVerts[i] = flipTransforms[i * 2].position;

		// -- Top
		for(int i = 0; i < sides; ++i)
			verts[i] = flipVerts[i] + new Vector3(0f, vertOffset, 0f);

		// -- Bottom
		for(int i = 0; i < sides; ++i)
			verts[i + sides] = flipVerts[i] + new Vector3(0f, -vertOffset, 0f);
	}

    private void CalculateFlipTransforms()
    {
        // Calculate the verts of the N-Gon
        Vector3[] tempVerts = new Vector3[sides];

        // Set each transform's position to the starting loc of the gon
        for(int i = 0; i < sides; ++i)
        {
            tempVerts[i] = this.transform.position;
        }

        // Calculate the angle between the central intersecting lines
        float degrees = 360f / sides;
        float theta = degrees * (Mathf.PI / 180f);

        // First dude is at the top!
		// NOTE: For 3D I'm going to replace y with z
        tempVerts[0].z += size;
        print(size);

        // Loop through the rest of the points and find their locations
		for (int i = 1; i < tempVerts.Length; ++i)
        {
            tempVerts[i].x = tempVerts[i - 1].x * Mathf.Cos(theta) - tempVerts[i - 1].z * Mathf.Sin(theta);
            tempVerts[i].z = tempVerts[i - 1].x * Mathf.Sin(theta) + tempVerts[i - 1].z * Mathf.Cos(theta);
		}

		// Create the transforms used for piece flipping
		CreateFlipTransforms(tempVerts);
    }

	private void CreateFlipTransforms(Vector3[] tempVerts)
	{
		Vector3 curPoint, nextPoint, midPoint;
		curPoint = tempVerts[0];
		int count = 0;

		// Build the forward
		Vector3 forward = curPoint - this.transform.position;

		// Make it, child it, add it to the array
		GameObject flip = Instantiate(flipTransformPrefab, curPoint, Quaternion.LookRotation(forward)) as GameObject;
		flip.transform.parent = flipTransformContainer;
		flipTransforms[count++] = flip.transform;

		for(int i = 1; i < tempVerts.Length; ++i)
		{
			// Get the next point and build the forward
			nextPoint = tempVerts[i];

			// -- Mid Point
			// Find the edge flip transform position and calc the forward
			midPoint = (curPoint + nextPoint) / 2f;
			forward = midPoint - this.transform.position;

			// Make it, child it, add it to the array
			flip = Instantiate(flipTransformPrefab, midPoint, Quaternion.LookRotation(forward)) as GameObject;
			flip.transform.parent = flipTransformContainer;
			flipTransforms[count++] = flip.transform;

			// -- Next Point
			// Calc the forward
			forward = nextPoint - this.transform.position;

			// Make it, child it, add it to the array
			flip = Instantiate(flipTransformPrefab, nextPoint, Quaternion.LookRotation(forward)) as GameObject;
			flip.transform.parent = flipTransformContainer;
			flipTransforms[count++] = flip.transform;

			curPoint = nextPoint;
		}

		// Do the last one.  holy shit I hope this works.
		midPoint = (tempVerts[tempVerts.Length - 1] + tempVerts[0]) / 2f;
		forward = midPoint - this.transform.position;

		// Make it, child it, add it to the array
		flip = Instantiate(flipTransformPrefab, midPoint, Quaternion.LookRotation(forward)) as GameObject;
		flip.transform.parent = flipTransformContainer;
		flipTransforms[count] = flip.transform;
	}

	// Update is called once per frame
	void Update () 
    {
		if(drawDebugLines == true)
			for (int i = 0; i < flipTransforms.Length; ++i)
				Debug.DrawLine(flipTransforms[i].position, flipTransforms[(i + 1) % flipTransforms.Length].position);
	}
}

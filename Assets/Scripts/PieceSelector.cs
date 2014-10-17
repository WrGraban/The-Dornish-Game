using UnityEngine;
using System.Collections;

public class PieceSelector : MonoBehaviour 
{
	private Piece piece;

	// Use this for initialization
	void Start () 
    {
		piece = this.GetComponent<Piece>();
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    void OnMouseDown()
    {
		GameManager.Instance.ClickPiece(piece);
    }
}

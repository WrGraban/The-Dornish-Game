using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PieceDraftingMenu : MonoBehaviour 
{
	// All of the references needed by the menu
	public Text p1PieceCountText;
	public Text p2PieceCountText;

	public Text p1SelectedPieces;
	public Text p2SelectedPieces;

	private int p1PieceCount = 1;
	private int p2PieceCount = 1;

	public int totalPiecesAllowed = 4;

	public Text curPlayerName;
	private string currentPlayer;
	public string player1Name;
	public string player2Name;

	// Use this for initialization
	void Start ()
	{
		p1PieceCountText.text = "1 / " + totalPiecesAllowed;
		p2PieceCountText.text = "1 / " + totalPiecesAllowed;

		currentPlayer = player1Name;
		curPlayerName.text = player1Name;
	}

	// Update is called once per frame
	void Update () 
	{
	
	}

	public void PieceSelected(string name)
	{
		if (currentPlayer == player1Name)
		{
			p1PieceCount += 1;
			p1PieceCountText.text = p1PieceCount + " / " + totalPiecesAllowed;
			currentPlayer = player2Name;
			curPlayerName.text = player2Name;

			// Add the selection to the list
			p1SelectedPieces.text += "\n" + name;
		}
		else
		{
			p2PieceCount += 1;
			p2PieceCountText.text = p2PieceCount + " / " + totalPiecesAllowed;
			currentPlayer = player1Name;
			curPlayerName.text = player1Name;

			// Add the selection to the list
			p2SelectedPieces.text += "\n" + name;
		}

		// Check for the condition that signals the end of mode
		if(p2PieceCount == totalPiecesAllowed && p1PieceCount == totalPiecesAllowed)
		{
			print("Draft phase complete!");
		}
	}
}

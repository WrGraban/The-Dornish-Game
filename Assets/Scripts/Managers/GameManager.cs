using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
    public Player[] players;
    public int curPlayer = 0;

	public Text curPlayerText;
	private Piece curSelectedPiece = null;

	public string[] player1Pieces;
	public string[] player2Pieces;

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
                _instance = GameObject.Find("Manager Object").GetComponent<GameManager>();

            return _instance;
        }
    }

	public void NextPlayerTurn()
	{
		curPlayer = ++curPlayer % players.Length;
		curPlayerText.text = players[curPlayer].name + "'s Turn";
	}

	public void ClickPiece(Piece piece)
	{
		if (curSelectedPiece == null)
		{
			piece.SelectPiece();
			curSelectedPiece = piece;
			return;
		}

		if (curSelectedPiece == piece)
		{
			piece.UnselectPiece();
			curSelectedPiece = null;
		}
		else
		{
			curSelectedPiece.UnselectPiece();
			piece.SelectPiece();
			curSelectedPiece = piece;
		}
	}
}
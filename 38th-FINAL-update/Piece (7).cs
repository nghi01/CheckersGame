using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool isWhite;
    public bool isKing;

    public (bool, bool) ValidMove(Piece[,] board, int x1, int y1, int x2, int y2)
    {
        //if moving on top of another piece
        if (board[x2, y2] != null)
            return (false, false);

        int deltaMoveX = Mathf.Abs(x1 - x2);
        int deltaMoveY = y2 - y1;

        // WHITE TEAM or KING
        if (isWhite || isKing)
        {
            if (deltaMoveX == 1)
            {
                if (deltaMoveY == 1)
                    return (true,false);
            }
            else if (deltaMoveX == 2)
            {
                if (deltaMoveY == 2)
                {
                    Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isWhite != isWhite)
                    {
                        return (true, true);
                    }
                }
            }
            
        }

        // BLACK TEAM or KING
        if (!isWhite || isKing)
        {
            if (deltaMoveX == 1)
            {
                if (deltaMoveY == -1)
                    return (true, false);
            }
            else if (deltaMoveX == 2)
            {
                if (deltaMoveY == -2)
                {
                    Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isWhite != isWhite)
                    {
                        return (true, true);
                    }
                }
            }           
           
        }

        return (false, false);
    }
}

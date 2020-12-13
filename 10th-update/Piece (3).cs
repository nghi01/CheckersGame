using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{

    public bool isWhite;
    public bool isKing;

    // coordinates of destination
    public bool IsForceToMove(Piece[,] board, int x, int y)
    {
        // White Team
        if (isWhite || isKing)
        { 
            // Top Left
            // b/c white piece is going up 
            // ??
            if (x >= 2 && y <= 5)
            {
                Piece p = board[ x-1, y+1 ];
                // if there is a piece, and it is not the same color as ours
                if (p != null && p.isWhite != isWhite)
                {
                    if (board[ x-2, y+2 ] == null)
                    {
                        return true;
                    }
                }
                
            }

            // Top Right
            // b/c white piece is going up
            // ?? 
            if (x <= 5 && y <= 5)
            {
                // x+1 bc need to check on the Right side
                Piece p = board[ x+1, y+1 ];
                // if there is a piece, and it is not the same color as ours
                if (p != null && p.isWhite != isWhite)
                {
                    if (board[ x+2, y+2 ] == null)
                    {
                        return true;
                    }
                }
            }
            
        }

        // Black team
        if (!isWhite || isKing)
        {
            // Bottom Left
            // b/c white piece is going up 
            if (x >= 2 && y >= 2)
            {
                Piece p = board[ x-1, y-1 ];
                // if there is a piece, and it is not the same color as ours
                if (p != null && p.isWhite != isWhite)
                {
                    if (board[ x-2, y-2 ] == null)
                    {
                        return true;
                    }
                }
            }

            // Bottom Right
            // b/c white piece is going up
            if (x <= 5 && y >= 2)
            {
                // x+1 bc need to check on the Right side
                Piece p = board[ x+1, y-1 ];
                // if there is a piece, and it is not the same color as ours
                if (p != null && p.isWhite != isWhite)
                {
                    if (board[ x+2, y-2 ] == null)
                    {
                        return true;
                    }
                }
            }
        }
        
        return false;

    }
    public bool ValidMove( Piece[,] board, int x1, int y1, int x2, int y2)
    {
        // If youre moving on top of another piece
        if (board[x2,y2] != null)
        {
            return false;
        }

        // use Abs because moving in two directions
        // how many tiles youre jumping in x
        // 1 tile = simple jump; 2 tiles = a kill
        int deltaMoveX = Mathf.Abs(x1 - x2);
        
        // need -1 if youre on the Black team
        // int deltaMoveY = y1 - y2;
        // destination - start
        int deltaMoveY = y2 - y1;
        
        if (isWhite || isKing) 
        {
            if (deltaMoveX == 1) // normal jump
            {
                // going up is a valid move
                if (deltaMoveY == 1)  
                {
                    return true;
                }
            }
            else if (deltaMoveX == 2) // killer jump
            {
                if (deltaMoveY == 2)
                {
                    // grab the piece in the middle
                    // what is on top of the tile in the middle of the destination and the start
                    // ????
                    Piece p = board[ (x1 + x2) / 2, (y1 + y2) / 2];

                    // check if allowed to jump
                    // not able to kill/ jump over our own team
                    // can jump if piece is not ours
                    if (p != null && p.isWhite != isWhite)
                    {
                        return true;
                    }
                }

            }
        }

        // doesnt need else if b/c in case isKing and doesnt meet whiteMove, might meet blackMove
        if (!isWhite || isKing)
            {
                if (deltaMoveX == 1) 
                {
                    // moving down the board
                    if (deltaMoveY == -1)
                    {
                        return true;
                    }
                }
                else if (deltaMoveX == 2)
                {
                    if (deltaMoveY == -2)
                    {
                        Piece p = board[ (x1 + x2) / 2, (y1 + y2) / 2];
                        if (p != null && p.isWhite != isWhite)
                        {
                            return true;
                        }
                    }

                }
            }

        return false;
    }

    void Start()
    {
        
    }
    void Update()
    {
        
    }
}

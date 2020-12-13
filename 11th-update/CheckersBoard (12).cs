using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersBoard : MonoBehaviour
{
  public float Height = 25.0f;
  public GameObject whitePiecePrefab;
  public GameObject blackPiecePrefab;
  public Piece[,] pieces = new Piece[8,8];
  private Vector3 boardoffset = new Vector3(-4.0f,0,-4.0f);
  private Vector3 pieceoffset = new Vector3(0.5f,0,0.5f);

  private bool isWhiteTurn;
  private bool isWhite;
  private bool hasKilled;

  private Piece selectedPiece;
  private List<Piece> forcedPieces;

  private Vector2 mouseOver;
  private Vector2 startDrag;
  private Vector2 endDrag;

  private void GenerateBoard()
  {
    /*Whiteteam*/
    for (int y = 0; y < 3; y++)
    {
      bool oddRow = (y % 2 == 0);
      for (int x = 0; x <8; x += 2)
      {
        GeneratePiece((oddRow) ? x : x+1,y);
      }
    }
    /*blackteam*/
    for (int y = 7; y > 4; y--)
    {
      bool oddRow = (y % 2 == 0);
      for (int x = 0; x <8; x += 2)
      {
        GeneratePiece((oddRow) ? x : x+1,y);
      }
    }
  }
  //   int b = 1;
  //   for (int a =1; a<8;a=a+2)
  //   {
  //     GeneratePiece(a,b);
  //   }
  //
  //   for (int y=7;y>4;y=y-2)
  //   {
  //     for (int x=1; x<8;x = x+2)
  //     {
  //       GeneratePiece(x,y);
  //     }
  //   }
  //   b = 6;
  //   for (int a = 0;a<8;a=a+2)
  //   {
  //     GeneratePiece(a,b);
  //   }
  // }

  private void GeneratePiece(int x,int y)
  {
    bool isPieceWhite = (y > 3) ? false : true;
    GameObject gp = Instantiate((isPieceWhite)?whitePiecePrefab:blackPiecePrefab) as GameObject;
    gp.transform.SetParent(transform);
    Piece p = gp.GetComponent<Piece>();
    pieces[x,y]= p;
    MovePiece(p,x,y);
  }

  // private void GeneratePiece2(int x,int y)
  // {
  //   GameObject gp = Instantiate(blackPiecePrefab) as GameObject;
  //   gp.transform.SetParent(transform);
  //   Piece p = gp.GetComponent<Piece>();
  //   pieces[x,y]= p;
  //   MovePiece(p,x,y);
  // }

  private void MovePiece(Piece p, int x, int y)
  {
    p.transform.position = (Vector3.right * x)+ (Vector3.forward * y) + boardoffset + pieceoffset;
  }

  private void UpdateMouseOver()
  {
    RaycastHit hit;
    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit,Height,LayerMask.GetMask("Board")))
    {
      mouseOver.x = (int)(hit.point.x-boardoffset.x);
      mouseOver.y = (int)(hit.point.z-boardoffset.z);
    }
    else
    {
      mouseOver.x = -1;
      mouseOver.y = -1;
    }
    // Debug.Log(mouseOver);

  }

  private void UpdatePieceDrag(Piece p)
  {
    RaycastHit hit;
    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit,Height,LayerMask.GetMask("Board")))
    {
      // if hit something
      p.transform.position = hit.point + Vector3.up;
    }

  }

  private void SelectPiece(int x, int y)
  {
    // Out of Bounds
    // dont do this if(x<0 || x >= pieces.Length || y<0 || y>=pieces.Length)
    if(x<0 || x >= 8 || y<0 || y>= 8)
    {
      return;
    }

    Piece p = pieces[x,y];
    if (p != null)
    {
      selectedPiece = p;
      startDrag = mouseOver;
      Debug.Log(selectedPiece.name);
    }

  }
  //
  private void TryMove(int x1, int y1, int x2, int y2)
  {
  //   forcedPieces = ScanForPossibleMove();
  //
    //Multiplayer Support
    startDrag = new Vector2(x1, y1);
    endDrag = new Vector2(x2, y2);
    selectedPiece = pieces[x1, y1];
    MovePiece(selectedPiece, x2, y2);


    // end position is out of bounds
    if (x2<0 || x2 >= 8 || y2<0 || y2 >= 8)
    {
      if (selectedPiece != null)
      {
        // everytime the move is not allowed
        MovePiece(selectedPiece, x1, y1);
        startDrag = Vector2.zero;
        selectedPiece = null;
        return;
      }
    }

    if (selectedPiece != null)
    {
      // if it has not moved
      if (endDrag == startDrag)
      {
        // if move is cancelled
        MovePiece(selectedPiece, x1, y1);
        startDrag = Vector2.zero;
        selectedPiece = null;
        return;
      }

      // check if its a valid move
      if (selectedPiece.ValidMove(pieces, x1, y1, x2, y2))
      {
        // Did we kill anything?
        // If this is a jump
        // IS THIS A TYPO???
        if (Mathf.Abs(x2-x2) == 2)
        {
          //remove the piece in the middle
          Piece p = pieces[ (x1 + x2) / 2, (y1 + y2) / 2 ];
          if (p != null)
          {
            pieces[(x1 + x2) / 2, (y1 + y2) / 2 ] = null;
            // might be gameObject
            Destroy(p.gameObject);
            hasKilled = true;
          }
        }
        pieces[x2,y2] = selectedPiece;
        pieces[x1,y2] = null;
        MovePiece(selectedPiece, x2, y2);

        EndTurn();
      }
    }
  }
  //
  //       // Were we supposed to kill anything?
  //
  //       // if forcedtomove BUT didnt kill anything --> invalid  move
  //       if (forcedPieces.Count != 0 && !hasKilled)
  //       {
  //         MovePiece(selectedPiece, x1, y1);
  //         startDrag = Vector2.zero;
  //         selectedPiece = null;
  //         return;
  //       }
  //
  //
  //     // invalid move
  //     else
  //     {
  //       MovePiece(selectedPiece, x1, y1);
  //       startDrag = Vector2.zero;
  //       selectedPiece = null;
  //       return;
  //     }
  //
  //   }
  //   // MovePiece(selectedPiece, x2, y2);
  // }
  //
  private void EndTurn()
  {
    selectedPiece = null;
    startDrag = Vector2.zero;
    isWhiteTurn = !isWhiteTurn;
    //hasKilled = false;
    CheckVictory();
  }

  private void CheckVictory()
  {
  }
  //
  // private List<Piece> ScanForPossibleMove()
  // {
  //   forcedPieces = new List<Piece>();
  //
  //   // Check all the pieces
  //   for (int i = 0; i < 8; i++)
  //     for (int j = 0; j < 8; j++)
  //       if (pieces[i,j] != null && pieces[i,j].isWhite == isWhiteTurn)
  //         if ( pieces[i,j].IsForceToMove(pieces, i, j) )
  //           forcedPieces.Add( pieces[i,j] );
  //
  //   return forcedPieces;
  // }
  //
  // Start is called before the first frame update
  void Start()
  {
    isWhiteTurn = true;
    GenerateBoard();
  }

  // Update is called once per frame
  void Update()
  {
    UpdateMouseOver();
    //Debug.Log(mouseOver);

    // if it is my turn
    {
      int x = (int)mouseOver.x;
      int y = (int)mouseOver.y;

      if (selectedPiece != null)
      {
        UpdatePieceDrag(selectedPiece);
      }
      if (Input.GetMouseButtonDown(0))
      {
        SelectPiece(x,y);
      }
      if (Input.GetMouseButtonUp(0))
      {
        TryMove((int)startDrag.x, (int)startDrag.y, x, y);
      }
  //
  //   }
    }
  }
}

// RULE notes
// If can jump over to kill, then must do it

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using System.Math;

public class CheckersBoard : MonoBehaviour
{
    public Piece[,] pieces = new Piece[8, 8];

    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;

    public bool isWhite;

    private Piece selectedPiece;

    private Vector3 boardOffset = new Vector3(-4.0f, 0, -4.0f);
    private Vector3 pieceOffset = new Vector3(0.5f, 0, 0.5f);

    private Vector2 mouseOver;
    private Vector2 starDrag;
    private Vector2 endDrag;

    private bool isWhiteTurn;
    private bool hasKilled;
    private List<Piece> forcePieces;
    private List<Piece> allpieces;
    List<(int, int)> skipped = new List<(int, int)>();

    private int whitePieceCount = 12;
    private int blackPieceCount = 12;
    private int white_kings = 0;
    private int black_kings = 0;

    bool gameOver = false;

    public GameObject WhiteWinTextObject;
    public GameObject BlackWinTextObject;


    private int evaluate()
    {
      // keep track of Kings to motivate AI to move forward
      int evaluation = (int) ( whitePieceCount - blackPieceCount + (white_kings * 0.5 - black_kings * 0.5) );
      return evaluation;
    }

    // position is current state of the board
    // black player - AI is the minimizingPlayer
    private int minimax(Piece[,] position, int depth, bool maximizingPlayer)
    {
      if (depth==0 || gameOver==true)
      {
        // return position.evaluate, position;
        return 0;
      }
      
      if (maximizingPlayer == true)
      {
        int maxEval = -(int.MaxValue);
        return maxEval;
      }
      else
      {
        int minEval = int.MaxValue;
        return minEval;
      }
      
    }

    private Piece[,] deepcopy(Piece[,] sourceBoard)
    {
      Piece[,] result = new Piece[8, 8];

      for (int i = 0; i < 8; i++)
      {
          for (int j = 0; j < 8; j++)
          {
              result[i, j] = new Piece();
              result[i, j].isWhite = sourceBoard[i, j].isWhite;
              result[i, j].isKing = sourceBoard[i, j].isKing;
          }
      }
      return result;
    }

    // just for the AI
    private List<Piece[,]> get_boardstates(Piece[,] board, bool isWhite)
    {
      List<Piece[,]> board_states = new List<Piece[,]>();
      Piece[,] temp_board = deepcopy(board);

      // loop through all the pieces and their moves
      // choose a piece and its move
      // make a move on the temp_board
      // check to see if new positions show on the temp_board
      // add temp_board to board_states

      // keep track of pieces on the Other team as well
      // when a piece is killed, keep a COUNTER, 
      // and mark said piece somehow to skip over it next time in future states
      for (int i = 0; i < 8; i++)
      {
        for (int j = 0; j < 8; j++)
        {
          
        }
      }
      return board_states;
    }

    // private List<int[,]> get_all_moves(Piece[,] board, bool isWhite)
    // {
    //   List<Piece[,]> board_states = new List<Piece[,]>();

    //   List<Piece> allpieces = get_all_pieces(isWhite);
    //   Dictionary<(int, int), List<(int, int)>> valid_moves = new Dictionary<(int, int), List<(int, int)>>();
    //   for int(i = 0; i < allpieces.Count; i++)
    //   {
        
    //     valid_moves = board.get_valid_moves(piece)
       
    //     for state, skip in valid_moves.items()
    //         temp_board = deepcopy(board)
    //         temp_piece = temp_board.get_piece(piece.row, piece.col)
    //         new_board = simulate_move(temp_piece, move, temp_board, game, skip)
    //         moves.append(new_boardstate)
    //   }
       
      
    //   return board_states
    // }
    

    // private List<Piece> get_all_pieces(bool isWhite) {
    //   allpieces = new List<Piece>();
    //   for (int row = 0; row < 8; row++) {
    //     for (int col = 0; col < 8; col++) {
    //       Piece p = pieces[col, row];
    //       if (p != null && p.isWhite == isWhite) {
    //         allpieces.Add(p);
    //         //Destroy(p.gameObject);
    //       }
    //     }
    //   }
    //   return allpieces;
    // }

    // private Piece get_piece(int col, int row) {
    //   // Piece p = pieces[col, row];
    //   // Destroy(p.gameObject);
    //   return pieces[col,row];
    // }
    // //When called put pieces[col,row]

    // // private void addtoDictionary()
    // // {
    // //   int col = 2;
    // //   int row = 2;
    // //   Piece p = pieces[2, 2];
    // //   //p.isWhite = true;
    // //   Dictionary<(int, int), List<(int, int)>> moves = get_valid_moves(p, col, row);
    // // }

    // private Dictionary<(int, int), List<(int, int)>> get_valid_moves(Piece p, int col, int row)
    // {
    //   Dictionary<(int, int), List<(int, int)>> moves = new Dictionary<(int, int), List<(int, int)>>();
    //   int left = col-1;
    //   int right = col+1;
    //   int localrow = row;
    //   Debug.Log("x: " + col);
    //   Debug.Log("y: " + row);
    //   if ((p.isWhite==isWhite) || (p.isKing)) {
    //     Debug.Log("iswhite");
    //     Dictionary<(int, int), List<(int, int)>> traversemove = traverse_left(localrow-1, System.Math.Max(localrow-3, -1), -1, p.isWhite, left, skipped);
    //     AddOrUpdate(moves, traversemove);
    //     traversemove = traverse_right(localrow-1, System.Math.Max(localrow-3, -1), -1, p.isWhite, right, skipped);
    //     AddOrUpdate(moves, traversemove);
    //   }
    //   if ((p.isWhite!=isWhite) || (p.isKing)) {
    //     Debug.Log("isblack");
    //     Dictionary<(int, int), List<(int, int)>> traversemove = traverse_left(localrow+1, System.Math.Min(localrow+3, 8), 1, p.isWhite, left, skipped);
    //     AddOrUpdate(moves, traversemove);
    //     traversemove = traverse_right(localrow+1, System.Math.Min(localrow+3, 8), 1, p.isWhite, right, skipped);
    //     AddOrUpdate(moves, traversemove);
    //   }

    //   return moves;
    // }

    // private void AddOrUpdate(Dictionary<(int, int), List<(int, int)>> mainmove, Dictionary<(int, int), List<(int, int)>> traversemove)
    // {
    //   //List<string> keyList = new List<string>(this.traversemove.Keys);
    //   foreach (KeyValuePair<(int, int), List<(int, int)>> item in traversemove)
    //   // for (int x=0; x<traversemove.Count; x++)
    //   {
    //     (int, int) key = item.Key;
    //     //Debug.Log("printing key: " + key);
    //     List<(int, int)> value = item.Value;
    //     // foreach((int, int) x in value) {
    //     //   Debug.Log("printing value" + x);
    //     // }
    //     if (mainmove.ContainsKey(key))
    //     {
    //       mainmove[key] = value;
    //     }
    //     else
    //     {
    //       mainmove.Add(key, value);
    //     }
    //   }

    // }

    // private Dictionary<(int, int), List<(int, int)>> traverse_left(int start, int stop, int step, bool isWhite, int left, List<(int, int)> skipped)
    // {
    //   Dictionary<(int, int), List<(int, int)>> moves = new Dictionary<(int, int), List<(int, int)>>();
    //   List<(int, int)> last = new List<(int, int)>();
    //   skipped = new List<(int, int)>();
    //   for (int row = start; row < stop; row+=step) {
    //     if (left < 0) {
    //       break;
    //     }
    //     Piece currentp = pieces[row,left];
    //     (int, int) current = (row, left);
    //     if (currentp == null) {
    //       if (skipped.Count>0 && last.Count==0) {
    //         break;
    //       }
    //       else if (skipped.Count>0) {
    //         (int, int) newtuple = (row, left);
    //         last = last.Concat(skipped).ToList();
    //         moves.Add(newtuple, last);
    //       }
    //       else {
    //         (int, int) newtuple = (row, left);
    //         moves.Add(newtuple, last);
    //       }

    //       if (last.Count>0) {
    //         int r;
    //         if (step == -1) {
    //           r = System.Math.Max(row-3, 0);
    //         }
    //         else {
    //           r = System.Math.Min(row+3, 8);
    //         }
    //         Dictionary<(int, int), List<(int, int)>> traversemove = traverse_left(row+step, r, step, isWhite, left-1, skipped=last);
    //         AddOrUpdate(moves, traversemove);
    //         traversemove = traverse_right(row+step, r, step, isWhite, left+1, skipped=last);
    //         AddOrUpdate(moves, traversemove);
    //       }
    //       break;

    //     }
    //     else if (currentp.isWhite == isWhite) {
    //       break;
    //     }
    //     else {
    //       last = new List<(int, int)>();
    //       last.Add(current);
    //     }
    //     left--;
    //   }
    //   return moves;
    // }

    // private Dictionary<(int, int), List<(int, int)>> traverse_right(int start, int stop, int step, bool isWhite, int right, List<(int, int)> skipped)
    // {
    //   Dictionary<(int, int), List<(int, int)>> moves = new Dictionary<(int, int), List<(int, int)>>();
    //   List<(int, int)> last = new List<(int, int)>();
    //   skipped = new List<(int, int)>();
    //   for (int row = start; row < stop; row+=step) {
    //     if (right >= 8) {
    //       break;
    //     }
    //     Piece currentp = pieces[row, right];
    //     (int, int) current = (row, right);
    //     if (currentp == null) {
    //       if (skipped.Count>0 && last.Count==0) {
    //         break;
    //       }
    //       else if (skipped.Count>0) {
    //         (int, int) newtuple = (row, right);
    //         last = last.Concat(skipped).ToList();
    //         moves.Add(newtuple, last);
    //       }
    //       else {
    //         (int, int) newtuple = (row, right);
    //         moves.Add(newtuple, last);
    //       }

    //       if (last.Count>0) {
    //         int r;
    //         if (step == -1) {
    //           r = System.Math.Max(row-3, 0);
    //         }
    //         else {
    //           r = System.Math.Min(row+3, 8);
    //         }
    //         Dictionary<(int, int), List<(int, int)>> traversemove = traverse_left(row+step, r, step, isWhite, right-1, skipped=last);
    //         AddOrUpdate(moves, traversemove);
    //         traversemove = traverse_right(row+step, r, step, isWhite, right+1, skipped=last);
    //         AddOrUpdate(moves, traversemove);
    //       }
    //       break;

    //     }
    //     else if (currentp.isWhite == isWhite) {
    //       break;
    //     }
    //     else {
    //       last = new List<(int, int)>();
    //       last.Add(current);
    //     }
    //     right++;
    //   }
    //   return moves;
    // }

    private void chooseRandom_PieceandMove(Dictionary<(int, int), List<(int, int)>> moves)
    {

      bool noValidMove = true;

      foreach (KeyValuePair<(int, int), List<(int, int)>> kvp in moves)
      {
        // check size of the list of moves for each piece/key
        if (kvp.Value.Count>0)
        {
          noValidMove = false;
        }
      }

      // there are pieces left BUT they can't move because there are no valid moves
      if (noValidMove)
      {
        CheckVictory(noValidMove);
      }

      else
      {
        // generate random number in range moves.Count to choose a random key
        // choose a random move from the Key
        int keynum =  Random.Range(0, moves.Count+1);  
        Debug.Log("keynum " + keynum);

        (int,int) chosenkey;
        (int,int) chosenmove = (0,0);

        chosenkey = moves.ElementAt(keynum).Key;

        foreach (KeyValuePair<(int, int), List<(int, int)>> kvp in moves)
          {
            Debug.Log("key: " + kvp.Key);
            if (kvp.Key == chosenkey)
            {
              Debug.Log("KEY FOUND");
              Debug.Log("chosenkey: " + chosenkey);

              int valuenum = Random.Range(0, kvp.Value.Count);
              Debug.Log("valuenum " + valuenum);

              int counterval = 0;
              foreach( (int, int) val in kvp.Value )
              {
                Debug.Log("val: " + val);   
                if (counterval == valuenum)
                {
                  Debug.Log("VALUE FOUND");
                  chosenmove = val;
                  Debug.Log("chosenmove: " + chosenmove);
                  break;
                }
                else
                {
                  counterval++;  
                } 
              } 
              break;
            }
          }

        // Debug.Log("CHOSENKEY: " + chosenkey + " CHOSENMOVE: " + chosenmove);  

        // when the game changes turn
        // isWhiteTurn = !isWhiteTurn;
        // isWhite = !isWhite;
        // hasKilled = false;
        // CheckVictory();

        //AI moves
        TryMove(chosenkey.Item1, chosenkey.Item2, chosenmove.Item1, chosenmove.Item2);        
      }
      
    }


    private void handle_coordinates((int,int) piecetuple, Dictionary<(int, int), List<(int, int)>> moves, List<(int, int)> destinations)
    {
      if (moves.ContainsKey(piecetuple))
      {
        moves[piecetuple] = destinations;
      }
      else
      {
        moves.Add(piecetuple, destinations);
      }
    }


    //Generates list of potential moves for each piece
    private Dictionary<(int, int), List<(int, int)>> generate_coordinates(List<(int,int)> teampieces)
    {
      Dictionary<(int, int), List<(int, int)>> moves = new Dictionary<(int, int), List<(int, int)>>();
      foreach((int,int) piecetuple in teampieces)
      {
        List<(int,int)> destinations = new List<(int,int)>();
        int x = piecetuple.Item1;
        int y = piecetuple.Item2;
        Piece p = pieces[x, y];
        
        // BLACK team is the AI player, only store the this team in the Dictionary for now
        if (p.isWhite == false) 
          {                     
            for (int col=0; col<8; col++)
              {
                for (int row=0; row<8; row++)
                  {
                    int x2 = col;
                    int y2 = row;
                    if (p.ValidMove(pieces, x, y, x2, y2))
                      {
                      (int, int) dest = (x2, y2);
                      destinations.Add(dest);
                    }
                  }
              }

            if (destinations.Count>0)
            {
              handle_coordinates(piecetuple, moves, destinations);
            }
          }
      }
      return moves;
    }

    public List<(int,int)> get_team_pieces(Piece[,] board, bool isWhite)
    {
      List<(int,int)> teampieces = new List<(int,int)>();
      for (int col=0; col<8; col++)
      {
        for (int row=0; row<8; row++)
        {
          Piece p = board[col, row];
          //Checks if piece exists and if the piece is part of the moving team
          if (p!=null && p.isWhite==isWhite)
            {
              //Adding tuple containing coordinates for the piece
              (int, int) newpiece = (col, row);
              teampieces.Add(newpiece);
            }
        }
      }
      return teampieces;
    }

    public void print_team(Dictionary<(int, int), List<(int, int)>> moves)
    {
      Debug.Log("New Team");
      foreach (KeyValuePair<(int, int), List<(int, int)>> kvp in moves){
        //  Debug.Log("Key = {0} " + kvp.Key);
         foreach((int, int) val in kvp.Value) {
           Debug.Log("Key " + kvp.Key + ", Value = " + val);
         }
       }
    }

    private void Start()
    {
        WhiteWinTextObject.SetActive(false);
        BlackWinTextObject.SetActive(false);
        isWhiteTurn = true;
        isWhite = true;
        GenerateBoard();
        // List<(int,int)> teampieces = get_team_pieces(pieces, isWhite);
        // print_team(teampieces);
        //addtoDictionary();
        //get_piece(0,0);
        //get_all_pieces(!isWhite);
        forcePieces = new List<Piece>();
    }

    private void Update()
    {
        UpdateMouseOver();
        //if is my turn
        IsMyTurn();
        //addtoDictionary();
    }

    private void IsMyTurn()
    {
        // if ((isWhite) ? isWhiteTurn : !isWhiteTurn)
        
        // Human Moves (white team)
        if (isWhite == true)
        {
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            if (Input.GetMouseButtonDown(0))
            {
                SelectPiece(x, y);
            }

            if (Input.GetMouseButtonUp(0))
            {
                TryMove((int)starDrag.x, (int)starDrag.y, x, y);
            }

            if (selectedPiece != null)
            {
                UpdatePieceDrag(selectedPiece);
            }
        }
        // AI Moves (black team)
        else
        {
          List<(int,int)> teampieces = get_team_pieces(pieces, isWhite);
          Dictionary<(int, int), List<(int, int)>> moves = generate_coordinates(teampieces);
          chooseRandom_PieceandMove(moves);
        }
    }

    private void SelectPiece(int x, int y)
    {

        // List<(int,int)> teampieces = get_team_pieces(pieces, isWhite);
        // Dictionary<(int, int), List<(int, int)>> moves = generate_coordinates(teampieces);
        // chooseRandom_PieceandMove(moves);
        // print_team(moves);

        //check if out of bounds
        if (x < 0 || x >= 8 || y < 0 || y >= 8)
        {
            return;
        }

        Piece p = pieces[x, y];
        // Debug.Log("current position ");
        // Debug.Log(p);
        // Dictionary<(int, int), List<(int, int)>> moves = get_valid_moves(p, x, y);
        // foreach (KeyValuePair<(int, int), List<(int, int)>> kvp in moves){
        //    Debug.Log("Key = {0}" + kvp.Key);
        //    foreach((int, int) val in kvp.Value) {
        //      Debug.Log("printing value" + val);
        //    }
        //  }

        if (p != null && p.isWhite == isWhite)
        {
            if (forcePieces.Count == 0)
            {
                selectedPiece = p;
                starDrag = mouseOver;
            }
            else
            {
                if (forcePieces.Find(fp => fp == p) == null)
                {
                    return;
                }
                selectedPiece = p;
                starDrag = mouseOver;
            }
        }
    }

    private void TryMove(int x1, int y1, int x2, int y2)
    {
      //  forcePieces = ScanForPossibleMove();

        starDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];

        //if out of bounds or has not move
        if (x2 < 0 || x2 >= pieces.Length || y2 < 0 || y2 >= pieces.Length || endDrag == starDrag)
        {
            if (selectedPiece != null)
                MovePiece(selectedPiece, x1, y1);

            starDrag = Vector2.zero;
            selectedPiece = null;
            return;
        }

        if (selectedPiece != null)
        {
            //check valid move
            if (selectedPiece.ValidMove(pieces, x1, y1, x2, y2))
            {
                if (Mathf.Abs(x2 - x1) == 2)
                {
                    Piece p = pieces[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null)
                    {
                        pieces[(x1 + x2) / 2, (y1 + y2) / 2] = null;
                        Destroy(p.gameObject);
                        if (p.isWhite == true)
                        {
                          whitePieceCount--;
                        }
                        else
                        {
                          blackPieceCount--;
                        }
                        hasKilled = true;
                    }
                }

                if (forcePieces.Count != 0 && !hasKilled)
                {
                    ReturnFirstPosition(x1, y1);
                }

                pieces[x2, y2] = selectedPiece;
                pieces[x1, y1] = null;
                MovePiece(selectedPiece, x2, y2);
                EndTurn();
            }
            else
            {
                ReturnFirstPosition(x1, y1);
            }
        }
    }

    private void ReturnFirstPosition(int x, int y)
    {
        MovePiece(selectedPiece, x, y);
        starDrag = Vector2.zero;
        selectedPiece = null;
        return;
    }

    private void EndTurn()
    {
        bool noValidMove = false;
        int x = (int)endDrag.x;
        int y = (int)endDrag.y;

        if (selectedPiece != null)
        {
            if (selectedPiece.isWhite && !selectedPiece.isKing && y == 7)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.Rotate(Vector3.right * 180);
            }
            else if (!selectedPiece.isWhite && !selectedPiece.isKing && y == 0)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.Rotate(Vector3.right * 180);
            }
        }

        selectedPiece = null;
        starDrag = Vector2.zero;


        // when the game changes turn
        isWhiteTurn = !isWhiteTurn;
        isWhite = !isWhite;

        hasKilled = false;
        CheckVictory(noValidMove);
    }

    // private void CheckVictory()
    // {
    //     var ps = FindObjectsOfType<Piece>();
    //     bool hasWhite = false, hasBlack = false;

    //     for (int i = 0; i < ps.Length; i++)
    //     {
    //         if (ps[i].isWhite)
    //             hasWhite = true;
    //         else
    //             hasBlack = true;
    //     }

    //     if (!hasWhite)
    //         Victory(false);

    //     if (!hasBlack)
    //         Victory(true);
    // }

    private void CheckVictory(bool noValidMove)
    {
      if (whitePieceCount == 0)
      {
        Debug.Log("Black team won");
        BlackWinTextObject.SetActive(true);        
        gameOver = true;
      }
      else if (blackPieceCount == 0)
      {
        Debug.Log("White team won");
        WhiteWinTextObject.SetActive(true);
        gameOver = true;

      }
      else if ( (whitePieceCount > blackPieceCount) && noValidMove)
      {
        Debug.Log("White team won");
        WhiteWinTextObject.SetActive(true);
        gameOver = true;
      }

    }

    private void Victory(bool iswhite)
    {
        if (iswhite)
            Debug.Log("White team win");
        else
            Debug.Log("Black team win");
    }

    private void UpdateMouseOver()
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int)(hit.point.x - boardOffset.x);
            mouseOver.y = (int)(hit.point.z - boardOffset.z);
        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }

    private void UpdatePieceDrag(Piece p)
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            p.transform.position = hit.point + Vector3.up;
        }
    }

    private void GenerateBoard()
    {
        //addtoDictionary();
        //white pieces
        for (int y = 0; y < 3; y++)
        {
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece((oddRow) ? x : x + 1, y);
            }
        }

        //black pieces
        for (int y = 7; y > 4; y--)
        {
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece((oddRow) ? x : x + 1, y);
            }
        }
    }

    private void GeneratePiece(int x, int y)
    {
        bool isPieceWhite = (y > 3) ? false : true;

        GameObject go = Instantiate((isPieceWhite) ? whitePiecePrefab : blackPiecePrefab) as GameObject;
        go.transform.SetParent(transform);
        Piece p = go.GetComponent<Piece>();
        pieces[x, y] = p;
        MovePiece(p, x, y);
    }

    private void MovePiece(Piece p, int x, int y)
    {
        p.transform.position = (Vector3.right * x) + (Vector3.forward * y) + boardOffset + pieceOffset;
    }
}

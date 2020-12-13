﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public GameObject HvHObjectButtonObject;
    public GameObject EasyButtionObject;
    public GameObject MediumButtionObject;

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

      for (int row = 0; row < 8; row++)
      {
          for (int col = 0; col < 8; col++)
          {
            Piece p = sourceBoard[col,row];
            result[col, row] = p;

            // copies both colors and king status
            if (p != null)
            {
              result[col, row].isWhite = p.isWhite;
              result[col, row].isKing = p.isKing;
            }
          }
      }
      return result;
    }

    // just for the AI
    private List<Piece[,]> get_boardstates(Piece[,] board, bool isWhite)
    {
      Debug.Log("XXXXXXX");
      List<Piece[,]> board_states = new List<Piece[,]>();
      Piece[,] temp_board = deepcopy(board);
      Piece[,] temp_boardChild = new Piece[8,8];

      // get all pieces and valid moves (and put them in Dictionary) for the tempboard
      List<(int,int)> blackteam_piecestemp = get_team_pieces(temp_board, false);
      Dictionary<(int, int), List<(int, int)>> tempboardmoves = generate_coordinates(blackteam_piecestemp);

      // create new board states for ALL the possible moves by looping
      foreach (KeyValuePair<(int, int), List<(int, int)>> kvp in tempboardmoves)
      {
        Debug.Log("Oooooo");
        foreach( (int, int) val in kvp.Value )
          {
            temp_boardChild = deepcopy(temp_board);
            Debug.Log("THE MOVE: " + kvp.Key.Item1 + " " + kvp.Key.Item2 + " " + val.Item1 + " " + val.Item2);
            temp_boardChild = simulateTryMove(kvp.Key.Item1, kvp.Key.Item2, val.Item1, val.Item2, temp_boardChild);
            break;
          }
        break;
      }

      List<(int,int)> coords = new List<(int,int)>();
      List<(int,int)> childcoords = new List<(int,int)>();

      for (int row = 0; row < 8; row++)
      {
        for (int col = 0; col < 8; col++)
        {
          Piece p = temp_board[col, row];
          Piece ptemp = temp_boardChild[col, row];
          if (p != null)
          {
            coords.Add((col,row));
          }
          if (p != null)
          {
            childcoords.Add((col,row));
          }
        }
      }

      Debug.Log("coords");
      foreach ( (int,int) x in coords)
      {
        Debug.Log(x);
      }

      Debug.Log("childcoords");
      foreach ( (int,int) x in childcoords)
      {
        Debug.Log(x);
      }


      // loop through all the pieces and their moves
      // choose a piece and its move
      // make that move on the temp_board
      // check to see if new positions show on the temp_board
      // add temp_board to board_states

      // keep track of pieces on the Other team as well
      // when a piece is killed, keep a COUNTER,
      // and mark said piece somehow to skip over it next time in future states

      return board_states;

    }

    private void checkCopying(Piece[,] board)
    {
      Piece[,] temp_board = deepcopy(board);

      List<(int,int)> coords = new List<(int,int)>();
      List<(int,int)> coordstemp = new List<(int,int)>();
      List<(bool,bool)> checkWhK = new List<(bool,bool)>();
      List<(bool,bool)> checkWhKtemp = new List<(bool,bool)>();

      for (int row = 0; row < 8; row++)
      {
        for (int col = 0; col < 8; col++)
        {
          Piece p = board[col, row];
          Piece ptemp = temp_board[col, row];
          if (p != null)
          {
            coords.Add((col,row));
          }
          if (p != null)
          {
            coordstemp.Add((col,row));
          }
        }
      }

      for (int row = 0; row < 8; row++)
      {
        for (int col = 0; col < 8; col++)
        {
          Piece p = board[col, row];
          Piece ptemp = temp_board[col, row];
          if (p != null)
          {
            coords.Add((col,row));
          }
          if (ptemp != null)
          {
            coordstemp.Add((col,row));
          }
        }
      }

      Debug.Log("coords");
      foreach ( (int,int) x in coords)
      {
        Debug.Log(x);
      }

      Debug.Log("coordstemp");
      foreach ( (int,int) x in coordstemp)
      {
        Debug.Log(x);
      }


      for (int row = 0; row < 8; row++)
        {
          for (int col = 0; col < 8; col++)
          {
            Piece p = board[col, row];
            Piece ptemp = temp_board[col, row];
            if (p != null)
            {
              checkWhK.Add( (board[col, row].isWhite, board[col, row].isKing) );
            }
            if (ptemp != null)
            {
              checkWhK.Add( (temp_board[col, row].isWhite, temp_board[col, row].isKing) );
            }
          }
        }

      Debug.Log("bools");
      foreach ( (bool,bool) x in checkWhK)
      {
        Debug.Log(x);
      }

      Debug.Log("boolstemp");
      foreach ( (bool,bool) x in checkWhK)
      {
        Debug.Log(x);
      }

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

    public int level;
    public bool humans;

    public void Easy()
    {
      level=1;
      EasyButtionObject.SetActive(false);
      MediumButtionObject.SetActive(false);
      HvHObjectButtonObject.SetActive(false);
    }

    public void Medium()
    {
      level=2;
      EasyButtionObject.SetActive(false);
      MediumButtionObject.SetActive(false);
      HvHObjectButtonObject.SetActive(false);
    }

    public void Humans()
    {
      humans = true;
      EasyButtionObject.SetActive(false);
      MediumButtionObject.SetActive(false);
      HvHObjectButtonObject.SetActive(false);
    }

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
        // Debug.Log("keynum " + keynum);

        (int,int) chosenkey;
        (int,int) chosenmove = (0,0);

        chosenkey = moves.ElementAt(keynum).Key;

        foreach (KeyValuePair<(int, int), List<(int, int)>> kvp in moves)
          {
            // Debug.Log("key: " + kvp.Key);
            if (kvp.Key == chosenkey)
            {
              // Debug.Log("KEY FOUND");
              // Debug.Log("chosenkey: " + chosenkey);

              int valuenum = Random.Range(0, kvp.Value.Count);
              // Debug.Log("valuenum " + valuenum);

              int counterval = 0;
              foreach( (int, int) val in kvp.Value )
              {
                // Debug.Log("val: " + val);
                if (counterval == valuenum)
                {
                  // Debug.Log("VALUE FOUND");
                  chosenmove = val;
                  // Debug.Log("chosenmove: " + chosenmove);
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

      // update a list of moves to each piece's current position (piecetuple)
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
        Dictionary<(int, int), List<(int, int)>> kill_moves = new Dictionary<(int, int), List<(int, int)>>();

        foreach((int,int) piecetuple in teampieces)
        {
          List<(int,int)> destinations = new List<(int,int)>();
          List<(int,int)> kill_destinations = new List<(int,int)>();
          int x = piecetuple.Item1;
          int y = piecetuple.Item2;
          Piece p = pieces[x, y];

          // BLACK team is the AI player, only store the this team's moves in the Dictionary for now
          if (p.isWhite == false)
            {
              for (int col=0; col<8; col++)
                {
                  for (int row=0; row<8; row++)
                    {
                      int x2 = col;
                      int y2 = row;
                      // Move is Valid
                      if (p.ValidMove(pieces, x, y, x2, y2).Item1)
                        {
                        (int, int) dest = (x2, y2);
                        destinations.Add(dest);
                      }

                      // Move is a Kill Move
                      if (p.ValidMove(pieces, x, y, x2, y2).Item2)
                      {
                        (int, int) dest = (x2, y2);
                        kill_destinations.Add(dest);
                      }

                    }
                }

              // if there are Kill Moves with the possibility of multiple Kills
              if (kill_destinations.Count>0)
              {
                handle_coordinates(piecetuple, kill_moves, kill_destinations);
              }


              // if the list is NOT EMPTY - the piece has valid moves
              if (destinations.Count>0)
              {
                handle_coordinates(piecetuple, moves, destinations);
              }
            }
        }

        if (level==2)
        {
          if (kill_moves.Count > 0)
            {
              return kill_moves;
            }
          else
            {
              return moves;
            }
        }
        else
        {
          return moves;
        }

      }

// private void chooseRandom_PieceandMove1( Dictionary<(int, int), List<(List<(int,int)>,bool)>>  moves )
//     {

//       bool noValidMove = true;

//       foreach (KeyValuePair<(int, int), List<(List<(int,int)>,bool)>> kvp in moves)
//       {
//         // check size of the list of moves for each piece/key
//         if (kvp.Value.Count>0)
//         {
//           noValidMove = false;
//         }
//       }

//       // there are pieces left BUT they can't move because there are no valid moves
//       if (noValidMove)
//       {
//         CheckVictory(noValidMove);
//       }
//       else
//       {
//         // generate random number in range moves.Count to choose a random key
//         // choose a random move from the Key
//         int keynum =  Random.Range(0, moves.Count+1);
//         // Debug.Log("keynum " + keynum);

//         (int,int) chosenkey;
//         (int,int) chosenmove = (0,0);

//         chosenkey = moves.ElementAt(keynum).Key;

//         foreach (KeyValuePair<(int, int), List<(List<(int,int)>,bool)>> kvp in moves)
//           {
//             // Debug.Log("key: " + kvp.Key);
//             if (kvp.Key == chosenkey)
//             {
//               // Debug.Log("KEY FOUND");
//               // Debug.Log("chosenkey: " + chosenkey);

//               int valuenum = Random.Range(0, kvp.Value.Count);
//               // Debug.Log("valuenum " + valuenum);

//               int counterval = 0;
//               foreach( (List<(int,int)>,bool) val in kvp.Value )
//               {
//                 // Debug.Log("val: " + val);
//                 if (counterval == valuenum)
//                 {
//                   // Debug.Log("VALUE FOUND");
//                   chosenmove = val;
//                   // Debug.Log("chosenmove: " + chosenmove);
//                   break;
//                 }
//                 else
//                 {
//                   counterval++;
//                 }
//               }
//               break;
//             }
//           }

//           // Debug.Log("CHOSENKEY: " + chosenkey + " CHOSENMOVE: " + chosenmove);

//           // when the game changes turn
//           // isWhiteTurn = !isWhiteTurn;
//           // isWhite = !isWhite;
//           // hasKilled = false;
//           // CheckVictory();

//           //AI moves
//           TryMove(chosenkey.Item1, chosenkey.Item2, chosenmove.Item1, chosenmove.Item2);
//         }

//       }


//       // update a list of moves to each piece's current position (piecetuple)
//       private void handle_coordinates1((int,int) piecetuple, Dictionary<(int, int), List<(List<(int,int)>,bool)>> moves, List<(List<(int,int)>,bool)> outer_list)
//       {
//         if (moves.ContainsKey(piecetuple))
//         {
//           moves[piecetuple] = outer_list;
//         }
//         else
//         {
//           moves.Add(piecetuple, outer_list);
//         }
//       }


      //Generates list of potential moves for each piece
      // private Dictionary<(int, int), List<(List<(int,int)>,bool)>> generate_coordinates1(List<(int,int)> teampieces)
      // {
      //   Dictionary< (int, int), List< (List<(int,int)>,bool) > > moves = new Dictionary< (int, int), List<(List<(int,int)>,bool)> >();
      //   Dictionary< (int, int), List< (List<(int,int)>,bool) > > killmoves = new Dictionary< (int, int), List<(List<(int,int)>,bool)> >();

      //   foreach((int,int) piecetuple in teampieces)
      //   {
      //     List<(List<(int,int)>,bool)> outer_list = new List<(List<(int,int)>,bool)>();
      //     List<(int,int)> destinations = new List<(int,int)>();
      //     List<(int,int)> future_destinations = new List<(int,int)>();
      //     int x = piecetuple.Item1;
      //     int y = piecetuple.Item2;
      //     Piece p = pieces[x, y];

      //     // BLACK team is the AI player, only store the this team's moves in the Dictionary for now
      //     if (p.isWhite == false)
      //       {
      //         for (int col=0; col<8; col++)
      //           {
      //             for (int row=0; row<8; row++)
      //             {
      //                 int x2 = col;
      //                 int y2 = row;

      //                 // Move is Valid
      //                 if (p.ValidMove(pieces, x, y, x2, y2).Item1)
      //                   {
      //                     (int, int) dest = (x2, y2);
      //                     destinations.Add(dest);
      //                     outer_list.Add( destinations, p.ValidMove(pieces, x, y, x2, y2).Item2 );
      //                   }

      //             }
      //           }

      //         // if there are Kill Moves with the possibility of multiple Kills
      //         if (future_destinations.Count>0)
      //         {

      //         }


      //         // // if there are Kill Moves with the possibility of multiple Kills
      //         // if (future_destinations.Count>0)
      //         // {
      //         //   handle_coordinates(piecetuple, kill_moves, future_destinations);
      //         // }


      //         // if the list is NOT EMPTY - the piece has valid moves
      //         if (outer_list.Count>0)
      //         {
      //           handle_coordinates1(piecetuple, moves, outer_list);
      //         }
      //       }

      //   }
      //   return moves;
      // }




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
        HvHObjectButtonObject.SetActive(true);
        EasyButtionObject.SetActive(true);
        MediumButtionObject.SetActive(true);
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
        // get_boardstates(pieces, true);
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
        bool condition;

        // Two-Player mode
        if (humans)
        {
          condition = ((isWhite) ? isWhiteTurn : !isWhiteTurn);
        }

        // Play against Black AI
        else
        {
          condition = (isWhite == true);
        }
        // Two Human Players state
        // if ((isWhite) ? isWhiteTurn : !isWhiteTurn)

        // Human Moves (white team)
        if (condition)
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
          // Medium_Level(moves, kill_moves);
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

    private Piece[,] simulateTryMove(int x1, int y1, int x2, int y2, Piece[,] board)
    {

      // create new board_state for the new move
        Piece p_old = board[x1,y1];
        board[x2,y2] = p_old;
        board[x2,y2].isWhite = p_old.isWhite;
        board[x2,y2].isKing = p_old.isKing;

        Destroy(board[x1,y1]);

        return board;


      //   selectedPiece = board[x1, y1];

      //   if (selectedPiece != null)
      //   {
      //       //check valid move
      //       if (selectedPiece.ValidMove(board, x1, y1, x2, y2))
      //       {
      //           if (Mathf.Abs(x2 - x1) == 2)
      //           {
      //               Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
      //               if (p != null)
      //               {
      //                   board[(x1 + x2) / 2, (y1 + y2) / 2] = null;
      //                   Destroy(p.gameObject);
      //                   if (p.isWhite == true)
      //                   {
      //                     whitePieceCount--;
      //                   }
      //                   else
      //                   {
      //                     blackPieceCount--;
      //                   }
      //                   hasKilled = true;
      //               }
      //           }

      //           // REMOVE
      //           // if (forcePieces.Count != 0 && !hasKilled)
      //           // {
      //           //     ReturnFirstPosition(x1, y1);
      //           // }

      //           board[x2, y2] = selectedPiece;
      //           board[x1, y1] = null;
      //           MovePiece(selectedPiece, x2, y2);
      //           EndTurn();
      //       }
      //       else
      //       {
      //           ReturnFirstPosition(x1, y1);
      //       }
      //   }
      // return board;
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
            if (selectedPiece.ValidMove(pieces, x1, y1, x2, y2).Item1)
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
      // if (whitePieceCount == 1)
      // {
      //   BlackWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 2)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 3)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 4)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 5)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 6)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 7)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 8)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 9)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 10)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 11)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 12)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }

      // if (whitePieceCount == 1)
      // {
      //   BlackWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 2)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 3)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 4)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 5)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 6)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 7)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 8)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 9)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 10)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 11)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }
      // else if (whitePieceCount == 12)
      // {
      //   WhiteWinTextObject.SetActive(true);
      // }

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

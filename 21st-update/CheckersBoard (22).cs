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
  //  List<Piece> skipped = new List<Piece>();

    private List<Piece> get_all_pieces(bool isWhite) {
      allpieces = new List<Piece>();

      for (int row = 0; row < 8; row++) {
        for (int col = 0; col < 8; col++) {
          Piece p = pieces[col, row];
          if (p != null && p.isWhite == isWhite) {
            allpieces.Add(p);
            //Destroy(p.gameObject);
          }
        }
      }
      return allpieces;
    }

    private Piece get_piece(int col, int row) {
      // Piece p = pieces[col, row];
      // Destroy(p.gameObject);
      return pieces[col,row];
    }
    //When called put pieces[col,row]

    // private void addtoDictionary()
    // {
    //   int col = 2;
    //   int row = 2;
    //   Piece p = pieces[2, 2];
    //   //p.isWhite = true;
    //   Dictionary<(int, int), List<(int, int)>> moves = get_valid_moves(p, col, row);
    // }

    private Dictionary<(int, int), List<(int, int)>> get_valid_moves(Piece p, int col, int row)
    {
      Dictionary<(int, int), List<(int, int)>> moves = new Dictionary<(int, int), List<(int, int)>>();
      int left = col-1;
      int right = col+1;
      int localrow = row;
      Debug.Log("x: " + col);
      Debug.Log("y: " + row);
      if ((p.isWhite==isWhite) || (p.isKing)) {
        Debug.Log("iswhite");
        Dictionary<(int, int), List<(int, int)>> traversemove = traverse_left(localrow-1, System.Math.Max(localrow-3, -1), -1, p.isWhite, left, skipped);
        AddOrUpdate(moves, traversemove);
        traversemove = traverse_right(localrow-1, System.Math.Max(localrow-3, -1), -1, p.isWhite, right, skipped);
        AddOrUpdate(moves, traversemove);
      }
      if ((p.isWhite!=isWhite) || (p.isKing)) {
        Debug.Log("isblack");
        Dictionary<(int, int), List<(int, int)>> traversemove = traverse_left(localrow+1, System.Math.Min(localrow+3, 8), 1, p.isWhite, left, skipped);
        AddOrUpdate(moves, traversemove);
        traversemove = traverse_right(localrow+1, System.Math.Min(localrow+3, 8), 1, p.isWhite, right, skipped);
        AddOrUpdate(moves, traversemove);
      }

      return moves;
    }

    private void AddOrUpdate(Dictionary<(int, int), List<(int, int)>> mainmove, Dictionary<(int, int), List<(int, int)>> traversemove)
    {
      //List<string> keyList = new List<string>(this.traversemove.Keys);
      foreach (KeyValuePair<(int, int), List<(int, int)>> item in traversemove)
      // for (int x=0; x<traversemove.Count; x++)
      {
        (int, int) key = item.Key;
        //Debug.Log("printing key: " + key);
        List<(int, int)> value = item.Value;
        // foreach((int, int) x in value) {
        //   Debug.Log("printing value" + x);
        // }
        if (mainmove.ContainsKey(key))
        {
          mainmove[key] = value;
        }
        else
        {
          mainmove.Add(key, value);
        }
      }

    }

    private Dictionary<(int, int), List<(int, int)>> traverse_left(int start, int stop, int step, bool isWhite, int left, List<(int, int)> skipped)
    {
      Dictionary<(int, int), List<(int, int)>> moves = new Dictionary<(int, int), List<(int, int)>>();
      List<(int, int)> last = new List<(int, int)>();
      skipped = new List<(int, int)>();
      for (int row = start; row < stop; row+=step) {
        if (left < 0) {
          break;
        }
        Piece currentp = pieces[row,left];
        (int, int) current = (row, left);
        if (currentp == null) {
          if (skipped.Count>0 && last.Count==0) {
            break;
          }
          else if (skipped.Count>0) {
            (int, int) newtuple = (row, left);
            last = last.Concat(skipped).ToList();
            moves.Add(newtuple, last);
          }
          else {
            (int, int) newtuple = (row, left);
            moves.Add(newtuple, last);
          }

          if (last.Count>0) {
            int r;
            if (step == -1) {
              r = System.Math.Max(row-3, 0);
            }
            else {
              r = System.Math.Min(row+3, 8);
            }
            Dictionary<(int, int), List<(int, int)>> traversemove = traverse_left(row+step, r, step, isWhite, left-1, skipped=last);
            AddOrUpdate(moves, traversemove);
            traversemove = traverse_right(row+step, r, step, isWhite, left+1, skipped=last);
            AddOrUpdate(moves, traversemove);
          }
          break;

        }
        else if (currentp.isWhite == isWhite) {
          break;
        }
        else {
          last = new List<(int, int)>();
          last.Add(current);
        }
        left--;
      }
      return moves;
    }

    private Dictionary<(int, int), List<(int, int)>> traverse_right(int start, int stop, int step, bool isWhite, int right, List<(int, int)> skipped)
    {
      Dictionary<(int, int), List<(int, int)>> moves = new Dictionary<(int, int), List<(int, int)>>();
      List<(int, int)> last = new List<(int, int)>();
      skipped = new List<(int, int)>();
      for (int row = start; row < stop; row+=step) {
        if (right >= 8) {
          break;
        }
        Piece currentp = pieces[row, right];
        (int, int) current = (row, right);
        if (currentp == null) {
          if (skipped.Count>0 && last.Count==0) {
            break;
          }
          else if (skipped.Count>0) {
            (int, int) newtuple = (row, right);
            last = last.Concat(skipped).ToList();
            moves.Add(newtuple, last);
          }
          else {
            (int, int) newtuple = (row, right);
            moves.Add(newtuple, last);
          }

          if (last.Count>0) {
            int r;
            if (step == -1) {
              r = System.Math.Max(row-3, 0);
            }
            else {
              r = System.Math.Min(row+3, 8);
            }
            Dictionary<(int, int), List<(int, int)>> traversemove = traverse_left(row+step, r, step, isWhite, right-1, skipped=last);
            AddOrUpdate(moves, traversemove);
            traversemove = traverse_right(row+step, r, step, isWhite, right+1, skipped=last);
            AddOrUpdate(moves, traversemove);
          }
          break;

        }
        else if (currentp.isWhite == isWhite) {
          break;
        }
        else {
          last = new List<(int, int)>();
          last.Add(current);
        }
        right++;
      }
      return moves;
    }


    private void Start()
    {
        isWhiteTurn = true;
        isWhite = true;
        GenerateBoard();
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
        if ((isWhite) ? isWhiteTurn : !isWhiteTurn)
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
    }

    private void SelectPiece(int x, int y)
    {
        //check if out of bounds
        if (x < 0 || x >= 8 || y < 0 || y >= 8)
        {
            return;
        }

        Piece p = pieces[x, y];
        Debug.Log("current position ");
        Debug.Log(p);
        Dictionary<(int, int), List<(int, int)>> moves = get_valid_moves(p, x, y);
        foreach (KeyValuePair<(int, int), List<(int, int)>> kvp in moves){
           Debug.Log("Key = {0}" + kvp.Key);
           foreach((int, int) val in kvp.Value) {
             Debug.Log("printing value" + val);
           }
         }
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

      //  if (ScanForPossibleMove(selectedPiece, x, y).Count != 0 && hasKilled)
      //      return;

        isWhiteTurn = !isWhiteTurn;
        isWhite = !isWhite;
        hasKilled = false;
        CheckVictory();
    }

    private void CheckVictory()
    {
        var ps = FindObjectsOfType<Piece>();
        bool hasWhite = false, hasBlack = false;

        for (int i = 0; i < ps.Length; i++)
        {
            if (ps[i].isWhite)
                hasWhite = true;
            else
                hasBlack = true;
        }

        if (!hasWhite)
            Victory(false);

        if (!hasBlack)
            Victory(true);

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

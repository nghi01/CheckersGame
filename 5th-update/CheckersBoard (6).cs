using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersBoard : MonoBehaviour
{
  public float Height = 25.0f;
  public GameObject whitePiecePrefab;
  public GameObject blackPiecePrefab;
  public Piece[,] pieces = new Piece[8,8];
  private Vector3 boardvectors = new Vector3(-3.5f,0.15f,-3.5f);
  private Vector3 fixing = new Vector3(4,0,4);

  private Piece selectedPiece;
  
  private Vector2 mouseOver;
  private Vector2 startDrag;
  private Vector2 endDrag;

private void GenerateBoard(){
  /*Whiteteam*/
  for (int y = 0; y < 3; y = y+2){
    for (int x = 0; x <8; x = x +2){
      GeneratePiece1(x,y);
    }
  }
  int b = 1;
  for (int a =1; a<8;a=a+2){
    GeneratePiece1(a,b);
  }

  for (int y=7;y>4;y=y-2){
    for (int x=1; x<8;x = x+2){
      GeneratePiece2(x,y);
    }
  }
  b = 6;
  for (int a = 0;a<8;a=a+2){
    GeneratePiece2(a,b);
  }
}

private void GeneratePiece1(int x,int y){
  GameObject gp = Instantiate(whitePiecePrefab) as GameObject;
  gp.transform.SetParent(transform);
  Piece p = gp.GetComponent<Piece>();
  pieces[x,y]= p;
  MovePiece(p,x,y);
}

private void GeneratePiece2(int x,int y){
  GameObject gp = Instantiate(blackPiecePrefab) as GameObject;
  gp.transform.SetParent(transform);
  Piece p = gp.GetComponent<Piece>();
  pieces[x,y]= p;
  MovePiece(p,x,y);
}
private void MovePiece(Piece p, int x, int y){
  p.transform.position = (Vector3.right * x)+ (Vector3.forward * y) + boardvectors;
}

private void UpdateMouseOver(){
  RaycastHit hit;
  if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit,Height,LayerMask.GetMask("Board"))){
    mouseOver.x = (int)(hit.point.x+fixing.x);
    mouseOver.y = (int)(hit.point.z+fixing.z);
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
  if(x<0 || x >= pieces.Length || y<0 || y>=pieces.Length)
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

private void TryMove(int x1, int y1, int x2, int y2)
{
  //Multiplayer Support
  startDrag = new Vector2(x1, y1);
  endDrag = new Vector2(x2, y2);
  selectedPiece = pieces[x1, y1];

  // end position is out of bounds
  if (x2<0 || x2 >= pieces.Length || y2<0 || y2 >= pieces.Length)
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

  }

  }

  // MovePiece(selectedPiece, x2, y2);
}

    // Start is called before the first frame update
    void Start()
    {
      GenerateBoard();
    }

    // Update is called once per frame
    void Update()
    {
      UpdateMouseOver();

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
      }
    }
}

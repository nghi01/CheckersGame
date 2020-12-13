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
  private Vector3 fixing = new Vector3(3,0,3);

  private Vector2 mouseOver;


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
    mouseOver.x = (int)hit.point.x+fixing.x;
    mouseOver.y = (int)hit.point.z+fixing.z;
  }
  else
  {
    mouseOver.x = -1;
    mouseOver.y = -1;
  }
  Debug.Log(mouseOver);

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



    }
}

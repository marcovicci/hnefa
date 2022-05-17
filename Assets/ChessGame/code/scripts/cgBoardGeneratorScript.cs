using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

////[ExecuteInEditMode]
public class cgBoardGeneratorScript : MonoBehaviour
{
    /// <summary>
    /// The prefab used when generating white squares.
    /// </summary>
    public GameObject whiteSquarePrefab;
    /// <summary>
    /// The prefab used when generating black squares.
    /// </summary>
    public GameObject blackSquarePrefab;
    /// <summary>
    /// The prefab used when generating pieces.
    /// </summary>
    public GameObject piecePrefab;
    /// <summary>
    /// Determines the spacing of generated squares.
    /// </summary>
    public Vector2 squareSpacing = new Vector2(.6f, .6f);
    /// <summary>
    /// Determines the scale of squares.
    /// </summary>
    public Vector2 squareScale = new Vector2(1f, 1f);
    /// <summary>git sta
    /// The cgChessBoardScript.
    /// </summary>
    public cgChessBoardScript boardScript;
    /// <summary>
    /// The desired width of the generated board.
    /// </summary>
    public byte boardWidth = 6;
    /// <summary>
    /// The desired height of the generated board.
    /// </summary>
    public byte boardHeight = 7;
    /// <summary>
    /// Should it be displayed as 3d?
    /// </summary>
    public bool use3d = false;
    /// <summary>
    /// The top left starting point of the board, where the first square is placed, dictating the placement of all following squares.
    /// </summary>
    public Vector3 squareStartPoint = new Vector3(-2.2f, 2.2f, 0);
    /// <summary>
    /// Do you want to customize placements(true)? Or cram the default positions onto your generated board(Note: if width is less that 5 no king will be placed).
    /// </summary>
    public bool customizePlacements = false;
    /// <summary>
    /// The placement of pieces on the generated board.
    /// </summary>
    public Type[] piecePlacements;

    /// <summary>
    /// All possible chess types.
    /// </summary>
    public enum Type
    {
        WhitePawn = 1,
        WhiteRook = 2,
        WhiteKnight = 3,
        WhiteBishop = 4,
        WhiteQueen = 5,
        WhiteKing = 6,
        Empty = 0,
        BlackPawn = -1,
        BlackRook = -2,
        BlackKnight = -3,
        BlackBishop = -4,
        BlackQueen = -5,
        BlackKing = -6

    }

    //Piece placements should define where pieces on the custom board should be placed, you can see an example for a 6 x 7 board below.
    private List<sbyte> _piecePlacements = null;
    private cgBoard _board;

    // Use this for initialization
    void Start()
    {


    }
    public void generate(List<sbyte> customPlacements = null)
    {
        //Destroy existing squares before we generate the new squares to fit the new board width and height.
        _destroyChildren(boardScript.chessPieceHolder);
        _destroyChildren(boardScript.chessSquareHolder);

        // Debug.Log("boardscript "+boardScript.chessSquareHolder);

        ////Add placemetns for the tiny board as is available from cgMenuScript
        //cgCustomBoardSettings.AddPiecePlacement(6, 7, new List<sbyte>{
        //            -2,-3,-5,-6,-3,-2,
        //            -1,-1,-1,-1,-1,-1,
        //             0, 0, 0, 0, 0, 0,
        //             0, 0, 0, 0, 0, 0,
        //             0, 0, 0, 0, 0, 0,
        //             1, 1, 1, 1, 1, 1,
        //             2, 3, 5, 6, 3, 2
        //            });
        _piecePlacements = customPlacements;// cgCustomBoardSettings.GetPiecePlacements(this.boardWidth, this.boardHeight);
        Debug.Log("piece placements " + _piecePlacements.Count);
        _board = new global::cgBoard(_piecePlacements, boardWidth, boardHeight);

        byte totalSquareCount = (byte)(boardWidth * boardHeight);
        List<cgSquareScript> squares = new List<cgSquareScript>();
        for (byte b = 0; b < totalSquareCount; b++)
        {

            float y = Mathf.Floor(b / boardWidth);
            float x = b - (y * boardWidth);
            bool white = (b + y) % 2 == 0;
            GameObject newSquare = ((GameObject)GameObject.Instantiate(white ? whiteSquarePrefab : blackSquarePrefab));
            newSquare.transform.SetParent(boardScript.chessSquareHolder.transform);
            y *= squareSpacing.y;
            x *= squareSpacing.x;
            y = -y;
            newSquare.transform.localScale = new Vector3(squareScale.x, squareScale.y, newSquare.transform.localScale.z);
            newSquare.transform.localPosition = new Vector3(x + squareStartPoint.x, y - squareStartPoint.y, squareStartPoint.z);
            newSquare.GetComponent<cgSquareScript>().uniqueName = _board.SquareNames[b];
            //UnityEngine.Debug.Log("Scale: "+newSquare.transform.localScale);
            squares.Add(newSquare.GetComponent<cgSquareScript>());


            //newSquare.GetComponentInChildren<TextMesh>().text= b.ToString();
            //newSquare.GetComponentInChildren<TextMesh>().GetComponent<MeshRenderer>().sortingOrder = 9;
        }
        for (byte b = 0; b < _piecePlacements.Count; b++)
        {
            if (_piecePlacements[b] != 0)
            {
                //Create a piece and place it accordingly.
                cgChessPieceScript piece = ((GameObject)GameObject.Instantiate(piecePrefab)).GetComponent<cgChessPieceScript>();
                piece.SetType(_piecePlacements[b]);
                piece.transform.SetParent(boardScript.chessPieceHolder.transform);
            }

        }
        boardScript.displayAs3d = use3d;
        boardScript.setBoardTo(_board);
        //We delay the start by 1 frame to allow unity garbage collector to destroy and remove any gameobjects that we've destroyed in this scope.
    }



    private void _destroyChildren(GameObject childHolder)
    {
        UnityEngine.Debug.Log("Destroing children " + childHolder.transform.childCount);
        int existingPieces = childHolder.transform.childCount;
        for (int i = existingPieces; i > 0; i--)
        {
            GameObject.DestroyImmediate(childHolder.transform.GetChild(i - 1).gameObject);
        }
    }

}

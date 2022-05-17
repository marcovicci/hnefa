using UnityEngine;
using System.Collections;
using System;
/// <summary>
/// This script controls the piece on the board, it alters graphics according to promotions/reverts and registers mouse down and mouse up events for dragging purposes
/// </summary>
public class cgChessPieceScript : MonoBehaviour
{
    /// <summary>
    /// Is this piece white?
    /// </summary>
    public bool white
    {
        get
        {
            return (type > 0);
        }
    }

    /// <summary>
    /// The current square being occupied by this instance.
    /// </summary>
    public cgSquareScript square;

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

        BlackPawn = -1,
        BlackRook = -2,
        BlackKnight = -3,
        BlackBishop = -4,
        BlackQueen = -5,
        BlackKing = -6

    }


    /// <summary>
    /// The type of this piece.
    /// </summary>
    public Type type = Type.WhitePawn;

    /// <summary>
    /// Should we display as 3d?(true) or 2d(false).
    /// </summary>
    public bool displayAs3D;


    /// <summary>
    /// If displaying as 2d, we will use the sprites insde this Holder.
    /// </summary>
    public GameObject twoDPieceHolder;

    /// <summary>
    /// If displaying as 3d, we will use the models insde this Holder.
    /// </summary>
    public GameObject threeDPieceHolder;

    /// <summary>
    /// the current pieceholder
    /// </summary>
    private GameObject _pieceholder;

    /// <summary>
    /// Is this instance dead?
    /// </summary>
    public bool dead = false;
    private Action<cgChessPieceScript> _onDown;
    private Action<cgChessPieceScript> _onUp;

    /// <summary>
    /// Set mouse callbacks to allow this instance to be dragged and dropped.
    /// </summary>
    /// <param name="onDown">Callback for mouse down</param>
    /// <param name="onUp">Callback for mouse up</param>
    public void SetCallbacks(Action<cgChessPieceScript> onDown, Action<cgChessPieceScript> onUp)
    {
        _onDown = onDown;
        _onUp = onUp;
        
    }
    void OnMouseDown()
    {
        if (_onDown != null && !dead) _onDown(this);
        
        
    }
    void OnMouseUp()
    {
        if (_onUp != null && !dead) _onUp(this);
    }

    /// <summary>
    /// Set the type of this piece, changes its sprite accordingly.
    /// Useful when reverting moves, or when pawns are promoted.
    /// </summary>
    /// <param name="toType">The type to change to.</param>
    public void SetType(Type toType)
    {

        string typeName = toType.ToString();
        if (!displayAs3D)
        {//Display as 2d.
            threeDPieceHolder.SetActive(false);
            twoDPieceHolder.SetActive(true);

            foreach (Transform child in twoDPieceHolder.transform)
            {
                if (child.gameObject.name == typeName) child.gameObject.SetActive(true);
                else child.gameObject.SetActive(false);
            }
        }
        else if (displayAs3D)
        {
            //Display as 3d.
            threeDPieceHolder.SetActive(true);
            twoDPieceHolder.SetActive(false);

            foreach (Transform child in threeDPieceHolder.transform)
            {
                if (child.gameObject.name == typeName) child.gameObject.SetActive(true);
                else child.gameObject.SetActive(false);
            }

            }
        type = toType;
    }
    /// <summary>
    /// Set the type of this piece, changes its sprite accordingly.
    /// Useful when reverting moves, or when pawns are promoted.
    /// </summary>
    /// <param name="toType">The type to change to.</param>
    public void SetType(int toType)
    {
        Type type = (Type)toType;
        SetType(type);
    }

    /// <summary>
    /// Start at provided square.
    /// </summary>
    /// <param name="startSquare">the starting square.</param>
    public void StartAtSquare(cgSquareScript startSquare)
    {
        square = startSquare;
        dead = false;
        if (startSquare != null)
        {
            //piece.SetStartNode(startnode.node);
            transform.position = new Vector3(startSquare.transform.position.x, startSquare.transform.position.y, startSquare.transform.position.z);
        }
        //UnityEngine.Debug.Log("Start square: " + startSquare);

    }

    /// <summary>
    /// Move to a new square.
    /// </summary>
    /// <param name="newSquare">the new square to move to.</param>
    public void moveToSquare(cgSquareScript newSquare)
    {
        transform.position = new Vector3(newSquare.transform.position.x, newSquare.transform.position.y, newSquare.transform.position.z);

        square = newSquare;
    }
}

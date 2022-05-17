using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// We generate all possible moves(regardless of blocking) for all pieces once here.
/// This technique saves a huge amount of computation for the Engine, as it only has to look up these moves and test their legality on a given board - instead of actually generating them each time a move is performed.
/// </summary>
[System.Serializable]
public class cgMoveGenerator
{
    #region Directions
    private sbyte _LEFT = -1;
    private sbyte _LEFT_DOWN = 7;
    private sbyte _DOWN = 8;
    private sbyte _RIGHT_DOWN = 9;
    private sbyte _RIGHT = 1;
    private sbyte _RIGHT_UP = -7;
    private sbyte _UP = -8;
    private sbyte _LEFT_UP = -9;

    private List<int> _leftDir = new List<int>() { -9, -1, 7 };
    private List<int> _rightDir = new List<int>() { -7, 1, 9 };
    //private List<int> _topDir = new List<int>() { -9, -8, -7 };
    //private List<int> _downDir = new List<int>() { 7, 8, 9 };
    #endregion

    private List<int> _rightBorder = new List<int>() { 7, 15, 23, 31, 39, 47, 55, 63 };
    private List<int> _leftBorder = new List<int>() { 0, 8, 16, 24, 32, 40, 48, 56 };


    private List<int> _whitePawnRow = new List<int>() { 48, 49, 50, 51, 52, 53, 54, 55 };
    private List<int> _blackPawnRow = new List<int>() { 8, 9, 10, 11, 12, 13, 14, 15 };
    public byte _maxCellIndex = 64;
    /// <summary>
    /// Generate all possible moves for a board of the provided size.
    /// </summary>
    /// <param name="width">Number of columns of board</param>
    /// <param name="height">Number of rows of board</param>
    public cgMoveGenerator(byte width = 8, byte height = 8)
    {
        _maxCellIndex = (byte)(width*height);
        _DOWN = (sbyte)width;
        _UP = (sbyte)-width;
        _RIGHT_DOWN = (sbyte)(_RIGHT + _DOWN);
        _RIGHT_UP = (sbyte)(_RIGHT + _UP);
        _LEFT_DOWN = (sbyte)(_LEFT + _DOWN);
        _LEFT_UP = (sbyte)(_LEFT + _UP);
        _leftDir = new List<int>() { _LEFT_DOWN, _LEFT, _LEFT_UP };
        _rightDir = new List<int>() { _RIGHT_DOWN, _RIGHT, _RIGHT_UP };
        //_topDir = new List<int>() { _LEFT_UP, _UP, _RIGHT_UP };
        //_downDir = new List<int>() { _LEFT_DOWN, _DOWN, _RIGHT_DOWN };
        int secondRowIndex = width;
        int secondLastRowIndex = width * (height - 2);
        _whitePawnRow = new List<int>();
        _blackPawnRow = new List<int>();
        for (var u = 0; u < width; u++)
        {
            _blackPawnRow.Add(secondRowIndex + u);
            _whitePawnRow.Add(secondLastRowIndex + u);
        }
        _rightBorder = new List<int>();
        _leftBorder = new List<int>();
        // UnityEngine.Debug.Log("ROW: " + height + " COL:" + width);
        // System.Console.WriteLine("ROW: " + height + " COL:" + width);
        for (var i = 0; i < height; i++)
        {
            _rightBorder.Add((width * (i + 1)) - 1);
            _leftBorder.Add((width * i));
        }
    }

    #region Emulation
    /// <summary>
    /// Emulates all possible bishop movements from target index position.
    /// </summary>
    /// <param name="pos">The 0-64 index position</param>
    /// <returns>All possible moves.</returns>
    public List<int> EmulateBishopAt(int pos)
    {
        List<int> directions = new List<int>() { _LEFT_UP, _RIGHT_UP, _LEFT_DOWN, _RIGHT_DOWN };
        return FindRayMoves(directions, pos);
    }

    /// <summary>
    /// Emulates all possible king movement from target index position
    /// </summary>
    /// <param name="indexPosition">The 0-64 index position</param>
    /// <returns></returns>
    public List<int> EmulateKingAt(int indexPosition)
    {
        List<int> moves = new List<int>();
        List<int> directions = new List<int>() { _LEFT, _LEFT_DOWN, _DOWN, _RIGHT_DOWN, _RIGHT, _RIGHT_UP, _UP,_LEFT_UP };
        foreach (int dir in directions)
        {
            int prevIndex = indexPosition;
            int nextIndex = indexPosition + dir;
            // If its on right border and going right, it cannot go further hence continue. Same for leftbreach
            bool leftBreach = _leftBorder.Contains(prevIndex) && _leftDir.Contains(dir);
            bool rightBreach = _rightBorder.Contains(prevIndex) && _rightDir.Contains(dir);
            if (nextIndex >= 0 && nextIndex < _maxCellIndex && !leftBreach && !rightBreach) moves.Add(nextIndex);
        }
        return moves;
    }

    /// <summary>
    /// Emulates all possible rook movements from target index position.
    /// </summary>
    /// <param name="pos">The 0-64 index position</param>
    /// <returns>All possible moves.</returns>
    public List<int> EmulateRookAt(int pos)
    {
        List<int> directions = new List<int>() { _UP, _LEFT, _DOWN, _RIGHT };
        return FindRayMoves(directions, pos);
    }

    /// <summary>
    /// Emulates all possible queen movements from target index position.
    /// </summary>
    /// <param name="pos">The 0-64 index position</param>
    /// <returns>All possible moves.</returns>
    public List<int> EmulateQueenAt(int pos)
    {
        List<int> directions = new List<int>() { _LEFT, _LEFT_DOWN, _DOWN, _RIGHT_DOWN, _RIGHT, _RIGHT_UP, _UP, _LEFT_UP };
        return FindRayMoves(directions, pos);
    }

    /// <summary>
    /// Emulates all possible knight movements from target index position. And the knight is a tricky motherF!?cker to generate moves for.
    /// </summary>
    /// <param name="pos">The 0-64 index position</param>
    /// <returns>All possible moves.</returns>
    public List<int> EmulateKnightAt(int indexPosition)
    {
        List<int> eligibleIndexPositions = new List<int>();

        List<int> pattern1 = new List<int>() { _UP, _UP, _LEFT };
        List<int> pattern2 = new List<int>() { _UP, _UP, _RIGHT };
        List<int> pattern3 = new List<int>() { _LEFT, _LEFT, _UP };
        List<int> pattern4 = new List<int>() { _LEFT, _LEFT, _DOWN };
        List<int> pattern5 = new List<int>() { _DOWN, _DOWN, _LEFT };
        List<int> pattern6 = new List<int>() { _DOWN, _DOWN, _RIGHT };
        List<int> pattern7 = new List<int>() { _RIGHT, _RIGHT, _UP };
        List<int> pattern8 = new List<int>() { _RIGHT, _RIGHT, _DOWN };

        //Each of these 8 patterns represent one possible move for the knight, each number in the pattern signifies a direction.
        List<List<int>> patterns = new List<List<int>>() { pattern1, pattern2, pattern3, pattern4, pattern5, pattern6, pattern7, pattern8 };
        //Going  into each of the 8 patterns
        foreach (List<int> pattern in patterns)
        {
            int prevIndex = indexPosition;
            int nextIndex = indexPosition;
            int count = 0;
            // If its on right border and going right, it cannot go further.
            bool leftBreach = _leftBorder.Contains(prevIndex) && _leftDir.Contains(pattern[count]);
            bool rightBreach = _rightBorder.Contains(prevIndex) && _rightDir.Contains(pattern[count]);
            while (nextIndex >= 0 && nextIndex < _maxCellIndex && !leftBreach && !rightBreach && count < pattern.Count + 1)
            {

                if (count == (pattern.Count))
                {
                    //reached the end of the pattern, since the while loop hasn't broken we can safely assume the square is inside the board, and thus add it.
                    eligibleIndexPositions.Add(nextIndex);
                    break;
                }
                prevIndex = nextIndex;
                nextIndex = nextIndex + pattern[count];
                leftBreach = (_leftBorder.Contains(prevIndex) && _leftDir.Contains(pattern[count]));
                rightBreach = (_rightBorder.Contains(prevIndex) && _rightDir.Contains(pattern[count]));
                count++;
            }
            // each -1 in the array signifies the beginning of a new 'ray'
            //Since knights can't be blocked theres no need for the ray.

        }
        return eligibleIndexPositions;
    }

    /// <summary>
    /// Emulates all possible pawn movements from target index position.
    /// </summary>
    /// <param name="pos">The 0-64 index position</param>
    /// <returns>All possible moves.</returns>
    public List<int> EmulatePawnAt(int indexPosition, bool white)
    {
        List<int> moves = new List<int>();
        List<int> directions = new List<int>();
        if (white) directions.Add(_UP);
        else if (!white) directions.Add(_DOWN);

        //add basic forward move.
        foreach (int dir in directions)
        {
            int prevIndex = indexPosition;
            int nextIndex = indexPosition + dir;
            // If its on right border and going right, it cannot go further hence continue.
            bool leftBreach = _leftBorder.Contains(prevIndex) && _leftDir.Contains(dir);
            bool rightBreach = _rightBorder.Contains(prevIndex) && _rightDir.Contains(dir);
            if (nextIndex >= 0 && nextIndex < _maxCellIndex && !leftBreach && !rightBreach) moves.Add(nextIndex);
        }



        //add double move if still at start row.
        if (white && _whitePawnRow.Contains(indexPosition))
        {
            moves.Add(indexPosition + (_UP+_UP));
            moves.Add(-1);//Minus one to signify end of ray.
        }
        else if (!white && _blackPawnRow.Contains(indexPosition))
        {
            moves.Add(indexPosition + (_DOWN+_DOWN));
            moves.Add(-1);//Minus one to signify end of ray.
        }

        //add attack moves
        directions = new List<int>();
        if (white)
        {
            directions.Add(_LEFT_UP);
            directions.Add(_RIGHT_UP);
        }
        else if (!white)
        {
            directions.Add(_LEFT_DOWN);
            directions.Add(_RIGHT_DOWN);
        }
        moves.Add(-2);
        foreach (int dir in directions)
        {

            int prevIndex = indexPosition;
            int nextIndex = indexPosition + dir;
            // If its on right border and going right, it cannot go further hence continue.
            bool leftBreach = _leftBorder.Contains(prevIndex) && _leftDir.Contains(dir);
            bool rightBreach = _rightBorder.Contains(prevIndex) && _rightDir.Contains(dir);
            if (nextIndex >= 0 && nextIndex < _maxCellIndex && !leftBreach && !rightBreach)
            {
                moves.Add(nextIndex);
            }
        }
        //UnityEngine.Debug.Log("all pawn moves on:" + indexPosition + " as White:" + white + " " + pceGlobal.ListToString(moves));
        return moves;
    }

    /// <summary>
    /// Moves as far in each direction as possible  without going outside the chess board.
    /// </summary>
    /// <param name="directions">Directions, as specified by how much it should add to the current position, i.e -9 = top left, -8 = top, -7 = top right, -1=left, 1 = right etc.</param>
    /// <param name="indexPosition">Starting position</param>
    /// <returns>A List of all possible moves inside the 1x64 chess board</returns>
    public List<int> FindRayMoves(List<int> directions, int indexPosition)
    {
        List<int> eligibleIndexPositions = new List<int>();
        //Going  into each of the 8 directions.
        foreach (int dir in directions)
        {
            int prevIndex = indexPosition;
            int nextIndex = indexPosition + dir;
            // If its on right border and going right, it cannot go further hence continue.
            bool leftBreach = _leftBorder.Contains(prevIndex) && _leftDir.Contains(dir);
            bool rightBreach = _rightBorder.Contains(prevIndex) && _rightDir.Contains(dir);
            while (nextIndex >= 0 && nextIndex < _maxCellIndex && !leftBreach && !rightBreach)
            {

                eligibleIndexPositions.Add(nextIndex);
                prevIndex = nextIndex;
                nextIndex = nextIndex + dir;
                leftBreach = (_leftBorder.Contains(prevIndex) && _leftDir.Contains(dir));
                rightBreach = (_rightBorder.Contains(prevIndex) && _rightDir.Contains(dir));
            }
            // each -1 in the array signifies the beginning of a new 'ray'
            eligibleIndexPositions.Add(-1);

        }
        return eligibleIndexPositions;
    }
    #endregion

}


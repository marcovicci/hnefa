using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulator : MonoBehaviour
{
    // here be references 
    private PieceManager mPieceManager;
    private GameBoard mBoard;
    private Director mDirector;

    // Scores for each side, for evaluating moves
    private float blackScore = 0.0f;
    private float whiteScore = 0.0f;

    // Win/lose values, for evaluating moves, and king position
    public bool mIsKingAlive = true;
    public bool mIsKingFree = false;
    public Vector2Int mKingPos = new Vector2Int(5, 5);

    // lists, arrays, tuples etc (I am going a little bonkers) for pieces and cells
    public BasePiece[] mVirtualWhite = new BasePiece[121];
    public BasePiece[] mVirtualBlack = new BasePiece[121];
    public SingleCell[,] mAllCells = new SingleCell[11, 11];
    public List<(SingleCell, BasePiece)> mPossibleMoves;

    // This next one is weird - it's storing the white pieces, black pieces and cells.
    // In theory we can use this to roll back to previous states and build the board from there.
    public List<(BasePiece[], BasePiece[], SingleCell[,])> mBoardHistory;

    public void Start()
    {
        // Find the director script attached to the same object.
        mDirector = GetComponent<Director>();
        // Fetch the Piece Manager and Game Board off the director script.
        mBoard = mDirector.mBoard.GetComponent<GameBoard>();
        mPieceManager = mDirector.mPieceManager.GetComponent<PieceManager>();
        
    }

    public void CopyInValues()
    {
        // The first thing we might want to do is actually grab the real board's current values.
        // Then we can run simulations from that state.
        // Using CopyTo we will actually create new GameObjects and not just new references to the same objects - 
        // which is great for our simulation, but let's see if that makes everything explode later.
        mPieceManager.mWhitePieces.CopyTo(mVirtualWhite, 0);
        mPieceManager.mBlackPieces.CopyTo(mVirtualBlack, 0);

        // We also need to create a virtual board. This is gonna be pretty gross.
        // To differentiate from the real board and make cleanup easier, we'll parent everything to this game director object we're attached to.
        // This is mostly the same code from GameBoard.Create() but there's no reason to actually position the cells visually.
        // So we'll remove that portion after testing and all the virtual cells can be hidden off to the side.
        for (int y = 0; y < 11; y++)
        { // I LOVE A GOOD NESTED FOR LOOP
            for (int x = 0; x < 11; x++)
            {
            // Actually creating the cell from our prefab!
            GameObject newCell = Instantiate(mBoard.mCellPrefab, transform);

            // Positioning it
            RectTransform newRectTransform = newCell.GetComponent<RectTransform>();
            // We want the pivot point centered in the cell.
            // So we're offsetting each cell by mnCellSize and then also adding half of mCellSize to accomplish that.
            newRectTransform.anchoredPosition = new Vector2((x * mBoard.mnCellSize) + (mBoard.mnCellSize / 2), (y * mBoard.mnCellSize) + (mBoard.mnCellSize / 2));

            // Properly calling that Setup() function in each cell
            mAllCells[x, y] = newCell.GetComponent<SingleCell>();
            mAllCells[x, y].Setup(new Vector2Int(x,y), mDirector.mBoard, mBoard.mnCellSize);
            }
        }

        // Great, all done with the gross stuff... haha just kidding. We can now place our virtual pieces lists in our virtual board.
        // Check the function below for more details there.
        PlaceVirtualPieces(mVirtualWhite);
        PlaceVirtualPieces(mVirtualBlack);

        // Finally, we should write these into our board history. 
        mBoardHistory.Add((mVirtualWhite, mVirtualBlack, mAllCells));
    }  

    public void PlaceVirtualPieces(BasePiece[] thisTeam)
    {
        // This is mostly the same code from PieceManager.PlacePieces() but uses our virtual board instead of the real board.
        // Also toggles a special "is virtual" boolean for that piece.
        int i = 0;
        for (int y = 0; y < 11; y++)
        { // I LOVE A GOOD NESTED FOR LOOP
            for (int x = 0; x < 11; x++)
            {
            if (thisTeam[i] != null)
            {
                thisTeam[i].Place(mAllCells[x, y]);
                thisTeam[i].mIsVirtual = true;
                thisTeam[i].mArrayPosition = i;
            }
            i++;
            }
        }
    }

    public void EvaluateMoves(BasePiece[] thisTeam)
    {
        // Awesome, our virtual board is setup so let's actually evaluate moves for a team. 
        // First we're running CheckPathing on each piece, then using a tuple to store the moves and pieces.
        foreach (BasePiece piece in thisTeam)
        {
            piece.CheckPathing();
            foreach (SingleCell cell in piece.mHighlightedCells) 
            { // I love a good nested... yeah, you know this joke already
                mPossibleMoves.Add((cell, piece));
            }
        }

        // Now we should have a massive tuple containing cells (moves) and the pieces associated with those moves. Yay!
        // Storing it this way means that if we need to prioritize a particular piece moving (for emotional reasons) we can single out moves with that piece.
        // And of course, once a move is selected, it knows which piece can be moved there. 

    }

    public (float, float) ScoreBoardState()
    {
        // This is the function for actually evaluating who is "winning" in a current board state and how well they're doing.
        // First we initialize those score values.
        blackScore = 0.0f;
        whiteScore = 0.0f;

        // We get the jarl to send us information upon win/lose if he's virtual, so we can check that here.
        if (!mIsKingAlive)
        {
            // King's dead. Oops. Allocate score accordingly.
            blackScore += 1000.0f;
            whiteScore -= 1000.0f;
            return (blackScore, whiteScore);
        }
        if (mIsKingFree)
        {
            // King's very much alive and well. in fact, he's busted into a corner and escaped! This simulation went well!
            blackScore -= 1000.0f;
            whiteScore += 1000.0f;
            return (blackScore, whiteScore);
        }

        // If the game hasn't been won or lost we should move on to different ways of evaluating score instead.
        // A good place to start is counting remaining pieces.
        // So let's reward the team for each piece still on the board.
        // Because I store the pieces in this silly array where empty spaces count as nulls, it's harder to penalize for lost pieces.
        // So instead I'm penalizing the opposing side for pieces that still exist.
        // If I want to come back and do this later it could be good to compare the not-null pieces to the original number of pieces.
        // White side gets 13 pieces to start with, black side has 24. 
        foreach (BasePiece piece in mVirtualWhite)
        {
            if (piece != null)
            {
                whiteScore += 1.0f;
                blackScore -= 1.0f;
            }
        }

        foreach (BasePiece piece in mVirtualBlack)
        {
            if (piece != null)
            {
                whiteScore -= 1.0f;
                blackScore += 1.0f;
            }
        }

        // Let's also modify the score based on how far the King is from the corners.
        // We have the King's current square in mKingPos, and we know that if his X or Y is at 0 or 10 he's doing great.
        // So we can use Vector2Int.Distance to calculate how well he's doing.
        float bottomLeftDistance = Vector2Int.Distance(new Vector2Int(0,0), mKingPos);
        float topLeftDistance = Vector2Int.Distance(new Vector2Int(0,10), mKingPos);
        float bottomRightDistance = Vector2Int.Distance(new Vector2Int(10,0), mKingPos);
        float topRightDistance = Vector2Int.Distance(new Vector2Int(10,10), mKingPos);

        // We can just do math based on the shortest distance, so Mathf.Min is our friend here :)
        float shortestDistance = Mathf.Min(bottomLeftDistance, topLeftDistance, bottomRightDistance, topRightDistance);

        // Now we reward the black player less if the king is close to the corners,
        // and we penalize the white player less if the king is close to the corners.
        whiteScore -= shortestDistance;
        blackScore += shortestDistance;

        return (blackScore, whiteScore);
    }

    
}

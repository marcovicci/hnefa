using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.ComponentModel;
using System.Timers;
using System.Diagnostics;

/// <summary>
/// The script attached to the chessboard prefab, should have gameobjects with squarescripts, this class: verifies if the player can drag and drop pieces, handles whether or not the engine should make move, checks whether or not the game is over(and if so shows the game over prefab), flips the board, resets the board etc.
/// </summary>
[System.Serializable]
public class cgChessBoardScript : MonoBehaviour
{

    #region Properties & fields
    /// <summary>
    /// A sound to play whenever a move is made.
    /// </summary>
    public AudioClip moveSound;

    /// <summary>
    /// A sound to play when any player makes a move that checks the king.
    /// </summary>
    public AudioClip checkSound;

    /// <summary>
    /// A sound to play when the game is won.
    /// </summary>
    public AudioClip winSound;

    /// <summary>
    /// A sound to play when the game is lost or drawn.
    /// </summary>
    public AudioClip loseSound;

    /// <summary>
    /// The number of moves made.
    /// </summary>
    public int movesMade = 0;

    /// <summary>
    /// The textfield to log all moves made.
    /// </summary>
    public UnityEngine.UI.Text moveLog;

    /// <summary>
    /// The provided loadbar to display how far the analysis is.
    /// </summary>
    public GameObject engineProgressBar;

    /// <summary>
    /// The target to flip when the user click the Flip board button.
    /// </summary>
    public GameObject flipTarget;

    /// <summary>
    /// The target to flip when the user click the Flip board button.
    /// </summary>
    public GameObject chessPieceHolder;

    /// <summary>
    /// The target to flip when the user click the Flip board button.
    /// </summary>
    public GameObject chessSquareHolder;

    /// <summary>
    /// Game over prefab, instantiated when the game is won/drawn/lost
    /// </summary>
    public GameObject GameOverPrefab;

    /// <summary>
    /// Main menu prefab, instantiated when the user clicks Main menu button.
    /// </summary>
    public GameObject MainMenuPrefab;
    /// <summary>
    /// Main menu prefab, instantiated when the user clicks Main menu button.
    /// </summary>
    public GameObject ChessPiecePrefab;

    /// <summary>
    /// Is it whites turn to move? if false then its blacks turn.
    /// </summary>
    public bool whiteTurnToMove = true;

    /// <summary>
    /// Should the last move be highlighted on the board?
    /// </summary>
    public bool highlightLastMove = true;

    /// <summary>
    /// Should the all legal moves be highlighted when the player drags a piece?
    /// </summary>
    public bool highlightLegalMoves = true;

    /// <summary>
    /// Should the pieces be displayed as 3d pieces?
    /// </summary>
    public bool displayAs3d = true;

    /// <summary>
    /// All possible board modes.
    /// </summary>
    public enum BoardMode { Undefined, PlayerVsPlayer, PlayerVsEngine, EngineVsPlayer, EngineVsEngine }

    /// <summary>
    /// The current board mode.
    /// </summary>
    public BoardMode Mode = BoardMode.PlayerVsEngine;

    /// <summary>
    /// What location should dead black pieces be placed at?
    /// </summary>
    public Vector3 blackDeadPiecesLocation = new Vector3(4, -2, 0);
    /// <summary>
    /// What location should dead white pieces be placed at?
    /// </summary>
    public Vector3 whiteDeadPiecesLocation = new Vector3(3, -2, 0);
    /// <summary>
    /// Which notationtype should be used to notate the game?
    /// </summary>
    public cgNotation.NotationType NotationType = cgNotation.NotationType.Algebraic;

    /// <summary>
    /// Should the early moves of the engine be randomized slightly to allow for a wide array of variations?
    /// </summary>
    public bool randomizeEarlyEngineMoves = true;

    /// <summary>
    /// Should the board start on load, or do you want to manually start the board by called Start?
    /// </summary>
    public bool startOnLoad = true;
    /// <summary>
    /// What depth should the engine search seemingly weak moves to? The higher the value the better the AI and the longer the load time.
    /// </summary>
    public byte searchDepthWeak = 4;
    /// <summary>
    /// What depth should the engine search seemingly strong moves to? The higher the value the better the AI and the longer the load time.
    /// </summary>
    public byte searchDepthStrong = 4;

    /// <summary>
    /// Should the engine use multithreading? Please note: Multithreading is not supported on some platforms such as WebGL.
    /// </summary>
    public bool useMultiThreading = true;

    /// <summary>
    /// The instance of the game over screen.
    /// </summary>
    private GameObject _gameOverScreen;

    /// <summary>
    /// This is the underlying board representation, we test and find legality of moves on this.
    /// </summary>
    [SerializeField]
    private cgBoard _abstractBoard = null;//new cgBoard();

    /// <summary>
    /// All currently captured pieces.
    /// </summary>
    private List<cgChessPieceScript> _deadPieces = new List<cgChessPieceScript>();

    /// <summary>
    /// All currently uncaptured pieces on the board.
    /// </summary>
    private List<cgChessPieceScript> _livePieces = new List<cgChessPieceScript>();

    /// <summary>
    /// The AI opponent
    /// </summary>
    private cgEngine _engine;

    /// <summary>
    /// Number of dead white pieces.
    /// </summary>
    private int _deadWhitePieces = 0;

    /// <summary>
    /// Number of dead black pieces.
    /// </summary>
    private int _deadBlackPieces = 0;

    /// <summary>
    /// The current piece being dragged by the mouse.
    /// </summary>
    private cgChessPieceScript _downPiece;

    /// <summary>
    /// Logged moves, used by coordinate notation.
    /// </summary>
    private int _loggedMoves = 0;

    private cgSquareScript[] _squares;


    private List<List<cgSimpleMove>> _engineCallbackParams = new List<List<cgSimpleMove>>();
    private List<Action<List<cgSimpleMove>>> _engineCallbackFunctions = new List<Action<List<cgSimpleMove>>>();
    private List<float> _engineProgress = new List<float>();
    private BackgroundWorker _engineThread;
    private Stopwatch stopwatch;
    /// <summary>
    /// Can the player drag and move pieces? Yes if a human controls the current color whoms turn it is to move.
    /// </summary>
    public bool playerCanMove
    {
        get
        {
            return ((_humanPlayerIsBlack && !whiteTurnToMove) || (_humanPlayerIsWhite && whiteTurnToMove)) ? true : false;
        }
    }
    /// <summary>
    /// Get the engine.
    /// </summary>
    public cgEngine getEngine
    {
        get
        {
            return _engine;
        }
    }
    /// <summary>
    /// Is it a human playing the black pieces? Determined by the current boardmode.
    /// </summary>
    private bool _humanPlayerIsBlack
    {
        get
        {
            return ((Mode == BoardMode.EngineVsPlayer) || (Mode == BoardMode.PlayerVsPlayer));
        }
    }

    /// <summary>
    /// Is it a human playing white? Determined by the current boardmode.
    /// </summary>
    private bool _humanPlayerIsWhite
    {
        get
        {
            return ((Mode == BoardMode.PlayerVsEngine) || (Mode == BoardMode.PlayerVsPlayer));
        }
    }

    #endregion

    #region Initialization
    /// <summary>
    /// Start the game, with provided board.
    /// </summary>
    /// <param name="customBoard">The abstract board that we should match, if none specified we use existing one, if none exists we generate an 8x8 board</param>
    /// <param name="mode">Which mode the game is in, player vs player, player vs engine, engine vs engine etc.</param>
    public void start(cgBoard customBoard = null, BoardMode mode = BoardMode.Undefined)
    {
        if (customBoard == null)
        {
            if (this._abstractBoard != null)
            {
                customBoard = this._abstractBoard;
            }
            else
            {
                customBoard = new global::cgBoard();
            }
        }
        if (displayAs3d)
        {
            Camera.main.transform.localPosition = new Vector3(0, -2.28f, -12.58f);
            Quaternion newQuat = Camera.main.transform.localRotation;
            newQuat = Quaternion.Euler(-23.46f, newQuat.eulerAngles.y, newQuat.eulerAngles.z);
            Camera.main.transform.localRotation = newQuat;

        }
        #if UNITY_WEBGL 
            this.useMultiThreading = false;
        #endif

        _squares = getSquares();
        Mode = (mode != BoardMode.Undefined ? mode : Mode);
        _engine = new cgEngine(searchDepthWeak, searchDepthStrong);
        UnityEngine.Debug.Log(customBoard.boardToNeatString());
        setBoardTo(customBoard);

        //Determine if engine should make a starting move based on Mode.
        if (Mode == BoardMode.PlayerVsEngine && !whiteTurnToMove) MakeEngineMove(_abstractBoard.duplicate(), false, _engineCallback);
        else if (Mode == BoardMode.EngineVsPlayer && whiteTurnToMove) MakeEngineMove(_abstractBoard.duplicate(), true, _engineCallback);
        else if (Mode == BoardMode.EngineVsEngine) MakeEngineMove(_abstractBoard.duplicate(), true, _engineCallback);


    }
    /// <summary>
    /// Get all cgSquareScripts.
    /// </summary>
    /// <returns>All cgSquareScripts</returns>
    public cgSquareScript[] getSquares()
    {
        return chessSquareHolder.GetComponentsInChildren<cgSquareScript>();
    }
    /// <summary>
    /// Place pieces according to the current state of the underlying abstractboard.
    /// </summary>
    public void placePieces()
    {
        
        //Collect existing pieces, we instantiate new ones if we run out of existing pieces below.
        List<cgChessPieceScript> pieces = new List<cgChessPieceScript>();
        foreach (cgChessPieceScript piece in chessPieceHolder.GetComponentsInChildren<cgChessPieceScript>()) pieces.Add(piece);

        // UnityEngine.Debug.Log("Place pieces called. " + pieces.Count + " " + chessPieceHolder.GetComponentsInChildren<cgChessPieceScript>().Length+" "+_abstractBoard.squares.Count);
        for (byte i = 0; i < _abstractBoard.squares.Count; i++)
        {
            if (_abstractBoard.squares[i] != 0)
            {
                if (pieces.Count == 0)
                { //We instantiate more pieces if we run out of pieces to place.
                    cgChessPieceScript newPiece = GameObject.Instantiate(ChessPiecePrefab).GetComponent<cgChessPieceScript>();
                    newPiece.gameObject.transform.parent = chessPieceHolder.transform;
                    newPiece.displayAs3D = this.displayAs3d;
                    newPiece.gameObject.SetActive(true);
                    pieces.Add(newPiece);
                    //UnityEngine.Debug.Log("Instantiating piece");
                }
                cgChessPieceScript piece = pieces[pieces.Count - 1];
                //UnityEngine.Debug.Log("i: "+i+" squarenames: "+_abstractBoard.SquareNames.Length);
                piece.StartAtSquare(_getSquare(_abstractBoard.SquareNames[i]));
                _livePieces.Add(piece);
                piece.dead = false;
                piece.displayAs3D = this.displayAs3d;
                piece.SetType((int)_abstractBoard.squares[i]);
                pieces.Remove(piece);
                //UnityEngine.Debug.Log("Finding piece for " + _abstractBoard.squares[i]);
            }
        }

        //Unused pieces
        foreach (cgChessPieceScript piece in pieces)
        {
            GameObject.DestroyImmediate(piece.gameObject);
        }
        cgChessPieceScript[] allPieces = chessPieceHolder.GetComponentsInChildren<cgChessPieceScript>();
        foreach(cgChessPieceScript piece in allPieces)
        {
            if (piece.square == null)
                GameObject.Destroy(piece.gameObject);

        }
        //Give all active pieces callback functions for mouse events.
        foreach (cgChessPieceScript piece in _livePieces)
        {
            piece.SetCallbacks(_pieceDown, _pieceUp);
        }

    }
    #endregion

    #region Unity messages
    // Use this for initialization
    void Awake()
    {
        if (startOnLoad)
        {
            // UnityEngine.Debug.Log("Abstract board: " + this._abstractBoard);
            // UnityEngine.Debug.Log("board prop: " + this._abstractBoard.boardHeight);
            // foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(_abstractBoard))
            // {
            //     string name = descriptor.Name;
            //     object value = descriptor.GetValue(_abstractBoard);
            //     Console.WriteLine("{0}={1}", name, value);
            // }
            
            // List<sbyte> _placements =
            // new List<sbyte>{
            //                 -2,  0,  0, -2,  0, -3, -6,  0,
            //                 0, -1,  0,  0,  0, -1, -1,  0,
            //                 0,  0, -1, -4,  0,  0,  0,  0,
            //                 1,  0,  0, -3,  0,  0,  1,  0,
            //                 0,  0,  0, -5,  0,  0,  0,  0,
            //                 1,  0,  3,  0,  0,  5,  0,  1,
            //                 4,  0,  0,  3,  0,  2,  0,  0,
            //                 2,  0,  0,  0,  0,  0,  6,  0
            //                 };
            // this._abstractBoard = new cgBoard(_placements);
            this._abstractBoard.generateHypotheticalMoves();
            start(this._abstractBoard);
        }


    }
    // Update is called once per frame
    void Update()
    {
        lock (_engineCallbackParams)
        {
            if (_engineCallbackParams.Count > 0)
            {
                UnityEngine.Debug.Log(": " + _engineCallbackParams.Count+" is it more than 0 ? "+(_engineCallbackParams.Count>0));
                _engineCallbackFunctions[0](_engineCallbackParams[0]);
                _engineCallbackParams.RemoveAt(0);
                _engineCallbackFunctions.RemoveAt(0);
                //threadListener.Stop();
            }
        }
        lock (_engineProgress)
        {
            if (_engineProgress.Count > 0 && engineProgressBar != null)
            {
                float progress = _engineProgress[0];
                _engineProgress.RemoveAt(0);
                Vector3 nscale = engineProgressBar.transform.localScale;
                nscale.x = progress;
                engineProgressBar.transform.localScale = nscale;
            }
        }
        foreach (cgChessPieceScript cp in _livePieces)
        {
            if (cp.dead && !_deadPieces.Contains(cp)) _setDeadPiece(cp);
        }

        for (int i = _deadPieces.Count; i > 0; i--)
        {
            if (_livePieces.Contains(_deadPieces[i - 1])) _livePieces.Remove(_deadPieces[i - 1]);
        }
        if (_downPiece != null)
        {
            Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
            Ray cursorRay = Camera.main.ScreenPointToRay(cursorPoint);
            RaycastHit hit;
            if (Physics.Raycast(cursorRay, out hit, 100.0f))
            {
                _downPiece.transform.position = hit.point;
            }
        }

        if (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.LeftControl)) _copyGameToClipboard();
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C)) _copyGameToClipboard();


        if (Input.GetKey(KeyCode.V) && Input.GetKeyDown(KeyCode.LeftControl)) _pasteGameFromClipboard();
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V)) _pasteGameFromClipboard();

        if (Input.GetKey(KeyCode.F1)) UnityEngine.Debug.Break();
        if (Input.GetKey(KeyCode.A))
        {
            //_abstractBoard.boardToString()
            UnityEngine.Debug.Log(_abstractBoard.boardToString());
            //UnityEngine.Debug.Log(_abstractBoard._blackKingStart);
        }
        if (Input.GetKey(KeyCode.U))
        {
            _abstractBoard.revert();
            UnityEngine.Debug.Log(_abstractBoard.boardToString());
        }
        if (Input.GetKey(KeyCode.V))
        {//Perform long castling move.
            List<cgSimpleMove> moves = _abstractBoard.findStrictLegalMoves(_abstractBoard.whiteTurnToMove);
            cgSimpleMove move = null;
            foreach(cgSimpleMove mv in moves)
            {
                if (mv is cgCastlingMove)
                {
                    if (move == null)
                    {
                        move = mv;
                    }
                    else if (Math.Abs(move.to - move.from) < Math.Abs(mv.to - mv.to))
                    {
                        move = mv;
                    }
                }
            }
            _makeMove(move);
        }

    }
    #endregion

    #region UI Button functions
    /// <summary>
    /// Resets the board, called by the menu button called restart.
    /// </summary>
    public void ResetBoard()
    {
        //UnityEngine.Debug.Log("reset");
        cgBoard newboard = _abstractBoard.duplicate().revertToStart();

        if (_gameOverScreen != null)
        {
            //UnityEngine.Debug.Log("destroying:" + _gameOverScreen);
            GameObject.Destroy(_gameOverScreen);
            _gameOverScreen = null;
        }

        start(newboard, Mode);
    }
    /// <summary>
    /// Reverts the last move, called by the menu button called revert last move.
    /// </summary>
    public void RevertLastMove()
    {
        //UnityEngine.Debug.Log("reverting");
        _abstractBoard.revert();
        setBoardTo(_abstractBoard);
    }
    /// <summary>
    /// Instantiates and shows an instance of MainMenuPrefab. Called by menu button called Main Menu
    /// </summary>
    public void MainMenu()
    {
        if (_gameOverScreen != null)
        {
            GameObject.Destroy(_gameOverScreen);
            _gameOverScreen = null;
        }
        GameObject.Instantiate(MainMenuPrefab);
        GameObject.DestroyImmediate(gameObject);
    }
    /// <summary>
    /// Flips the board so if white is currently on top, it will be at the bottom. Preferable if playing as black.
    /// Called by the menu button called Flip Board.
    /// </summary>
    public void FlipBoard()
    {
        if (flipTarget != null)
        {
            int increment = 180;

            //Vector3 rotatePoint = new Vector3(0.000725314f, 0.001148298f, 0);
            //Vector3 rotatePoint = new Vector3(flipTarget.transform.position.x,flipTarget.transform.position.y, 0);
            flipTarget.transform.RotateAround(Vector3.zero, Vector3.forward, increment);
            //flipTarget.transform.rot
            //flipTarget.transform.Rotate(Vector3.forward, increment);
            foreach (cgChessPieceScript piece in _livePieces) piece.transform.Rotate(Vector3.forward, increment);
            foreach (cgChessPieceScript piece in _deadPieces) piece.transform.Rotate(Vector3.forward, increment);
        }

    }
    /// <summary>
    /// Use the engine to find a suggested move for the current position. Called by the menu button called Suggest Move.
    /// </summary>
    public void SuggestMove()
    {
        if (playerCanMove) MakeEngineMove(_abstractBoard.duplicate(), _abstractBoard.whiteTurnToMove, _engineSuggestion);
    }

    private void _engineSuggestion(List<cgSimpleMove> moves)
    {
        if (playerCanMove)
        {
            if (_abstractBoard.verifyLegality(moves[0])) _suggestMove(moves[0]);
            else
            {
                moves.RemoveAt(0);
                if (moves.Count > 0) _engineSuggestion(moves);
            }

        }
    }

    private void _suggestMove(cgSimpleMove move)
    {
        cgSquareScript departingSquare = _getSquare(this._abstractBoard.SquareNames[move.from]);
        cgSquareScript arrivalSquare = _getSquare(this._abstractBoard.SquareNames[move.to]);

        departingSquare.highlightTemporarily(new Color(0, .5f, 0));
        arrivalSquare.highlightTemporarily(new Color(0, .5f, 0));
    }
    #endregion

    /// <summary>
    /// Play provided sound, adds an audiosource if one does not exist on this gameobject.
    /// </summary>
    /// <param name="clip"></param>
    public void playSound(AudioClip clip)
    {
        if (clip == null) return;
        if (GetComponent<AudioSource>() == null) gameObject.AddComponent<AudioSource>();
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();

    }


    /// <summary>
    /// A piece has callbacked that the user has pressed it.
    /// </summary>
    /// <param name="piece"></param>
    private void _pieceDown(cgChessPieceScript piece)
    {

        if (highlightLegalMoves && playerCanMove)
        {
            List<cgSimpleMove> moves = _abstractBoard.findStrictLegalMoves(_abstractBoard.whiteTurnToMove);
            foreach (cgSimpleMove move in moves)
            {
                if (_abstractBoard.SquareNames[move.from] == piece.square.uniqueName)
                {
                    if (move is cgCastlingMove)
                    {//Highlighting rook instead of king destination when castling.
                        _getSquare(_abstractBoard.SquareNames[(move as cgCastlingMove).secondFrom]).changeColor(_getSquare(_abstractBoard.SquareNames[(move as cgCastlingMove).secondFrom]).legalMoveToColor);
                    }
                    else
                    {
                        _getSquare(_abstractBoard.SquareNames[move.to]).changeColor(_getSquare(_abstractBoard.SquareNames[move.from]).legalMoveToColor);
                    }
                }

            }
        }
        _downPiece = piece;


        //int indexPosition = cgGlobal.IndexFromCellName(_downPiece.square.uniqueName);

        //_abstractBoard.squares[indexPosition = 2//make the changes you want.
    }

    /// <summary>
    /// The user has released a dragged piece. Verify that its a legal move, if so perform the move and perform the next move if appropriate mode.
    /// </summary>
    /// <param name="piece"></param>
    private void _pieceUp(cgChessPieceScript piece)
    {
        if (_downPiece != null)
        {
            if (playerCanMove || Mode == BoardMode.PlayerVsPlayer)
            {
                cgSimpleMove legalMove = null;
                cgSquareScript closestSquare = _findSquareAt(_downPiece.transform.position);
                List<cgSimpleMove> legalMoves = _abstractBoard.findStrictLegalMoves(whiteTurnToMove);
                foreach (cgSimpleMove move in legalMoves)
                {
                    if (move is cgCastlingMove)
                    {
                        if (_abstractBoard.SquareNames[move.from] == _downPiece.square.uniqueName && _abstractBoard.SquareNames[(move as cgCastlingMove).secondFrom] == closestSquare.uniqueName)
                        {

                            legalMove = move;
                        }

                    }
                    else
                    {
                        if (_abstractBoard.SquareNames[move.from] == _downPiece.square.uniqueName && _abstractBoard.SquareNames[move.to] == closestSquare.uniqueName)
                        {
                            legalMove = move;
                        }
                    }
                }
                //test legality of move here.

                if (legalMove != null && _abstractBoard.verifyLegality(legalMove))
                {
                    _makeMove(legalMove);
                    if (Mode == BoardMode.PlayerVsEngine) MakeEngineMove(_abstractBoard.duplicate(), false, _engineCallback);
                    else if (Mode == BoardMode.EngineVsPlayer) MakeEngineMove(_abstractBoard.duplicate(), true, _engineCallback);
                }
                else piece.moveToSquare(piece.square);
            }
            else piece.moveToSquare(piece.square);
            _downPiece = null;
        }
        else
        {
            piece.moveToSquare(piece.square);
            _downPiece = null;
        }
        if (highlightLastMove)
        {//revert colors if highlighting is active
            foreach (cgSquareScript square in _squares) square.changeColor(square.startColor);
        }
    }

    /// <summary>
    /// Find the square location at the provided position, used to find the square where the user is dragging and dropping a piece.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private cgSquareScript _findSquareAt(Vector3 position)
    {
        float best = Vector3.Distance(position, _squares[0].transform.position);
        cgSquareScript square = _squares[0];

        foreach (cgSquareScript candsquare in _squares)
        {
            if (Vector3.Distance(position, candsquare.transform.position) < best)
            {
                best = Vector3.Distance(position, candsquare.transform.position);
                square = candsquare;
            }
        }
        return square;
    }

    /// <summary>
    /// The engine returns with its prefered move.
    /// </summary>
    /// <param name="move">The prefered move.</param>
    private void _engineCallback(List<cgSimpleMove> moves)
    {

        if (!playerCanMove)
        {
            cgSimpleMove move = moves[0];
            if (this._abstractBoard.moves.Count < 10 && randomizeEarlyEngineMoves)
            {//We are in the very early game and we should randomize the engine moves early, so we will make a random-ish choice here instad of the prefered choice.
                //random-ish in the sense, that we will take a random move of one of the moves that are in the better half of all possible moves.
                
                List<cgSimpleMove> candidates = _findAllMovesNear(moves, 200);
                UnityEngine.Debug.Log("Candidates: " + candidates.Count);
                int choice = UnityEngine.Random.Range(0, candidates.Count);

                move = moves[choice];
            }
            if (_abstractBoard.verifyLegality(move)) _makeMove(move);
            else
            {
                moves.Remove(move);
                if (_engine.moves.Count > 0) _engineCallback(moves);
                else
                {
                    _checkGameOver();
                }
            }


        }
    }

    private List<cgSimpleMove> _findAllMovesNear(List<cgSimpleMove> moves, int spill)
    {
        List<cgSimpleMove> candidates = new List<cgSimpleMove>();
        int best = moves[0].val;
        foreach(cgSimpleMove mv in moves)
        {
            if (Math.Abs(mv.val - best) < spill)
                candidates.Add(mv);
        }
        return candidates;
    }

    /// <summary>
    /// Game over, instantiate the game over screen and let it display the provided message.
    /// </summary>
    /// <param name="display"></param>
    private void _gameOver(bool whiteWins, bool blackWins)
    {
        string gameOverString = "Game Over. ";
        if (whiteWins)
            gameOverString = "White Wins!";
        else if (blackWins)
            gameOverString = "Black Wins!";
        else if (!blackWins && !whiteWins)
            gameOverString = "Its a draw!";

        if (_gameOverScreen == null)
        {
            _gameOverScreen = GameObject.Instantiate(GameOverPrefab);
            _gameOverScreen.GetComponent<cgGameOverScript>().initialize(gameOverString, ResetBoard, MainMenu);
        }
    }

    /// <summary>
    /// Check if the game is over, should be called after every move.
    /// </summary>
    private void _checkGameOver()
    {
        bool isChecked = _abstractBoard.isChecked(whiteTurnToMove);
        List<cgSimpleMove> responses = _abstractBoard.findLegalMoves(_abstractBoard.whiteTurnToMove);
        for (int i = responses.Count; i > 0; i--)
        {
            if (!_abstractBoard.verifyLegality(responses[i - 1]))
            {
                responses.RemoveAt(i - 1);
            }
        }

        if (responses.Count == 0 && isChecked)
        {
            //Checkmate.
            _gameOver(!whiteTurnToMove, whiteTurnToMove);
            if ((_humanPlayerIsBlack && whiteTurnToMove) || (_humanPlayerIsWhite && !whiteTurnToMove))
            {
                playSound(winSound);
            }
            else playSound(loseSound);

        }
        else if (isChecked)
        {
            //Checked but not checkmate.
            playSound(checkSound);
        }
        else if (responses.Count == 0 && !isChecked)
        {
            //Draw by stalemate - no legal moves available to player whose turn it is to move.
            _gameOver(false, false);
            playSound(loseSound);
        }
        else
        {
            //Test Draw by material insuffience

            //Collect living non-king pieces, to test draw by material insuffience
            bool blackBishopsEvenSquares = false;
            bool blackBishopsUnevenSquares = false;
            bool blackKnights = false;
            bool whiteBishopsEvenSquares = false;
            bool whiteBishopsUnevenSquares = false;
            bool whiteKnights = false;

            bool drawByMaterial = false;
            bool hasEnoughOtherMaterial = false;
            for (byte i = 0; i < _abstractBoard.squares.Count; i++)
            {
                sbyte sb = _abstractBoard.squares[i];
                if (sb != 0 && Math.Abs(sb) != 6)//Count everything but empty squares and king pieces.
                {
                    if (sb == 3)
                        whiteKnights = true;
                    else if (sb == 4)
                    {
                        if (i / 2 == 0)
                            whiteBishopsEvenSquares = true;
                        else
                            whiteBishopsUnevenSquares = true;
                    }
                    else if (sb == -3)
                        blackKnights = true;
                    else if (sb == 4)
                    {
                        if (i / 2 == 0)
                            blackBishopsEvenSquares = true;
                        else
                            blackBishopsUnevenSquares = true;
                    }
                    else
                    {//pawn, queen or rook is alive, no need to test further material insuffience
                        hasEnoughOtherMaterial = true;
                        break;
                    }
                }
            }

            if (!hasEnoughOtherMaterial)
            {
                //White bishop on even square only piece left and possibly other black bishops on even square(which doesnt matter.)
                if (whiteBishopsEvenSquares && !whiteBishopsUnevenSquares && !blackBishopsUnevenSquares && !blackKnights && !whiteKnights)
                    drawByMaterial = true;
                //White bishop on uneven square left and possibly other black bishops on uneven square(which doesnt matter.)
                else if (whiteBishopsUnevenSquares && !whiteBishopsEvenSquares && !blackBishopsEvenSquares && !blackKnights && !whiteKnights)
                    drawByMaterial = true;
                //black bishop on even square only piece left 
                else if (blackBishopsEvenSquares && !whiteBishopsEvenSquares && !whiteBishopsUnevenSquares && !blackBishopsUnevenSquares && !blackKnights && !whiteKnights)
                    drawByMaterial = true;
                //black bishop on uneven square only piece left.
                else if (blackBishopsUnevenSquares && !whiteBishopsEvenSquares && !whiteBishopsUnevenSquares && !blackBishopsEvenSquares && !blackKnights && !whiteKnights)
                    drawByMaterial = true;
                //black knight only piece left.
                else if (blackKnights && !whiteBishopsEvenSquares && !whiteBishopsUnevenSquares && !blackBishopsEvenSquares && !blackBishopsUnevenSquares && !whiteKnights)
                    drawByMaterial = true;
                //white knight only piece left.
                else if (whiteKnights && !blackKnights && !whiteBishopsUnevenSquares && !blackBishopsEvenSquares && !blackBishopsUnevenSquares && !blackKnights)
                    drawByMaterial = true;
            }
            if (drawByMaterial)
            {
                _gameOver(false, false);
                playSound(loseSound);
            }
        }
    }

    /// <summary>
    /// Peform the provided move on the visual board and the abstract board, with no legality checks - thus should be performed prior to calling this.
    /// </summary>
    /// <param name="move"></param>
    private void _makeMove(cgSimpleMove move)
    {
        UnityEngine.Debug.Log("White: " + _abstractBoard.whiteTurnToMove);
        movesMade++;
        playSound(moveSound);
        _abstractBoard.move(move);
        _writeLog(move);
        //_abstractBoard.debugReadBoard();
        if (_getPieceOn(_abstractBoard.SquareNames[move.to]) != null && !(move is cgCastlingMove))
        {
            _setDeadPiece(_getPieceOn(_abstractBoard.SquareNames[move.to]));
        }
        cgChessPieceScript piece = _getPieceOn(_abstractBoard.SquareNames[move.from]);
        if (move is cgCastlingMove)
        {
            cgChessPieceScript piece2 = _getPieceOn(_abstractBoard.SquareNames[(move as cgCastlingMove).secondFrom]);
            if (piece2) piece2.moveToSquare(_getSquare(_abstractBoard.SquareNames[(move as cgCastlingMove).secondTo]));
        }
        else if (move is cgEnPassantMove)
        {
            cgChessPieceScript piece2 = _getPieceOn(_abstractBoard.SquareNames[(move as cgEnPassantMove).attackingSquare]);
            piece2.dead = true;
        }
        piece.moveToSquare(_getSquare(_abstractBoard.SquareNames[move.to]));

        whiteTurnToMove = _abstractBoard.whiteTurnToMove;
        _checkGameOver();
        if (highlightLastMove)
        {
            //Color copyFrom = _getSquare(cgGlobal.SquareNames[move.to]).startColor;
            Color color = _getSquare(_abstractBoard.SquareNames[move.to]).recentMoveColor;

            _getSquare(_abstractBoard.SquareNames[move.to]).highlightTemporarily(color);
        }

        //Debug.Log("making move. " + _abstractBoard.whiteTurnToMove+" moves "+_abstractBoard.moves.Count);
        //UnityEngine.Debug.Log("Time elapsed: " + stopwatch.Elapsed);

        if (Mode == BoardMode.EngineVsEngine) MakeEngineMove(_abstractBoard.duplicate(), _abstractBoard.whiteTurnToMove, _engineCallback);
    }

    /// <summary>
    /// Called when the engine should generate a new move.
    /// </summary>
    /// <param name="board">The current board state.</param>
    /// <param name="moveAsWhiteP">Move as white(true) or black(false).</param>
    /// <param name="callback">Where the prefered move will be returned.</param>
    public void MakeEngineMove(cgBoard board, bool moveAsWhiteP, Action<List<cgSimpleMove>> callback)
    {
        // _engineCallbackFunctions = new List<Action<List<cgSimpleMove>>>();
        _engineCallbackFunctions.Add(callback);
        // UnityEngine.Debug.Log("Making engine move, with multithreading: " + useMultiThreading+" "+_engineCallbackFunctions.Count);
        if (useMultiThreading)
        {
            this._makeEngineMoveMulti(board, moveAsWhiteP, callback);
        }
        else {
            this._makeEngineMoveMono(board, moveAsWhiteP, callback);
        }
    }
    /// <summary>
    /// Called when the engine should generate a new move using a new thread(multi threaded)..
    /// </summary>
    /// <param name="board">The current board state.</param>
    /// <param name="moveAsWhiteP">Move as white(true) or black(false).</param>
    /// <param name="callback">Where the prefered move will be returned.</param>
    private void _makeEngineMoveMulti(cgBoard board, bool moveAsWhiteP, Action<List<cgSimpleMove>> callback) {
        if (_engineThread != null)
        {
            _engineThread.CancelAsync();
        }
        _engineThread = new BackgroundWorker();
        //The thread for the engine to make its computations to decide on a move using a new thread(multi threaded).
        Action<object, DoWorkEventArgs> _threadMakeMove = (object sender,
             DoWorkEventArgs e) =>
        {
            Action<List<cgSimpleMove>> completeCallback = (List<cgSimpleMove> moves) =>
            {
                //print("compelte callback: " + moves.Count);
                //System.Reflection.MethodBase.Invoke(new Action(()=> _threadCallback(),null);
                lock (_engineCallbackParams)
                {
                    _engineCallbackParams.Add(moves);
                }
            };
            Action<float> progessCallback = (float progress) =>
            {
                //UnityEngine.Debug.Log("Progress registered. " + progress);
                lock (_engineProgress)
                {
                    _engineProgress.Add(progress);
                }
            };
            _engine.makeMove(board, moveAsWhiteP, completeCallback, progessCallback);

        };
        _engineThread.DoWork += new DoWorkEventHandler(_threadMakeMove);
        //Thread _engineThread = new Thread(_threadMakeMove);
        _engineThread.RunWorkerCompleted += _engineThread_RunWorkerCompleted;
        _engineThread.ProgressChanged += _engineThread_ProgressChanged;
        _engineThread.WorkerSupportsCancellation = true;
        UnityEngine.Debug.Log("Starting thread.");

        //(board, moveAsWhiteP, callback
        _engineThread.RunWorkerAsync();
    }
    /// <summary>
    /// Called when the engine should generate a new move using corutine on a singlethread(mono threaded).
    /// </summary>
    /// <param name="board">The current board state.</param>
    /// <param name="moveAsWhiteP">Move as white(true) or black(false).</param>
    /// <param name="callback">Where the prefered move will be returned.</param>
    public void _makeEngineMoveMono(cgBoard board, bool moveAsWhiteP, Action<List<cgSimpleMove>> callback)
    {
        Action<List<cgSimpleMove>> completeCallback = (List<cgSimpleMove> moves) =>
        {
            //print("compelte callback: " + moves.Count);
            //System.Reflection.MethodBase.Invoke(new Action(()=> _threadCallback(),null);
            lock (_engineCallbackParams)
            {
                UnityEngine.Debug.Log("Callback and added moves: " + moves.Count);
                _engineCallbackParams.Add(moves);
            }
        };
        Action<float> progessCallback = (float progress) =>
        {
            //UnityEngine.Debug.Log("Progress registered. " + progress);
            lock (_engineProgress)
            {
                _engineProgress.Add(progress);
            }
        };
        _engine.makeMoveMono(this._startCoroutine, board, moveAsWhiteP, completeCallback, progessCallback);

    }
    private void _startCoroutine(IEnumerator ienum) {
        StartCoroutine(ienum);
    }
    private void OnApplicationQuit()
    {
        //_engineThread.CancelAsync();
    }
    private void _engineThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        Vector3 nscale = this.engineProgressBar.transform.localScale;
        nscale.x = e.ProgressPercentage;
        //UnityEngine.Debug.Log(" progress percentage: " + e.ProgressPercentage);
        //this.engineProgressBar.transform.localScale = new Vector3()
    }

    private void _engineThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        UnityEngine.Debug.Log("Worker thread done and done. " + e.Error.Message + " " + e.Error.StackTrace);

    }

    private cgSquareScript _getSquare(string p)
    {
        foreach (cgSquareScript square in getSquares()) if (square != null && square.uniqueName == p) return square;
        return null;
    }

    private cgChessPieceScript _getPieceOn(string p)
    {
        foreach (cgChessPieceScript cp in _livePieces) if (cp.square != null && cp.square.uniqueName == p) return cp;
        return null;
    }

    private void _setDeadPiece(cgChessPieceScript cp)
    {
        cp.dead = true;
        cp.square = null;
        if (!_deadPieces.Contains(cp))
        {
            //UnityEngine.Debug.Log("cp white: "+cp.white);

            if (cp.white)
            {
                cp.transform.position = whiteDeadPiecesLocation + new Vector3(0, (float)(_deadWhitePieces * .4), 0);
            }
            else if (!cp.white)
            {
                cp.transform.position = blackDeadPiecesLocation + new Vector3(0, (float)(_deadBlackPieces * .4), 0);
            }
            _deadPieces.Add(cp);
            if (cp.white) _deadWhitePieces++;
            else _deadBlackPieces++;
        }
    }

    /// <summary>
    /// Paste the game notation from clipboard onto the board.
    /// </summary>
    private void _pasteGameFromClipboard()
    {
        string curgame = GUIUtility.systemCopyBuffer;
        _abstractBoard = new cgBoard();
        UnityEngine.Debug.Log("Pasted game from clipboard: " + curgame);
        cgNotation notation = new cgNotation(_abstractBoard);
        notation.Read(curgame);

        _abstractBoard.LoadGame(notation);
        setBoardTo(_abstractBoard);
    }

    /// <summary>
    /// Copy game notation to clipboard, if for instance the user wants to save his current game.
    /// </summary>
    private void _copyGameToClipboard()
    {
        //Debug.Log("herp");
        string curgame;
        cgNotation notation = new cgNotation(_abstractBoard);
        curgame = notation.writeFullNotation(cgNotation.NotationType.Algebraic, cgNotation.FormatType.None);
        GUIUtility.systemCopyBuffer = curgame;
        //moveLog.text += "ctrl+c";
    }

    /// <summary>
    /// Set the board to the provided abstract board, write any moves provided in said abstract board to the log, etc.
    /// </summary>
    /// <param name="board"></param>
    public void setBoardTo(cgBoard board)
    {
        _abstractBoard = board;
        _livePieces = new List<cgChessPieceScript>();
        _deadPieces = new List<cgChessPieceScript>();
        _deadWhitePieces = 0;
        _deadBlackPieces = 0;
        movesMade = _abstractBoard.moves.Count;
        if (moveLog != null)
            moveLog.text = "Moves: \n";
        _loggedMoves = 0;
        foreach (cgSimpleMove move in board.moves) _writeLog(move);
        whiteTurnToMove = _abstractBoard.whiteTurnToMove;
        placePieces();
    }


    /// <summary>
    /// Write move to log.
    /// </summary>
    /// <param name="move"></param>
    private void _writeLog(cgSimpleMove move)
    {
        if (moveLog != null)
        {
            if (NotationType == cgNotation.NotationType.Coordinate)
            {
                if (_loggedMoves % 2 == 0) moveLog.text += "\n";
                else moveLog.text += " | ";
                moveLog.text += _abstractBoard.SquareNames[move.from] + "-" + _abstractBoard.SquareNames[move.to];
            }
            else if (NotationType == cgNotation.NotationType.Algebraic)
            {
                moveLog.text = "Moves:\n";
                cgNotation note = new cgNotation(_abstractBoard.duplicate());
                moveLog.text = note.getLogFriendlyNotation();
            }
            _loggedMoves++;
        }
    }



}

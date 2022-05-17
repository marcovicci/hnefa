using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// This script lets you create custom boards in the editor. You should consider saving the generated board as prefab and make whatever changes(moving stuff around etc.,
/// </summary>
[CustomEditor(typeof(cgBoardGeneratorScript))]
public class cgBoardGeneratorEditor : Editor
{

    private cgBoardGeneratorScript _generator;


    public override void OnInspectorGUI()
    {
        _generator = (cgBoardGeneratorScript)target;
#if UNITY_EDITOR
        if (GUILayout.Button("Generate", GUILayout.Height(40)))
        {
            //The code in this scope is executed when the Generate button is pressed.
            List<sbyte> placementsSbytes = new List<sbyte>();
            if (!_generator.customizePlacements)
            {
                placementsSbytes = cgCustomBoardSettings.GetPiecePlacements(_generator.boardWidth, _generator.boardHeight);
            }
            else
            {
                foreach (cgBoardGeneratorScript.Type t in _generator.piecePlacements)
                {
                    placementsSbytes.Add((sbyte)t);
                }
            }

            _generate(placementsSbytes);
        }
        byte preHeight = _generator.boardHeight;
        byte preWidth = _generator.boardWidth;
        _generator.boardHeight = (byte)EditorGUILayout.IntField("Height", _generator.boardHeight);
        _generator.boardWidth = (byte)EditorGUILayout.IntField("Width", _generator.boardWidth);
        _generator.squareStartPoint = EditorGUILayout.Vector3Field("Start point", _generator.squareStartPoint);
        _generator.squareScale = EditorGUILayout.Vector2Field("Scale", _generator.squareScale);
        _generator.squareSpacing = EditorGUILayout.Vector2Field("Spacing", _generator.squareSpacing);
        _generator.use3d = EditorGUILayout.Toggle("3d", _generator.use3d);

        _generator.whiteSquarePrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("White square"), _generator.whiteSquarePrefab, typeof(GameObject), true);
        _generator.blackSquarePrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Black square"), _generator.blackSquarePrefab, typeof(GameObject), true);
        _generator.piecePrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Piece"), _generator.piecePrefab, typeof(GameObject), true);
        _generator.customizePlacements = EditorGUILayout.Toggle("Customize Piece Placements", _generator.customizePlacements);

        if (preWidth != _generator.boardWidth || preHeight != _generator.boardHeight)
        {
            _initializePlacements();
            UnityEngine.Debug.Log("Generated custom placements.");
        }
        if (_generator.customizePlacements)
        {
            //The code in this scope is executed when customizePlacements is set to true.
            if (_generator.piecePlacements == null || _generator.piecePlacements.Length != (_generator.boardWidth * _generator.boardHeight))
                _initializePlacements();
            UnityEngine.Debug.Log("Generator new pieceplacements");
            for (int u = 0; u < _generator.boardHeight; u++)
            {
                EditorGUILayout.BeginHorizontal("Placements");
                for (int i = 0; i < _generator.boardWidth; i++)
                {
                    GUILayoutOption[] options = new GUILayoutOption[] {
                            GUILayout.MaxWidth(70)
                    };
                    _generator.piecePlacements[(u * _generator.boardWidth) + i] = (cgBoardGeneratorScript.Type)EditorGUILayout.EnumPopup("", _generator.piecePlacements[(u * _generator.boardWidth) + i], options);
                }

                EditorGUILayout.EndHorizontal();
            }


            if (GUILayout.Button("Wipe board", GUILayout.Height(40)))
            {
                for (int i = 0; i < _generator.piecePlacements.Length; i++)
                {
                    _generator.piecePlacements[i] = cgBoardGeneratorScript.Type.Empty;
                }

            }
        }

        if (GUI.changed) EditorUtility.SetDirty(target);
#endif
    }

    private void _initializePlacements()
    {

        List<sbyte> placementsSbytes = cgCustomBoardSettings.GetPiecePlacements(_generator.boardWidth, _generator.boardHeight);
        _generator.piecePlacements = new cgBoardGeneratorScript.Type[_generator.boardWidth * _generator.boardHeight];
        for (int i = 0; i < _generator.piecePlacements.Length; i++)
        {
            _generator.piecePlacements[i] = (cgBoardGeneratorScript.Type)placementsSbytes[i];
        }
    }

    private void _generate(List<sbyte> placements)
    {
        //Instantiate default cgBoard and cut out all pieces and squares, insert new ones as specified by these settings.
        GameObject boardObject = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath("Assets/ChessGame/prefabs/ChessBoardEmpty.prefab", typeof(GameObject)));
        boardObject.name = "ChessBoard " + _generator.boardWidth + "x" + _generator.boardHeight;
        PrefabUtility.UnpackPrefabInstance(boardObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        cgChessBoardScript board = boardObject.GetComponent<cgChessBoardScript>();
        _generator.boardScript = board;
        _generator.generate(placements);
    }
}

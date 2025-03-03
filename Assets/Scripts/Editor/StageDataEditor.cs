using System;
using System.Collections.Generic;
using TANK3.Data.StageManagement;
using TANK3.Data.StageObject;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageData))]
public class StageDataEditor : Editor
{
    private StageData stageData;

    private void OnEnable()
    {
        stageData = (StageData)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField($"GUID:{stageData.guid}");
        // コピー用ボタン
        if (GUILayout.Button("Copy GUID"))
        {
            EditorGUIUtility.systemCopyBuffer = stageData.guid;
        }
        DrawDefaultInspector();  // これで他のフィールド（sceneNameなど）はそのまま表示

        // stageHeight と stageWidth の変更を検出
        int newHeight = stageData.stageHeight;
        int newWidth = stageData.stageWidth;

        // サイズ変更時にデータを保持しつつ拡張
        ResizeStageDesc(ref stageData.stageDesc, newWidth, newHeight);
        ResizeEnemyTiles(ref stageData.enemyTiles, newWidth, newHeight);
        ResizeObstacleTiles(ref stageData.obstacleTiles, newWidth, newHeight);

        if (stageData.stageDesc != null && stageData.enemyTiles != null)
        {
            EditorGUILayout.LabelField("Stage Description (1D Array)");
            List<Tuple<int, int>> EnemyDataPos = new();
            List<Tuple<int, int>> ObstacleDataPos = new();

            // 1次元配列の編集を行う
            for (int y = 0; y < stageData.stageHeight; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < stageData.stageWidth; x++)
                {
                    // Indexの計算
                    int index = y * stageData.stageWidth + x;
                    stageData.stageDesc[index] = (TileType)EditorGUILayout.EnumPopup(stageData.stageDesc[index], GUILayout.Width(100));

                    if (stageData.stageDesc[index] == TileType.Enemy)
                    {
                        EnemyDataPos.Add(new Tuple<int, int>(x, y));
                    }
                    if (stageData.stageDesc[index] == TileType.Obstacle)
                    {
                        ObstacleDataPos.Add(new Tuple<int, int>(x, y));
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.LabelField("Enemy Description");
            foreach (var pos in EnemyDataPos)
            {
                int x = pos.Item1;
                int y = pos.Item2;
                EditorGUILayout.LabelField($"Position:({x},{y})");

                int index = y * stageData.stageWidth + x;
                if (stageData.enemyTiles[index] == null)
                {
                    stageData.enemyTiles[index] = new EnemyTileData();
                }

                EditorGUI.BeginChangeCheck();
                stageData.enemyTiles[index].enemyData = (Enemy)EditorGUILayout.ObjectField("Enemy Data", stageData.enemyTiles[index].enemyData, typeof(Enemy), false);
                stageData.enemyTiles[index].rotation = EditorGUILayout.FloatField("Enemy Rotation", stageData.enemyTiles[index].rotation);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(stageData);
                }
            }

            EditorGUILayout.LabelField("Obstacle Description");
            foreach (var pos in ObstacleDataPos)
            {
                int x = pos.Item1;
                int y = pos.Item2;
                EditorGUILayout.LabelField($"Position:({x},{y})");

                int index = y * stageData.stageWidth + x;
                if (stageData.obstacleTiles[index] == null)
                {
                    stageData.obstacleTiles[index] = new ObstacleTileData();
                }

                EditorGUI.BeginChangeCheck();
                stageData.obstacleTiles[index].obstacleData = (Obstacle)EditorGUILayout.ObjectField("Obstacle Data", stageData.obstacleTiles[index].obstacleData, typeof(Obstacle), false);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(stageData);
                }
            }
        }

        // sceneData.stageDescがnullの場合、初期化するボタンを表示
        if (stageData.stageDesc == null)
        {
            if (GUILayout.Button("Initialize StageDesc"))
            {
                stageData.stageDesc = new List<TileType>(new TileType[stageData.stageWidth * stageData.stageHeight]);
            }
        }

        // GUI更新
        if (GUI.changed)
        {
            EditorUtility.SetDirty(stageData);
        }
    }

    private void ResizeStageDesc(ref List<TileType> list, int newWidth, int newHeight)
    {
        int newSize = newWidth * newHeight;
        if (list == null || list.Count != newSize)
        {
            list = new List<TileType>(new TileType[newSize]);
        }
    }

    private void ResizeEnemyTiles(ref List<EnemyTileData> list, int newWidth, int newHeight)
    {
        int newSize = newWidth * newHeight;
        if (list == null || list.Count != newSize)
        {
            list = new List<EnemyTileData>(new EnemyTileData[newSize]);
        }
    }

    private void ResizeObstacleTiles(ref List<ObstacleTileData> list, int newWidth, int newHeight)
    {
        int newSize = newWidth * newHeight;
        if (list == null || list.Count != newSize)
        {
            list = new List<ObstacleTileData>(new ObstacleTileData[newSize]);
        }
    }
}

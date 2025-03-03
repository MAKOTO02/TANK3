using UnityEngine;
using TANK3.Data.StageObject;
using System.Collections.Generic;

namespace TANK3.Data.StageManagement
{
    [CreateAssetMenu(fileName ="NewStageData", menuName ="Data/Stage/StageData", order =2)]
    public class StageData:ScriptableObject
    {
        [HideInInspector]
        public string guid;
        public string stageName;
        public int stageHeight;
        public int stageWidth;
        public int enemyCount;
        public List<TileType> stageDesc;
        public List<EnemyTileData> enemyTiles;
        public List<ObstacleTileData> obstacleTiles;

        public void OnEnable()
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = System.Guid.NewGuid().ToString();
            }
        }
    }

    public enum TileType
    {
        Empty,       // 空
        Obstacle,        // 壁
        Enemy,       // 敵
        Player,      // プレイヤー
    }

    [System.Serializable]
    public class EnemyTileData
    {
        public Enemy enemyData;  // 敵のデータ
        public float rotation;       // 向き（回転）
    }

    [System.Serializable]
    public class ObstacleTileData
    {
        public Obstacle obstacleData;
    }

}
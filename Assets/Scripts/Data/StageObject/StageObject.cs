using UnityEngine;

namespace TANK3.Data.StageObject
{
    public abstract class StageObject : ScriptableObject
    {
        public string guid;            // ユニークなID
        public string objectName;
        public GameObject GameObjectPrefab;

        // 共通の処理をここに定義できます
        public abstract void Interact(); // 例えば、敵は攻撃、障害物はプレイヤーと衝突など

        void OnEnable()
        {
            if(string.IsNullOrEmpty(guid))
            {
                guid = System.Guid.NewGuid().ToString();
            }
        }
    }
}
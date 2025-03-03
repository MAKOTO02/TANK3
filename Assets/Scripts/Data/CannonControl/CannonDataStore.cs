using System.Collections.Generic;
using UnityEngine;
namespace TANK3.Data.CannonDataManagement 
{
    [CreateAssetMenu(fileName = "NewCannonDataStore", menuName = "Data/Cannon/CannonDataStore", order = 1)]
    public class CannonDataStore : ScriptableObject
    {
        public Dictionary<string, CannonData> CannonDataDictionary = new();
        public List<CannonData> dataList = new();

        void OnEnable()
        {
            CannonDataDictionary.Clear();
            foreach (var data in dataList)
            {
                data.OnEnable();
                CannonDataDictionary[data.guid] = data;
            }
        }

        public void OnDestroy()
        {
            Debug.Log("CannonDataStore: OnDestroy");
        }
    }

    [System.Serializable]
    public class CannonData
    {
        [HideInInspector]
        public string guid;
        public CannonTurnType turnType;
        public float CannonTurnSpeed = 2.0f;

        public void OnEnable()
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = System.Guid.NewGuid().ToString();
            }
        }
    }
}
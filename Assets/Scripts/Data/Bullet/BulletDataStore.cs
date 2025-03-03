using System;
using System.Collections.Generic;
using UnityEngine;

namespace TANK3.Data.BulletManagement
{
    [CreateAssetMenu(fileName = "NewBulletDataStore", menuName = "Data/Bullet/BulletDataStore", order =1)]
    public class BulletDataStore : ScriptableObject
    {
        public Dictionary<string, BulletData> BulletDataDictionary = new();
        public List<BulletData> dataList = new();

        void OnEnable()
        {
            BulletDataDictionary.Clear();
            foreach (var data in dataList)
            {
                data.OnEnable();
                BulletDataDictionary.Add(data.guid, data);
            }
        }

        public void OnDestroy()
        {
            Debug.Log("BulletDataStore: OnDestroy");
        }
    }

    [System.Serializable]
    public class BulletData
    {
        [HideInInspector]
        public string guid;
        public string BulletType;
        public Trajectory Trajectory;
        public int limit;    // ê‚É‘¶İ‚Å‚«‚é©‹@‚Ì’e‚Ì”‚ğ‚±‚±‚ÉŠi”[.
        public float bulletSpeed;  // ’e‚Ì‰‘¬‚ğ§Œä‚·‚é•Ï”.
        public int durationTimes; // ’e‚Ì”½Ë‰ñ”‚ÌãŒÀ.
        public float bulletScale;

        public void OnEnable()
        {
            if (guid == null)
            {
                guid = System.Guid.NewGuid().ToString();
            }
        }
    }
}
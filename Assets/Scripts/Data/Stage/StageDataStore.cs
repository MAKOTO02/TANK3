using System.Collections.Generic;
using UnityEngine;

namespace TANK3.Data.StageManagement
{
    [CreateAssetMenu(fileName = "NewStageDataStore", menuName = "Data/Stage/StageDataStore", order = 1)]
    public class StageDataStore : ScriptableObject
    {
        public Dictionary<string, StageData> sceneDataDictionary;
        [HideInInspector]
        public List<StageData> dataList;

        private void OnEnable()
        {
            if (dataList == null)
            {
                dataList = new ();
            }
            if (sceneDataDictionary == null)
            {
                sceneDataDictionary = new ();
            }
            sceneDataDictionary.Clear();
            foreach (var data in dataList)
            {
                data.OnEnable();
                sceneDataDictionary[data.guid] = data;
            }
        }

        public void OnDestroy()
        {
            Debug.Log("SceneDataStore: OnDestroy");
        }
    }
}

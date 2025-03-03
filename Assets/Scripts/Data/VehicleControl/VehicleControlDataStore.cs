using System.Collections.Generic;
using UnityEngine;

namespace TANK3.Data.VehicleManagement
{
    [CreateAssetMenu(fileName = "NewVehicleControlDataStore", menuName = "Data/VehicleControl/VehicleControlDataStore", order = 1)]
    public class VehicleControlDataStore : ScriptableObject
    {
        public Dictionary<string, VehicleContorolData> VehicleControlDataDictionary = new();
        public List<VehicleContorolData> dataList = new();

        void OnEnable()
        {
            VehicleControlDataDictionary.Clear();
            foreach (var data in dataList)
            {
                data.OnEnable();
                VehicleControlDataDictionary[data.guid] = data;
            }
        }

        public void OnDestroy()
        {
            Debug.Log("VehicleControlDataStore: OnDestroy");
        }
    }

    [System.Serializable]
    public class VehicleContorolData
    {
        [HideInInspector]
        public string guid;
        public VehicleContorlType type;
        public float MoveForwardSpeedLimit = 20.0f;
        public float Accel = 800.0f;
        public float VehicleTurnSpeed = 80.0f;

        public void OnEnable()
        {
            if (guid == null)
            {
                guid = System.Guid.NewGuid().ToString();
            }
        }
    }
}


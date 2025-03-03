using System;
using System.Collections.Generic;

namespace TANK3.Data.Save
{
    [Serializable]
    public class SaveData
    {
        public List<string> clearedStageIds;
        public string lastPlayedStageId;
        public List<string> obtainedBulletIds;
        public List<string> obtainedVehicleControlIds;
        public List<string> obtainedCannonTypeIds;

        // デフォルトコンストラクタ（新規作成時のため）
        public SaveData()
        {
            clearedStageIds = new List<string>();
            lastPlayedStageId = string.Empty;
            obtainedBulletIds = new List<string>();
            obtainedVehicleControlIds = new List<string>();
            obtainedCannonTypeIds = new List<string>();
        }

        // 既存データの復元用コンストラクタ
        public SaveData(List<string> clearedStageIds, string lastPlayedStageId,
            List<string> obtainedBulletIds, List<string> obtainedVehicleControlIds,
            List<string> obtainedCannonTypeIds)
        {
            this.clearedStageIds = clearedStageIds ?? new List<string>();
            this.lastPlayedStageId = lastPlayedStageId ?? string.Empty;
            this.obtainedBulletIds = obtainedBulletIds ?? new List<string>();
            this.obtainedVehicleControlIds = obtainedVehicleControlIds ?? new List<string>();
            this.obtainedCannonTypeIds = obtainedCannonTypeIds ?? new List<string>();
        }


        // セーブデータをリセットするメソッド
        public void Clear()
        {
            clearedStageIds.Clear();
            lastPlayedStageId = string.Empty;
            obtainedBulletIds.Clear();
            obtainedVehicleControlIds.Clear();
            obtainedCannonTypeIds.Clear();
        }
    }
}

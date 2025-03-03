using System.Collections;
using System.Collections.Generic;
using System.IO;
using TANK3.Data.Save;
using Unity.VisualScripting;
using UnityEngine;

//「最後に使ったスロット番号」を記録して、ロード時にそのスロットを自動選択する（PlayerPrefs を使うと簡単！）.
//セーブデータのバージョン管理やバックアップ機能を追加.
//セーブ時に「確認ダイアログ」を表示するUIを追加.
namespace TANK3.Manager.Save
{
    public class SaveManager : Singleton<SaveManager>
    {
        // GameManagerクラスから更新する.
        public SaveData currentSaveData;    // ロードや新規作成で取得.
        public HashSet<string> clearedStageIds = new();
        public string lastPlayedStageId = "";
        public HashSet<string> obtainedBulletIds = new();
        public HashSet<string> obtainedVehicleControlIds = new();
        public HashSet<string> obtainedCannonTypeIds = new();

        public SaveData UpdateCurrentSaveData()
        {
            SaveData updated = Instance.ConvertToList(clearedStageIds, lastPlayedStageId, obtainedBulletIds, obtainedVehicleControlIds, obtainedCannonTypeIds);
            return updated;
        }

        private string GetSavePath(int slotNum)
        {
            return $"{Application.persistentDataPath}/save_{slotNum}.json";
        }

        public void SaveGame(SaveData data, int slotNum)
        {
            try
            {
                string savePath = GetSavePath(slotNum);
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(savePath, json);
                Debug.Log("ゲームデータを保存しました: " + savePath);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("セーブ中にエラーが発生しました: " + ex.Message);
            }
        }

        public SaveData LoadGame(int slotNum)
        {
            string SavePath = Instance.GetSavePath(slotNum);
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                Debug.Log("ゲームデータを読み込みました: " + SavePath);
                return data;
            }
            else
            {
                Debug.LogWarning("セーブデータが見つかりません。");
                return null;
            }
        }

        public bool HasSaveData(int slotNum)
        {
            string SavePath = Instance.GetSavePath(slotNum);
            return File.Exists(SavePath);
        }

        public void DeleteSave(int slotNum)
        {
            string SavePath = Instance.GetSavePath(slotNum);
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("セーブデータを削除しました。");
            }
        }

        public bool TryGetCurrentSaveData(out SaveData saveData)
        {
            saveData = UpdateCurrentSaveData();
            return saveData != null;
        }

        public SaveData ConvertToList(HashSet<string> clearedStageIds, string lastPlayedStageId,
            HashSet<string> obtainedBulletIds, HashSet<string> obtainedVehicleControlIds,
            HashSet<string> obtainedCannonTypeIds)
        {
            // HashSet が null でなければ List に変換、null なら空の List を生成
            List<string> clearedList = clearedStageIds != null ? new List<string>(clearedStageIds) : new List<string>();
            List<string> bulletList = obtainedBulletIds != null ? new List<string>(obtainedBulletIds) : new List<string>();
            List<string> vehicleList = obtainedVehicleControlIds != null ? new List<string>(obtainedVehicleControlIds) : new List<string>();
            List<string> cannonList = obtainedCannonTypeIds != null ? new List<string>(obtainedCannonTypeIds) : new List<string>();

            // lastPlayedStageId が null なら空文字を代入
            string lastPlayed = lastPlayedStageId ?? string.Empty;

            // 新しい SaveData を生成して返す
            return new SaveData(clearedList, lastPlayed, bulletList, vehicleList, cannonList);
        }
    }
}
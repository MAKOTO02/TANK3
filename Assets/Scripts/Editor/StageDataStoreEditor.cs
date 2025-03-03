using UnityEditor;
using TANK3.Data.StageManagement;
using UnityEngine;

[CustomEditor(typeof(StageDataStore))]
public class StageDataStoreEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StageDataStore StageDataStore = (StageDataStore)target;

        foreach(var data in StageDataStore.dataList)
        {
            EditorGUILayout.LabelField("Stage Name:", data.stageName);
            EditorGUILayout.LabelField("GUID:", data.guid);

            // コピー用ボタン
            if (GUILayout.Button("Copy GUID"))
            {
                EditorGUIUtility.systemCopyBuffer = data.guid;
            }
        }

        if (GUILayout.Button("Add New StageData"))
        {
            // 新しいSceneDataのインスタンスを作成してリストに追加
            StageData newSceneData = ScriptableObject.CreateInstance<StageData>();
            AssetDatabase.CreateAsset(newSceneData, $"Assets/Data/StageData/NewStageData_{newSceneData.guid}.asset");
            AssetDatabase.SaveAssets();

            // 追加されたSceneDataアセットをdataListに反映
            StageDataStore.dataList.Add(newSceneData);
            EditorUtility.SetDirty(StageDataStore);
        }

        // シリアライズされたオブジェクトを保存
        serializedObject.ApplyModifiedProperties();

        DrawDefaultInspector();
    }
}

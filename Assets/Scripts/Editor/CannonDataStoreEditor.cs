using UnityEditor;
using TANK3.Data.CannonDataManagement;
using UnityEngine;

[CustomEditor(typeof(CannonDataStore))]
public class CannonDataStoreEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CannonDataStore cannonDataStore = (CannonDataStore)target;

        foreach(var data in cannonDataStore.dataList)
        {
            EditorGUILayout.LabelField("Cannon Turn Type:", data.turnType.ToString());
            EditorGUILayout.LabelField("GUID:", data.guid);


            // コピー用ボタン
            if (GUILayout.Button("Copy GUID"))
            {
                EditorGUIUtility.systemCopyBuffer = data.guid;
            }
        }

        DrawDefaultInspector();
    }
}

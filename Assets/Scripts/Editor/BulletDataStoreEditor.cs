using TANK3.Data.BulletManagement;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(BulletDataStore))]
public class BulletDataStoreEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BulletDataStore bulletDataStore = (BulletDataStore)target;

        foreach (var data in bulletDataStore.dataList)
        {
            EditorGUILayout.LabelField("Bullet Name:", data.BulletType);
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

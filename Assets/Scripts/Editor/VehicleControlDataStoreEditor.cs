using System.Collections;
using System.Collections.Generic;
using TANK3.Data.BulletManagement;
using TANK3.Data.VehicleManagement;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VehicleControlDataStore))]
public class VehicleControlDataStoreEditor : Editor
{
    public override void OnInspectorGUI()
    {
        VehicleControlDataStore VehicleControlDataStore = (VehicleControlDataStore)target;

        foreach (var data in VehicleControlDataStore.dataList)
        {
            EditorGUILayout.LabelField("VehicleContorlType :", data.type.ToString());
            EditorGUILayout.LabelField("GUID:", data.guid);

            // コピー用ボタン
            if (GUILayout.Button("Copy GUID"))
            {
                EditorGUIUtility.systemCopyBuffer = data.guid.ToString();
            }
        }

        DrawDefaultInspector();
    }
}

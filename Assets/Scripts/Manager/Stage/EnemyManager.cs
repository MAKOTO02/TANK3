using System.Collections;
using System.Collections.Generic;
using TANK3.Data.StageManagement;
using TANK3.Manager.Scene;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    private int currentEnemies;
    public int totalEnemies;
    public StageData stageData;

    public void Initialize()
    {
        currentEnemies = totalEnemies;
    }

    public void OnEnemyDestroyed()
    {
        currentEnemies--;
        if(currentEnemies <= 0)
        {
            if (stageData = null) Debug.Log("SceneData‚Ì“o˜^‚ÉŽ¸”s‚µ‚Ä‚¢‚Ü‚·B");
            if (GameManager.Instance == null) Debug.Log("GameMangager‚ª”­Œ©‚Å‚«‚Ü‚¹‚ñ.");
            stageData = SceneLoadManager.Instance.StageDataStore.dataList[2];
            GameManager.Instance.StageClear(stageData);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TANK3.Data.Save;
using TANK3.Data.StageManagement;
using TANK3.Manager.Scene;
using TANK3.Manager.UI;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    bool gameStarted;
    public bool GameStarted
    {
        get => gameStarted;
        set
        {
            if (gameStarted == value) return;

            gameStarted = value;
            OnGameStartedChanged?.Invoke(gameStarted);
            UpdateActiveState();
            Debug.Log($"invoked OnGameStartedChanged{gameStarted}");
        }
    }
    bool gamePaused;
    public bool GamePaused
    {
        get => gamePaused;
        set
        {
            if (gamePaused == value) return; // 変化がなければ何もしない

            gamePaused = value;
            OnGamePauseChanged?.Invoke(gamePaused); // 変更時にイベント発火
            UpdateActiveState();
            Debug.Log($"invoked OnGamePauseChanged{gamePaused}");
        }
    }
    

    public event Action<bool> OnGamePauseChanged = delegate {};
    public event Action<bool> OnGameStartedChanged = delegate {};
    public event Action<bool> OnActiveStateChanged = delegate { };
    public event Action<bool> StageCleared = delegate { };   // Stageがクリアされたら invokeを発火. 

    protected override void Awake()
    {
        base.Awake();
        if(!Initialize())
        {
            // どうしよう.
            // タイトルメニューに戻る.
        }
    }

    protected override void Start()
    {
        base.Start();
        SoundManager.Play("march");
    }


    bool Initialize()
    {
        GameStarted = false;
        GamePaused = false;
        // さらに処理を追加.
        return true;
    }

    public void ExitGame()
    {
        // ゲームの終了処理.
    }

    public void StartGame()
    {
        Instance.GameStarted = true;
        Instance.GamePaused = false;
        Debug.Log("GameStart!");
    }

    public void GameOver(StageData stageData)
    {
        Debug.Log($"GameOver: {stageData.stageName}");
        Instance.GameStarted = false;
        Instance.GamePaused = false;
        ExitStage();
    }

    public void StageClear(StageData stageData)
    {
        if (stageData == null) Debug.Log("stageData　が null です.");
        Debug.Log($"StageClear: {stageData.stageName}");
        Instance.GameStarted = false;
        Instance.GamePaused = false;
        Instance.ExitStage() ;
    }

    void ExitStage()
    {
        Instance.StartCoroutine(UIManager.Instance.ShowUI(UIState.ResultsScreen,true,false,false));
        SceneLoadManager.Instance.LoadAsync(SceneLoadManager.Instance.BackSceneName, SceneLoadManager.Instance.StageDataStore.dataList[2]); // EntryPointをロード.
    }

    private void UpdateActiveState()
    {
        bool isActive = GameStarted && !GamePaused;
        OnActiveStateChanged?.Invoke(isActive);
    }

    protected override void OnDestroy()
    {

        base.OnDestroy();
    }

    protected override void OnAwake()
    {
        string name = "GmaeManager";
        InitializeSingletonObject(name);
    }
}

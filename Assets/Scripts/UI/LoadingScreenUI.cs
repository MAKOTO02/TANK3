using System.Collections;
using TANK3.Data.StageManagement;
using TANK3.Manager.Scene;
using TANK3.Manager.UI;
using UnityEngine;
namespace TANK3.UI
{
    public class LoadingScreenUI : Singleton<LoadingScreenUI>
    {
        public string sceneName;
        public StageData stageData;
        // 初期化済みフラグを追加
        public bool isInitialized = false;
        bool hasGeneratedStage = false;

        private void OnEnable()
        {
            SceneLoadManager.Instance.OnSceneLoaded += InitializeScene;
        }
        public void OnLoadingScreen()
        {
            Instance.hasGeneratedStage = false;
            UIManager.Instance.StartCoroutine(WaitForInitializationAndLoad());
        }

        void InitializeScene(string sceneName)
        {
            // シーンのロードを待つ.
            Debug.Log($"LoadingScreenUI:Scene{sceneName}のロードが完了しました");

            // すでに生成済みなら何もしない
            if (Instance.hasGeneratedStage)
            {
                Debug.Log("Stage already generated. Skipping generation.");
                return;
            }
            StageGenerator.Instance.Generate(stageData);
            Instance.hasGeneratedStage = true;
            UIManager.Instance.StartCoroutine(WaitAndShowUI());
        }

        protected override void OnDestroy()
        {
            if (SceneLoadManager.Instance != null) SceneLoadManager.Instance.OnSceneLoaded -= InitializeScene;
            base.OnDestroy();
        }

        private IEnumerator WaitAndShowUI()
        {
            yield return new WaitForSeconds(2f); // 2秒待機
            yield return UIManager.Instance.StartCoroutine(UIManager.Instance.ShowUI(UIState.InGame, true, false, false));
            GameManager.Instance.StartGame();
        }

        private IEnumerator WaitForInitializationAndLoad()
        {
            // 初期化が完了していない場合、毎フレーム待機する
            while (!Instance.isInitialized)
            {
                Debug.Log("LoadingScreenUI:待機中");
                yield return null;
            }

            Debug.Log($"SceneName={Instance.sceneName}:sceneData={Instance.stageData}");
            // 初期化完了後、シーンロード処理を実行する
            SceneLoadManager.Instance.LoadAsync(Instance.sceneName, Instance.stageData);
        }
    }
}

using System.Collections;
using TANK3.UI.View;
using TANK3.Data.StageManagement;
using TANK3.Manager.UI;
using TANK3.Manager.Scene;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace TANK3.UI.Presenter
{
    public class StageSelectMenuPresenter : Singleton<StageSelectMenuPresenter>
    {
        private StageSelectMenuView view;  // Inspectorで設定

        // 保存用に、押されたボタンの参照を保持
        private Button clickedButton;

        protected override void Awake()
        {
            base.Awake();
            view = StageSelectMenuView.Instance;
        }

        private void OnEnable()
        {
            view.OnStageButtonClicked += OnStageButtonClicked;
        }

        private void OnDisable()
        {
            view.OnStageButtonClicked -= OnStageButtonClicked;
        }

        private void OnStageButtonClicked(int stageIndex)
        {
            if (EventSystem.current == null)
            {
                Debug.LogError("EventSystem がシーンに存在しません。");
                return;
            }

            GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
            if (selectedObj == null)
            {
                Debug.LogError("現在選択されている GameObject がありません。");
                return;
            }

            Debug.Log(selectedObj.gameObject);
            Instance.clickedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            view.SetAllButtonsInteractable(false);

            StageData stageData = SceneLoadManager.Instance.StageDataStore.dataList[stageIndex];
            string sceneName = SceneLoadManager.Instance.FrontSceneName;

            EnemyManager.Instance.stageData = stageData;
            UIManager.Instance.StartCoroutine(WaitUITransition(sceneName, stageData));
        }

        private IEnumerator WaitUITransition(string sceneName, StageData stageData)
        {
            yield return UIManager.Instance.StartCoroutine(UIManager.Instance.ShowUI(UIState.LoadingScreen, false, false, false));
            // UIManager の LoadingScreen UI がアクティブになるまで待機
            while (LoadingScreenUI.Instance == null || !LoadingScreenUI.Instance.gameObject.activeInHierarchy)
            {
                yield return null;
            }
            // LoadingScreenUI にステージデータとシーン名を設定
            LoadingScreenUI.Instance.stageData = stageData;
            LoadingScreenUI.Instance.sceneName = sceneName;
            // 初期化完了フラグを立てる
            LoadingScreenUI.Instance.isInitialized = true;
            // シーン遷移が開始されたら、再び全ボタンを有効化（またはUIManager側で管理する）
            view.SetAllButtonsInteractable(true);
        }

        void SetActiveEventSystem()
        {
            var eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null)
            {
                EventSystem.current = eventSystem;
                Debug.Log("EventSystem set to: " + eventSystem.gameObject.name);
            }
            else
            {
                Debug.LogWarning("No EventSystem found in the scene.");
            }
        }

        public void OnStageSelectMenu()
        {
            SetActiveEventSystem();
        }

    }
}
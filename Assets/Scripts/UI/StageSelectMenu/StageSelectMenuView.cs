using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEditor.Build.Content;
using TANK3.Manager.Scene;

namespace TANK3.UI.View
{
    public class StageSelectMenuView : Singleton<StageSelectMenuView>
    {
        [SerializeField] private GameObject buttonPrefab;   // プレハブをInspectorで設定
        [SerializeField] private Transform gridParent;        // GridLayoutGroupの親
        private int stageCount;
        public Button[] StageButtons { get; private set; }
        public event Action<int> OnStageButtonClicked;  // どのステージボタンが押されたかをPresenterへ通知

        protected override void Awake()
        {
            base.Awake();
            stageCount = SceneLoadManager.Instance.StageDataStore.dataList.Count - 1;
            CreateStageButtons();
        }

        private void CreateStageButtons()
        {
            StageButtons = new Button[stageCount];
            for (int i = 1; i <= stageCount; i++)
            {
                int stageIndex = i;  // ラムダキャプチャ用
                GameObject newButton = Instantiate(buttonPrefab, gridParent);
                TMP_Text tmpText = newButton.GetComponentInChildren<TMP_Text>();
                if (tmpText != null)
                {
                    tmpText.text = $"Stage {stageIndex}";
                }
                Button btn = newButton.GetComponent<Button>();
                StageButtons[i-1] = btn;
                // ボタンがクリックされたら、Presenterに通知する
                btn.onClick.AddListener(() =>
                {
                    OnStageButtonClicked?.Invoke(stageIndex);
                });
            }
        }

        // 全てのボタンの interactable 状態を一括で制御する
        public void SetAllButtonsInteractable(bool interactable)
        {
            if (StageButtons != null)
            {
                foreach (var btn in StageButtons)
                {
                    btn.interactable = interactable;
                }
            }
        }
    }
}

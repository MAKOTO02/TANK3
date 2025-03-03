using System.Collections;
using TANK3.Data.Save;
using TANK3.Manager.UI;
using UnityEngine;
using UnityEngine.UI;
using TANK3.Manager.Save;
using UnityEngine.EventSystems;

namespace TANK3.UI
{
    public class SaveDataSelectMenuUI : Singleton<SaveDataSelectMenuUI>
    {
        public SaveDataSelectMode mode; // 外部から渡す.
        [SerializeField] int saveSlotNum = 3;
        [SerializeField] float timeout = 2.0f;
        public Button clickedButton;
        Button[] saveSlotButtons;

        protected override void Awake()
        {
            base.Awake();
            saveSlotButtons = GetComponentsInChildren<Button>();
        }

        public void SaveDataClicked(int slotNum)
        {
            // ボタンを無効化
            clickedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            DisableAllButtons();

            if (slotNum < 0 || slotNum >= saveSlotNum)
            {
                Debug.LogError("不正なスロット番号が渡されました: " + slotNum);
                EnableAllButtons();
                return;
            }
            switch (mode)
            {
                case SaveDataSelectMode.Save:
                    Save(slotNum);
                    StartCoroutine(WaitThenEnableButtons("セーブしました！"));
                    break;
                case SaveDataSelectMode.Create:
                    if(CreateSaveData(slotNum))
                    {
                        UIManager.Instance.StartCoroutine(WaitThenShowUI(UIState.ModeSelectMenu, "新しいセーブデータを作成しました！"));
                    }
                    else
                    {
                        // メッセージは必要？
                    }
                    break;
                case SaveDataSelectMode.Load:
                    if(LoadSaveData(slotNum))
                    {
                        UIManager.Instance.StartCoroutine(WaitThenShowUI(UIState.ModeSelectMenu, "ロードしました！"));
                    }
                    else
                    {
                        StartCoroutine(WaitReaction("セーブデータのロードに失敗しました."));
                    }
                    break;
                default:
                    Debug.LogError("SaveDataSelectMenuUI: SaveDataSelectModeが渡されていません.");
                    break;
            }
        }

        void Save(int slotNum)
        {
            if(SaveManager.Instance.TryGetCurrentSaveData(out var saveData))
            {
                SaveManager.Instance.SaveGame(saveData, slotNum);
            }
            else
            {
                Debug.LogError("セーブデータの作成に失敗しました.");
            }
        }

        bool LoadSaveData(int slotNum)
        {
            // Load処理.
            SaveData loadedData = SaveManager.Instance.LoadGame(slotNum);
            if (loadedData != null)
            {
                Debug.Log($"スロット {slotNum + 1} のデータをロードしました。");
                SaveManager.Instance.currentSaveData = loadedData;
                return true;
            }
            else
            {
                Debug.LogWarning($"スロット {slotNum + 1} にセーブデータがありません。");
                return false;
            }
        }

        bool CreateSaveData(int slotNum)
        {
            if(SaveManager.Instance.HasSaveData(slotNum))
            {
                // 確認メッセージ(確認用のUIを表示?).
                // NO → return false;
                // Yes →　次の処理.
            }

            // SaveDataを作成.
            SaveData newSaveData = new SaveData();
            SaveManager.Instance.SaveGame(newSaveData, slotNum);
            Debug.Log($"スロット {slotNum} に新しいセーブデータを作成しました。");
            return true;
        }

        IEnumerator WaitReaction(string msg)
        {
            float startTime = Time.time;
            // キー入力を待つ.
            UIManager.Instance.ShowMessage(msg); // 後で実装.
            yield return new WaitUntil(() => Input.anyKeyDown || (Time.time - startTime > timeout));
            UIManager.Instance.HideMessage();
        }

        IEnumerator WaitThenEnableButtons(string msg)
        {
            yield return StartCoroutine(WaitReaction(msg));
            EnableAllButtons();
        }

        IEnumerator WaitThenShowUI(UIState targetState, string msg)
        {
            yield return WaitReaction(msg);
            yield return UIManager.Instance.StartCoroutine(UIManager.Instance.ShowUI(targetState));
        }

        void DisableAllButtons()
        {
            if (Instance.saveSlotButtons != null)
            {
                foreach (var button in Instance.saveSlotButtons)
                {
                    button.interactable = false;
                }
            }
        }

        void EnableAllButtons()
        {
            if (Instance.saveSlotButtons != null)
            {
                foreach (var button in Instance.saveSlotButtons)
                {
                    button.interactable = true;
                }
            }
        }

        public void OnSaveSelectMenu()
        {
            EnableAllButtons();
        }
    }

    public enum SaveDataSelectMode
    {
        Save,
        Load,
        Create
    }
}
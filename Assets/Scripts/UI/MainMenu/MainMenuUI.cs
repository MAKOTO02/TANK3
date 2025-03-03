using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TANK3.Manager.UI;
using System.Collections;

namespace TANK3.UI.Model
{
    public class MainMenuUI : Singleton<MainMenuUI>
    {
        Button firstButton;
        List<GameObject> children;

        public Sprite selectedSprite;
        public Sprite deselectedSprite;

        private void OnEnable()
        {
            firstButton = GetComponentInChildren<Button>();
            SetActiveEventSystem();
            if (firstButton != null) EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
            firstButton.gameObject.GetComponent<Image>().sprite = selectedSprite;
        }
        protected override void Start()
        {
            // 子のオブジェクトを取得.
            children = GetComponentsInChildren<Transform>(true)
                                                .Select(t => t.gameObject)
                                                .Where(go => go != gameObject)
                                                .ToList();
            foreach (var child in children)
            {
                if (child.TryGetComponent<ButtonManager>(out var button))
                {
                    child.GetComponent<Image>().sprite = deselectedSprite;
                    button.selectedSprite = selectedSprite;
                    button.deselectedSprite = deselectedSprite;
                }
            }
            firstButton.gameObject.GetComponent<Image>().sprite = selectedSprite;
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

        public void ContinueButtonClicked()
        {
            UIManager.Instance.StartCoroutine(WaitUITransition(SaveDataSelectMode.Load));
        }

        public void NewGameButtonClicked()
        {
            UIManager.Instance.StartCoroutine(WaitUITransition(SaveDataSelectMode.Create));
        }

        public void SettingsButtonClicked()
        {
            UIManager.Instance.StartCoroutine(UIManager.Instance.ShowUI(UIState.SettingsMenu));
        }

        public void ReturnToTitleButtonClicked()
        {
            UIManager.Instance.ResetUIStack();
            UIManager.Instance.StartCoroutine(UIManager.Instance.ShowUI(UIState.TitleScreen, false));
        }

        public void ExitButtonClicked()
        {
            GameManager.Instance.ExitGame();
        }

        IEnumerator WaitUITransition(SaveDataSelectMode mode)
        {
            yield return UIManager.Instance.StartCoroutine(UIManager.Instance.ShowUI(UIState.SaveDataSelectMenu));

            while (SaveDataSelectMenuUI.Instance == null || !SaveDataSelectMenuUI.Instance.gameObject.activeInHierarchy)
            {
                yield return null;  // 次のフレームまで待機
            }

            SaveDataSelectMenuUI.Instance.mode = mode;
        }
    }
}
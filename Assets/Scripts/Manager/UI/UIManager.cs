using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using TANK3.Manager.Scene;
using TMPro;
using UnityEngine.UI;
using System.Xml.Linq;

namespace TANK3.Manager.UI
{
    public class UIManager : Singleton<UIManager>
    {
        //--- PUBILIC ---//
        public UIState? CurrentState
        {
            get => currentState;
            set
            {
                Debug.Log($"set currentState:{currentState?.ToString() ?? "null"}");
                if (currentState != value)
                {
                    currentState = value;
                    if (currentState != null) OnUIStateChanged?.Invoke(currentState);
                    Debug.Log($"invoked currentState:{currentState?.ToString() ?? "null"}");
                }
            }
        }

        public Action<UIState?> OnUIStateChanged = delegate { };
        public TMP_FontAsset newFontAsset;
        public Sprite messagePanelSprite;

        //--- PRIVATE ---//
        [SerializeField] private List<UIElement> uiElements = new();
        Dictionary<UIState, UIElement> uiDictionary = new();

        Coroutine fadeCoroutine = null; // fadeアニメーションの管理用.
        Stack<UIState> uiStack = new(); // back機能の実装用.

        UIState? currentState = null;
        GameObject PopUp;
        GameObject Panel;
        GameObject OverlayPanel;
        GameObject message;

        protected override void Awake()
        {
            base.Awake();
            InitializeUIDictionary();
            Instance.CreateMessagePanel();
        }
        protected override void Start()
        {
            base.Start();
            foreach (var ui in uiDictionary.Values)
            {
                if (ui.uiObject.TryGetComponent(out CanvasGroup canvasGroup))
                {
                    ui.uiObject.SetActive(false);
                }
            }

            StartCoroutine(ShowUI(UIState.TitleScreen, true, false, false));
            ResetUIStack();
        }

        // Update is called once per frame
        void Update()
        {
            // ここに機能を追加.
        }

        public IEnumerator ShowUI(UIState uiState, bool addToStack = true, bool useFadeOut = true, bool useFadeIn = true, float fadeDuration = 0.5f)
        {
            if (uiState == CurrentState && fadeCoroutine == null) yield break;

            // 既存のフェード処理を停止
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }

            foreach (var ui in uiDictionary.Values)
            {
                if (ui.uiObject == null) Debug.Log(ui.state);
                if (ui.uiObject.TryGetComponent(out CanvasGroup canvasGroup))
                {
                    if (useFadeOut)
                    {
                        fadeCoroutine = StartCoroutine(FadeUI(canvasGroup, false, fadeDuration));
                        yield return fadeCoroutine;
                    }
                    else
                        ui.uiObject.SetActive(false);
                }
                else
                    ui.uiObject.SetActive(false);
            }

            if (uiDictionary.TryGetValue(uiState, out var newUI))
            {
                if (newUI.uiObject.TryGetComponent(out CanvasGroup newCanvasGroup))
                {
                    if (useFadeIn)
                    {
                        fadeCoroutine = StartCoroutine(FadeUI(newCanvasGroup, true, fadeDuration));
                        yield return fadeCoroutine;
                    }
                    else
                    {
                        newUI.uiObject.SetActive(true);
                        newCanvasGroup.alpha = 1.0f;
                    }
                }
                else
                {
                    newUI.uiObject.SetActive(true);
                }
                CurrentState = uiState;
                if (addToStack) uiStack.Push((UIState)CurrentState);
            }
            else
            {
                Debug.LogWarning($"UIState {uiState} not found in UIManager.");
            }

            Debug.Log($"stack count:{uiStack.Count}");
            fadeCoroutine = null;
        }

        public IEnumerator ShowUIAdditive(UIState uiState, bool useFade, float fadeDuration = 0.3f)
        {
            // 既存のフェード処理を停止
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }

            // Additive モードの場合は、既存の UI をそのまま残し、新しい UI を重ねて表示する
            if (uiDictionary.TryGetValue(uiState, out var newUI))
            {
                if (newUI.uiObject.TryGetComponent(out CanvasGroup newCanvasGroup))
                {
                    if (useFade)
                    {
                        // 必要ならフェードイン処理
                        fadeCoroutine = StartCoroutine(FadeUI(newCanvasGroup, true, fadeDuration));
                        yield return fadeCoroutine;
                    }
                    else
                    {
                        newUI.uiObject.SetActive(true);
                    }
                }
                else
                {
                    newUI.uiObject.SetActive(true);
                }
            }
            else
            {
                Debug.LogWarning($"UIState {uiState} not found in UIManager.");
            }
        }

        public IEnumerator HideUI(UIState uiState, bool useFade, float fadeDuration = 0.3f)
        {
            // 既存のフェード処理を停止
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }

            // Additive モードの場合は、既存の UI をそのまま残し、新しい UI を重ねて表示する
            if (uiDictionary.TryGetValue(uiState, out var UIToHide))
            {
                if (UIToHide.uiObject.TryGetComponent(out CanvasGroup newCanvasGroup))
                {
                    if (useFade)
                    {
                        // 必要ならフェードアウト処理.
                        fadeCoroutine = StartCoroutine(FadeUI(newCanvasGroup, false, fadeDuration));
                        yield return fadeCoroutine;
                    }
                    else
                    {
                        UIToHide.uiObject.SetActive(false);
                    }
                }
                else
                {
                    UIToHide.uiObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning($"UIState {uiState} not found in UIManager.");
            }
        }

        public IEnumerator ShowConfiguredUIAdditive(UIState uiState,bool useFade, UIConfig config, string msg = "", float fadeDuration = 0.3f)
        {
            // 設定を対象のUIオブジェクトに適用する
            if (uiDictionary.TryGetValue(uiState, out var uiData))
            {
                ApplyUIConfig(uiData.uiObject, config);
            }
            else
            {
                Debug.LogWarning($"UIState {uiState} not found in UIManager.");
            }

            TMP_Text tmpText = uiData.uiObject.GetComponentInChildren<TMP_Text>(true);
            if (tmpText != null)
            {
                // TextMeshProUGUIが存在する場合、そのtextプロパティを利用する
                tmpText.text = msg;
            }
            else
            {
                // TextMeshProUGUIがない場合、必要なら代替処理（例えば、標準のTextコンポーネントを試すなど）を行う
                Text standardText =uiData. uiObject.GetComponent<Text>();
                if (standardText != null)
                {
                    standardText.text = "メッセージを設定";
                }
                else
                {
                    Debug.LogWarning("対象のUIにTextMeshProUGUIまたはTextコンポーネントが見つかりませんでした。");
                }
            }

            // 設定されたフェード時間で Additive UI を表示するコルーチンを呼び出す
            yield return StartCoroutine(ShowUIAdditive(uiState,useFade, fadeDuration));
        }

        private void ApplyUIConfig(GameObject uiObject, UIConfig config)
        {
            if (uiObject == null || config == null)
                return;

            // Canvas の設定
            Canvas canvas = uiObject.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = config.sortingOrder;
            }

            // RectTransform の設定
            RectTransform rt = uiObject.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = config.anchorMin;
                rt.anchorMax = config.anchorMax;
                rt.pivot = config.pivot;
                rt.anchoredPosition = config.anchoredPosition;
                rt.sizeDelta = config.sizeDelta;
            }
        }

        public void GoBack(bool useFadeOut = true, bool useFadeIn = true, float fadeDuration = 0.3f)
        {
            if (uiStack.Count > 1)
            {
                uiStack.Pop();
                UIState previousState = uiStack.Peek();
                StartCoroutine(Instance.ShowUI(previousState, false, useFadeOut, useFadeIn, fadeDuration));
                CurrentState = previousState;
            }
            Debug.Log($"Current state:{currentState}");
        }

        public void GoBackWithParams()
        {
            Debug.Log($"(GoBackWithParams)stack count:{Instance.uiStack.Count}");
            Instance.GoBack(false, false);  // 引数を適切に設定
        }

        public void ResetUIStack()
        {
            uiStack.Clear(); // UI スタックをリセット
            if (CurrentState != null) uiStack.Push((UIState)CurrentState); // 必要に応じて最初の画面を再度プッシュ
        }

        void HandleUIStateChange(UIState? newState)
        {
            Debug.Log($"[HandleUIStateChange] Received state: {newState}");
            if (newState.HasValue && uiDictionary.TryGetValue(newState.Value, out var element))
            {
                element.onActivate?.Invoke(); // Inspector で登録した処理を実行
            }
            else
            {
                Debug.LogWarning($"No action found for UIState: {newState}");
            }
        }

        public void OnInGame()
        {
            GameManager.Instance.GamePaused = false;
        }
        private void InitializeUIDictionary()
        {
            uiDictionary.Clear();
            foreach (var element in uiElements)
            {
                if (!uiDictionary.ContainsKey(element.state))
                {
                    GameObject uiObject = Instantiate(element.uiObject);
                    DontDestroyOnLoad(uiObject);
                    uiDictionary[element.state] = new UIElement(element.state, uiObject, element.onActivate);
                }
                else
                {
                    Debug.LogWarning($"Duplicate UIState detected: {element.state}");
                }
            }
        }

        private IEnumerator FadeUI(CanvasGroup canvasGroup, bool fadeIn, float duration = 0.3f)
        {
            float startAlpha = fadeIn ? 0 : 1;
            float endAlpha = fadeIn ? 1 : 0;
            float time = 0;
            if (fadeIn)
            {
                // フェードインの場合、最初にオブジェクトをアクティブにする
                canvasGroup.gameObject.SetActive(true);
            }
            while (time < duration)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = endAlpha;

            if (!fadeIn)
            {
                canvasGroup.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            OnUIStateChanged += HandleUIStateChange;
        }

        protected override void OnDestroy()
        {
            OnUIStateChanged -= HandleUIStateChange;
            base.OnDestroy();
        }

        protected override void OnAwake()
        {
            string name = "UIManager";
            InitializeSingletonObject(name);
        }


        public void ModeSelected()
        {
            Instance.StartCoroutine(Instance.ShowUI(UIState.StageSelectMenu));
        }

        public void PauseButtonClicked()
        {
            Instance.StartCoroutine(Instance.ShowUI(UIState.PauseMenu, true, false, false));
        }

        public void CreateMessagePanel()
        {
            // PopUp 作成
            PopUp = new GameObject("PopUp");
            var canvas = PopUp.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // 他の Canvas より高い値に設定する
            canvas.overrideSorting = true;
            PopUp.AddComponent<GraphicRaycaster>();

            // Panel 作成
            Panel = new GameObject("Panel");
            Panel.AddComponent<RectTransform>();
            Panel.AddComponent<CanvasRenderer>();
            var image = Panel.AddComponent<Image>();
            image.color = Color.gray;
            if (messagePanelSprite != null)
                image.sprite = messagePanelSprite;
            Panel.transform.SetParent(PopUp.transform, false);

            // 全画面を覆うオーバーレイパネルの作成
        OverlayPanel = new GameObject("OverlayPanel");
            OverlayPanel.AddComponent<RectTransform>();
            OverlayPanel.AddComponent<CanvasRenderer>();
            var overlayImage = OverlayPanel.AddComponent<Image>();
            // 例：半透明の黒
            overlayImage.color = new Color(0, 0, 0, 0.5f);
            // 入力をブロックするための CanvasGroup を追加
            var overlayCanvasGroup = OverlayPanel.AddComponent<CanvasGroup>();
            overlayCanvasGroup.blocksRaycasts = true;
            // オーバーレイパネルを PopUp の子にする
            OverlayPanel.transform.SetParent(PopUp.transform, false);
            // RectTransform を全画面に設定
            RectTransform overlayRect = OverlayPanel.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.pivot = new Vector2(0.5f, 0.5f);
            overlayRect.anchoredPosition = Vector2.zero;
            overlayRect.sizeDelta = Vector2.zero;

            // RectTransform の初期設定
            RectTransform panelRect = Panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(300, 150);

            // Message 作成
            message = new GameObject("Message");
            message.AddComponent<RectTransform>();
            message.AddComponent<CanvasRenderer>();
            var tmpro = message.AddComponent<TextMeshProUGUI>();
            if (tmpro != null && newFontAsset != null)
            {
                tmpro.font = newFontAsset;
            }
            tmpro.text = string.Empty;
            tmpro.alignment = TextAlignmentOptions.Center;
            message.transform.SetParent(Panel.transform, false);
            // Message の RectTransform 調整（例: 中央配置）
            RectTransform messageRect = message.GetComponent<RectTransform>();
            messageRect.anchorMin = new Vector2(0.5f, 0.5f);
            messageRect.anchorMax = new Vector2(0.5f, 0.5f);
            messageRect.pivot = new Vector2(0.5f, 0.5f);
            messageRect.anchoredPosition = Vector2.zero;
            messageRect.sizeDelta = new Vector2(280, 100);

            DontDestroyOnLoad(PopUp);
            PopUp.SetActive(false);
        }

        public void ShowMessage(string msg)
        {
            Instance.PopUp.SetActive(true);
            Instance.message.GetComponent<TMP_Text>().text = msg;
            RectTransform panelRect = Instance.Panel.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(msg.Length * 32, 150);
            RectTransform messageRect = Instance.message.GetComponent<RectTransform>();
            messageRect.sizeDelta = new Vector2(msg.Length * 32 -20, 100);
        }

        public void HideMessage()
        {
            Instance.message.GetComponent<TMP_Text>().text = string.Empty;
            Instance.PopUp.SetActive(false);
        }
    }
}
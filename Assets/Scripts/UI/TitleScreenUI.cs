using System;
using System.Collections;
using System.Collections.Generic;
using TANK3.Manager.UI;
using UnityEngine;

namespace TANK3.UI
{
    public class TitleScreenUI : Singleton<TitleScreenUI>
    {
        public CanvasGroup CanvasGroup;
        readonly float t = 2.0f;    // 周期.

        // Update is called once per frame
        void Update()
        {
            CanvasGroup.alpha = Mathf.PingPong(Time.time / t, 1f);  // 文字を点滅させる.
        }

        public void OnScreenTitle()
        {
            UIManager.Instance.StartCoroutine(Instance.WaitForInput());
        }

        IEnumerator WaitForInput()
        {
            yield return new WaitUntil(() => Input.anyKeyDown); // キー入力を待機.
            Debug.Log("Key pressed, switching to MainMenu");
            UIManager.Instance.StartCoroutine(UIManager.Instance.ShowUI(UIState.MainMenu));
        }
    }
}
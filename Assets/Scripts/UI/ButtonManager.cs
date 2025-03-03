using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TANK3.UI
{
    public class ButtonManager : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public Sprite selectedSprite;
        public Sprite deselectedSprite;
        public Image buttonImage;
        private Button button;

        void Start()
        {
            buttonImage = GetComponent<Image>();
            button = GetComponent<Button>();

            // ボタンがクリックされたときのイベントを追加
            if (button != null)
            {
                button.onClick.AddListener(OnClick);  // OnClickメソッドをリスナーとして追加
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (selectedSprite != null && buttonImage != null)
            {
                buttonImage.sprite = selectedSprite;
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (deselectedSprite != null && buttonImage != null)
            {
                buttonImage.sprite = deselectedSprite;
            }
        }

        public void OnClick()
        {
            buttonImage.sprite = deselectedSprite;
        }

        void OnDestroy()
        {
            // ボタンが破棄される前にリスナーを削除
            if (button != null)
            {
                button.onClick.RemoveListener(OnClick);
            }
        }
    }
}

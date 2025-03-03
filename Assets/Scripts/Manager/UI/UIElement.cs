using UnityEngine;
using UnityEngine.Events;

namespace TANK3.Manager.UI
{
    [System.Serializable]
    public struct UIElement
    {
        public UIState state;
        public GameObject uiObject;
        public UnityEvent onActivate;

        public UIElement(UIState state, GameObject uiObject, UnityEvent onActivate)
        {
            this.state = state;
            this.uiObject = uiObject;
            this.onActivate = onActivate;
        }
    }
}

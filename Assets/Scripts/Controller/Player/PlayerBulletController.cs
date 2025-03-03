using System.Collections.Generic;
using TANK3.Manager.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TANK3.UI;

namespace TANK3.Controller
{
    public class PlayerBulletController : BulletController
    {
        public GraphicRaycaster raycaster;
        EventSystem eventSystem;

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override void OnStart()
        {
            base.OnStart();
            if(eventSystem == null) eventSystem = FindAnyObjectByType<EventSystem>();
            raycaster = InGameUI.Instance.GetComponent<GraphicRaycaster>();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUIObject()) return;
                RecycleFire();
            }
        }
        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
        }

        private bool IsPointerOverUIObject()
        {
            PointerEventData eventData = new PointerEventData(eventSystem);
            eventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(eventData, results);

            return results.Count > 0;
        }
    }
}

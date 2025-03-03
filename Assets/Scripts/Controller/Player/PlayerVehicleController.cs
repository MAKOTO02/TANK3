using UnityEngine;

namespace TANK3.Controller
{
    public class PlayerVehicleController : VehicleController
    {
        public override void OnAwake()
        {
            base.OnAwake();
        }
        public override void OnStart()
        {
            base.OnStart();
        }

        // Update is called once per frame
        public override void OnUpdate()
        {
            Receive();
            base.OnUpdate();
        }
        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
        }

        public void Receive()
        {
            moveInput = Input.GetAxis("Vertical");
            turnInput = Input.GetAxis("Horizontal");
        }
    }
}

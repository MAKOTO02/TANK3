using UnityEngine;
using TANK3.Data.VehicleManagement;

namespace TANK3.Controller
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Animator))]
    public class VehicleController : MonoBehaviour
    {
        //------ PUBLIC VARIABLES ------//
        public VehicleControlDataStore VehicleControlDataStore;
        public string vehicleId;

        //------ PRIVATE VARIABLES ------//
        private float MoveForwardSpeedLimit;
        private float Accel;
        private float VehicleTurnSpeed;
        private Animator animator;

        //----- PROTECTED VARIABLES -----//
        protected Rigidbody thisRigidbody;
        protected float moveInput = 0;
        protected float turnInput = 0;


        public virtual void OnAwake()
        {
            // DO NOTHING
        }
        public virtual void OnStart()
        {
            thisRigidbody = GetComponent<Rigidbody>();
            thisRigidbody.mass = 100.0f;
            thisRigidbody.useGravity = true;
            animator = GetComponent<Animator>();
            if (VehicleControlDataStore.VehicleControlDataDictionary[vehicleId] == null) Debug.LogWarning("VehicleController: VehicleIdの設定が間違ています.");
            MoveForwardSpeedLimit = VehicleControlDataStore.VehicleControlDataDictionary[vehicleId].MoveForwardSpeedLimit;
            Accel = VehicleControlDataStore.VehicleControlDataDictionary[vehicleId].Accel;
            VehicleTurnSpeed = VehicleControlDataStore.VehicleControlDataDictionary[vehicleId].VehicleTurnSpeed;
        }

        public virtual void OnUpdate()
        {
            Move();
            Turn();
            PlayAnimation();
        }

        public virtual void OnLateUpdate()
        {
            // DO NOTHING
        }

        void Move()
        {
            // すぐ下に地面があるかを判定
            // 地面の判定については、Deubug等していないので、挙動は不明です.
            if (!Physics.Raycast(transform.position, Vector3.down, out _, 1.0f)) return;
            // 下に地面があるときは入力に従い動かす.
            if (thisRigidbody.velocity.magnitude < MoveForwardSpeedLimit)
            {
                thisRigidbody.AddForce(transform.forward * Accel * moveInput, ForceMode.Force);    // rb.MovePositionだと壁を貫通するので変更
            }
            // 慣性を消去し、滑らないようにする.
            DeleteSpeed();
        }
        void Turn()
        {
            thisRigidbody.MoveRotation(thisRigidbody.rotation * Quaternion.Euler(0, turnInput * VehicleTurnSpeed * Time.deltaTime, 0));
        }
        void PlayAnimation()
        {
            float moveAnimSpeed = thisRigidbody.velocity.magnitude * 8 / MoveForwardSpeedLimit; // moveSpeed が最大 のとき再生速度 8 になるように補正して、再生速度をセット.
            float turnAnimSpeed = 4.0f; // 今回、ターンのアニメーションでは加速がないので、一定の値を採用します.
            animator.SetFloat("MoveSpeed", moveAnimSpeed);
            animator.SetFloat("TurnSpeed", turnAnimSpeed);
            if (moveInput > 0)
            {
                animator.SetBool("rep", true);
                animator.SetBool("reprev", false);
            }
            else if (moveInput < 0)
            {
                animator.SetBool("rep", false);
                animator.SetBool("reprev", true);
            }
            else if (moveInput == 0)
            {

                animator.SetBool("rep", false);
                animator.SetBool("reprev", false);
            }

            if (turnInput > 0)
            {
                animator.SetBool("TurnR", true);
                animator.SetBool("TurnL", false);
            }
            else if (turnInput < 0)
            {
                animator.SetBool("TurnR", false);
                animator.SetBool("TurnL", true);
            }
            else if (turnInput == 0)
            {

                animator.SetBool("TurnL", false);
                animator.SetBool("TurnR", false);
            }
        }

        void DeleteSpeed()
        {
            if (Mathf.Abs(moveInput) < 0.05f)
            {
                thisRigidbody.velocity = new Vector3(0.0f, thisRigidbody.velocity.y, 0.0f);
            }
        }
    }
}

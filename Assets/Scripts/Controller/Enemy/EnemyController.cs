using System.Collections;
using UnityEngine;

namespace TANK3.Controller
{
    // 簡易的な敵AIです。
    // EscapeとAttackの二つのモードを10秒ごとに切り替えます.
    public class EnemyController : VehicleController
    {
        //----- PUBLIC VARIABLES -----//
        public string enemyType = "NORMAL";

        //----- PRIVATE VARIABLES -----//
        [SerializeField] private Rigidbody targetRigidbody;
        [SerializeField] private GameObject targetCannon;
        [SerializeField] private bool autoTarget = true;
        private Vector3 PlayerPosition;
        private int mode = -1;

        private void Awake()
        {
            targetRigidbody = null; // 分離しておく.
        }
        private IEnumerator ModeChange()
        {
            while (true)
            {
                if (Random.value > 0.5f) mode *= -1;
                yield return new WaitForSeconds(10.0f);
            }
        }
        public override void OnAwake()
        {
            base.OnAwake();
        }
        public override void OnStart()
        { 
            base.OnStart();
            StartCoroutine(ModeChange());
        }

        // Update is called once per frame
        public override void OnUpdate()
        {
            base.OnUpdate();
            GetComponent<EnemyCannonController>().targetRigidbody = targetRigidbody;
            // プレイヤーとの相対的な位置を計算.
            if (targetRigidbody == null) return;
            PlayerPosition = targetRigidbody.transform.position - thisRigidbody.transform.position;

            if (mode == 1)
            {
                Attack(50.0f);
                return;
            }
            if (mode == -1)
            {
                Escape(100.0f);
                return;
            }
            Debug.Log(mode);
        }
        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log("TriggerEnter!");
            if (!autoTarget) return;
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Playerを発見しました.");
                Debug.Log(other.gameObject);
                targetRigidbody = other.gameObject.GetComponent<Rigidbody>();
                if (targetRigidbody == null) Debug.Log("PlayerのRigidBodyが発見できません.");
                else Debug.Log(targetRigidbody.gameObject);
                targetCannon = other.gameObject.transform.Find("Cannon").gameObject;
                if (targetCannon == null) Debug.Log("PlayerのCannonが発見できません.");
                else Debug.Log(targetCannon.transform.parent.gameObject);
            }
            else Debug.Log(other.gameObject);
        }

        /// <summary>
        /// Adjusts movement based on the player's distance.
        /// </summary>
        /// <param name="desiredDistance">The desired distance from the player.</param>
        void AdjustMovementBasedOnDistance(float desiredDistance)
        {
            if (PlayerPosition.magnitude > desiredDistance)
            {
                moveInput = 1;
            }
            else
            {
                moveInput = -1;
            }
        }

        /// <summary>
        /// Initiates an attack by turning towards the player and adjusting movement based on distance.
        /// </summary>
        /// <param name="desiredDistance">The desired distance for the attack.</param>
        void Attack(float desiredDistance)
        {
            TurnTo(PlayerPosition);
            AdjustMovementBasedOnDistance(desiredDistance);
        }
        /// <summary>
        /// Maintain a vertical orientation towards the player and perform long-range shooting.
        /// </summary>
        /// <param name="desiredDistance">The distance to be maintained.</param>
        void Escape(float desiredDistance)
        {
            // Face the direction that is perpendicular to the player.
            TurnTo(PlayerPosition, 90.0f);

            // Check conditions for movement.
            float angleCosine = Mathf.Cos(Vector3.Angle(targetCannon.GetComponent<Rigidbody>().transform.up, thisRigidbody.transform.forward) * Mathf.PI / 180.0f);

            if (Mathf.Abs(angleCosine) < 0.2f)
            {
                // Move when the player's Cannon captures the aircraft or when the player gets too close.
                moveInput = -1;
                return;
            }
            if (desiredDistance > PlayerPosition.magnitude)
            {
                // Move when the player is farther away.
                moveInput = 1;
                return;
            }
            // Otherwise, move randomly.
            moveInput = Random.Range(-1.0f, 1.0f);

        }

        /// <summary>
        /// Adjusts the turnInput to face the vector in the specified direction. 
        /// Apply the correction of the angle in deltaAngle (in degrees, 360-degree system).
        /// </summary>
        /// <param name="direction">The direction vector to face.</param>
        /// <param name="deltaAngle">Additional angle correction (in degrees).</param>
        void TurnTo(Vector3 direction, float deltaAngle = 0.0f)
        {
            var signedAngle = (Vector3.SignedAngle(thisRigidbody.transform.forward, direction, Vector3.up) + deltaAngle) * Mathf.PI / 180.0f;
            turnInput = Mathf.Sin(signedAngle);
        }
    }
}


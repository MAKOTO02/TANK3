using UnityEngine;
namespace TANK3.Controller 
{
    [RequireComponent(typeof(EnemyBulletController))]
    public class EnemyCannonController : CannonController
    {
        public Rigidbody targetRigidbody;
        protected Vector3 PlayerPosition;
        protected Vector3 PlayerVelocity;
        public float BulletSpeed;

        public override void OnAwake()
        {
            base.OnAwake();
        }
        public override void OnStart()
        {
            base.OnStart();
            Debug.Log($"EnemyCannonController: Bullet Speed = {BulletSpeed}");
        }

        // Update is called once per frame
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (targetRigidbody != null)
            {
                CalculateTargetDirection();
                TurnCannon();
            }
        }
        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
        }

        void CalculateTargetDirection()
        {
            PlayerPosition = targetRigidbody.transform.position - GetComponent<Rigidbody>().transform.position;
            PlayerVelocity = targetRigidbody.velocity;

            float maxRange = 1.0f;
            float minRange = 0.0f;

            if (PlayerPosition.magnitude > 50 || PlayerVelocity.magnitude > 5)
            {
                maxRange = 1.2f;
                minRange = 0.5f;
            }

            float estimatedTime = (PlayerPosition.magnitude / BulletSpeed) * Random.Range(minRange, maxRange);
            AimDirection = PlayerPosition + PlayerVelocity * estimatedTime;
        }
    }
}
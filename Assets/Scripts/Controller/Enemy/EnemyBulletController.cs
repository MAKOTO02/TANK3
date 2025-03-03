using System.Collections;
using UnityEngine;

namespace TANK3.Controller
{
    public class EnemyBulletController : BulletController
    {
        private IEnumerator ShotAtRegularInterval()
        {
            float StandardInterval = 2.0f;

            while (true)
            {
                yield return new WaitForSeconds(StandardInterval + Random.Range(-1.0f, 1.0f));
                RecycleFire();
            }
        }

        public override void OnAwake()
        { 
            base.OnAwake(); 
        }
        public override void OnStart()
        {
            base.OnStart();
            var cannonController = GetComponent<EnemyCannonController>();
            cannonController.BulletSpeed = bulletSpeed;
            StartCoroutine(ShotAtRegularInterval());
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
        }
        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
        }
    }
}
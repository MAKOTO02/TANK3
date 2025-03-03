using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TANK3.Data.BulletManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TANK3.Controller
{
    public class BulletController : MonoBehaviour
    {
        // 弾のデータ.
        public BulletDataStore bulletDataStore;
        public string bulletId;

        private int limit;    // 場に存在できる自機の弾の数をここに格納.
        public float bulletSpeed;  // 弾の初速を制御する変数.
        protected float bulletScale;
        protected int bulletDurationTimes;
        protected Trajectory trajectory;

        // 弾を発射するためのメンバ.
        public GameObject bulletMark;   // 弾を発射する起点となる所に(砲台の子オブジェクトとして)くっつけておくオブジェクト.生成などの際に位置を参照する.

        private GameObject bulletPrefab; // Resourcesからプレハブをロードするための仮変数.
        private bool isPrefabLoaded = false;
        private GameObject bulletCopy; // 実際に発射される弾.
        private static readonly int defaultQueueLimit = 20;    // メモリを使いすぎないよう、Queueの上限を決めておく。
        private Queue<GameObject> bulletPool; // 場に出た弾をQueueに入れておいてリサイクルする。
        protected Vector3 CannonForward;

        // Start is called before the first frame update
        public virtual void OnAwake()
        {
            StartCoroutine(LoadBulletPrefabAsync());
            bulletMark.SetActive(false);    // 目印となるオブジェクトは邪魔なので、activeをfalseにセットしておく.
        }

        public virtual void OnStart()
        {
            if (bulletDataStore.BulletDataDictionary[bulletId] == null) Debug.LogWarning("BulletController: bulletIdが間違っています.");
            limit = bulletDataStore.BulletDataDictionary[bulletId].limit;
            bulletSpeed = bulletDataStore.BulletDataDictionary[bulletId].bulletSpeed;
            Debug.Log($"BulletController: bulletSpeed = {bulletSpeed}");
            bulletScale = bulletDataStore.BulletDataDictionary[bulletId].bulletScale;
            bulletDurationTimes = bulletDataStore.BulletDataDictionary[bulletId].durationTimes;
            bulletPool = new(Mathf.Min(defaultQueueLimit, limit));
        }

        public virtual void OnUpdate()
        {
            CannonForward = gameObject.GetComponent<CannonController>().CannonForward;
        }

        public virtual void OnLateUpdate()
        {
            // DO NOTHING
        }

        private IEnumerator LoadBulletPrefabAsync()
        {
            if (isPrefabLoaded) yield break;

            ResourceRequest request = Resources.LoadAsync<GameObject>("bullet");
            yield return request;

            if (request.asset != null)
            {
                bulletPrefab = (GameObject)request.asset;
                isPrefabLoaded = true;
            }
            else
            {
                Debug.Log("BulletController: Resourcesフォルダにbulletのプレハブが見つかりません");
            }
        }
        bool GenerateBulletCopy()
        {
            if (bulletPrefab == null || bulletMark == null)
            {
                Debug.LogWarning("BulletController: 弾のプレハブまたは発射位置が設定されていません");
                return false;
            }
            bulletCopy = Instantiate(bulletPrefab, bulletMark.transform.position, Quaternion.identity);
            if (bulletCopy == null)
            {
                Debug.LogWarning("BulletController: Bulletの生成に失敗しました.");
            }
            bulletCopy.transform.localScale = Vector3.one * bulletScale;    // 弾の大きさを設定と合わせる.
            bulletCopy.GetComponent<BulletCollisionManager>().durationTimes = bulletDurationTimes;
            if (bulletPool.Count >= limit)
            {
                GameObject oldBullet = bulletPool.Dequeue();
                Destroy(oldBullet);
            }
            bulletPool.Enqueue(bulletCopy);
            if (gameObject.CompareTag("Player")) DontDestroyOnLoad(bulletCopy);
            return true;
        }
        void FireLinear()
        {
            Debug.Log($"bulletCopy == null?:{bulletCopy==null}");
            bulletCopy.transform.position = bulletMark.transform.position;  // 位置をマークの位置に
            bulletCopy.GetComponent<Bullet>().SetShouldExplode(false);
            bulletCopy.GetComponent<BulletSpeedManager>().SetSpeed(bulletSpeed);    // 射出速度をセットする.
            Debug.Log($"BulletController({gameObject}): CannonForward = {CannonForward}, bulletSpeed = {bulletSpeed}");
            bulletCopy.GetComponent<Rigidbody>().velocity = CannonForward * bulletSpeed;
            bulletCopy.GetComponent<BulletCollisionManager>().ResetCount(); // 衝突回数をリセットする.
            bulletCopy.SetActive(true); // bulletCopyをactiveにする.
            SoundManager.Play("fire");
        }

        void FireParabora()
        {
            // a
        }
        protected void RecycleFire()
        {
            // Queueに入っている弾の数が上限より小さいなら、新しく生成しQueueに追加.
            Debug.Log($"BulletPoolCount:{bulletPool.Count} Limit:{limit}");
            if (bulletPool.Count < limit)
            {
                if (GenerateBulletCopy())  //  バレットのコピーを生成しQueueに入れる.
                {
                    Debug.Log("BulletController: 弾を発射しました.");
                    if (trajectory == Trajectory.LINEAR) FireLinear(); //  弾を初期化して射出する.
                    else if (trajectory == Trajectory.PARABORA) FireParabora();

                    return;
                }
                Debug.LogWarning("BulletController: Bulletの生成に失敗しました.");
            }

            // 弾が上限に達したら、Queueに入っているものを参照してリサイクルする.
            if (bulletPool.Count == 0) return;
            int count = bulletPool.Count;   // foreachを使うと、その間bulletPoolにアクセスできなくなる.
            for (var i = 0; i < count; i++)
            {
                bulletPool.Enqueue(bulletCopy);
                bulletCopy = bulletPool.Dequeue();
                if (bulletCopy == null)
                {
                    // もし null なら、新しく生成するか、キューから除外する
                    Debug.LogWarning("RecycleFire: bulletCopy が null または破棄されています。");
                    continue;
                }

                if (!bulletCopy.activeSelf)
                {
                    if (trajectory == Trajectory.LINEAR) FireLinear();
                    else if (trajectory == Trajectory.PARABORA) FireParabora();
                    break;
                }
            }
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TANK3.Manager.Effects;

/// <summary>
/// Bulletクラスは、弾の速度と弾の反射回数を管理します。
/// desiredSpeedは発射の際に初期化して下さい。
/// </summary>

[RequireComponent(typeof(SphereCollider))]
public class Bullet : MonoBehaviour
{
    //----- PRIVATE VARIABLES -----//
    private bool ShouldExplode;
   
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        ShouldExplode = false;
        GetComponent<BulletCollisionManager>().OnRequestDisableObject += DestroyObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldExplode)
        {
            gameObject.SetActive(true);
            Explode();
            InitiateBullet();
        }
    }

    void OnSceneUnloaded(Scene scene)
    {
        InitiateBullet();
    }
    void InitiateBullet()
    {
        ShouldExplode = false;
        gameObject.SetActive(false);
    }
    void Explode()
    {
        SoundManager.Play("hit");
        EffectManager.Instance.PlayEffect(transform.position);
        InitiateBullet();
    }

    void DestroyObject(GameObject obj)
    {
        if (obj != null)
        {
            Destroy(obj);
        }
    }

    private void OnDestroy()
    {
        GetComponent<BulletCollisionManager>().OnRequestDisableObject -= DestroyObject;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    //----- PUBLIC METHODS -----//
    public void SetShouldExplode(bool setting)
    {
        ShouldExplode = setting;
    }
}

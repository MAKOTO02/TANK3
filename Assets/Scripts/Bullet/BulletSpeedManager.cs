using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Bullet))]
public class BulletSpeedManager : MonoBehaviour
{
    //----- PRIVATE VARIABLES -----//
    private float desiredSpeed;
    private Rigidbody bulletBody;
    private Bullet bullet;

    // Start is called before the first frame update
    void Start()
    {
        bulletBody = GetComponent<Rigidbody>();
        bullet = GetComponent<Bullet>();
    }

    // Update is called once per frame
    void Update()
    {
        InvalidSpeedCheck();
        InvalidDirectionCheck();
    }

    void InvalidSpeedCheck()
    {
        float bulletSpeed = bulletBody.velocity.magnitude;
        if (bulletSpeed > desiredSpeed + 1.0f)
        {
            bullet.SetShouldExplode(true);
            Debug.Log(bulletSpeed.ToString() + ": 速度上限に引っ掛かりました");
            return;
        }
        if (bulletSpeed < desiredSpeed - 1.0f)
        {
            //bullet.SetShouldExplode(true);
            //Debug.Log(bulletSpeed.ToString() + ": 速度下限に引っ掛かりました");
            return;
        }
    }

    void InvalidDirectionCheck()
    {
        Vector3 newVelocity = bulletBody.velocity; // 現在の速度をコピー
        newVelocity.y = 0.0f; // y成分をゼロにする
        bulletBody.velocity = newVelocity; // 速度を新しいベクトルに設定
    }
    //----- PUBLIC METHODS -----//
    public void SetSpeed(float speed)
    {
        desiredSpeed = speed;
    }
}

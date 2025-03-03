using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //------ PUBLIC VARIABLES ------//
    public Transform target;    // unity側で参照を与える
    public float sensitiveRotate = 5.0f;    // 感度
    //------ PRIVATE VARIABLES ------//
    // private Camera cam = GetComponent<Camera>();
    private Vector3 offsetPos;

    void Start()
    {
        offsetPos = transform.position - target.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target != null)
        {
            AdjustPos();
            MouseRightInput();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Reset();
            }
        }
    }

    void AdjustPos()
    {
        // 機体の追尾処理 
        transform.position = target.position + transform.rotation * offsetPos;
    }
    void MouseRightInput()
    {
        // マウスの追尾処理
        // 右クリックされているとき処理
        if (Input.GetMouseButton(1))
        {
            float RotateX = Input.GetAxis("Mouse X") * sensitiveRotate;
            transform.Rotate(0.0f, RotateX, 0.0f);
        }
    }
    void Reset()
    {
        transform.rotation = target.rotation;
        transform.position = target.position + transform.rotation * offsetPos;
    }
}

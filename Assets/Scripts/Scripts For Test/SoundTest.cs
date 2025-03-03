using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //左クリック
        {
            SoundManager.Play("shot"); //サウンドマネージャーを使用して効果音再生
        }
        if (Input.GetMouseButtonDown(1)) //右クリック
        {
            SoundManager.Play("hit"); //サウンドマネージャーを使用して効果音再生
        }
    }
}
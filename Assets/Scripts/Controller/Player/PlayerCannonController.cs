using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCannonController : CannonController
{
    public Camera MainCamera;

    // Update is called once per frame
    public override void OnUpdate()
    {
        AimDirection = MainCamera.transform.forward;
        base.OnUpdate();
        TurnCannon();
    }
}

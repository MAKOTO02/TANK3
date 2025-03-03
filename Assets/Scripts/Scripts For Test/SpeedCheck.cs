using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpeedCheck : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Debug.Log("ë¨ìx: " + GetComponent<Rigidbody>().velocity.magnitude.ToString() + "Å@éûçè: " + Time.time.ToString());
    }
}

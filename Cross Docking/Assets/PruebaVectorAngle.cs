using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaVectorAngle : MonoBehaviour
{
    float angle;
    private void Update()
    {
        angle += Input.GetAxis("Horizontal");
        angle = Mathf.Clamp(angle, -360, 360);
        Quaternion target = Quaternion.Euler(0, angle, 0); // any value as you see fit
        transform.rotation = target;

        //Vector3.Angle//
    }
}
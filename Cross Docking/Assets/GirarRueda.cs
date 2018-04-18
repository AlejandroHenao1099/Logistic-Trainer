using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirarRueda : MonoBehaviour
{

    public float velocidadGiro = 3f;

    private void Update()
    {
        //Quaternion rotacion = Quaternion.Euler(0, Input.GetAxis("Horizontal") * velocidadGiro, 0f);
        transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * velocidadGiro, Space.Self);
        // Vector3 direccion = rotacion * transform.forward;
        // transform.localRotation = Quaternion.LookRotation(direccion);
    }
}
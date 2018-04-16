using System;
using UnityEngine;

public class DetectorTrigger : MonoBehaviour
{
    public Action<GameObject> OnCollideObject;
    public Action<GameObject> OnLostObject;
    private bool objetoDetectadoLocal;
    private bool puedoDetectarLocal = true;

    public bool objetoDetectado { get { return objetoDetectadoLocal; } set { objetoDetectadoLocal = value; } }
    public bool puedoDetectar { get { return puedoDetectarLocal; } set { puedoDetectarLocal = value; } }

    private void OnTriggerStay(Collider other)
    {
        if (puedoDetectarLocal == true)
        {
            if (objetoDetectado == false && OnCollideObject != null)
            {
                objetoDetectado = true;
                OnCollideObject(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (puedoDetectarLocal == true)
        {
            if (objetoDetectado == true)
            {
                objetoDetectado = false;
                OnLostObject(other.gameObject);
            }
        }
    }

    public void ReiniciarDetector()
    {
        objetoDetectadoLocal = false;
        puedoDetectarLocal = true;
    }
}
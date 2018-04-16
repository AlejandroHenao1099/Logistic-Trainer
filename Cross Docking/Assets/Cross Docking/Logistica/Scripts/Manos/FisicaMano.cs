using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FisicaMano : MonoBehaviour
{
    public Action<Transform> OnHandReady;
    public Action OnHandNoReady;
    public GameObject objetoTrigger { get; private set; }
    [SerializeField] private Transform mano;
    private Transform transformObjetoTrigger;

    private bool vectorAgregado;
    private bool objetoAgarrado;

    public void AgarrarObjeto()
    {
        if (objetoTrigger != null && !vectorAgregado && !objetoAgarrado)
        {
            transformObjetoTrigger = objetoTrigger.transform;
            objetoTrigger.GetComponent<Rigidbody>().isKinematic = true;
            mano.SetParent(transformObjetoTrigger.GetChild(0));
            vectorAgregado = true;
            objetoAgarrado = true;
            OnHandReady(transformObjetoTrigger);
        }
    }

    public void SoltarObjeto()
    {
        if (vectorAgregado == true && objetoAgarrado == true)
        {
            mano.SetParent(transform.GetChild(0));
            mano.localPosition = Vector3.zero;
            mano.localRotation = Quaternion.identity;
            mano.localScale = Vector3.one * 1.5f;
            objetoTrigger.GetComponent<Rigidbody>().isKinematic = false;
            transformObjetoTrigger = null;
            objetoTrigger = null;
            vectorAgregado = false;
            objetoAgarrado = false;
            OnHandNoReady();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!objetoAgarrado)
            objetoTrigger = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!objetoAgarrado)
            objetoTrigger = null;
    }
}
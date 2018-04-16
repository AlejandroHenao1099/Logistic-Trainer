using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorPosicionManos : MonoBehaviour
{
    [SerializeField] private FisicaMano fisicaDerecha;
    [SerializeField] private FisicaMano fisicaIzquierda;
    public Transform manoDerecha;
    public Transform manoIzquierda;
    private Transform objetoAMover;

    private Action OnUpdate;

    private Vector3 vectorUp;

    private bool actualizando;
    private bool cajaPosicionada;

    private void Awake()
    {
        fisicaDerecha.OnHandReady += ActivarActualizacion;
        fisicaDerecha.OnHandNoReady += DesactivarActualizacion;

        fisicaIzquierda.OnHandReady += ActivarActualizacion;
        fisicaIzquierda.OnHandNoReady += DesactivarActualizacion;
    }

    private void Update()
    {
        if (!actualizando) return;
        if (OnUpdate != null) OnUpdate();
    }

    private void ActivarActualizacion(Transform objeto)
    {
        if (!cajaPosicionada)
        {
            objetoAMover = objeto;
            actualizando = true;

            // Quaternion rotacionCaja = objetoAMover.GetChild(0).rotation;
            // objetoAMover.rotation = CalcularRotacionMedia();
            // objetoAMover.GetChild(0).rotation = rotacionCaja;

            cajaPosicionada = true;

            // Vector3 posicionCaja = objetoAMover.GetChild(0).position;
            // objetoAMover.position = CalcularPuntoMedio();
            // objetoAMover.GetChild(0).position = posicionCaja;

            // OnUpdate += ActualizarPosicion;
            // OnUpdate += ActualizarRotacion;
            OnUpdate += CalcularDireccionVectorMedio;
            CalcularPosicionInicial();
            //Fisica mano   mano.SetParent(transform.GetChild(0));
            //Luego de soltar el objeto organizar el hijo para qe quede en la posicion correcta
        }
    }

    private void DesactivarActualizacion()
    {
        if (cajaPosicionada)
        {
            OnUpdate -= CalcularDireccionVectorMedio;
            actualizando = false;
            Vector3 posicion = objetoAMover.GetChild(0).position;
            Quaternion rotacion = objetoAMover.GetChild(0).rotation;
            objetoAMover.position = posicion;
            objetoAMover.rotation = rotacion;
            objetoAMover.GetChild(0).localPosition = Vector3.zero;
            objetoAMover.GetChild(0).localRotation = Quaternion.identity;
            objetoAMover = null;
            cajaPosicionada = false;
            // OnUpdate -= ActualizarPosicion;
            // OnUpdate -= ActualizarRotacion;
        }
    }

    private void ActualizarPosicion()
    {
        objetoAMover.position = CalcularPuntoMedio();
    }

    private void ActualizarRotacion()
    {
        objetoAMover.rotation = CalcularRotacionMedia();
    }

    private void CalcularPosicionInicial()
    {
        vectorUp = (manoIzquierda.position - manoDerecha.position).normalized;
        vectorUp += manoDerecha.position;
        Vector3 posicionMedia = Vector3.Lerp(manoDerecha.position, manoIzquierda.position, 0.5f);
        objetoAMover.position = posicionMedia;
        Quaternion rotacion = objetoAMover.GetChild(0).rotation;
        objetoAMover.LookAt(vectorUp);
        objetoAMover.GetChild(0).rotation = rotacion;
    }

    private void CalcularDireccionVectorMedio()
    {
        vectorUp = (manoIzquierda.position - manoDerecha.position).normalized;
        vectorUp += manoDerecha.position;
        Vector3 posicionMedia = Vector3.Lerp(manoDerecha.position, manoIzquierda.position, 0.5f);
        objetoAMover.position = posicionMedia;
        objetoAMover.LookAt(vectorUp);
    }

    private Vector3 CalcularPuntoMedio()
    {
        Vector3 posDer = manoDerecha.position;
        Vector3 posIzq = manoIzquierda.position;



        // float derX = posDer.x;
        // float izqX = posIzq.x;

        // float parcialX = Mathf.Lerp(Mathf.Min(derX, izqX), Mathf.Max(derX, izqX), 0.5f);

        // float derY = posDer.y;
        // float izqY = posIzq.y;

        // float parcialY = Mathf.Lerp(Mathf.Min(derY, izqY), Mathf.Max(derY, izqY), 0.5f);

        // float derZ = posDer.z;
        // float izqZ = posIzq.z;

        // float parcialZ = Mathf.Lerp(Mathf.Min(derZ, izqZ), Mathf.Max(derZ, izqZ), 0.5f);

        // Vector3 posObjeto = new Vector3(parcialX, parcialY, parcialZ);
        Vector3 posObjeto = Vector3.Lerp(posDer, posIzq, 0.5f);
        return posObjeto;
    }

    private Quaternion CalcularRotacionMedia()
    {
        Quaternion rotDer = manoDerecha.rotation;
        Quaternion rotIzq = manoIzquierda.rotation;

        // float derX = rotDer.x;
        // float izqX = rotIzq.x;

        // float parcialX = Mathf.Lerp(Mathf.Min(derX, izqX), Mathf.Max(derX, izqX), 0.5f);

        // float derY = rotDer.y;
        // float izqY = rotIzq.y;

        // float parcialY = Mathf.Lerp(Mathf.Min(derY, izqY), Mathf.Max(derY, izqY), 0.5f);

        // float derZ = rotDer.z;
        // float izqZ = rotIzq.z;

        // float parcialZ = Mathf.Lerp(Mathf.Min(derZ, izqZ), Mathf.Max(derZ, izqZ), 0.5f);

        // float derW = rotDer.w;
        // float izqW = rotIzq.w;

        // float parcialW = Mathf.Lerp(Mathf.Min(derW, izqW), Mathf.Max(derW, izqW), 0.5f);

        // Quaternion rotacion = new Quaternion(parcialX, parcialY, parcialZ, parcialW);
        Quaternion rotacion = Quaternion.Slerp(rotDer, rotIzq, 0.5f);
        return rotacion;
    }
}
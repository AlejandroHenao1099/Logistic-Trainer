using System;
using System.Collections;
using UnityEngine;

namespace Cross_Docking
{
    public class ControladorPosicionManos : MonoBehaviour
    {
        public Transform manoDerecha;
        public Transform manoIzquierda;
        private Transform objetoAMover;

        private Action OnUpdate;

        private Vector3 vectorForward;

        private bool actualizando;
        private bool cajaPosicionada;


        private void Update()
        {
            if (!actualizando) return;
            if (OnUpdate != null) OnUpdate();
        }

        public void ActivarActualizacion(Transform objeto)
        {
            if (!cajaPosicionada)
            {
                objetoAMover = objeto.parent;
                objetoAMover.GetComponent<Rigidbody>().isKinematic = true;
                actualizando = true;

                cajaPosicionada = true;

                CalcularPosicionInicial();
                OnUpdate += CalcularDireccionVectorMedio;
            }
        }

        public void DesactivarActualizacion()
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
                objetoAMover.GetComponent<Rigidbody>().isKinematic = false;
                objetoAMover = null;
                cajaPosicionada = false;
            }
        }

        private void CalcularPosicionInicial()
        {
            vectorForward = (manoIzquierda.position - manoDerecha.position).normalized;
            vectorForward += manoDerecha.position;
            Vector3 posicionMedia = Vector3.Lerp(manoDerecha.position, manoIzquierda.position, 0.5f);
            Vector3 posicion = objetoAMover.GetChild(0).position;
            Quaternion rotacion = objetoAMover.GetChild(0).rotation;
            objetoAMover.position = posicionMedia;
            objetoAMover.LookAt(vectorForward);
            objetoAMover.GetChild(0).position = posicion;
            objetoAMover.GetChild(0).rotation = rotacion;
        }

        private void CalcularDireccionVectorMedio()
        {
            vectorForward = (manoIzquierda.position - manoDerecha.position).normalized;
            //vectorForward += manoDerecha.position;
            Vector3 posicionMedia = Vector3.Lerp(manoDerecha.position, manoIzquierda.position, 0.5f);
            objetoAMover.position = posicionMedia;

            Vector3 vectorUp = Quaternion.Euler(-90f, 0f, 0f) * vectorForward;

            Quaternion rotacionMirar = Quaternion.LookRotation(vectorForward, vectorUp);
            objetoAMover.rotation = rotacionMirar;
            //objetoAMover.LookAt(vectorForward);
        }
    }
}
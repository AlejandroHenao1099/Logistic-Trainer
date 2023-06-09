﻿using UnityEngine;

namespace Cross_Docking
{
    public class ControladorManos : MonoBehaviour
    {
        [SerializeField] private Mano derecha;
        [SerializeField] private Mano izquierda;

        private ControladorInput inputDerecha;
        private ControladorInput inputIzquierdo;
        private ControladorPosicionManos controladorPosicionManos;

        private Transform posicionDerecha;
        private Transform posicionIzquierda;

        private TipoDeMovilidad tipoDeMovilidad;
        private ObjetoInteractible objetoInteractible;

        private bool objetoEnMano;
        private bool vectorManosAgregado;

        private void Awake()
        {
            inputDerecha = derecha.GetComponent<ControladorInput>();
            inputIzquierdo = izquierda.GetComponent<ControladorInput>();
            posicionDerecha = derecha.transform;
            posicionIzquierda = izquierda.transform;
            controladorPosicionManos = GetComponent<ControladorPosicionManos>();
            derecha.OnGrabObjTwoControl += VerificarManos;
            izquierda.OnGrabObjTwoControl += VerificarManos;
            derecha.OnGrabObjOneControl += AgarrarObjetoUnaMano;
            izquierda.OnGrabObjOneControl += AgarrarObjetoUnaMano;

            derecha.GetComponent<LaserPointer>().OnGrabCar += DejarDeTeletransportar;
            izquierda.GetComponent<LaserPointer>().OnGrabCar += DejarDeTeletransportar;
            derecha.GetComponent<LaserPointer>().OnReleaseCar += Teletransportar;
            izquierda.GetComponent<LaserPointer>().OnReleaseCar += Teletransportar;
        }

        private void Update()
        {
            if (!objetoEnMano)
                return;

            VerificarInputs();
            VerificarAgarreObjeto();
        }

        private void AgarrarObjetoUnaMano(ObjetoInteractible interactible, Transform manoEnviadora)
        {
            manoEnviadora.GetComponent<Mano>().OnReleaseObjOneControl = interactible.Detener;
            interactible.Iniciar(manoEnviadora);
        }

        private void VerificarManos(ObjetoInteractible interactible)
        {
            if (objetoEnMano == false && (derecha.manoLista && izquierda.manoLista))
            {
                if (derecha.objetoEnMano == izquierda.objetoEnMano)
                {
                    if (interactible.tipoDeMovilidadObjeto == TipoDeMovilidad.Libre)
                    {
                        controladorPosicionManos.ActivarActualizacion(derecha.objetoEnMano.transform);
                        objetoEnMano = true;
                        tipoDeMovilidad = interactible.tipoDeMovilidadObjeto;
                    }
                    else if (interactible.tipoDeMovilidadObjeto == TipoDeMovilidad.SoloRotacion)
                    {
                        interactible.Iniciar();
                        objetoInteractible = interactible;
                        tipoDeMovilidad = interactible.tipoDeMovilidadObjeto;
                        objetoEnMano = true;
                    }
                }
            }
        }

        private void VerificarInputs()
        {
            if (inputDerecha.Controller.GetHairTriggerUp() || inputIzquierdo.Controller.GetHairTriggerUp())
            {
                SoltarObjetoDobleMano();
            }
        }

        private void VerificarAgarreObjeto()
        {
            Vector3 posDerecha = posicionDerecha.position;
            Vector3 posIzquierda = posicionIzquierda.position;
            if (Vector3.Distance(posDerecha, posIzquierda) > 1.5f)
            {
                SoltarObjetoDobleMano();
            }
        }

        private void SoltarObjetoDobleMano()
        {
            if (tipoDeMovilidad == TipoDeMovilidad.Libre)
            {
                controladorPosicionManos.DesactivarActualizacion();
            }
            else if (tipoDeMovilidad == TipoDeMovilidad.SoloRotacion)
            {
                objetoInteractible.Detener();
            }
            derecha.SoltarObjetoDobleMano();
            izquierda.SoltarObjetoDobleMano();
            objetoEnMano = false;
            tipoDeMovilidad = TipoDeMovilidad.Ninguno;
            objetoInteractible = null;
        }

        private void DejarDeTeletransportar()
        {
            derecha.GetComponent<LaserPointer>().teletransportar = false;
            izquierda.GetComponent<LaserPointer>().teletransportar = false;
        }

        private void Teletransportar()
        {
            derecha.GetComponent<LaserPointer>().teletransportar = true;
            izquierda.GetComponent<LaserPointer>().teletransportar = true;
        }
    }
}
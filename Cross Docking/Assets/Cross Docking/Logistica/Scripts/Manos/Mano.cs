using System;
using UnityEngine;

namespace Cross_Docking
{
    public class Mano : MonoBehaviour
    {
        private enum TipoObjetoMano
        {
            Ninguno, UnaMano, DosManos
        }
        private TipoObjetoMano tipoObjetoMano;
        private ControladorInput controladorInput;
        public Action<ObjetoInteractible> OnHandReady;
        public GameObject objetoEnMano { get; set; }
        private Transform objetoEstatico;

        private GameObject objetoColisionando;

        public bool manoLista { get; private set; }
        private bool actualizarObjetoNoMovible;

        private void Awake()
        {
            controladorInput = GetComponent<ControladorInput>();
        }

        private void Update()
        {
            if (controladorInput.triggerPresionado == true)
                if (objetoColisionando)
                    DeterminarAgarreObjeto();

            if (controladorInput.triggerPresionado == false)
                if (objetoEnMano)
                    DeterminarSoltarObjeto();

            if (!actualizarObjetoNoMovible)
                return;

            ActualizarRotacionObjetoNoMovible();
        }

        private void DeterminarAgarreObjeto()
        {
            ObjetoInteractible interactible = objetoColisionando.transform.GetComponent<ObjetoInteractible>();

            if (interactible != null && interactible.agarreDobleMano == true)
                AgarrarObjetoDosManos(interactible);
            else if (interactible != null && interactible.agarreDobleMano == false)
                AgarrarObjetoUnaMano();
        }

        private void DeterminarSoltarObjeto()
        {
            if (tipoObjetoMano == TipoObjetoMano.UnaMano)
                SoltarObjetoUnaMano();
        }

        private void AgarrarObjetoDosManos(ObjetoInteractible interactible)
        {
            manoLista = true;
            objetoEnMano = interactible.gameObject;
            OnHandReady(interactible);
            tipoObjetoMano = TipoObjetoMano.DosManos;
        }

        public void SoltarObjetoDobleMano()
        {
            manoLista = false;
            tipoObjetoMano = TipoObjetoMano.Ninguno;
        }

        private void AgarrarObjetoUnaMano()
        {
            tipoObjetoMano = TipoObjetoMano.UnaMano;
            objetoEnMano = objetoColisionando;
            objetoColisionando = null;

            if (objetoEnMano.GetComponent<ObjetoInteractible>().objetoMovible == true)
            {
                FixedJoint fixedJoint = AgregarFixedJoint();
                fixedJoint.connectedBody = objetoEnMano.GetComponent<Rigidbody>();
            }
            else
            {
                objetoEstatico = objetoEnMano.transform;
                actualizarObjetoNoMovible = true;
            }
        }

        private FixedJoint AgregarFixedJoint()
        {
            FixedJoint fx = gameObject.AddComponent<FixedJoint>();
            fx.breakForce = 20000f;
            fx.breakTorque = 20000f;
            return fx;
        }

        private void SoltarObjetoUnaMano()
        {
            if (objetoEnMano.GetComponent<ObjetoInteractible>().objetoMovible == true)
            {
                if (GetComponent<FixedJoint>())
                {
                    GetComponent<FixedJoint>().connectedBody = null;
                    Destroy(GetComponent<FixedJoint>());
                    Vector3 velocidad = controladorInput.Controller.velocity;
                    velocidad.x = -velocidad.x;
                    velocidad.z = -velocidad.z;
                    objetoEnMano.GetComponent<Rigidbody>().velocity = velocidad;
                    objetoEnMano.GetComponent<Rigidbody>().angularVelocity = -controladorInput.Controller.angularVelocity;
                }
            }
            else
            {
                objetoEnMano.GetComponent<Rigidbody>().angularVelocity = -controladorInput.Controller.angularVelocity;
                actualizarObjetoNoMovible = false;
                objetoEstatico = null;
            }

            tipoObjetoMano = TipoObjetoMano.Ninguno;
            objetoEnMano = null;
        }

        private void ActualizarRotacionObjetoNoMovible()
        {
            Vector3 targetDelta = transform.position - objetoEstatico.position;
            targetDelta.y = 0;

            float diferenciaDeAngulo = Vector3.Angle(objetoEstatico.forward, targetDelta);

            Vector3 cross = Vector3.Cross(objetoEstatico.forward, targetDelta);

            objetoEstatico.GetComponent<Rigidbody>().angularVelocity = cross * diferenciaDeAngulo * 50f;
        }

        private void EstablecerObjetoColisionando(Collider col)
        {
            if (objetoColisionando || !col.GetComponent<ObjetoInteractible>())
                return;

            objetoColisionando = col.gameObject;
        }

        public void OnTriggerEnter(Collider other)
        {
            EstablecerObjetoColisionando(other);
        }

        public void OnTriggerStay(Collider other)
        {
            EstablecerObjetoColisionando(other);
        }

        public void OnTriggerExit(Collider other)
        {
            if (!objetoColisionando)
                return;

            objetoColisionando = null;
        }
    }
}
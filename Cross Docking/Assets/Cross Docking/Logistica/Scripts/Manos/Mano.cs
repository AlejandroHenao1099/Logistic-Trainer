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
        public Action<ObjetoInteractible> OnHandReady;
        public GameObject objetoEnMano { get; set; }

        private GameObject objetoColisionando;

        public bool manoLista { get; private set; }
        private bool manoOcupada;

        private SteamVR_TrackedObject trackedObject;

        public SteamVR_Controller.Device Controller
        {
            get { return SteamVR_Controller.Input((int)trackedObject.index); }
        }

        private void Awake()
        {
            trackedObject = GetComponent<SteamVR_TrackedObject>();
        }

        private void Update()
        {
            if (Controller.GetHairTriggerDown())
                if (objetoColisionando)        
                    DeterminarAgarreObjeto();  

            if (Controller.GetHairTriggerUp())   
                if (objetoEnMano)
                   DeterminarSoltarObjeto();  
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
            manoOcupada = true;
            manoLista = true;
            objetoEnMano = interactible.gameObject;
            OnHandReady(interactible);
            tipoObjetoMano = TipoObjetoMano.DosManos;
        }

        public void SoltarObjetoDobleMano()
        {
            manoLista = false;
            manoOcupada = false;
            tipoObjetoMano = TipoObjetoMano.Ninguno;
        }

        private void AgarrarObjetoUnaMano()
        {
            manoOcupada = true;
            tipoObjetoMano = TipoObjetoMano.UnaMano;
            objetoEnMano = objetoColisionando;
            objetoColisionando = null;
            FixedJoint fixedJoint = AgregarFixedJoint();
            fixedJoint.connectedBody = objetoEnMano.GetComponent<Rigidbody>();
        }

        private FixedJoint AgregarFixedJoint()
        {
            FixedJoint fx = gameObject.AddComponent<FixedJoint>();
            fx.breakForce = 1000000f;
            fx.breakTorque = 1000000f;
            return fx;
        }

        private void SoltarObjetoUnaMano()
        {
            if (GetComponent<FixedJoint>())
            {
                GetComponent<FixedJoint>().connectedBody = null;
                Destroy(GetComponent<FixedJoint>());
                Vector3 velocidad = Controller.velocity;
                velocidad.x = -velocidad.x;
                velocidad.z = -velocidad.z;
                objetoEnMano.GetComponent<Rigidbody>().velocity = velocidad;
                objetoEnMano.GetComponent<Rigidbody>().angularVelocity = -Controller.angularVelocity;
            }
            manoOcupada = false;
            tipoObjetoMano = TipoObjetoMano.Ninguno;
            objetoEnMano = null;
        }

        public void OnTriggerEnter(Collider other)
        {
            EstablecerObjetoCOlisionando(other);
        }

        public void OnTriggerStay(Collider other)
        {
            EstablecerObjetoCOlisionando(other);
        }

        public void OnTriggerExit(Collider other)
        {
            if (!objetoColisionando)
                return;

            objetoColisionando = null;
        }

        private void EstablecerObjetoCOlisionando(Collider col)
        {
            if (objetoColisionando || !col.GetComponent<ObjetoInteractible>())
                return;

            objetoColisionando = col.gameObject;
        }
    }
}
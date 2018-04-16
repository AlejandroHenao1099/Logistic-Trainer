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
        public Action<Interactible> OnHandReady;
        private ControladorInput controladorInput;
        public GameObject objetoEnMano { get; set; }

        public bool manoLista { get; private set; }
        private bool manoOcupada;

        private void Awake()
        {
            controladorInput = GetComponent<ControladorInput>();
        }

        private void Update()
        {
            if (manoOcupada == true && controladorInput.triggerPresionado == false)
                if (tipoObjetoMano == TipoObjetoMano.UnaMano)
                    SoltarObjeto();

        }

        private void EstablecerTipoObjetoDobleMano(Interactible interactible)
        {
            AgarrarObjeto(interactible.gameObject);
        }

        private void EstablecerTipoObjetoIndividual(Interactible interactible)
        {
            manoLista = true;
            objetoEnMano = interactible.gameObject;
            OnHandReady(interactible);
        }

        private void AgarrarObjeto(GameObject objeto)
        {
            FixedJoint fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = objeto.GetComponent<Rigidbody>();
            fixedJoint.breakForce = 20000f;
            fixedJoint.breakTorque = 20000f;
            manoOcupada = true;
            tipoObjetoMano = TipoObjetoMano.UnaMano;
        }

        private void SoltarObjeto()
        {
            FixedJoint fixedJoint = gameObject.GetComponent<FixedJoint>();
            Rigidbody rigObjeto = fixedJoint.connectedBody;
            Rigidbody rigMano = GetComponent<Rigidbody>();
            Destroy(fixedJoint);

            rigObjeto.velocity = rigMano.velocity;
            rigObjeto.angularVelocity = rigMano.angularVelocity;
            manoOcupada = false;
            tipoObjetoMano = TipoObjetoMano.Ninguno;
        }

        private void OnTriggerStay(Collider other)
        {
            if (manoOcupada == false)
            {
                if (controladorInput.triggerPresionado == true)
                {
                    Interactible interactible = other.transform.GetComponent<Interactible>();

                    if (interactible != null && interactible.agarreDobleMano == true)
                        EstablecerTipoObjetoDobleMano(interactible);
                    else if (interactible != null && interactible.agarreDobleMano == false)
                        EstablecerTipoObjetoIndividual(interactible);
                }
            }
        }
    }
}
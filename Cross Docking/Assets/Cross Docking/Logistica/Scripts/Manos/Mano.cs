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
        public Action<ObjetoInteractible> OnGrabObjTwoControl;
        public Action<ObjetoInteractible, Transform> OnGrabObjOneControl;
        public Action OnReleaseObjOneControl;
        public GameObject objetoEnMano { get; set; }

        private GameObject objetoColisionando;

        public bool manoLista { get; private set; }

        private void Awake()
        {
            controladorInput = GetComponent<ControladorInput>();
        }

        private void Update()
        {
            if (controladorInput.Controller.GetHairTriggerDown())
                if (objetoColisionando)
                    DeterminarAgarreObjeto();

            if (controladorInput.Controller.GetHairTriggerUp())
                if (objetoEnMano)
                    DeterminarSoltarObjeto();
        }

        private void DeterminarAgarreObjeto()
        {
            ObjetoInteractible interactible = objetoColisionando.transform.GetComponent<ObjetoInteractible>();

            if (interactible.tipoDeAgarreObjeto == TipoDeAgarre.DosManos)
                AgarrarObjetoDosManos(interactible);

            else if (interactible.tipoDeAgarreObjeto == TipoDeAgarre.UnaMano)
            {
                if (interactible.objetoDisponible)
                    AgarrarObjetoUnaMano();
            }
            // else if (interactible != null && interactible.tipoDeAgarreObjeto == TipoDeAgarre.Ambos)
            //     AgarrarObjetoAmbasManos(interactible);
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
            OnGrabObjTwoControl(interactible);
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


            if (objetoEnMano.GetComponent<ObjetoInteractible>().tipoDeMovilidadObjeto == TipoDeMovilidad.Libre)
                OnGrabObjOneControl(objetoEnMano.GetComponent<ObjetoInteractible>(), transform);
            else
                OnGrabObjOneControl(objetoEnMano.GetComponent<ObjetoInteractible>(), transform);
        }

        private void AgarrarObjetoAmbasManos(ObjetoInteractible interactible)
        {
            // manoLista = true;
            // objetoEnMano = interactible.gameObject;
            // tipoObjetoMano = TipoObjetoMano.DosManos;
            // OnHandReady(interactible);
        }

        private void SoltarObjetoUnaMano()
        {
            OnReleaseObjOneControl();
            tipoObjetoMano = TipoObjetoMano.Ninguno;
            objetoEnMano = null;
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
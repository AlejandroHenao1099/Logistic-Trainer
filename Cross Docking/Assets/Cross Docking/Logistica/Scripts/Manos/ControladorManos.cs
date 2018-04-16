using UnityEngine;

namespace Cross_Docking
{
    public class ControladorManos : MonoBehaviour
    {
        [SerializeField] private Mano derecha;
        [SerializeField] private Mano izquierda;

        private ControladorPosicionManos controladorPosicionManos;

        private Transform posicionDerecha;
        private Transform posicionIzquierda;

        private bool objetoEnMano;
        private bool vectorManosAgregado;

        private void Awake()
        {
            controladorPosicionManos = GetComponent<ControladorPosicionManos>();
            derecha.OnHandReady += VerificarManos;
            izquierda.OnHandReady += VerificarManos;
        }

        private void Update()
        {
            VerificarAgarreObjeto();
        }

        private void VerificarManos(Interactible interactible)
        {
            if (objetoEnMano == false && (derecha.manoLista && izquierda.manoLista))
            {
                if (derecha.objetoEnMano == izquierda.objetoEnMano)
                {
                    controladorPosicionManos.ActivarActualizacion(derecha.objetoEnMano.transform);
                    objetoEnMano = true;
                }
            }
        }

        private void VerificarAgarreObjeto()
        {
            if (objetoEnMano)
            {
                Vector3 posDerecha = posicionDerecha.position;
                Vector3 posIzquierda = posicionIzquierda.position;
                if (Vector3.Distance(posDerecha, posIzquierda) > 1.5f)
                {
                    SoltarObjetoDobleMano();
                }
            }
        }

        private void SoltarObjetoDobleMano()
        {

        }
    }
}
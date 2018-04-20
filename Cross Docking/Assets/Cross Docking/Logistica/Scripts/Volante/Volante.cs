using UnityEngine;

namespace Cross_Docking
{
    public class Volante : ObjetoInteractible
    {
        private Transform direccionVolante;
        private Transform padreVolante;
        [HideInInspector] public Transform derecha, izquierda;

        public float anguloY;
        private bool derechaLista, izquierdaLista;
        private bool manejando;

        private void Start()
        {
            direccionVolante = transform.GetChild(0).GetChild(0);
            padreVolante = transform.GetChild(0);
        }

        private void Update()
        {
            if (!manejando)
                return;

            CalularRotacionVolante();
        }

        public override void Iniciar()
        {
            Quaternion rotacion = direccionVolante.rotation;
            CalularRotacionVolante();
            direccionVolante.rotation = rotacion;
            manejando = true;
        }

        public override void Detener()
        {
            manejando = false;
        }

        private void CalularRotacionVolante()
        {
            Vector3 posicionHijo = direccionVolante.localPosition;

            Vector3 posicionLocalDerecha = direccionVolante.InverseTransformPoint(derecha.position);
            Vector3 posicionLocalIzquierda = direccionVolante.InverseTransformPoint(izquierda.position);
            posicionLocalDerecha.y = posicionLocalIzquierda.y = posicionHijo.y;

            Vector3 posicionGlobalDerecha = direccionVolante.TransformPoint(posicionLocalDerecha);
            Vector3 posicionGlobalIzquierda = direccionVolante.TransformPoint(posicionLocalIzquierda);

            Vector3 direccionVer = (posicionGlobalIzquierda - posicionGlobalDerecha).normalized;

            padreVolante.rotation = Quaternion.LookRotation(direccionVer);

            //anguloY = direccionVolante.localRotation.eulerAngles.y;
            float angulo = Vector3.SignedAngle(direccionVolante.forward, transform.forward, direccionVolante.up);
            anguloY = angulo;

            Vector3 eulerRotacion = padreVolante.rotation.eulerAngles;
            eulerRotacion.x = 0f;
            Quaternion rotacionFinal = Quaternion.Euler(eulerRotacion);
            padreVolante.localRotation = rotacionFinal;
        }

        private void VerificarControles(Transform objetoEntrante)
        {
            if (!manejando)
            {
                if (objetoEntrante == derecha)
                    derechaLista = true;
                else if (objetoEntrante == izquierda)
                    izquierdaLista = true;

                if (derechaLista && izquierdaLista)
                    Iniciar();
            }
            else if (manejando)
            {
                if (objetoEntrante == derecha)
                    derechaLista = false;
                else if (objetoEntrante == izquierda)
                    izquierdaLista = false;

                if (!derechaLista || !izquierdaLista)
                    Detener();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (manejando)
                return;
            VerificarControles(other.transform);
        }

        private void OnTriggerStay(Collider other)
        {
            if (manejando)
                return;
            VerificarControles(other.transform);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!manejando)
                return;
            VerificarControles(other.transform);
        }
    }
}
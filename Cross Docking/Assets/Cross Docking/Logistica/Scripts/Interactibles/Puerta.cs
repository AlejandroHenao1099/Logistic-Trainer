using System.Collections;
using UnityEngine;

namespace Cross_Docking
{
    public class Puerta : ObjetoInteractible
    {
        private Transform mano;
        private Transform miTransform;
        private Rigidbody miRigidbody;

        private bool actualizando;

        private void Awake()
        {
            miRigidbody = GetComponent<Rigidbody>();
            miTransform = transform;
			objetoDisponible = true;
        }

        public override void Iniciar(Transform mano)
        {
            objetoDisponible = false;
            this.mano = mano;
            actualizando = true;
        }

        public override void Detener()
        {
            objetoDisponible = true;
            miRigidbody.angularVelocity = -mano.GetComponent<ControladorInput>().Controller.angularVelocity;
            mano = null;
            actualizando = false;
        }

        private void Update()
        {
            if (!actualizando) return;
            Actualizar();
        }

        private void Actualizar()
        {
            Vector3 targetDelta = mano.position - miTransform.position;
            targetDelta.y = 0;

            float diferenciaDeAngulo = Vector3.Angle(miTransform.forward, targetDelta);

            Vector3 cross = Vector3.Cross(miTransform.forward, targetDelta);

            miRigidbody.angularVelocity = cross * diferenciaDeAngulo * 50f;
        }
    }
}
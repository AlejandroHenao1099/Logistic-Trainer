using UnityEngine;

namespace Cross_Docking
{
    public class Caja : ObjetoInteractible
    {
        private Transform mano;
        private Rigidbody miRigidbody;

        private bool fixedAgregado;

        private void Awake()
        {
            miRigidbody = GetComponent<Rigidbody>();
        }
        public override void Iniciar(Transform mano)
        {
            this.mano = mano;
            if (!fixedAgregado)
            {
                FixedJoint fixedJoint = AgregarFixedJoint();
                fixedJoint.connectedBody = miRigidbody;
                fixedAgregado = true;
            }
        }

        private FixedJoint AgregarFixedJoint()
        {
            FixedJoint fx = mano.gameObject.AddComponent<FixedJoint>();
            fx.breakForce = 20000f;
            fx.breakTorque = 20000f;
            return fx;
        }

        public override void Detener()
        {
            fixedAgregado = false;
            if (mano.GetComponent<FixedJoint>())
            {
                mano.GetComponent<FixedJoint>().connectedBody = null;
                Destroy(mano.GetComponent<FixedJoint>());
                ControladorInput controladorInput = mano.GetComponent<ControladorInput>();
                Vector3 velocidad = controladorInput.Controller.velocity;
                velocidad.x = -velocidad.x;
                velocidad.z = -velocidad.z;
                miRigidbody.velocity = velocidad;
                miRigidbody.angularVelocity = -controladorInput.Controller.angularVelocity;
            }
        }
    }
}
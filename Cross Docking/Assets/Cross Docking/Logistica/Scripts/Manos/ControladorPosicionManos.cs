using System;
using UnityEngine;

public enum DireccionUp
{
    Arriba, Abajo
}

namespace Cross_Docking
{
    public class ControladorPosicionManos : MonoBehaviour
    {
        public Transform manoDerecha;
        public Transform manoIzquierda;
        private Action OnUpdate;

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
                objetoMovible = objeto.parent;
                actualizando = true;
                cajaPosicionada = true;
                Comenzar();
            }
        }

        public void DesactivarActualizacion()
        {
            if (cajaPosicionada)
            {
                Terminar();
                actualizando = false;
                objetoMovible = null;
                cajaPosicionada = false;
            }
        }

        public DireccionUp direccionUp;
        private Transform objetoMovible;

        private Vector3 ejeRotacion;
        private Vector3 direccion;

        private float ultimoAnguloZ;
        private float ultimoAnguloX;

        [Range(0f, 0.95f)]
        public float puntoLimite = 0.8f;

        private float anguloEjeY;
        private float anguloEjeZ;
        private float anguloEjeX;

        private bool actualizarRotacion = true;
        private bool anguloObtenido;
        private bool interpolar = true;

        private float punto;

        private void Comenzar()
        {
            objetoMovible.GetComponent<Rigidbody>().isKinematic = true;
            Vector3 posHijo = objetoMovible.GetChild(0).position;
            Quaternion rotHijo = objetoMovible.GetChild(0).rotation;
            interpolar = false;
            ActualizarPosicion();
            ActualizarRotacion();
            interpolar = true;
            objetoMovible.GetChild(0).position = posHijo;
            objetoMovible.GetChild(0).rotation = rotHijo;
            OnUpdate += Actualizar;
        }

        private void Terminar()
        {
            Vector3 posicion = objetoMovible.GetChild(0).position;
            Quaternion rotacion = objetoMovible.GetChild(0).rotation;
            objetoMovible.position = posicion;
            objetoMovible.rotation = rotacion;
            objetoMovible.GetChild(0).localPosition = Vector3.zero;
            objetoMovible.GetChild(0).localRotation = Quaternion.identity;
            objetoMovible.GetComponent<Rigidbody>().isKinematic = false;
            OnUpdate -= Actualizar;
        }

        private void Actualizar()
        {
            ActualizarPosicion();
            ActualizarRotacion();
        }

        private void ActualizarPosicion()
        {
            objetoMovible.position = Vector3.Lerp(manoDerecha.position, manoIzquierda.position, 0.5f);
        }

        private void ActualizarRotacion()
        {
            direccion = (manoDerecha.position - manoIzquierda.position).normalized;
            VerificarAnguloCartesianoEjeZ();
            VerificarAnguloCartesianoEjeX();
            PoderVerificar();

            if (actualizarRotacion)
            {
                if (direccionUp == DireccionUp.Arriba)
                    VerificarAnguloCartesianoGlobalEjeY();
                else if (direccionUp == DireccionUp.Abajo)
                    VerificarAnguloCartesianoEjeYInverso();

                Vector3 arriba = Quaternion.Euler(ejeRotacion.normalized) * direccion;
                Quaternion rotacionRelativa = Quaternion.LookRotation(direccion, arriba);
                if (interpolar)
                    objetoMovible.rotation = Quaternion.Slerp(objetoMovible.rotation, rotacionRelativa, Time.deltaTime * 6f);
                else
                    objetoMovible.rotation = rotacionRelativa;
            }
            else
            {
                Quaternion rotacionRelativa = Quaternion.LookRotation(direccion, objetoMovible.up);
                objetoMovible.rotation = Quaternion.Slerp(objetoMovible.rotation, rotacionRelativa, Time.deltaTime * 10f);
            }
        }

        private void PoderVerificar()
        {
            punto = Vector3.Dot(direccion, Vector3.up);

            if (punto >= puntoLimite || punto <= -puntoLimite)
            {
                actualizarRotacion = false;
                if (!anguloObtenido)
                {
                    ultimoAnguloZ = anguloEjeZ;
                    ultimoAnguloX = anguloEjeX;
                    anguloObtenido = true;
                }
            }
            else
            {
                actualizarRotacion = true;
                if (anguloObtenido)
                {
                    if (VerificarCambioEje())
                    {
                        CambiarOrientacionOpuesta();
                    }
                    anguloObtenido = false;
                }
            }
        }

        private bool VerificarCambioEje()
        {
            int anteriorAnguloX = ultimoAnguloX >= 0f ? 1 : -1;
            int anteriorAnguloZ = ultimoAnguloZ >= 0f ? 1 : -1;

            int actualAnguloX = anguloEjeX >= 0f ? 1 : -1;
            int actualAnguloZ = anguloEjeZ >= 0f ? 1 : -1;

            if ((actualAnguloX != anteriorAnguloX) || (actualAnguloZ != anteriorAnguloZ))
            {
                return true;
            }
            return false;
        }

        private void CambiarOrientacionOpuesta()
        {
            if (direccionUp == DireccionUp.Arriba)
                direccionUp = DireccionUp.Abajo;
            else
                direccionUp = DireccionUp.Arriba;
        }

        private void VerificarAnguloCartesianoGlobalEjeY()
        {
            Vector3 direccionY = direccion;
            direccionY.y = 0;
            anguloEjeY = Vector3.SignedAngle(direccionY, Vector3.forward, Vector3.up);

            if (anguloEjeY <= 0f && anguloEjeY >= -90f)
                ActualizarEjeCuadranteI(anguloEjeY);
            else if (anguloEjeY >= 0f && anguloEjeY <= 90f)
                ActualizarEjeCuadranteII(anguloEjeY);
            else if (anguloEjeY >= 90f && anguloEjeY <= 180f)
                ActualizarEjeCuadranteIII(anguloEjeY);
            else if (anguloEjeY <= -90f && anguloEjeY >= -180f)
                ActualizarEjeCuadranteIV(anguloEjeY);
        }

        private void ActualizarEjeCuadranteI(float angulo)
        {
            float interpolacion = Mathf.Abs(angulo) / 90f;
            Vector3 vectorEjeX = new Vector3(-90f, 0f, 0f);
            Vector3 vectorEjeZ = new Vector3(0f, 0f, 90f);

            Vector3 vectorFinal = Vector3.Lerp(vectorEjeX, vectorEjeZ, interpolacion);
            ejeRotacion = vectorFinal;
        }

        private void ActualizarEjeCuadranteII(float angulo)
        {
            float interpolacion = Mathf.Abs(angulo) / 90f;
            Vector3 vectorEjeX = new Vector3(-90f, 0f, 0f);
            Vector3 vectorEjeZ = new Vector3(0f, 0f, -90f);

            Vector3 vectorFinal = Vector3.Lerp(vectorEjeX, vectorEjeZ, interpolacion);
            ejeRotacion = vectorFinal;
        }

        private void ActualizarEjeCuadranteIII(float angulo)
        {
            float interpolacion = (Mathf.Abs(angulo) - 90f) / 90f;
            Vector3 vectorEjeX = new Vector3(90f, 0f, 0f);
            Vector3 vectorEjeZ = new Vector3(0f, 0f, -90f);

            Vector3 vectorFinal = Vector3.Lerp(vectorEjeZ, vectorEjeX, interpolacion);
            ejeRotacion = vectorFinal;
        }

        private void ActualizarEjeCuadranteIV(float angulo)
        {
            float interpolacion = (Mathf.Abs(angulo) - 90f) / 90f;
            Vector3 vectorEjeX = new Vector3(90f, 0f, 0f);
            Vector3 vectorEjeZ = new Vector3(0f, 0f, 90f);

            Vector3 vectorFinal = Vector3.Lerp(vectorEjeZ, vectorEjeX, interpolacion);
            ejeRotacion = vectorFinal;
        }

        private void VerificarAnguloCartesianoEjeZ()
        {
            Vector3 direccionZ = direccion;
            direccionZ.z = 0;
            anguloEjeZ = Vector3.SignedAngle(direccionZ, Vector3.up, -Vector3.forward);
        }

        private void VerificarAnguloCartesianoEjeX()
        {
            Vector3 direccionX = direccion;
            direccionX.x = 0;
            anguloEjeX = Vector3.SignedAngle(direccionX, Vector3.up, Vector3.right);
        }

        private void VerificarAnguloCartesianoEjeYInverso()
        {
            Vector3 direccionInversaY = direccion;
            direccionInversaY.y = 0;
            anguloEjeY = Vector3.SignedAngle(direccionInversaY, Vector3.forward, Vector3.up);

            if (anguloEjeY <= 0f && anguloEjeY >= -90f)
                ActualizarEjeCuadranteIDown(anguloEjeY);
            else if (anguloEjeY >= 0f && anguloEjeY <= 90f)
                ActualizarEjeCuadranteIIDown(anguloEjeY);
            else if (anguloEjeY >= 90f && anguloEjeY <= 180f)
                ActualizarEjeCuadranteIIIDown(anguloEjeY);
            else if (anguloEjeY <= -90f && anguloEjeY >= -180f)
                ActualizarEjeCuadranteIVDown(anguloEjeY);
        }

        private void ActualizarEjeCuadranteIDown(float angulo)
        {
            float interpolacion = Mathf.Abs(angulo) / 90f;
            Vector3 vectorEjeX = new Vector3(90f, 0f, 0f);
            Vector3 vectorEjeZ = new Vector3(0f, 0f, -90f);

            Vector3 vectorFinal = Vector3.Lerp(vectorEjeX, vectorEjeZ, interpolacion);
            ejeRotacion = vectorFinal;
        }

        private void ActualizarEjeCuadranteIIDown(float angulo)
        {
            float interpolacion = Mathf.Abs(angulo) / 90f;
            Vector3 vectorEjeX = new Vector3(90f, 0f, 0f);
            Vector3 vectorEjeZ = new Vector3(0f, 0f, 90f);

            Vector3 vectorFinal = Vector3.Lerp(vectorEjeX, vectorEjeZ, interpolacion);
            ejeRotacion = vectorFinal;
        }

        private void ActualizarEjeCuadranteIIIDown(float angulo)
        {
            float interpolacion = (Mathf.Abs(angulo) - 90f) / 90f;
            Vector3 vectorEjeX = new Vector3(-90f, 0f, 0f);
            Vector3 vectorEjeZ = new Vector3(0f, 0f, 90f);

            Vector3 vectorFinal = Vector3.Lerp(vectorEjeZ, vectorEjeX, interpolacion);
            ejeRotacion = vectorFinal;
        }

        private void ActualizarEjeCuadranteIVDown(float angulo)
        {
            float interpolacion = (Mathf.Abs(angulo) - 90f) / 90f;
            Vector3 vectorEjeX = new Vector3(-90f, 0f, 0f);
            Vector3 vectorEjeZ = new Vector3(0f, 0f, -90f);

            Vector3 vectorFinal = Vector3.Lerp(vectorEjeZ, vectorEjeX, interpolacion);
            ejeRotacion = vectorFinal;
        }
    }
}
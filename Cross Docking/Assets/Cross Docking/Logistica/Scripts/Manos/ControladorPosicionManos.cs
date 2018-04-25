using System;
using System.Collections;
using UnityEngine;

public enum Cuadrantes
{
    I, II, III, IV,
}

namespace Cross_Docking
{
    public class ControladorPosicionManos : MonoBehaviour
    {
        public Transform manoDerecha;
        public Transform manoIzquierda;
        private Transform objetoAMover;

        private Action OnUpdate;

        private Vector3 vectorForwardEstable;

        private bool actualizando;
        private bool cajaPosicionada;

        public bool formaEstable;

        private void Update()
        {
            if (!actualizando) return;
            if (OnUpdate != null) OnUpdate();
        }

        public void ActivarActualizacion(Transform objeto)
        {
            if (formaEstable)
            {
                if (!cajaPosicionada)
                {
                    objetoAMover = objeto.parent;
                    objetoAMover.GetComponent<Rigidbody>().isKinematic = true;
                    actualizando = true;

                    cajaPosicionada = true;

                    CalcularPosicionInicial();
                    OnUpdate += CalcularDireccionVectorMedio;
                }
            }
            else
            {
                if (!cajaPosicionada)
                {
                    objetoAMover = objeto;
                    actualizando = true;
                    cajaPosicionada = true;

                    Comenzar();
                }
            }

        }

        public void DesactivarActualizacion()
        {
            if (formaEstable)
            {
                if (cajaPosicionada)
                {
                    OnUpdate -= CalcularDireccionVectorMedio;
                    actualizando = false;
                    Vector3 posicion = objetoAMover.GetChild(0).position;
                    Quaternion rotacion = objetoAMover.GetChild(0).rotation;
                    objetoAMover.position = posicion;
                    objetoAMover.rotation = rotacion;
                    objetoAMover.GetChild(0).localPosition = Vector3.zero;
                    objetoAMover.GetChild(0).localRotation = Quaternion.identity;
                    objetoAMover.GetComponent<Rigidbody>().isKinematic = false;
                    objetoAMover = null;
                    cajaPosicionada = false;
                }
            }
            else
            {
                if (cajaPosicionada)
                {
                    Terminar();
                    actualizando = false;
                    objetoAMover = null;
                    cajaPosicionada = false;
                }
            }
        }

        private void CalcularPosicionInicial()
        {
            vectorForwardEstable = (manoIzquierda.position - manoDerecha.position).normalized;
            vectorForwardEstable += manoDerecha.position;
            Vector3 posicionMedia = Vector3.Lerp(manoDerecha.position, manoIzquierda.position, 0.5f);
            Vector3 posicion = objetoAMover.GetChild(0).position;
            Quaternion rotacion = objetoAMover.GetChild(0).rotation;
            objetoAMover.position = posicionMedia;
            objetoAMover.LookAt(vectorForwardEstable);
            objetoAMover.GetChild(0).position = posicion;
            objetoAMover.GetChild(0).rotation = rotacion;
        }

        private void CalcularDireccionVectorMedio()
        {
            vectorForwardEstable = (manoIzquierda.position - manoDerecha.position).normalized;
            vectorForwardEstable += manoDerecha.position;
            Vector3 posicionMedia = Vector3.Lerp(manoDerecha.position, manoIzquierda.position, 0.5f);
            objetoAMover.position = posicionMedia;

            objetoAMover.LookAt(vectorForwardEstable);
        }

        private void CalcularDireccion()
        {

        }




        private Cuadrantes cuadranteDerecha;
        private Cuadrantes cuadranteIzquierda;
        private Transform padreCubo, cubo;
        private Transform cabeza;

        private Transform transformPadre;

        private IEnumerator corutina;
        private WaitUntil waitUntilCompleteRotation;

        private float interpolacionCubo;
        private float dot;
        private float puntoLimite = 0.8f;

        private Quaternion rotacionCubo;

        private bool apuntarDown;
        private bool apuntarUp;
        private bool apuntarForward = true;
        private bool apuntarBack;

        private bool interpolar = false;

        private Vector3 vectorForward = Vector3.zero;

        private void Awake()
        {
            waitUntilCompleteRotation = new WaitUntil(InterRotacionCubo);
            cabeza = manoDerecha.parent.GetChild(2);
        }

        private void Comenzar()
        {
            cubo = objetoAMover;
            padreCubo = cubo.parent;
            transformPadre = padreCubo.parent;

            transformPadre.GetComponent<Rigidbody>().isKinematic = true;
            OnUpdate += ActualizarUpdate;
        }

        private void Terminar()
        {
            transformPadre.GetComponent<Rigidbody>().isKinematic = false;
            transformPadre = null;
            padreCubo = null;
            cubo = null;
            OnUpdate -= ActualizarUpdate;
        }

        /*IMPORTANTE NO BORRAR
        Para calcular los giros que ha dado el volante se calcula con vector3.SignedAngle, ahora dependiendo de su valor se le asignara un cuadrante
        como en el plano cartesiano. P/e Si el valor es 10, entonces su cuadrante es el 2 porque estaria adelante y a la izquierda del objeto, 
        y para verificar los ciclos, se pregunta sobre el cuadrante anterior, si es el 4 el anterior(que seria el ultimo) y avanza al primero
        entonces cuenta como un ciclo, y se resta un ciclo si ocurre lo contrario
         */

        private void ActualizarUpdate()
        {
            CalcularPlanoCartesiano();
            Vector3 posRight = manoDerecha.position;
            Vector3 posLeft = manoIzquierda.position;

            Vector3 vectorX = (posRight - posLeft).normalized;

            dot = Vector3.Dot(vectorX, Vector3.up);

            vectorForward = (posRight - posLeft).normalized;

            Vector3 halfPos = Vector3.Lerp(manoDerecha.position, manoIzquierda.position, 0.5f);
            transformPadre.position = halfPos;

            Vector3 directionForward = Vector3.zero;

            Vector3 posRelRight = posRight;
            Vector3 posRelLeft = posLeft;
            posRelRight.y = posRelLeft.y = transformPadre.position.y;
            DefinirEjeRotacion(dot);

            CalcularEjesCubo(posRight, posLeft, ref directionForward);

            Vector3 direccionDerecha = (manoIzquierda.position - manoDerecha.position).normalized;
            Vector3 direccionArriba = Vector3.Cross(directionForward, direccionDerecha);

            padreCubo.rotation = Quaternion.LookRotation(directionForward, direccionArriba);

            if (interpolar)
            {
                interpolar = false;
                cubo.rotation = rotacionCubo;
                if (corutina != null) StopCoroutine(corutina);
                corutina = InterpolarRotacion();
                interpolacionCubo = 0;
                StartCoroutine(corutina);
            }
        }

        private void CalcularPlanoCartesiano()
        {
            Vector3 posicionPadre = transformPadre.position;
            Vector3 posicionCabeza = cabeza.position;
            Vector3 posicionDerecha = manoDerecha.position;
            Vector3 posicionIzquierda = manoIzquierda.position;
            posicionDerecha.y = posicionIzquierda.y = posicionCabeza.y = posicionPadre.y;

            Vector3 planoY = (posicionPadre - posicionCabeza);
            Vector3 planoX = Quaternion.Euler(0, 90f, 0) * planoY;
            Vector3 planoZ = Vector3.Cross(planoY, planoX);

            Vector3 posicionDerechaSobreElPlano = (posicionDerecha - posicionPadre).normalized;
            float anguloDerecha = Vector3.SignedAngle(posicionDerechaSobreElPlano, planoY, planoZ);

            Vector3 posicionIzquierdaSobreElPlano = (posicionIzquierda - posicionPadre).normalized;
            float anguloIzquierda = Vector3.SignedAngle(posicionIzquierdaSobreElPlano, planoY, planoZ);

            VerificarCuadrante(anguloDerecha, ref cuadranteDerecha);
            VerificarCuadrante(anguloIzquierda, ref cuadranteIzquierda);
        }

        private void VerificarCuadrante(float angulo, ref Cuadrantes cuadrante)
        {
            if (angulo > -90f && angulo < 0f)
                cuadrante = Cuadrantes.I;
            else if (angulo > 0f && angulo < 90f)
                cuadrante = Cuadrantes.II;
            else if (angulo > 90f && angulo < 180f)
                cuadrante = Cuadrantes.III;
            else if (angulo > -180f && angulo < -90f)
                cuadrante = Cuadrantes.IV;
        }

        private void DefinirEjeRotacion(float punto)
        {
            if (punto > puntoLimite && !apuntarUp)
            {
                interpolar = true;
                rotacionCubo = cubo.rotation;
                apuntarUp = true;
                apuntarForward = apuntarBack = apuntarDown = false;
            }
            else if (punto < -puntoLimite && !apuntarDown)
            {
                interpolar = true;
                rotacionCubo = cubo.rotation;
                apuntarDown = true;
                apuntarUp = apuntarForward = apuntarBack = false;
            }

            else if ((punto < puntoLimite && punto > -puntoLimite) && (cuadranteDerecha == Cuadrantes.I || cuadranteDerecha == Cuadrantes.IV) && !apuntarBack && !apuntarForward)
            {
                interpolar = true;
                rotacionCubo = cubo.rotation;
                apuntarForward = true;
                apuntarUp = apuntarBack = apuntarDown = false;
            }
            else if ((punto < puntoLimite && punto > -puntoLimite) && (cuadranteDerecha == Cuadrantes.II || cuadranteDerecha == Cuadrantes.III) && !apuntarForward && !apuntarBack)
            {
                interpolar = true;
                rotacionCubo = cubo.rotation;
                apuntarBack = true;
                apuntarUp = apuntarForward = apuntarDown = false;
            }
        }

        private void CalcularEjesCubo(Vector3 posRight, Vector3 posLeft, ref Vector3 directionForward)
        {
            if (apuntarForward)
            {
                transformPadre.forward = vectorForward;

                posRight.y = posLeft.y = transformPadre.position.y;
                directionForward = (posLeft - posRight).normalized;
                directionForward = Quaternion.Euler(0, -90, 0) * directionForward;
            }
            else if (apuntarUp)
            {
                transformPadre.up = vectorForward;

                posRight.x = posLeft.x = transformPadre.position.x;
                directionForward = (posLeft - posRight).normalized;
                directionForward = Quaternion.Euler(90, 0, 0) * directionForward;
            }
            else if (apuntarDown)
            {
                transformPadre.up = -vectorForward;

                posRight.x = posLeft.x = transformPadre.position.x;
                directionForward = (posLeft - posRight).normalized;
                directionForward = Quaternion.Euler(-90, 0, 0) * directionForward;
            }
            else if (apuntarBack)
            {
                transformPadre.forward = -vectorForward;

                posRight.y = posLeft.y = transformPadre.position.y;
                directionForward = (posLeft - posRight).normalized;
                directionForward = Quaternion.Euler(0, 90, 0) * directionForward;
            }
        }

        private IEnumerator InterpolarRotacion()
        {
            yield return waitUntilCompleteRotation;
        }

        private bool InterRotacionCubo()
        {
            if (cubo == null) return true;
            Quaternion interpolacion = Quaternion.Slerp(cubo.rotation, padreCubo.rotation, interpolacionCubo);
            cubo.rotation = interpolacion;
            interpolacionCubo += 0.01f;
            if (interpolacionCubo >= 1)
            {
                interpolacionCubo = 1;
                interpolacion = Quaternion.Slerp(cubo.rotation, padreCubo.rotation, interpolacionCubo);
                cubo.rotation = interpolacion;
                return true;
            }
            return false;
        } 
    }
}
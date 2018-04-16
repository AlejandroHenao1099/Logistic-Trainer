using System.Collections;
using UnityEngine;

public class ControladorManos : MonoBehaviour
{
    [SerializeField] private Mano derecha;
    [SerializeField] private Mano izquierda;

    [SerializeField] private FisicaMano fisicaDerecha;
    [SerializeField] private FisicaMano fisicaIzquierda;

    [SerializeField] private ControladorInput inputDerecho;
    [SerializeField] private ControladorInput inputIzquierdo;

    private Transform posicionDerecha;
    private Transform posicionIzquierda;

    private bool objetoEnMano;
    private bool vectorManosAgregado;

    private bool derechaCerrada;
    private bool izquierdaCerrada;

    private void Awake()
    {
        posicionDerecha = fisicaDerecha.transform;
        posicionIzquierda = fisicaIzquierda.transform;

        derecha.OnHandReady += VerificarManos;
        izquierda.OnHandReady += VerificarManos;
    }

    private void Update()
    {
        VerificarInputsCerrar();
        VerificarInputsAbrir();
        VerificarAgarreObjeto();
    }

    private void VerificarInputsCerrar()
    {
        //if (Input.GetKeyDown(KeyCode.R) && derechaCerrada == false)
        if(inputDerecho.triggerPresionado && derechaCerrada == false)
        {
            derechaCerrada = true;
            derecha.ActivarDedos();
        }

        //if (Input.GetKeyDown(KeyCode.L) && izquierdaCerrada == false)
        if(inputIzquierdo.triggerPresionado && izquierdaCerrada == false)
        {
            izquierdaCerrada = true;
            izquierda.ActivarDedos();
        }
    }

    private void VerificarInputsAbrir()
    {
        //if (Input.GetKeyUp(KeyCode.E) && derechaCerrada == true && objetoEnMano == true)
        if (!inputDerecho.triggerPresionado && derechaCerrada == true && objetoEnMano == true)
        {
            derechaCerrada = false;
            SoltarDerecha();
            derecha.NormalizarDedos();
        }
        //else if (Input.GetKeyUp(KeyCode.E) && derechaCerrada == true)
        else if (!inputDerecho.triggerPresionado && derechaCerrada == true)
        {
            derechaCerrada = false;
            derecha.NormalizarDedos();
        }
        //if (Input.GetKeyUp(KeyCode.K) && izquierdaCerrada == true && objetoEnMano == true)
        if (!inputIzquierdo.triggerPresionado && izquierdaCerrada == true && objetoEnMano == true)
        {
            izquierdaCerrada = false;
            SoltarIzquierda();
            izquierda.NormalizarDedos();
        }
        //else if (Input.GetKeyUp(KeyCode.K) && izquierdaCerrada == true)
        else if (!inputIzquierdo.triggerPresionado && izquierdaCerrada == true)
        {
            izquierdaCerrada = false;
            izquierda.NormalizarDedos();
        }
    }

    private void VerificarManos()
    {
        if (objetoEnMano == false && (derecha.manoLista && izquierda.manoLista))
        {
            if (fisicaDerecha.objetoTrigger == fisicaIzquierda.objetoTrigger)
            {
                DesactivarMano();
                objetoEnMano = true;
            }
        }
    }

    private void DesactivarMano()
    {
        if (!vectorManosAgregado)
        {
            fisicaDerecha.AgarrarObjeto();
            fisicaIzquierda.AgarrarObjeto();
            vectorManosAgregado = true;
        }
    }

    private void VerificarAgarreObjeto()
    {
        if (objetoEnMano)
        {
            Vector3 posDerecha = posicionDerecha.position;
            Vector3 posIzquierda = posicionIzquierda.position;
            if (Vector3.Distance(posDerecha, posIzquierda) > 3)
            {
                SoltarDerecha();
                SoltarIzquierda();
            }
        }
    }

    private void SoltarDerecha()
    {
        derecha.ActivarCollidersDedos(false);
        objetoEnMano = false;
        if (vectorManosAgregado)
        {
            fisicaDerecha.SoltarObjeto();
            fisicaIzquierda.SoltarObjeto();
            vectorManosAgregado = false;
        }
        derecha.CambiarColorMano(Color.red);
    }

    private void SoltarIzquierda()
    {
        izquierda.ActivarCollidersDedos(false);
        objetoEnMano = false;
        if (vectorManosAgregado)
        {
            fisicaDerecha.SoltarObjeto();
            fisicaIzquierda.SoltarObjeto();
            vectorManosAgregado = false;
        }
        izquierda.CambiarColorMano(Color.red);
    }
}
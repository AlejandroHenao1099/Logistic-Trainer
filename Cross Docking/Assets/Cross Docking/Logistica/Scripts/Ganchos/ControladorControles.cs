using System;
using UnityEngine;

public class ControladorControles : MonoBehaviour
{
    public Action<GameObject> OnGrabObject;
    public Action OnControlsFar;

    public DetectorTrigger detectorDerecho;
    public DetectorTrigger detectorIzquierdo;
    public FisicasControl fisicaIzquierda;
    public FisicasControl fisicaDerecha;

    private GameObject primerObjetoEncontrado;
    private Transform transGanDer;
    private Transform transGanIzq;

    private bool objetoAgarrado;

    private void Awake()
    {
        transGanDer = fisicaDerecha.transform;
        transGanIzq = fisicaIzquierda.transform;

        detectorDerecho.OnCollideObject += CompararObjsCol;
        detectorIzquierdo.OnCollideObject += CompararObjsCol;

        detectorDerecho.OnLostObject += EliminarReferencia;
        detectorIzquierdo.OnLostObject += EliminarReferencia;
    }

    private void Update()
    {
        if (objetoAgarrado == true)
            VerificarDistanciaControles();
    }

    private void VerificarDistanciaControles()
    {
        float distanciaControles = Vector3.Distance(transGanDer.position, transGanIzq.position);
        if (distanciaControles > 3 && OnControlsFar != null)
            OnControlsFar();
    }

    private void CompararObjsCol(GameObject objetoColisionado)
    {
        if (primerObjetoEncontrado == null)
            primerObjetoEncontrado = objetoColisionado;
        else if (primerObjetoEncontrado == objetoColisionado)
        {
			objetoAgarrado = true;
            AgarrarObjeto();
        }
    }

    private void EliminarReferencia(GameObject objetoPerdido)
    {
        if (primerObjetoEncontrado != null && primerObjetoEncontrado == objetoPerdido)
            primerObjetoEncontrado = null;
    }

    private void AgarrarObjeto()
    {
        fisicaDerecha.AgarrarObjetoFJ(primerObjetoEncontrado);
        fisicaIzquierda.AgarrarObjetoFJ(primerObjetoEncontrado);
        detectorDerecho.puedoDetectar = false;
        detectorIzquierdo.puedoDetectar = false;
        if (OnGrabObject != null)
            OnGrabObject(primerObjetoEncontrado);
    }

	public void SoltarObjeto()
	{
		fisicaIzquierda.SoltarObjetoFJ(primerObjetoEncontrado);
		fisicaDerecha.SoltarObjetoFJ(primerObjetoEncontrado);
        detectorDerecho.ReiniciarDetector();
        detectorIzquierdo.ReiniciarDetector();
		primerObjetoEncontrado = null;
		objetoAgarrado = false;
	}
}
using System;
using System.Collections;
using UnityEngine;

public class Mano : MonoBehaviour
{
    public Action OnHandReady;

    private TriggerDedo[] collidersTrigger = new TriggerDedo[5];
    private Collider[] collidersDedos = new Collider[5];
    private Dedo[] dedos = new Dedo[5];
    private Renderer[] materialesMano;

    private int[] indicesDedos = new int[5];
    private int indiceDedoActual;

    public bool manoLista { get; private set; }

    private void Awake()
    {
        collidersTrigger = GetComponentsInChildren<TriggerDedo>();
        materialesMano = GetComponentsInChildren<Renderer>();
        dedos = GetComponentsInChildren<Dedo>();
        AsignarAcciones();
        CLonarCollider();
        CambiarColorMano(Color.red);
        ActivarCollidersDedos(false);
    }

    private void AsignarAcciones()
    {
        for (int i = 0; i < collidersTrigger.Length; i++)
            collidersTrigger[i].OnTouch += VerificarToque;
    }

    private void CLonarCollider()
    {
        for (int i = 0; i < collidersTrigger.Length; i++)
            collidersDedos[i] = collidersTrigger[i].GetComponent<Collider>();
    }

    public void ActivarDedos()
    {
        for (int i = 0; i < dedos.Length; i++)
            dedos[i].Reproducir();
        ActivarCollidersDedos(true);
    }

    public void NormalizarDedos()
    {
        ActivarCollidersDedos(false);
        manoLista = false;
        for (int i = 0; i < dedos.Length; i++)
            dedos[i].ReanudarAnimator();
        CambiarColorMano(Color.red);
    }

    private void VerificarToque(int indice)
    {
        indicesDedos[indiceDedoActual] = indice;
        for (int i = 0; i < indicesDedos.Length; i++)
        {
            if (indicesDedos[i] != 0 && i == 3)
            {
                StartCoroutine(RetrasoMano());
                return;
            }
        }
        indiceDedoActual++;
    }

    private IEnumerator RetrasoMano()
    {
        yield return new WaitForSeconds(0.15f);
        manoLista = true;
        CambiarColorMano(Color.green);
        if (OnHandReady != null)
            OnHandReady();
        indiceDedoActual = 0;
    }

    public void CambiarColorMano(Color colorCambio)
    {
        for (int i = 0; i < materialesMano.Length; i++)
            materialesMano[i].material.color = colorCambio;
    }

    public void ActivarCollidersDedos(bool estadoActual)
    {
        for (int i = 0; i < collidersDedos.Length; i++)
            collidersDedos[i].enabled = estadoActual;
    }
}
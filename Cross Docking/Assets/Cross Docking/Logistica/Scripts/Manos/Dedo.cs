using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dedo : MonoBehaviour
{
    [SerializeField] private TriggerDedo detectorDedo;
    private Animator animacionDedo;
    private int hashFinger = Animator.StringToHash("Doblar");

    private void Awake()
    {
        animacionDedo = GetComponent<Animator>();
        detectorDedo.OnTouchMesh += DetenerAnimator;
    }

    public void Reproducir()
    {
        animacionDedo.SetBool(hashFinger, true);
    }

    private void DetenerAnimator()
    {
        animacionDedo.speed = 0f;
        detectorDedo.GetComponent<Collider>().enabled = false;
    }

    public void ReanudarAnimator()
    {
        animacionDedo.SetBool(hashFinger, false);
        animacionDedo.speed = 1;
        //StartCoroutine(RetrasoActivacion());
    }

    private IEnumerator RetrasoActivacion()
    {
        yield return new WaitForSeconds(0.2f);
        detectorDedo.GetComponent<Collider>().enabled = true;
    }
}
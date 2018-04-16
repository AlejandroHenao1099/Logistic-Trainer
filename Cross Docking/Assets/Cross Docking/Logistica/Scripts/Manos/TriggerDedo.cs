using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerDedo : MonoBehaviour
{
	[SerializeField] private int indice;
    public Action OnTouchMesh;
    public Action<int> OnTouch;

    private void OnTriggerEnter(Collider other)
    {
        OnTouchMesh();
        OnTouch(indice);
    }
}

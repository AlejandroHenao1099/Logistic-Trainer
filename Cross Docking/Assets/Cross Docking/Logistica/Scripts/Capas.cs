using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capas : MonoBehaviour
{

    public Transform objeto1;
    public Transform objeto2;

    public LayerMask capaObj = 1 << 9;

    // Use this for initialization
    void Start()
    {
        int capa1 = objeto1.gameObject.layer;
        int capa2 = objeto2.gameObject.layer;

        int capaObj1 = 1 << capa1;

		print(capaObj == capaObj1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

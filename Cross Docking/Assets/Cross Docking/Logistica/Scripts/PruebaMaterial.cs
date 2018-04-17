using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaMaterial : MonoBehaviour
{

    public GameObject teleport;



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
			print("Red");
            teleport.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
			print("Gren");
            teleport.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
			print("White");
            teleport.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }
}

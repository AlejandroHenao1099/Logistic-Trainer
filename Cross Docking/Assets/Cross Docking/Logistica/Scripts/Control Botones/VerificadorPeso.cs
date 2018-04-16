using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerificadorPeso : MonoBehaviour 
{
	public int peso;

	private void OnCollisionEnter(Collision other)
	{
		if(peso == other.gameObject.GetComponent<Rigidbody>().mass)
		{
			print("Pesos Iguales");
		}
		else 
		{
			print("El peso no es similar");
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Enum
public enum Destino
{
	National,Urban
}
#endregion
public class DestinoCajas : MonoBehaviour 
{
	public Destino destinoCajas;
	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("National") && destinoCajas == Destino.National)
		{
			print("Destination National");
		}
		else if(other.CompareTag("Urban") && destinoCajas == Destino.Urban)
		{
			print("Destination Urban");
		}
		else
		{
			print("Destinantion Fail");
		}
	}
}

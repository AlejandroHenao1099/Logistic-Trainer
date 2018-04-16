using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionNationalUrban : MonoBehaviour 
{
	[Tooltip("Gameobject[0] = camera inicial / Gameobject[1] = camera del jugador en escena / Gameobject[2] = enable cajas nacionales / Gameobject[3] = cajas urbanas")]
	public GameObject[] enableDesable;
	private void Start () 
	{
		enableDesable[0].SetActive(true);
		enableDesable[1].SetActive(false);
		enableDesable[2].SetActive(false);
		enableDesable[3].SetActive(false);
	}

	private void Update()
	{
		ControllerInput();
	}
	
	private void ControllerInput() 
	{
		#region Nacional
		if(Input.GetKey(KeyCode.A))
		{
			enableDesable[0].SetActive(false);
			enableDesable[1].SetActive(true);
			enableDesable[2].SetActive(true);
		}
		#endregion
		#region Urbano
		else if(Input.GetKey(KeyCode.D))
		{
			enableDesable[0].SetActive(false);
			enableDesable[1].SetActive(true);
			enableDesable[3].SetActive(true);
		}
		#endregion
	}
}

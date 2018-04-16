using UnityEngine;

public class GrabingController : MonoBehaviour {

	private ControladorGanchos controladorGanchos;
	private ControladorControles controladorControles;

	private void Awake()
	{
		controladorGanchos = GetComponent<ControladorGanchos>();
		controladorControles = GetComponent<ControladorControles>();
		
		controladorControles.OnGrabObject += controladorGanchos.PosicionarGanchos;
		controladorControles.OnControlsFar += controladorControles.SoltarObjeto;
		controladorControles.OnControlsFar += controladorGanchos.RestaurarGanchos;
	}
}
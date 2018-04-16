using UnityEngine;

public class ControladorGanchos : MonoBehaviour
{
    public GameObject gancho;
    public GameObject ganchoControlIzq;
    public GameObject ganchoControlDer;
    private GameObject ganchoDerecho;
    private GameObject ganchoIzquierdo;
    // private Transform referenciaObjAgarrado;
    // private Transform transGanIzq;
    // private Transform transGanDer;
    // private Vector3 distanciaGanDer;
    // private Vector3 distanciaGanIzq;
    // private bool ganchosActivos;

    private void Awake()
    {
        ganchoDerecho = Instantiate(gancho, transform);
        ganchoIzquierdo = Instantiate(gancho, transform);
        // transGanDer = ganchoDerecho.transform;
        // transGanIzq = ganchoIzquierdo.transform;
        ganchoDerecho.SetActive(false);
        ganchoIzquierdo.SetActive(false);
    }
    // private void Update()
    // {
    // 	ActualizarPosicionGanchos();
    // }
    // private void ActualizarPosicionGanchos()
    // {
    // 	if(ganchosActivos == true)
    // 	{
    // 		transGanDer.position = referenciaObjAgarrado.position + distanciaGanDer;
    // 		transGanIzq.position = referenciaObjAgarrado.position + distanciaGanIzq;
    // 	}
    // }
    public void PosicionarGanchos(GameObject objetoAgarrado)
    {
        //referenciaObjAgarrado = objetoAgarrado.transform;
        ganchoDerecho.transform.SetPositionAndRotation(ganchoControlDer.transform.position, ganchoControlDer.transform.rotation);
        ganchoIzquierdo.transform.SetPositionAndRotation(ganchoControlIzq.transform.position, ganchoControlIzq.transform.rotation);
        //CalcularDistanciaObjeto(objetoAgarrado);
        AgregarFixJoiGancho(ganchoDerecho, objetoAgarrado);
		AgregarFixJoiGancho(ganchoIzquierdo, objetoAgarrado);
        MostrarGanchosFJ();
        //ganchosActivos = true;
    }
    private void AgregarFixJoiGancho(GameObject ganchoFJ, GameObject objetoAgarrado)
    {
        FixedJoint fixedJoint = ganchoFJ.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = objetoAgarrado.GetComponent<Rigidbody>();
        fixedJoint.breakForce = 10000;
        fixedJoint.breakTorque = 10000;
    }
    // private void CalcularDistanciaObjeto(GameObject objetoSeguir)
    // {
    //     distanciaGanDer = transGanDer.position - objetoSeguir.transform.position;
    //     distanciaGanIzq = transGanIzq.position - objetoSeguir.transform.position;
    // }
    private void MostrarGanchosFJ()
    {
        ganchoDerecho.SetActive(true);
        ganchoIzquierdo.SetActive(true);
        ControlarRendererGanchos(false);
    }

    private void ControlarRendererGanchos(bool activado)
    {
        MeshRenderer[] meshRenderersDer = ganchoControlDer.GetComponentsInChildren<MeshRenderer>();
        MeshRenderer[] meshRenderersIzq = ganchoControlIzq.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenderersDer.Length; i++)
        {
            meshRenderersIzq[i].enabled = activado;
            meshRenderersDer[i].enabled = activado;
        }
    }

	public void RestaurarGanchos()
	{
		ganchoDerecho.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        ganchoIzquierdo.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		QuitarFixJoiGancho(ganchoDerecho);
		QuitarFixJoiGancho(ganchoIzquierdo);
        OcultarGanchosFJ();
	}

	private void QuitarFixJoiGancho(GameObject ganchoFJ)
	{
		ganchoFJ.GetComponent<FixedJoint>().connectedBody = null;
		Destroy(ganchoFJ.GetComponent<FixedJoint>());
	}

    private void OcultarGanchosFJ()
    {
        ganchoDerecho.SetActive(false);
        ganchoIzquierdo.SetActive(false);
        ControlarRendererGanchos(true);
    }
}
using UnityEngine;
using System.Collections;

public class ForkController : MonoBehaviour
{

    public Transform fork; //Tenedor
    public float speedTranslate; //Platform travel speed //Velocidad de viaje de la plataforma
    private float guardarVelocidadTranlacion;
    public Vector3 maxY; //The maximum height of the platform //La altura máxima de la plataforma
    public Vector3 minY; //The minimum height of the platform //La altura mínima de la plataforma

    void Start()
    {
        guardarVelocidadTranlacion = speedTranslate;
    }

    private void Update()
    {
        speedTranslate = (fork.localPosition.y >= maxY.y && Input.GetKey(KeyCode.L)) || (fork.localPosition.y <= minY.y && Input.GetKey(KeyCode.K)) ? 0 : guardarVelocidadTranlacion;

        if (Input.GetKey(KeyCode.L))
        {
            fork.Translate(Vector3.up * speedTranslate * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.K))
        {
            fork.Translate(-Vector3.up * speedTranslate * Time.deltaTime);
        }
    }
}

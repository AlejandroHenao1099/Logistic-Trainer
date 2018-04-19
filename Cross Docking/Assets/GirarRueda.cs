using UnityEngine;

public class GirarRueda : MonoBehaviour
{
    public Transform direccionVolante;
    public Transform padreVolante;
    public Transform derecha, izquierda;

    private void Start()
    {
        direccionVolante = transform.GetChild(0).GetChild(0);
        padreVolante = transform.GetChild(0);

        Quaternion rotacion = direccionVolante.rotation;

        Vector3 posicionHijo = direccionVolante.localPosition;

        Vector3 posicionLocalDerecha = direccionVolante.InverseTransformPoint(derecha.position);
        Vector3 posicionLocalIzquierda = direccionVolante.InverseTransformPoint(izquierda.position);
        posicionLocalDerecha.y = posicionLocalIzquierda.y = posicionHijo.y;

        Vector3 posicionGlobalDerecha = direccionVolante.TransformPoint(posicionLocalDerecha);
        Vector3 posicionGlobalIzquierda = direccionVolante.TransformPoint(posicionLocalIzquierda);

        Vector3 direccionVer = (posicionGlobalIzquierda - posicionGlobalDerecha).normalized;

        padreVolante.rotation = Quaternion.LookRotation(direccionVer);

        Vector3 eulerRotacion = padreVolante.rotation.eulerAngles;
        eulerRotacion.x = 0f;
        Quaternion rotacionFinal = Quaternion.Euler(eulerRotacion);
        padreVolante.localRotation = rotacionFinal;

        direccionVolante.rotation = rotacion;
    }

    private void Update()
    {
        Vector3 posicionHijo = direccionVolante.localPosition;

        Vector3 posicionLocalDerecha = direccionVolante.InverseTransformPoint(derecha.position);
        Vector3 posicionLocalIzquierda = direccionVolante.InverseTransformPoint(izquierda.position);
        posicionLocalDerecha.y = posicionLocalIzquierda.y = posicionHijo.y;

        Vector3 posicionGlobalDerecha = direccionVolante.TransformPoint(posicionLocalDerecha);
        Vector3 posicionGlobalIzquierda = direccionVolante.TransformPoint(posicionLocalIzquierda);

        Vector3 direccionVer = (posicionGlobalIzquierda - posicionGlobalDerecha).normalized;

        padreVolante.rotation = Quaternion.LookRotation(direccionVer);

        Vector3 eulerRotacion = padreVolante.rotation.eulerAngles;
        eulerRotacion.x = 0f;
        Quaternion rotacionFinal = Quaternion.Euler(eulerRotacion);
        padreVolante.localRotation = rotacionFinal;
    }

    private void CalcularRotacionVolante()
    {

    }
}
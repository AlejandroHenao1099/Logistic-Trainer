using UnityEngine;

namespace Cross_Docking
{
    public class VerificadorPeso : MonoBehaviour
    {
        public int peso;

        private void OnCollisionEnter(Collision other)
        {
            ObjetoInteractible objetoInteractible = other.transform.GetComponentInChildren<ObjetoInteractible>();
            if (objetoInteractible != null)
            {
                if (peso == objetoInteractible.GetComponentInParent<Rigidbody>().mass)
                    print("Pesos Iguales");
                else
                    print("El peso no es similar");
            }
        }
    }
}
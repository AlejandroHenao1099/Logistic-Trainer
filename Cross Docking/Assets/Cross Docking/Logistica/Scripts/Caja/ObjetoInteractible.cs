using UnityEngine;

namespace Cross_Docking
{
    public class ObjetoInteractible : MonoBehaviour
    {
        [Tooltip("Determina si el objeto debe ser agarrado con los 2 controles o no")]
        public bool agarreDobleMano = false;

        [Tooltip("Determina si el objeto esta fijo, o si se puede mover")]
        public bool objetoMovible = true;
    }
}
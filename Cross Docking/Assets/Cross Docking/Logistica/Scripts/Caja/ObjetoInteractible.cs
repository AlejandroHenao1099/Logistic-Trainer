using UnityEngine;

namespace Cross_Docking
{
    public enum TipoDeAgarre
    {
        UnaMano, DosManos, Ambos
    }

    public enum TipoDeMovilidad
    {
        Libre, SoloRotacion
    }

    public class ObjetoInteractible : MonoBehaviour
    {
        [Tooltip("Determina si el objeto debe ser agarrado con los 2 controles o no")]
        public TipoDeAgarre tipoDeAgarreObjeto = TipoDeAgarre.UnaMano;

        [Tooltip("Determina si el objeto esta fijo, o si se puede mover")]
        public TipoDeMovilidad tipoDeMovilidadObjeto = TipoDeMovilidad.Libre;
    }
}
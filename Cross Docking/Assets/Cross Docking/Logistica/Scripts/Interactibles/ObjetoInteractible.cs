using UnityEngine;

namespace Cross_Docking
{
    public enum TipoDeAgarre
    {
        UnaMano, DosManos, Ambos, Ninguno
    }

    public enum TipoDeMovilidad
    {
        Libre, SoloRotacion, Ninguno
    }

    public class ObjetoInteractible : MonoBehaviour
    {
        [Tooltip("Determina si el objeto debe ser agarrado con los 2 controles o no")]
        public TipoDeAgarre tipoDeAgarreObjeto = TipoDeAgarre.UnaMano;

        [Tooltip("Determina si el objeto esta fijo, o si se puede mover")]
        public TipoDeMovilidad tipoDeMovilidadObjeto = TipoDeMovilidad.Libre;

        /// <summary>
        /// Determina si el objeto esta disponible para ser agarrado.
        /// Este booleano funciona más para objetos de UnaMano
        /// </summary>
        public bool objetoDisponible { get; protected set; }

        public virtual void Iniciar() { }
        public virtual void Iniciar(Transform mano) { }

        public virtual void Detener() { }
    }
}
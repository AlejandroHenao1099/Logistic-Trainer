using UnityEngine;
using UnityEngine.UI;

namespace Cross_Docking
{
    public class UICarro : MonoBehaviour
    {
        private Text texto;
        public PosicionarJugador posicionarJugador;

        void Start()
        {
            texto = GetComponent<Text>();
        }

        private void Update()
        {
            texto.text = "Axi = " + posicionarJugador.valorY;
        }
    }
}
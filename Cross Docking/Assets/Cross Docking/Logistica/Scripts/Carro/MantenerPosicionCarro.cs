using UnityEngine;

namespace Cross_Docking
{
    public class MantenerPosicionCarro : MonoBehaviour
    {
        public Transform posicionPuntoAcceso;
		private Transform miTransform;

		private void Start()
		{
			miTransform = transform;
		}

        private void Update()
        {
			miTransform.position = posicionPuntoAcceso.position;
        }
    }
}
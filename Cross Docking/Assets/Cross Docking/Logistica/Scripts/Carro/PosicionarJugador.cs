using System;
using UnityEngine;
using Valve.VR;

namespace Cross_Docking
{
    public class PosicionarJugador : MonoBehaviour
    {
        public Action OnEndCar;
        [Header("Carro")]
        public Transform carro;
        private NewCarUserControl controladorCarro;
        public Transform posicionJugador;

        [Header("Player")]
        public Transform cameraRig;
        private Transform izquierda;
        private Transform derecha;
        private Transform cabeza;

        private ControladorInput inputDerecho;
        private ControladorInput inputIzquierdo;

        private bool conducir;


        //Se debe alinear la posicion con el X y Z de la camara, y ademas se debe subir el Camera rig 0.65 en Y para que quede la altura adecuada

        private void Awake()
        {
            izquierda = cameraRig.GetChild(0);
            derecha = cameraRig.GetChild(1);
            cabeza = cameraRig.GetChild(2);
            inputDerecho = derecha.GetComponent<ControladorInput>();
            inputIzquierdo = izquierda.GetComponent<ControladorInput>();

            controladorCarro = carro.GetComponent<NewCarUserControl>();
        }

        private void Update()
        {
            if (!conducir)
                return;

            ObtenerInputAcelerador();

            if (inputDerecho.Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip) || inputIzquierdo.Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
                SalirAuto();
        }

        private void ObtenerInputAcelerador()
        {
            Vector2 acelerar = inputDerecho.Controller.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger);
            Vector2 frenar = inputIzquierdo.Controller.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger);
            controladorCarro.axisVertical = acelerar.x - frenar.x;
        }

        private void ObtenerInputGirar()
        {

        }

        public void Comenzar()
        {
            cameraRig.rotation = posicionJugador.rotation;
            Vector3 difference = cameraRig.position - cabeza.position;
            difference.y = 0;

            cameraRig.position = posicionJugador.position + difference + new Vector3(0f, 0.65f, 0f);
            cameraRig.SetParent(posicionJugador);
            conducir = true;
        }

        private void SalirAuto()
        {
            Vector3 difference = cameraRig.position - cabeza.position;
            difference.y = 0;

            cameraRig.position = transform.position + difference;
            cameraRig.SetParent(null);
            conducir = false;
            OnEndCar();
        }
    }
}
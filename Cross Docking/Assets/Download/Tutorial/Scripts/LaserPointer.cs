using System;
using UnityEngine;

namespace Cross_Docking
{
    public class LaserPointer : MonoBehaviour
    {
        public Action OnGrabCar;
        public Action OnReleaseCar;

        private ControladorInput controladorInput;
        private PosicionarJugador refAuto;

        public Transform cameraRigTransform;
        public Transform headTransform; // The camera rig's head

        public Vector3 teleportReticleOffset; // Offset from the floor for the reticle to avoid z-fighting
        private LayerMask capa = 1 << 8; // Mask to filter out areas where teleports are allowed

        public GameObject teleportReticlePrefab; // Stores a reference to the teleport reticle prefab.
        private GameObject reticle; // A reference to an instance of the reticle
        private Transform teleportReticleTransform; // Stores a reference to the teleport reticle transform for ease of use

        private Vector3 hitPoint; // Point where the raycast hits
        private bool shouldTeleport; // True if there's a valid teleport target
        private bool montarAuto;
        public bool teletransportar = true;

        private void Awake()
        {
            controladorInput = GetComponent<ControladorInput>();
        }

        private void Start()
        {
            reticle = Instantiate(teleportReticlePrefab);
            teleportReticleTransform = reticle.transform;
        }

        private void Update()
        {
            if (!teletransportar)
                return;

            if (controladorInput.Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
            {
                CalcularPuntoImpacto();
            }
            else
                reticle.SetActive(false);

            if (controladorInput.Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
                if (shouldTeleport)
                    Teleport();
                else if (montarAuto)
                {
                    OnGrabCar();
                    //DesactivarTeletransportacion();
                    refAuto.OnEndCar = ActivarTeletransportacion;
                    refAuto.Comenzar();
                }
            }
        }

        private void CalcularPuntoImpacto()
        {
            RaycastHit hit;

            if (Physics.Raycast(headTransform.position, headTransform.forward, out hit, 50, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                int capaObjeto = 1 << hit.transform.gameObject.layer;

                if (capaObjeto == capa)
                {
                    Vector3 puntoNormalizado = hit.point;
                    puntoNormalizado.y = 10000f;
                    hitPoint = hit.transform.GetComponent<Collider>().ClosestPointOnBounds(puntoNormalizado);

                    reticle.SetActive(true);
                    teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                    shouldTeleport = true;
                    montarAuto = false;
                    reticle.GetComponent<MeshRenderer>().material.color = Color.green;
                }
                else if (hit.transform.GetComponent<PosicionarJugador>() != null)
                {
                    refAuto = hit.transform.GetComponent<PosicionarJugador>();
                    hitPoint = hit.transform.position;
                    reticle.SetActive(true);
                    teleportReticleTransform.position = hitPoint + teleportReticleOffset + new Vector3(0, 0.1f, 0);

                    shouldTeleport = false;
                    montarAuto = true;
                    reticle.GetComponent<MeshRenderer>().material.color = Color.yellow;
                }
                else
                {
                    montarAuto = false;
                    hitPoint = hit.point;
                    reticle.SetActive(true);
                    teleportReticleTransform.position = hitPoint + teleportReticleOffset;

                    shouldTeleport = false;
                    reticle.GetComponent<MeshRenderer>().material.color = Color.red;
                }
            }
        }

        private void Teleport()
        {
            shouldTeleport = false;
            reticle.SetActive(false);
            Vector3 difference = cameraRigTransform.position - headTransform.position;
            difference.y = 0;

            cameraRigTransform.position = hitPoint + difference;
        }

        private void ActivarTeletransportacion()
        {
            teletransportar = true;
            OnReleaseCar();
        }
    }
}
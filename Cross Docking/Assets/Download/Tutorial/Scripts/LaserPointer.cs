using UnityEngine;

namespace SENA
{
    public class LaserPointer : MonoBehaviour
    {
        public Transform cameraRigTransform;
        public Transform headTransform; // The camera rig's head
        public Vector3 teleportReticleOffset; // Offset from the floor for the reticle to avoid z-fighting
        public LayerMask capa = 1 << 8; // Mask to filter out areas where teleports are allowed

        public GameObject teleportReticlePrefab; // Stores a reference to the teleport reticle prefab.
        private GameObject reticle; // A reference to an instance of the reticle
        private Transform teleportReticleTransform; // Stores a reference to the teleport reticle transform for ease of use

        private Vector3 hitPoint; // Point where the raycast hits
        private bool shouldTeleport; // True if there's a valid teleport target

        private SteamVR_TrackedObject trackedObj;
        private SteamVR_Controller.Device Controller
        {
            get { return SteamVR_Controller.Input((int)trackedObj.index); }
        }

        private void Awake()
        {
            trackedObj = GetComponent<SteamVR_TrackedObject>();
        }

        private void Start()
        {
            reticle = Instantiate(teleportReticlePrefab);
            teleportReticleTransform = reticle.transform;
        }

        private void Update()
        {
            // Is the touchpad held down?
            if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
            {
                RaycastHit hit;

                // Send out a raycast from the controller
                if (Physics.Raycast(headTransform.position, headTransform.forward, out hit, 50, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    //Show teleport reticle
                    int capaObjeto = 1 << hit.transform.gameObject.layer;

                    if (capaObjeto == capa)
                    {
                        Vector3 puntoNormalizado = hit.point;
                        puntoNormalizado.y = 10000f;
                        hitPoint = hit.transform.GetComponent<Collider>().ClosestPointOnBounds(puntoNormalizado);

                        reticle.SetActive(true);
                        teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                        shouldTeleport = true;
                        reticle.GetComponent<MeshRenderer>().material.color = Color.green;
                    }
                    else
                    {
                        hitPoint = hit.point;
                        reticle.SetActive(true);
                        teleportReticleTransform.position = hitPoint + teleportReticleOffset;

                        shouldTeleport = false;
                        reticle.GetComponent<MeshRenderer>().material.color = Color.red;
                    }
                }
            }
            else // Touchpad not held down, hide laser & teleport reticle
                reticle.SetActive(false);

            // Touchpad released this frame & valid teleport position found
            if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport)
                Teleport();
        }

        private void Teleport()
        {
            shouldTeleport = false; // Teleport in progress, no need to do it again until the next touchpad release
            reticle.SetActive(false); // Hide reticle
            Vector3 difference = cameraRigTransform.position - headTransform.position; // Calculate the difference between the center of the virtual room & the player's head
            difference.y = 0; // Don't change the final position's y position, it should always be equal to that of the hit point

            cameraRigTransform.position = hitPoint + difference; // Change the camera rig position to where the the teleport reticle was. Also add the difference so the new virtual room position is relative to the player position, allowing the player's new position to be exactly where they pointed. (see illustration)
        }
    }
}
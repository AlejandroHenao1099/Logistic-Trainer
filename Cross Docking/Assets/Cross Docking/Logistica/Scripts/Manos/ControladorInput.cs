using UnityEngine;

namespace Cross_Docking
{
    public class ControladorInput : MonoBehaviour
    {

        private SteamVR_TrackedObject trackedObject;

        public SteamVR_Controller.Device Controller
        {
            get { return SteamVR_Controller.Input((int)trackedObject.index); }
        }

        public bool triggerPresionado { get; private set; }

        private void Awake()
        {
            trackedObject = GetComponent<SteamVR_TrackedObject>();
        }


        private void Update()
        {
            if (Controller.GetHairTriggerDown())
                triggerPresionado = true;
            else if (Controller.GetHairTriggerUp())
                triggerPresionado = false;
        }
    }
}
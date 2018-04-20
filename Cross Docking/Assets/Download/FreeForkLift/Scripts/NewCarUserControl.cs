using UnityEngine;

[RequireComponent(typeof(NewCarController))]
public class NewCarUserControl : MonoBehaviour
{
    private NewCarController m_Car; // the car controller we want to use

    public float axisVertical;
    public float axisHorizontal;

    private void Awake()
    {
        // get the car controller
        m_Car = GetComponent<NewCarController>();
    }

    private void FixedUpdate()
    {
        //pass the input to the car!
        // float h = Input.GetAxis("Horizontal");
        // float v = Input.GetAxis("Vertical");


        m_Car.Move(axisHorizontal, axisVertical, axisVertical, 0);


        // #if !MOBILE_INPUT
        //             float handbrake = Input.GetAxis("Jump");
        //             m_Car.Move(h, v, v, handbrake);

        // #else
        //             m_Car.Move(h, v, v, 0f);
        // #endif
    }
}
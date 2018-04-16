using System;
using UnityEngine;


internal enum CarDriveType
{
    FrontWheelDrive,
    RearWheelDrive,
    FourWheelDrive
}

internal enum SpeedType
{
    MPH,
    KPH
}

public class NewCarController : MonoBehaviour
{
    [SerializeField] private int colWheels; //the number of wheels that can rotate, can be used for the trailer //La cantidad de ruedas que pueden girar, se puede usar para el remolque
    [SerializeField] private bool FrontSteerWheels = true; //the rotation of the front or rear wheels, made for a forklift truck //La rotación de las ruedas delanteras o traseras, hecha para una carretilla elevadora
    [SerializeField] private CarDriveType m_CarDriveType = CarDriveType.FourWheelDrive; //Enumeracion
    [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4]; //Array de WheelColliders
    [SerializeField] private Vector3 m_CentreOfMassOffset; //Centro de masa compensado, valor inicial 0,0,0
    [SerializeField] private float m_MaximumSteerAngle; //Angulo maximo de direccion
    [Range(0, 1)] [SerializeField] private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing //0 es física cruda, 1 el automóvil se agarrará en la dirección que está enfrentando
    [Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference // 0 no es control de tracción, 1 es interferencia completa
    [SerializeField] private float m_FullTorqueOverAllWheels; // par máximo sobre todas las ruedas / o //Torque completo sobre las ruedas
    [SerializeField] private float m_ReverseTorque; // torque inverso
    [SerializeField] private float m_MaxHandbrakeTorque; //par máximo del freno de mano
    [SerializeField] private float m_Downforce = 100f;  //fuerza hacia abajo
    [SerializeField] private SpeedType m_SpeedType; // Enumeracion
    [SerializeField] private float m_Topspeed = 200; //velocidad máxima
    [SerializeField] private static int NoOfGears = 5; //Número de engranajes
    [SerializeField] private float m_RevRangeBoundary = 1f; //límite de rango inverso
    [SerializeField] private float m_SlipLimit; // límite deslizante
    [SerializeField] private float m_BrakeTorque; // par de frenado

    private float m_SteerAngle; //ángulo de dirección
    private int m_GearNum; // Numero de engranajes
    private float m_GearFactor; // factor de engranaje
    private float m_OldRotation; // Vieja rotacion
    private float m_CurrentTorque; //Torque actual
    private Rigidbody m_Rigidbody;
    // private const float k_ReversingThreshold = 0.01f;

    // public bool Skidding { get; private set; }
    public float BrakeInput { get; private set; }  //entrada de freno
    //   public float CurrentSteerAngle{ get { return m_SteerAngle; }}
    public float CurrentSpeed { get { return m_Rigidbody.velocity.magnitude * 2.23693629f; } }  // Velocidad actual
    public float MaxSpeed { get { return m_Topspeed; } }  // Maxima velocidad
    public float Revs { get; private set; }  //Revoluciones
    public float AccelInput { get; private set; }  //entrada de aceleración

    private void Start()
    {
        m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;  //Esto toma un WheelCollider y toma la refrencia del rigidbody, y obtiene su centro de masa, y lo resetea 0,0,0
        //Se resetea el centro de masa para que no sea calculado por Unity, ademas al hacerlo a 0,0,0 el auto se vuelve más estable

        m_MaxHandbrakeTorque = float.MaxValue; //Se stablece al valor maximo del float

        m_Rigidbody = GetComponent<Rigidbody>();  //Se obtiene una referencia al rigidbody
        m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl * m_FullTorqueOverAllWheels);  //Se establece el torque actual, y se hace una operacion para su manejo // 1400 - (1 * 1400)
    }

    private void GearChanging()
    {
        float f = Mathf.Abs(CurrentSpeed / MaxSpeed);
        float upgearlimit = (1 / (float)NoOfGears) * (m_GearNum + 1);
        float downgearlimit = (1 / (float)NoOfGears) * m_GearNum;

        if (m_GearNum > 0 && f < downgearlimit)
        {
            m_GearNum--;
        }

        if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
        {
            m_GearNum++;
        }
    }


    // simple function to add a curved bias towards 1 for a value in the 0-1 range
    private static float CurveFactor(float factor)
    {
        return 1 - (1 - factor) * (1 - factor);
    }


    // unclamped version of Lerp, to allow value to exceed the from-to range
    private static float ULerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }


    private void CalculateGearFactor()
    {
        float f = (1 / (float)NoOfGears);
        // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
        // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
        var targetGearFactor = Mathf.InverseLerp(f * m_GearNum, f * (m_GearNum + 1), Mathf.Abs(CurrentSpeed / MaxSpeed));
        m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime * 5f);
    }


    private void CalculateRevs()
    {
        // calculate engine revs (for display / sound)
        // (this is done in retrospect - revs are not used in force/power calculations)
        CalculateGearFactor();
        var gearNumFactor = m_GearNum / (float)NoOfGears;
        var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
        var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
        Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
    }


    public void Move(float steering, float accel, float footbrake, float handbrake)  //Move(direccion, aceleracion, freno, freno de mano)
    {

        //clamp input values  //Limitamos los valores de entrada
        steering = Mathf.Clamp(steering, -1, 1);  
        AccelInput = accel = Mathf.Clamp(accel, 0, 1);
        BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
        handbrake = Mathf.Clamp(handbrake, 0, 1);

        //Set the steer on the front wheels. // Coloque la dirección en las ruedas delanteras.
        //Assuming that wheels 0 and 1 are the front wheels. // Suponiendo que las ruedas 0 y 1 son las ruedas delanteras.
        m_SteerAngle = steering * m_MaximumSteerAngle;  

        if (FrontSteerWheels)  //Aqui le damos el steerangle a las ruedas delanteras, este calculo se hace basicamente como GetAxis(Horizontal) * 30, y 30 seria el maximo angulo para girar
        {
            m_WheelColliders[0].steerAngle = m_SteerAngle;
            m_WheelColliders[1].steerAngle = m_SteerAngle;
        }
        else
        {
            m_WheelColliders[2].steerAngle = m_SteerAngle;
            m_WheelColliders[3].steerAngle = m_SteerAngle;
        }

        SteerHelper();
        ApplyDrive(accel, footbrake);
        CapSpeed();

        //Set the handbrake.
        //Assuming that wheels 2 and 3 are the rear wheels.
        if (handbrake > 0f)
        {
            var hbTorque = handbrake * m_MaxHandbrakeTorque;
            m_WheelColliders[2].brakeTorque = hbTorque;
            m_WheelColliders[3].brakeTorque = hbTorque;
        }

        CalculateRevs();
        GearChanging();

        AddDownForce();
        //CheckForWheelSpin();
        TractionControl();
    }


    private void CapSpeed()
    {
        float speed = m_Rigidbody.velocity.magnitude;
        switch (m_SpeedType)
        {
            case SpeedType.MPH:

                speed *= 2.23693629f;
                if (speed > m_Topspeed)
                    m_Rigidbody.velocity = (m_Topspeed / 2.23693629f) * m_Rigidbody.velocity.normalized;
                break;

            case SpeedType.KPH:
                speed *= 3.6f;
                if (speed > m_Topspeed)
                    m_Rigidbody.velocity = (m_Topspeed / 3.6f) * m_Rigidbody.velocity.normalized;
                break;
        }
    }


    private void ApplyDrive(float accel, float footbrake)
    {

        float thrustTorque;
        switch (m_CarDriveType)
        {
            case CarDriveType.FourWheelDrive:
                thrustTorque = accel * (m_CurrentTorque / 4f);
                for (int i = 0; i < colWheels; i++)
                {
                    m_WheelColliders[i].motorTorque = thrustTorque;
                }
                break;

            case CarDriveType.FrontWheelDrive:
                thrustTorque = accel * (m_CurrentTorque / 2f);
                m_WheelColliders[0].motorTorque = m_WheelColliders[1].motorTorque = thrustTorque;
                break;

            case CarDriveType.RearWheelDrive:
                thrustTorque = accel * (m_CurrentTorque / 2f);
                m_WheelColliders[2].motorTorque = m_WheelColliders[3].motorTorque = thrustTorque;
                break;

        }

        for (int i = 0; i < colWheels; i++)
        {
            if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
            {
                m_WheelColliders[i].brakeTorque = m_BrakeTorque * footbrake;
            }
            else if (footbrake > 0)
            {
                m_WheelColliders[i].brakeTorque = 0f;
                m_WheelColliders[i].motorTorque = -m_ReverseTorque * footbrake;
            }
        }
    }


    private void SteerHelper()
    {
        for (int i = 0; i < 4; i++) //Recorremos el for para obtener el Wheelhit que nos da informacion realista del estado de la rueda
        {
            WheelHit wheelhit;
            m_WheelColliders[i].GetGroundHit(out wheelhit);  //Obtenemos esa informacion con este metodo y lo almacenamos con esta variable que pide

            if (wheelhit.normal == Vector3.zero)  //Aqui si la normal de la rueda es Vector.zero entonces interrumpimos el metodo, si la normal es zero quiere decir que las ruedas estan en el aire
                return; // wheels arent on the ground so dont realign the rigidbody velocity  // las ruedas no están en el suelo así que no realinee la velocidad del Rigidbody
        }

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction ///// Esto si es necesario para evitar problemas de bloqueo de cardán que harán que el auto cambie repentinamente de dirección
        if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;  
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
        }
        m_OldRotation = transform.eulerAngles.y;
    }


    // this is used to add more grip in relation to speed
    private void AddDownForce()
    {
        m_WheelColliders[0].attachedRigidbody.AddForce(-transform.up * m_Downforce *
                                                     m_WheelColliders[0].attachedRigidbody.velocity.magnitude);
    }


    // crude traction control that reduces the power to wheel if the car is wheel spinning too much
    private void TractionControl()
    {
        WheelHit wheelHit;
        switch (m_CarDriveType)
        {
            case CarDriveType.FourWheelDrive:
                // loop through all wheels
                for (int i = 0; i < 4; i++)
                {
                    m_WheelColliders[i].GetGroundHit(out wheelHit);

                    AdjustTorque(wheelHit.forwardSlip);
                }
                break;

            case CarDriveType.RearWheelDrive:
                m_WheelColliders[2].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);

                m_WheelColliders[3].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);
                break;

            case CarDriveType.FrontWheelDrive:
                m_WheelColliders[0].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);

                m_WheelColliders[1].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);
                break;
        }
    }

    private void AdjustTorque(float forwardSlip)
    {
        if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
        {
            m_CurrentTorque -= 10 * m_TractionControl;
        }
        else
        {
            m_CurrentTorque += 10 * m_TractionControl;
            if (m_CurrentTorque > m_FullTorqueOverAllWheels)
            {
                m_CurrentTorque = m_FullTorqueOverAllWheels;
            }
        }
    }
}
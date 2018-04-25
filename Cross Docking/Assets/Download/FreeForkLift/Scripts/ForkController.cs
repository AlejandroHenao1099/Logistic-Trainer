using UnityEngine;
using Cross_Docking;

public class ForkController : MonoBehaviour
{
    public Transform fork;
    public Transform mast;
    public float speedTranslate; //Platform travel speed
    public Vector3 maxY; //The maximum height of the platform
    public Vector3 minY; //The minimum height of the platform
    public Vector3 maxYmast; //The maximum height of the mast
    public Vector3 minYmast; //The minimum height of the mast

    private bool mastMoveTrue = false; //Activate or deactivate the movement of the mast

    public ControladorInput controladorInput;


    private void FixedUpdate()
    {
        if (fork.localPosition.y >= maxYmast.y)
        {
            mastMoveTrue = true;
        }
        if (fork.localPosition.y <= maxYmast.y)
        {
            mastMoveTrue = false;
        }
        if (fork.localPosition.y >= maxY.y)
        {
            fork.localPosition = new Vector3(fork.localPosition.x, maxY.y, fork.localPosition.z);
        }
        if (fork.localPosition.y <= minY.y)
        {
            fork.localPosition = new Vector3(fork.localPosition.x, minY.y, fork.localPosition.z);
        }

        Vector3 posicionRelativaMast = fork.InverseTransformPoint(mast.position);

        //Me quede aqui

        if (posicionRelativaMast.y >= maxYmast.y)
        {
            mast.transform.localPosition = new Vector3(mast.transform.position.x, maxYmast.y, mast.transform.position.z);
        }

        if (mast.transform.localPosition.y <= minYmast.y)
        {
            mast.transform.position = new Vector3(mast.transform.position.x, minYmast.y, mast.transform.position.z);
        }

        Vector2 presTouchPad = controladorInput.Controller.GetAxis();

        if (presTouchPad.y > 0f)
        {
            fork.Translate(Vector3.up * speedTranslate * Time.deltaTime);
            if (mastMoveTrue)
            {
                mast.Translate(Vector3.up * speedTranslate * Time.deltaTime);
            }

        }
        if (presTouchPad.y < 0f)
        {
            fork.Translate(-Vector3.up * speedTranslate * Time.deltaTime);
            if (mastMoveTrue)
            {
                mast.Translate(-Vector3.up * speedTranslate * Time.deltaTime);
            }
        }
    }
}
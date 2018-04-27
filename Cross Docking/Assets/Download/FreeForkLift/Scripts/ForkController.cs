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

    public Vector2 axis = Vector2.zero;


    private void FixedUpdate()
    {
        Vector3 posicionRelativafork = fork.parent.InverseTransformPoint(fork.position);
        Vector3 posicionRelativaMast = fork.parent.InverseTransformPoint(mast.position);

        if (posicionRelativafork.y >= maxYmast.y)
        {
            mastMoveTrue = true;
        }
        if (posicionRelativafork.y <= maxYmast.y)
        {
            mastMoveTrue = false;
        }
        if (posicionRelativafork.y >= maxY.y)
        {
            fork.localPosition = new Vector3(fork.localPosition.x, maxY.y, fork.localPosition.z);
        }
        if (posicionRelativafork.y <= minY.y)
        {
            fork.localPosition = new Vector3(fork.localPosition.x, minY.y, fork.localPosition.z);
        }

        if (posicionRelativaMast.y >= maxYmast.y)
        {
            mast.transform.localPosition = new Vector3(mast.localPosition.x, maxYmast.y, mast.transform.localPosition.z);
        }

        if (mast.localPosition.y <= minYmast.y)
        {
            mast.transform.localPosition = new Vector3(mast.localPosition.x, minYmast.y, mast.transform.localPosition.z);
        }

        if (axis.y > 0f)
        {
            fork.Translate(Vector3.up * speedTranslate * Time.deltaTime);
            if (mastMoveTrue)
            {
                mast.Translate(Vector3.up * speedTranslate * Time.deltaTime);
            }

        }
        if (axis.y < 0f)
        {
            fork.Translate(-Vector3.up * speedTranslate * Time.deltaTime);
            if (mastMoveTrue)
            {
                mast.Translate(-Vector3.up * speedTranslate * Time.deltaTime);
            }
        }
    }
}
using UnityEngine;

public class FisicasControl : MonoBehaviour
{
    public void AgarrarObjetoFJ(GameObject objeto)
    {
        FixedJoint reFixedJoi = objeto.AddComponent<FixedJoint>();
        reFixedJoi.connectedBody = GetComponent<Rigidbody>();
        reFixedJoi.breakForce = 10000;
        reFixedJoi.breakTorque = 10000;
    }

    public void SoltarObjetoFJ(GameObject objeto)
    {
        if (objeto.GetComponent<FixedJoint>() != null)
        {
            FixedJoint[] reFixedJoi = objeto.GetComponents<FixedJoint>();
            Rigidbody reRigidbody = objeto.GetComponent<Rigidbody>();
            reFixedJoi[0].connectedBody = null;
            reFixedJoi[1].connectedBody = null;
            Destroy(reFixedJoi[0]);
            Destroy(reFixedJoi[1]);
			reRigidbody.velocity = GetComponent<Rigidbody>().velocity;
			reRigidbody.angularVelocity = GetComponent<Rigidbody>().angularVelocity;
        }
    }
}

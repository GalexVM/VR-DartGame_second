using UnityEngine;

public class OculusGrabTest : MonoBehaviour
{
    private OVRGrabbable grabbable;
    private Rigidbody rb;

    void Start()
    {
        grabbable = GetComponent<OVRGrabbable>();
        rb = GetComponent<Rigidbody>();

        if (grabbable == null)
        {
            Debug.LogError("OVRGrabbable no est� asignado al objeto.");
        }
        if (rb == null)
        {
            Debug.LogError("Rigidbody no est� asignado al objeto.");
        }
    }

    void Update()
    {
        if (grabbable.isGrabbed)
        {
            Debug.Log("El objeto est� siendo agarrado.");
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;
        }
    }
}

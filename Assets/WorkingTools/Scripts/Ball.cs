
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private OVRInput.Controller controller;
    static private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.B, controller))
        {
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
            transform.position = OVRInput.GetLocalControllerPosition(controller);

        }
    }
}

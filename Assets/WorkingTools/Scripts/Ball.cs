
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private OVRInput.Controller controller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.B, controller))
        {
            ball.transform.position = OVRInput.GetLocalControllerPosition(controller);

        }
    }
}

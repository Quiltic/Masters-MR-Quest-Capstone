using UnityEngine;

public class SunSpin : MonoBehaviour
{
    static float spinSpeed = 0.02f;
    Vector3 spinVector = new Vector3(0, spinSpeed, 0);

    // Just make the sun spin
    void Update()
    {
        transform.Rotate(spinVector);
    }
}

using UnityEngine;

public class KillBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        DestroyImmediate(other.gameObject);
    }

}

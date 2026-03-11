using UnityEngine;

public class FolowMe : MonoBehaviour
{

    public GameObject follow;
    public string name;

    private void Awake()
    {
        follow = GameObject.Find(name);
            //FindAnyObjectByType<OVRCameraRig>();
    }

    void Update()
    {
        transform.position = follow.transform.position;
        transform.rotation = follow.transform.rotation;
    }
}

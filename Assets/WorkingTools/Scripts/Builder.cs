using Meta.XR.MRUtilityKit;
using UnityEngine;

public class Builder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OVRScene.RequestSpaceSetup();
        MRUK.Instance.LoadSceneFromDevice();
        //MRUK.Instance.LoadSceneFromPrefab("Office_Small");
        //MRUK.Instance.LoadSceneFromJson("MySavedScene.json");

    }

    //void Update()
    //{

    //}
}

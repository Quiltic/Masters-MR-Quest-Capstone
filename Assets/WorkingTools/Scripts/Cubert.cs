using System;
using Unity.VisualScripting;
using UnityEngine;

public class Cubert : MonoBehaviour
{
    [SerializeField] public RoomAlignment ra;
    //[Serializable]

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        ra = GameObject.Find("RoomAlignment").GetComponent<RoomAlignment>();
    }

    void Start()
    {
        transform.position = ra.currentRoom.FloorAnchor.transform.position;
        //transform.eulerAngles = new Vector3(0,ra.currentRoom.FloorAnchor.transform.eulerAngles.y,0);
        transform.Rotate(0f, (float)ra.theta, 0f, Space.Self);

        // to possition the location of the host in realspace. idk if this will work
        //if (ra.currentRoom.FloorAnchor.transform.position != ra.roomDataReceived.roomFloorPosData) // for some reason the code never accepts isServer or isHost
        //{
        //    //Debug.LogWarning($"{ra.networkManager.isServer}, {ra.networkManager.isHost}");
        //    transform.position = ra.roomDataReceived.whereIAmInRoom;
        //    transform.rotation = ra.roomDataReceived.roomFloorRotationData;
        //}
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}

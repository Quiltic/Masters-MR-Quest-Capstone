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
        transform.rotation = ra.currentRoom.FloorAnchor.transform.rotation;

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

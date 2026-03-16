
using LearnXR.Core.Utilities;
using Meta.XR.MRUtilityKit;
using PurrNet;
using PurrNet.Packing;
using PurrNet.Transports;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class RoomAlignment : MonoBehaviour
{
    public NetworkManager networkManager;
    private sendData dataToSend;


    [SerializeField] public MRUK mruk;
    [SerializeField] public GameObject headset;


    private bool sceneHasBeenLoaded;
    public MRUKRoom currentRoom;
    public bool SceneAndRoomInfoAvailable => currentRoom != null && sceneHasBeenLoaded;

    public sendData serverRoom;


    public (Vector3, Quaternion) playerOffset;


    //private GameObject myRoom;

    // FOR ME ON Wensdat!
    // Use the wallsss as anchor points (scince they will most likely be the same across headsets)


    public GameObject blinder;


    [System.Serializable]

    public struct sendData : IPackedAuto
    {
        public Vector3 roomFloorPosData;
        public Quaternion roomFloorRotationData;
        public string stringData;
        

        //public override string ToString()
        //{
        //    return $"ID: {intData}, {stringData}";
        //}

        //public (Vector3, Quaternion) Get()
        //{
        //    return (roomFloorPosData, roomFloorRotationData);
        //}
    }


    private void OnEnable()
    {
        //Vector3 HostP = new Vector3(-1.77f,-0.1f,1.21f);
        //Vector3 ClientP = new Vector3(0.48f, -0.11f, 1.77f);
        //Debug.Log($"HostP: {distance(HostP,Vector3.zero)}"); // 2.146
        //Debug.Log($"ClientP: {distance(ClientP, Vector3.zero)}"); // 1.837
        //Debug.Log($"HostR: {new Quaternion(0.44f,-0.54f,-0.54f,-0.44f).eulerAngles}"); // 270,101.65
        //Debug.Log($"ClientR: {new Quaternion(-0.53f, -0.47f, -0.47f, 0.53f).eulerAngles}"); // 270, 276.87


        // Connect listeners

        networkManager.Subscribe<sendData>(gotData); // for data
        mruk.RoomCreatedEvent.AddListener(BindRoomInfo);

        //NetworkManager.onAnyClientConnectionState += BindRoomInfo;

        // for server, check if someone joined, Client does not need this
        //if (networkManager.isServer || networkManager.isHost)
        networkManager.onPlayerJoined += someoneJoined;
    }

    private void OnDisable()
    {
        // Disconnect listeners

        // for data send/receive between server and client
        if (networkManager)
            networkManager.Unsubscribe<sendData>(gotData);

        // for checking if someone joined
        if (networkManager.isServer)
            networkManager.onPlayerJoined -= someoneJoined;


        mruk.RoomCreatedEvent.RemoveListener(BindRoomInfo);
    }


    public void RoomEnabled()
    {
        sceneHasBeenLoaded = true;
        SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} has been enabled due to scene availability.");
    }

    public void BindRoomInfo(MRUKRoom room)
    {
        currentRoom = room; // for self
        //dataToSend.roomData = room; // for server
        dataToSend.roomFloorPosData = room.FloorAnchor.transform.position;
        dataToSend.roomFloorRotationData = room.transform.rotation;
        dataToSend.stringData = $"{nameof(MRUKDemo)} room was bound to {nameof(room)}.";
        SpatialLogger.Instance.LogInfo(dataToSend.stringData);

        //SpatialLogger.Instance.LogInfo(room.Anchors.ToString());
    }

    private void gotData(PlayerID player, sendData data, bool asServer)
    {
        Debug.Log($"Recieved Data: {data.stringData}");
        SpatialLogger.Instance.LogInfo($"Recieved Data: {data.stringData}");
        serverRoom = data;

        // This is where the magic happens

        double HostDistance = distance(data.roomFloorPosData, Vector3.zero); // (a)
        double ClientDistance = distance(currentRoom.FloorAnchor.transform.position, Vector3.zero); // (b)

        double Rotation = currentRoom.transform.rotation.eulerAngles.y; // (angle B)

        //if ( Rotation <= 180 ) {

        //if (Rotation > 180) { 
        //    Rotation = Rotation - 360;
        //}

        



        //Vector3 ClientP = new Vector3(0.48f, -0.11f, 1.77f);
        //Debug.Log($"HostP: {}"); // 2.146
        //Debug.Log($"ClientP: {}"); // 1.837
        //Debug.Log($"HostR: {new Quaternion(0.44f, -0.54f, -0.54f, -0.44f).eulerAngles}"); // 270,101.65
        //Debug.Log($"ClientR: {new Quaternion(-0.53f, -0.47f, -0.47f, 0.53f).eulerAngles}"); // 270, 276.87

        // I havent made the magic yet though...
    }

    private void someoneJoined(PlayerID player, bool isReconnect, bool asServer)
    {
        blinder.SetActive(false); // show that you loaded in. If your the server you will have already done this.

        if (networkManager.isServer || networkManager.isHost)
        {
            SpatialLogger.Instance.LogInfo($"Player '{player}' Connected");
            Debug.Log($"Player '{player}' Connected");


            if (!SceneAndRoomInfoAvailable) // this is very bad if it hits
                dataToSend.stringData = "No room data found.";

            networkManager.Send(player, dataToSend);
        }

        else if (networkManager.isClientOnly) // letting you know that you connected to the server
        {
            SpatialLogger.Instance.LogInfo($"I Connected");
            Debug.Log($"I Connected");
        }

    }


    private void Update()
    {
        SpatialLogger.Instance.Clear();


        SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} room orientation {currentRoom.FloorAnchor.transform.position} : {currentRoom.FloorAnchor.transform.rotation}.");
        SpatialLogger.Instance.LogInfo($"Headset Location: {headset.transform.position}");
    }

    private double distance(Vector3 a, Vector3 b)
    {
        float distance = 0;
        distance = (b.x - a.x) * (b.x - a.x); // idk squaring is weird in C#
        distance += (b.y - a.y) * (b.y - a.y);
        distance += (b.z - a.z) * (b.z - a.z);

        return (Math.Sqrt(distance));
    }

}

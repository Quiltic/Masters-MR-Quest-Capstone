
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
    public Vector2 whereAmIInRoom;


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
        public string jsonData;

        public Vector2 whereIAmInRoom;
        public Quaternion myRotationInRoom; // which i have no idea how to get yet but we are working on it.
        

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
        dataToSend.jsonData = $"{nameof(MRUKDemo)} room was bound to {nameof(room)}.";
        SpatialLogger.Instance.LogInfo(dataToSend.jsonData);

        dataToSend.whereIAmInRoom = getWhereIAmInRoom();
        SpatialLogger.Instance.LogInfo($"I think your HERE: {dataToSend.whereIAmInRoom.x}, {dataToSend.whereIAmInRoom.y}.");
        transform.position = dataToSend.whereIAmInRoom;


        //SpatialLogger.Instance.LogInfo(room.Anchors.ToString());
    }

    private void gotData(PlayerID player, sendData data, bool asServer)
    {
        Debug.Log($"Recieved Data: {data.jsonData}");
        SpatialLogger.Instance.LogInfo($"Recieved Data: {data.jsonData}");
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
                dataToSend.jsonData = "No room data found.";

            networkManager.Send(player, dataToSend);
        }

        else if (networkManager.isClientOnly) // letting you know that you connected to the server
        {
            SpatialLogger.Instance.LogInfo($"I Connected");
            Debug.Log($"I Connected");
        }

    }


    //private void Update()
    //{
    //    SpatialLogger.Instance.Clear();


    //    SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} room orientation {currentRoom.FloorAnchor.transform.position} : {currentRoom.FloorAnchor.transform.rotation}.");
    //    SpatialLogger.Instance.LogInfo($"Headset Location: {headset.transform.position}");
    //}

    private double distance(Vector3 a, Vector3 b)
    {
        float distance = 0;
        distance = (b.x - a.x) * (b.x - a.x); // idk squaring is weird in C#
        distance += (b.y - a.y) * (b.y - a.y);
        distance += (b.z - a.z) * (b.z - a.z);

        return (Math.Sqrt(distance));
    }

    private double sqr(double a)
    {
        return a*a;
    }

    private Vector2 getWhereIAmInRoom()
    {
        // https://math.stackexchange.com/questions/100448/finding-location-of-a-point-on-2d-plane-given-the-distances-to-three-other-know
        // https://math.stackexchange.com/questions/884807/find-x-location-using-3-known-x-y-location-using-trilateration
        double x0;
        double y0;

        Vector2[] walls = new Vector2[currentRoom.WallAnchors.Count];
        double[] walldistances = new double[currentRoom.WallAnchors.Count];


        int index = 0; // for indexing foreach loops
        // get the x,y,d for each datasource (we are ignoring z as it can be found with a simple lookup)
        foreach (MRUKAnchor wall in currentRoom.WallAnchors)
        {
            walls[index] = new Vector2( wall.transform.position.x, wall.transform.position.z );
            walldistances[index] = distance(wall.transform.position, Vector3.zero);
            Debug.Log($"{walls[index].x},{walls[index].y},{distance(wall.transform.position, Vector3.zero)}");
            index++;
        }

        index = 0;

        var A = (-2 * walls[0].x + 2 * walls[1].x);
        var B = (-2 * walls[0].y + 2 * walls[1].y);
        var C = sqr(walldistances[0]) - sqr(walldistances[1]) - sqr(walls[0].x) + sqr(walls[1].x) - sqr(walls[0].y) + sqr(walls[1].y);

        var D = (-2 * walls[1].x + 2 * walls[2].x);
        var E = (-2 * walls[1].y + 2 * walls[2].y);
        var F = sqr(walldistances[1]) - sqr(walldistances[2]) - sqr(walls[1].x) + sqr(walls[2].x) - sqr(walls[1].y) + sqr(walls[2].y);

        x0 = (C * E - F * B) / (E * A - B * D);
        y0 = (C * D - A * F) / (B * D - A * E);

        //var D = distance(walls[0],walls[1]); // written as |S1S2| in the link above

        //var a = sqr(walldistances[0]) - sqr(walldistances[1]) + sqr(D);
        //a /= 2 * D;
        //var h = walldistances[0] - a;

        //x0 = walls[0].x + h * (walls[1].y - walls[0].y) / D;
        //y0 = walls[0].y + h * (walls[1].x - walls[0].x) / D;

        return new Vector2((float)x0, (float)y0);
    }
}

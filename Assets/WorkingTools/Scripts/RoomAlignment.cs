
using LearnXR.Core.Utilities;
using Meta.XR.MRUtilityKit;
using PurrNet;
using PurrNet.Packing;
using PurrNet.Transports;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class RoomAlignment : MonoBehaviour
{
    public NetworkManager networkManager;
    public sendData dataToSend;
    public sendData roomDataReceived;

    

    // headset data
    [SerializeField] public MRUK mruk;
    [SerializeField] public GameObject headset;


    // roomdata
    private bool sceneHasBeenLoaded;
    public MRUKRoom currentRoom;
    public bool SceneAndRoomInfoAvailable => currentRoom != null && sceneHasBeenLoaded;

    // here to make people not look at the world before they load in (lazy but effective)
    public GameObject blinder;


    // *******************************
    // this is the angle we care about
    // *******************************

    public double theta;


    private PlayerID sendToPlayer;



    [System.Serializable]

    public struct sendData : IPackedAuto
    {

        //public GameObject chosenWall;
        
        //public string jsonData; // the full json for room data

        //public Vector2 whereIAmInRoom;
        //public Quaternion myRotationInRoom; // which i have no idea how to get yet but we are working on it.

        // used for ground truth position and to calculate rotation
        public Vector3 roomFloorPosData;

        //// the following is used to get ground truth rotation
        //public Rect mainWall;
        //public Rect secondaryWall;
        //public Quaternion roomFloorRotationData;

        public Vector3[] walls;
        
    }

    // idea: use the floor, and 2 walls to determine ground truth "north"
    // to do this we will make a triangle between the 3 objects and get the angle the floor faces twords a wall



    private void OnEnable()
    {
        //Vector3 HostP = new Vector3(-1.77f,-0.1f,1.21f);
        //Vector3 ClientP = new Vector3(0.48f, -0.11f, 1.77f);
        //Debug.Log($"HostP: {distance(HostP,Vector3.zero)}"); // 2.146
        //Debug.Log($"ClientP: {distance(ClientP, Vector3.zero)}"); // 1.837
        //Debug.Log($"HostR: {new Quaternion(0.44f,-0.54f,-0.54f,-0.44f).eulerAngles}"); // 270,101.65
        //Debug.Log($"ClientR: {new Quaternion(-0.53f, -0.47f, -0.47f, 0.53f).eulerAngles}"); // 270, 276.87

        // Connect listeners

        networkManager.Subscribe<sendData>(GotData); // for data
        mruk.RoomCreatedEvent.AddListener(BindRoomInfo);

        //NetworkManager.onAnyClientConnectionState += BindRoomInfo;

        // for server, check if someone joined, Client does not need this
        //if (networkManager.isServer || networkManager.isHost)
        networkManager.onPlayerJoined += SomeoneJoined;
    }

    private void OnDisable()
    {
        // Disconnect listeners

        // for data send/receive between server and client
        if (networkManager)
            networkManager.Unsubscribe<sendData>(GotData);

        // for checking if someone joined
        if (networkManager.isServer)
            networkManager.onPlayerJoined -= SomeoneJoined;


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

        //try
        //{
        //    dataToSend.jsonData = MRUK.Instance.SaveSceneToJsonString(true);
        //}
        //catch (Exception e) {
        //    Debug.LogError($"ERROR IN JSON: {e}");
        //    SpatialLogger.Instance.LogError($"ERROR IN JSON: {e}");
        //}
        
        
        SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} room was bound to {nameof(room)}."); 

        /* The below idea is to make a triangle from the floor and the first 2 rooms in a list.
         * We pretend that the angle from the floor to the first wall is the ground truth rotation
         * See gotData for the second half
         */

        // set walls data
        //dataToSend.mainWall = room.WallAnchors[0].PlaneRect.Value;
        //dataToSend.secondaryWall = room.WallAnchors[1].PlaneRect.Value;

        //dataToSend.roomFloorPosData = room.FloorAnchor.transform.position;
        //dataToSend.roomFloorRotationData = room.transform.rotation;


        dataToSend.walls = new Vector3[room.WallAnchors.Count];
        int i = 0;

        foreach (var wall in room.WallAnchors)
        {
            dataToSend.walls[i] = wall.transform.position;
            i++;
        }

        //networkManager.Send(sendToPlayer, dataToSend);
        //Debug.LogError(room.WallAnchors[0].PlaneRect.Value); // gives x,y,width,height
        //Debug.LogError(room.WallAnchors[0].transform.position); // gives position... probably in testing it doesent give that so


        //SpatialLogger.Instance.LogInfo(room.Anchors.ToString());

        //triangle(room.FloorAnchor, room.WallAnchors[0], room.WallAnchors[1]); // idk what to do with this yet







        /* Below is the idea for using principle component analisis
         * The problem is that if more objects existed on one side of the room in one scan it skews the "center" of the room
         */
        //dataToSend.chosenWall = room.WallAnchors[0].gameObject; // is parent
        //Vector2 localPos = new Vector2();
        //room.Anchors.ForEach(anchor => {
        //    localPos.x += anchor.transform.position.x;
        //    localPos.y += anchor.transform.position.z; // y is height so we dont care.
        //});
        //localPos.x /= room.Anchors.Count;
        //localPos.y /= room.Anchors.Count;

        //Debug.LogError(localPos);

        //dataToSend.whereIAmInRoom = localPos;//getWhereIAmInRoom();
        //SpatialLogger.Instance.LogInfo($"I think your HERE: {dataToSend.whereIAmInRoom}.");
        //transform.position = dataToSend.whereIAmInRoom;

    }

    private void GotData(PlayerID player, sendData data, bool asServer)
    {       
        
        try {
            //mruk.enabled = true;
            //if (player != sendToPlayer)
            //{
            //    mruk.LoadSceneFromJsonString(dataToSend.jsonData);
            //}
            Debug.Log($"Recieved Data From: {player}");
            SpatialLogger.Instance.LogInfo($"Recieved Data From: {player}");
            roomDataReceived = data;

            // This is where the magic happens

            // get the same walls for both headsets
            //(int, int) loc = GetMainSecondaryWalls(data.mainWall, data.secondaryWall);

            // make the center of the room the center of the universe
            //Vector2 correctedWall = new Vector2(currentRoom.WallAnchors[loc.Item1].transform.position.x, currentRoom.WallAnchors[loc.Item1].transform.position.z); // y is up/down
            //correctedWall.x -= currentRoom.FloorAnchor.transform.position.x;
            //correctedWall.y -= currentRoom.FloorAnchor.transform.position.z;

            // get phi
            //var phi = Math.Atan(correctedWall.y / correctedWall.x) * Mathf.Rad2Deg; // returns a radian, we need degrees

            // set true "north" (updated for starting angle)
            theta = currentRoom.FloorAnchor.transform.eulerAngles.y;

            //triangle(currentRoom.FloorAnchor.transform.position,
            //    currentRoom.WallAnchors[loc.Item1].transform.position,
            //    currentRoom.WallAnchors[loc.Item2].transform.position);

            //triangle(headset.transform.position,
            //    currentRoom.FloorAnchor.transform.position,
            //    currentRoom.WallAnchors[loc.Item1].transform.position);


            //Debug.Log($"Wall locations in array: {loc}");

            //SpatialLogger.Instance.LogInfo($"Phi {phi} /|\\ Theta: {theta}");
            //Debug.Log($"Phi {phi} /|\\ Theta: {theta}");

            if (PointSetRegisration.EstimateRotation(dataToSend.walls, data.walls, out Quaternion rot))
            {
                //Debug.Log(pointSetRegistration.RunICP(dataToSend.walls, data.walls, 50, 1e-5f));
                theta = rot.y;
                Debug.Log($"PSR Rotation {rot.eulerAngles}");
                SpatialLogger.Instance.LogInfo($"PSR Rotation {rot.eulerAngles}");
            }




            //double HostDistance = distance(data.roomFloorPosData, Vector3.zero); // (a)
            //double ClientDistance = distance(currentRoom.FloorAnchor.transform.position, Vector3.zero); // (b)

            //double Rotation = currentRoom.transform.rotation.eulerAngles.y; // (angle B)

        }

        catch (Exception e) {
            Debug.LogError($"ERROR IN GETDATA: {e}");
            SpatialLogger.Instance.LogError($"ERROR IN GETDATA: {e}");
        }
        


    }

    private void SomeoneJoined(PlayerID player, bool isReconnect, bool asServer)
    {
        
        blinder.SetActive(false); // show that you loaded in. If your the server you will have already done this.
        try
        {
            if (networkManager.isServer || networkManager.isHost)
            {

                //mruk.SceneSettings.DataSource = MRUK.SceneDataSource.DeviceWithJsonFallback;
                //mruk.enabled = true;
                SpatialLogger.Instance.LogInfo($"Player '{player}' Connected {(isReconnect ? ": Reconnected." : "")}");
                Debug.Log($"Player '{player}' Connected {(isReconnect ? ": Reconnected." : "")}");


                //if (!SceneAndRoomInfoAvailable) // this is very bad if it hits
                //    dataToSend.jsonData = "No room data found.";

                sendToPlayer = player;
                networkManager.Send(sendToPlayer, dataToSend);
            }

            else if (networkManager.isClientOnly) // letting you know that you connected to the server
            {
                //mruk.SceneSettings.DataSource = MRUK.SceneDataSource.Json;
                SpatialLogger.Instance.LogInfo($"I Connected");
                Debug.Log($"I Connected");
                //networkManager.SendToServer(dataToSend);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"ERROR IN Player Join: {e}");
            SpatialLogger.Instance.LogError($"ERROR IN Player Join: {e}");
        }
        

    }


    //private void Update()
    //{
    //    SpatialLogger.Instance.Clear();


    //    SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} room orientation {currentRoom.FloorAnchor.transform.position} : {currentRoom.FloorAnchor.transform.rotation}.");
    //    SpatialLogger.Instance.LogInfo($"Headset Location: {headset.transform.position}");
    //}



    private (int, int) GetMainSecondaryWalls(Rect mainWall, Rect secondaryWall)
    {
        // The locations for where the 2 walls are most likly are in this rooms array
        // we need this as the meshes dont line up perfectly or are garenteed to be ordered the same

        (int,float) mainWallLocation = (-1,200); // location, distance away
        (int, float) secondaryWallLocation = (-1,200);
        double tune = 0.15; // margin for error of this #

        int index = 0; // because enumerating in C# kinda sucks
        foreach (var wall in currentRoom.WallAnchors)
        {
            Rect wallData = wall.PlaneRect.Value; // we dont need any other data here

            float comparisonMain = mainWall.x / wallData.x;
            float comparisonSecond = secondaryWall.x / wallData.x;

            comparisonMain *= mainWall.y / wallData.y;
            comparisonSecond *= secondaryWall.y / wallData.y;

            comparisonMain *= mainWall.width / wallData.width;
            comparisonSecond *= secondaryWall.width / wallData.width;

            comparisonMain *= mainWall.height / wallData.height;
            comparisonSecond *= secondaryWall.height / wallData.height;


            comparisonMain -= 1;
            comparisonSecond -= 1;


            // we are looking for values that are as close to 0 as possible
            // if we get a perfect 0 that means we have the same wall with the same json data (only in simulation can this happen)
            // if we have -1 then they are the oposite facing wall

            //Debug.LogAssertion($"Main: {comparisonMain}, Second: {comparisonSecond}");

            var newOldCompMain =  Math.Abs(mainWallLocation.Item2) - Math.Abs(comparisonMain); // new wall is closer to 0 than old wall
            var newOldCompSecond = Math.Abs(secondaryWallLocation.Item2) - Math.Abs(comparisonSecond); // if new wall is bigger than old wall it gives negitive

            //Debug.LogAssertion($"Main Comp: {newOldCompMain}, Second Comp: {newOldCompSecond}");


            if ((comparisonMain < tune) && (comparisonMain > -tune) && (newOldCompMain > 0))
            {
                if (newOldCompMain > newOldCompSecond) // to reduce the chance that we return the same wall for both main and second 
                    mainWallLocation = (index, comparisonMain);
            }
            if ((comparisonSecond < tune) && (comparisonSecond > -tune) && (newOldCompSecond > 0))
            {
                if (newOldCompSecond > newOldCompMain)
                    secondaryWallLocation = (index, comparisonSecond);
            }


            index++;

        }


        return (mainWallLocation.Item1, secondaryWallLocation.Item1);

        }



    private void Triangle(Vector3 floor, Vector3 mainWall, Vector3 secondaryWall)
    {
        // get distances for triangle calc (its easer to get than angles)
        var floorToMainWall = Distance(floor, mainWall); // b
        var floorToSecondWall = Distance(floor, secondaryWall); // c
        var wallToWall = Distance(mainWall, secondaryWall); // a

        // simple triangle angle maths 
        var angleBetweenFloorMain = Math.Acos((Sqr(floorToMainWall) + Sqr(floorToSecondWall) - Sqr(wallToWall)) / (2 * floorToMainWall * floorToSecondWall)); // A
        var angleBetweenFloorSec = Math.Acos((Sqr(wallToWall) + Sqr(floorToSecondWall) - Sqr(floorToMainWall)) / (2*wallToWall*floorToSecondWall)); // B
        var angleBetweenWalls = Math.Acos((Sqr(wallToWall) + Sqr(floorToMainWall) - Sqr(floorToSecondWall)) / (2*wallToWall*floorToMainWall)); // C

        // rad to deg conversion
        angleBetweenFloorMain *= (180 / Math.PI); 
        angleBetweenFloorSec *= (180 / Math.PI);
        angleBetweenWalls *= (180 / Math.PI);

        string line1 = $"Angle b FloorMain: {angleBetweenFloorMain}, Angle b FloorSec: {angleBetweenFloorSec}, Angle b Walls: {angleBetweenWalls}";
        string line2 = $"Distance b FloorMain: {floorToMainWall}, Distance b FloorSec: {floorToSecondWall}, Distance b Walls: {wallToWall}";

        //SpatialLogger.Instance.LogInfo(line1);
        //SpatialLogger.Instance.LogInfo(line2);
        Debug.LogError(line1);
        Debug.LogError(line2);

        theta += currentRoom.FloorAnchor.transform.rotation.y - angleBetweenFloorMain;
        Debug.LogError($"Adjust Angle: {theta}");
        SpatialLogger.Instance.LogInfo($"Adjust Angle: {theta}");
    }


    private double Distance(Vector3 a, Vector3 b)
    {
        float distance = 0;
        distance = (b.x - a.x) * (b.x - a.x); 
        distance += (b.y - a.y) * (b.y - a.y);
        distance += (b.z - a.z) * (b.z - a.z);

        return (Math.Sqrt(distance));
    }

    private double Sqr(double a) // idk squaring is weird in C#
    {
        return a*a;
    }

    //private Vector2 GetWhereIAmInRoom_Trilateration()
    //{
    //    // https://math.stackexchange.com/questions/100448/finding-location-of-a-point-on-2d-plane-given-the-distances-to-three-other-know
    //    // https://math.stackexchange.com/questions/884807/find-x-location-using-3-known-x-y-location-using-trilateration
    //    double x0;
    //    double y0;

    //    Vector2[] walls = new Vector2[currentRoom.WallAnchors.Count];
    //    double[] walldistances = new double[currentRoom.WallAnchors.Count];


    //    int index = 0; // for indexing foreach loops
    //    // get the x,y,d for each datasource (we are ignoring z as it can be found with a simple lookup)
    //    foreach (MRUKAnchor wall in currentRoom.WallAnchors)
    //    {
    //        walls[index] = new Vector2( wall.transform.position.x, wall.transform.position.z );
    //        walldistances[index] = distance(wall.transform.position, Vector3.zero);
    //        Debug.Log($"{walls[index].x},{walls[index].y},{distance(wall.transform.position, Vector3.zero)}");
    //        index++;
    //    }

    //    index = 0;

    //    var A = (-2 * walls[0].x + 2 * walls[1].x);
    //    var B = (-2 * walls[0].y + 2 * walls[1].y);
    //    var C = sqr(walldistances[0]) - sqr(walldistances[1]) - sqr(walls[0].x) + sqr(walls[1].x) - sqr(walls[0].y) + sqr(walls[1].y);

    //    var D = (-2 * walls[1].x + 2 * walls[2].x);
    //    var E = (-2 * walls[1].y + 2 * walls[2].y);
    //    var F = sqr(walldistances[1]) - sqr(walldistances[2]) - sqr(walls[1].x) + sqr(walls[2].x) - sqr(walls[1].y) + sqr(walls[2].y);

    //    x0 = (C * E - F * B) / (E * A - B * D);
    //    y0 = (C * D - A * F) / (B * D - A * E);

    //    //var D = distance(walls[0],walls[1]); // written as |S1S2| in the link above

    //    //var a = sqr(walldistances[0]) - sqr(walldistances[1]) + sqr(D);
    //    //a /= 2 * D;
    //    //var h = walldistances[0] - a;

    //    //x0 = walls[0].x + h * (walls[1].y - walls[0].y) / D;
    //    //y0 = walls[0].y + h * (walls[1].x - walls[0].x) / D;

    //    return new Vector2((float)x0, (float)y0);
    //}
}
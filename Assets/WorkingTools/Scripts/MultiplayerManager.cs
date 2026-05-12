using LearnXR.Core.Utilities;
using PurrNet;
using PurrNet.Packing;
using System;
using UnityEngine;


public class MultiplayerManager : MonoBehaviour
{
    
    public NetworkManager networkManager;
    public sendData dataToSend;
    public sendData roomDataReceived;

    public RockMenu rockMenu;



    // headset data
    [SerializeField] public GameObject headset;

    // here to make people not look at the world before they load in (lazy but effective)
    public GameObject blinder;



    private PlayerID sendToPlayer;



    [System.Serializable]
    public struct sendData : IPackedAuto
    {
        public string calledAction;
        public int state;
        //public string calledMessage;
        //public Action<GameObject> lastCommand;
        //public GameObject lastGameObj;
    }




    private void OnEnable()
    {
        // Connect listeners

        networkManager.Subscribe<sendData>(GotData); // for data

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

    }


    private void GotData(PlayerID player, sendData data, bool asServer)
    {
        try
        {
            if (player == networkManager.localPlayer) // no need to repeat everything for ourselves
                return;

            print($"Recieved Data From: {player}");


            if (rockMenu.rockState == data.state) // we dont need to repeat what was already done
                return;

            rockMenu.rockState = data.state+2; // one less than whatever we need (in an extreamly roundabout way)
            rockMenu.ChangeAllRockStates(rockMenu.holdMyRocks);

            if (networkManager.isServer || networkManager.isHost)
            {
                dataToSend.calledAction = data.calledAction;
                dataToSend.state = data.state;

                networkManager.SendToAll(dataToSend);
            }
        }

        catch (Exception e)
        {
            printError($"ERROR IN GETDATA: {e}");
        }
    }

    public void SendDataMessage()
    {
        if (networkManager.isServer || networkManager.isHost)
        {
            networkManager.SendToAll(dataToSend);
        }

        else if (networkManager.isClientOnly)
        {
            networkManager.SendToServer(dataToSend);
        }
        //networkManager.Send(sendToPlayer, dataToSend);
    }

    private void SomeoneJoined(PlayerID player, bool isReconnect, bool asServer)
    {

        blinder.SetActive(false); // show that you loaded in. If your the server you will have already done this.
        try
        {
            if (networkManager.isServer || networkManager.isHost)
            {
                if (player == networkManager.localPlayer)
                    print("You are Host.");
                else
                    print($"Player '{player}' Connected {(isReconnect ? ": Reconnected." : "")}");

                //sendToPlayer = player;
                //networkManager.Send(sendToPlayer, dataToSend);
            }

            else if (networkManager.isClientOnly) // letting you know that you connected to the server
            {
                //mruk.SceneSettings.DataSource = MRUK.SceneDataSource.Json;
                print($"I Connected");
                //networkManager.SendToServer(dataToSend);
            }
        }
        catch (Exception e)
        {
            printError($"ERROR IN Player Join: {e}");
        }


    }
    
    public void print(string message)
    {
        Debug.Log(message);
        SpatialLogger.Instance.LogInfo(message);
    }
    public void printError(string message)
    {
        Debug.LogError(message);
        SpatialLogger.Instance.LogError(message);
    }
}


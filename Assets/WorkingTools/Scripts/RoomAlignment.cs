
using LearnXR.Core.Utilities;
using PurrNet;
using PurrNet.Transports;
using UnityEngine;

public class RoomAlignment : MonoBehaviour
{
    private NetworkManager networkManager = new();

    private void OnEnable()
    {
        NetworkManager.onAnyClientConnectionState += BindRoomInfo;
    }

    private void BindRoomInfo(ConnectionState state)
    {

        if (state == ConnectionState.Connected)
            SpatialLogger.Instance.LogInfo($"Someone Connected");
            //networkManager.SendToServer("eee", Channel.ReliableOrdered);
    }

    private void OnDisable()
    {
        NetworkManager.onAnyClientConnectionState -= BindRoomInfo;
        //mruk.RoomCreatedEvent.RemoveListener(BindRoomInfo);
    }

}

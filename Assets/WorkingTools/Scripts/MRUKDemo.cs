using LearnXR.Core.Utilities;
using Meta.XR.MRUtilityKit;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// file yoinked from https://youtu.be/3sVgwPxR4TE

public class MRUKDemo : MonoBehaviour
{

    [SerializeField] private MRUK mruk;
    [SerializeField] private OVRInput.Controller controller;
    [SerializeField] private GameObject objectForWallAnchorsPrefab;
    [SerializeField] private GameObject objectForGroundAnchors;
    [SerializeField] private GameObject headset;

    private bool sceneHasBeenLoaded;
    private MRUKRoom currentRoom;
    private List<GameObject> wallAnchorObjectsCreated = new();
    private bool SceneAndRoomInfoAvailable => currentRoom != null && sceneHasBeenLoaded;

    private void OnEnable()
    {
        mruk.RoomCreatedEvent.AddListener(BindRoomInfo);
        
    }

    private void OnDisable()
    {
        mruk.RoomCreatedEvent.RemoveListener(BindRoomInfo);
    }

    public void EnableMRUKDemo()
    {
        sceneHasBeenLoaded = true;
        SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} has been enabled due to scene availability.");
        SpatialLogger.Instance.LogInfo($"Headset Location: {headset.transform.position}");
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            SpatialLogger.Instance.LogInfo($"Button Clicked");
        }
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller) && SceneAndRoomInfoAvailable)
        {
            
            if (wallAnchorObjectsCreated.Count == 0)
            {
                foreach (var wallAnchor in currentRoom.WallAnchors)
                {
                    var createdWallObject = Instantiate(objectForWallAnchorsPrefab, Vector3.zero, Quaternion.identity, wallAnchor.transform);
                    createdWallObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    wallAnchorObjectsCreated.Add(createdWallObject);
                    SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} wall object created with Uuid: {wallAnchor.Anchor.Uuid}.");
                }
                SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} wall objects added to all walls.");
            }
            else
            {
                foreach (var wallObject in wallAnchorObjectsCreated)
                {
                    Destroy(wallObject);
                }
                wallAnchorObjectsCreated.Clear();
                SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} wall objects were deleated.");
            }
        }
    }

    public void BindRoomInfo(MRUKRoom room)
    {
        currentRoom = room;
        //Debug.Log(room.transform.position);
        //Debug.Log(room.transform.rotation);
        SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} room was bound to current room.");
        SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} room id {nameof(room)}.");
        //SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} room orientation {room.transform.position} : {room.transform.rotation}.");
        //SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} room {}");
    }
}


// from scene debugger
/// <summary>
/// Exports the current scene data to a JSON file if the specified condition is met.
/// </summary>
/// <param name="isOn">If set to true, the scene data will be exported to JSON.</param>
//public void ExportJSON(bool isOn)
//{
//    try
//    {
//        if (isOn)
//        {
//            bool exportGlobalMesh = true;
//            if (exportGlobalMeshJSONDropdown)
//            {
//                exportGlobalMesh = exportGlobalMeshJSONDropdown.options[exportGlobalMeshJSONDropdown.value].text.ToLower() == "true";
//            }
//            var scene = MRUK.Instance.SaveSceneToJsonString(
//            exportGlobalMesh
//            );
//            var filename = $"MRUK_Export_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.json";
//            var path = Path.Combine(Application.persistentDataPath, filename);
//            File.WriteAllText(path, scene);
//            Debug.Log($"Saved Scene JSON to {path}");
//        }
//    }
//    catch (Exception e)
//    {
//        SetLogsText("\n[{0}]\n {1}\n{2}",
//            nameof(ExportJSON),
//            e.Message,
//            e.StackTrace
//        );
//    }
//}
using PurrNet;
using System;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class RockMenu : MonoBehaviour
{

    private const float SpawnDistanceFromCamera = 0.75f;
    public OVRCameraRig _cameraRig;
    private Canvas _canvas;

    [Tooltip("Helper for ray interactions")]
    public OVRRayHelper RayHelper;
    [Tooltip("Input module for handling VR input")]
    public OVRInputModule InputModule;
    [Tooltip("Raycaster for handling ray interactions")]
    public OVRRaycaster Raycaster;
    [Tooltip("Gaze pointer for VR interactions")]
    public OVRGazePointer GazePointer;

    public GameObject holdMyRocks;
    public PlayerID player;

    public GameObject Log;
    public GameObject UsageBox;

    private Action<GameObject> lastCommand;
    private GameObject lastGameObj;
    public int rockState = 0; // 0 is kinematic, 1 is not kinematic (asteroids), and 2 is with gravity


    //public GameObject rockButtonHolder;
    //public GameObject rockButtonTemp;
    //public string jsonFile;

    private void Awake()
    {
        //StartCoroutine();
        _canvas = GetComponent<Canvas>();
        SnapMenuInFrontOfCamera();
        SetupInteractionDependencies();
        //    string folderPath = Application.dataPath + "/WorkingTools/Rocks";

        //    if (Directory.Exists(folderPath))
        //    {
        //        //string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly); // Fetch all files
        //        string[] folders = Directory.GetDirectories(folderPath, "*", SearchOption.TopDirectoryOnly); // Fetch all folders
        //        foreach (string folder in folders)
        //        {
        //            Debug.Log("Folder: " + folder);
        //            Resources.Load(folder); // If we do this method you need to put everything inside of the Resources folder
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Folder does not exist: " + folderPath);
        //    }
    }

    //private void CreateButton(string folderName,Sprite image)
    //{
    //    GameObject newButton = Instantiate(rockButtonTemp);
    //    newButton.name = folderName;

    //    TMP_Text text = newButton.GetComponentInChildren<TMPro.TMP_Text>();
    //    text.text = folderName.Substring(0,1).ToUpper() + folderName.Substring(1); // make the folder name capital

    //    newButton.GetComponentInChildren<Image>().sprite = image;

    //}



    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Start))
        {
            ToggleMenu();
        }
        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            lastCommand.Invoke(lastGameObj);
        }


        if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            ChangeAllRockStates(holdMyRocks);
        }
        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            DestroyAllMyRocks(holdMyRocks);
        }
        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            ToggleLogAndHelp(null);
        }
        

        Billboard();
    }

    public void CreateRock(GameObject rock)
    {
        GameObject newRock = Instantiate(rock);
        newRock.transform.SetParent(holdMyRocks.transform);
        newRock.transform.position = _cameraRig.rightControllerAnchor.gameObject.transform.position;
        newRock.transform.rotation = _cameraRig.rightControllerAnchor.gameObject.transform.rotation;
        ChangeRockState(newRock);

        if (lastCommand != CreateRock || lastGameObj != rock)
        {
            lastCommand = CreateRock;
            lastGameObj = rock;
        }
    }

    public void DestroyAllMyRocks(GameObject rockHolder)
    {
        while (rockHolder.transform.childCount > 0)
        {
            DestroyImmediate(rockHolder.transform.GetChild(0).gameObject);
        }
        if (lastCommand != DestroyAllMyRocks || lastGameObj != rockHolder)
        {
            lastCommand = DestroyAllMyRocks;
            lastGameObj = rockHolder;
        }
    }

    public void ChangeAllRockStates(GameObject rockHolder)
    {
        rockState++;
        rockState = rockState % 3; // loop back to 0

        foreach (Transform rock in rockHolder.transform)
        {
            ChangeRockState(rock.gameObject);
        }

        if (lastCommand != ChangeAllRockStates || lastGameObj != rockHolder)
        {
            lastCommand = ChangeAllRockStates;
            lastGameObj = rockHolder;
        }
    }

    public void ChangeRockState(GameObject rock)
    {
        Rigidbody rockBody = rock.GetComponentInChildren<Rigidbody>();
        rockBody.isKinematic = rockState<1; // 0 is kin
        // 1 is no grav no kin (asteroids)
        rockBody.useGravity = rockState == 2; // 2 is grav

    }





    void ToggleLogAndHelp(GameObject Dummy)
    {
        Log.SetActive(!Log.activeInHierarchy);
        UsageBox.SetActive(!UsageBox.activeInHierarchy);

        if (lastCommand != ToggleLogAndHelp || lastGameObj != null)
        {
            lastCommand = ToggleLogAndHelp;
            lastGameObj = null;
        }
    }


    // Makes the menu look at you
    private void Billboard()
    {
        if (!_canvas)
        {
            return;
        }

        var direction = _canvas.transform.position - _cameraRig.centerEyeAnchor.transform.position;
        if (direction.sqrMagnitude > 0.01f)
        {
            var rotation = Quaternion.LookRotation(direction);
            _canvas.transform.rotation = rotation;
        }
    }

    private void ToggleMenu()
    {
        _canvas.enabled = !_canvas.enabled;
        if (_canvas.enabled)
            SnapMenuInFrontOfCamera();
    }

    private void SnapMenuInFrontOfCamera()
    {
        transform.position = _cameraRig.centerEyeAnchor.transform.position +
                             _cameraRig.centerEyeAnchor.transform.forward * SpawnDistanceFromCamera;
    }

    // All the stuff for the controls
    private void SetupInteractionDependencies()
    {
        if (!_cameraRig)
        {
            return;
        }

        GazePointer.rayTransform = _cameraRig.centerEyeAnchor;
        InputModule.rayTransform = _cameraRig.rightControllerAnchor;
        Raycaster.pointer = _cameraRig.rightControllerAnchor.gameObject;
        if (_cameraRig.GetComponentsInChildren<OVRRayHelper>(false).Length > 0)
        {
            return;
        }

        var rightControllerHelper =
            _cameraRig.rightControllerAnchor.GetComponentInChildren<OVRControllerHelper>();
        if (rightControllerHelper)
        {
            rightControllerHelper.RayHelper =
                Instantiate(RayHelper, Vector3.zero, Quaternion.identity, rightControllerHelper.transform);
            rightControllerHelper.RayHelper.gameObject.SetActive(true);
        }

        var leftControllerHelper =
            _cameraRig.leftControllerAnchor.GetComponentInChildren<OVRControllerHelper>();
        if (leftControllerHelper)
        {
            leftControllerHelper.RayHelper =
                Instantiate(RayHelper, Vector3.zero, Quaternion.identity, leftControllerHelper.transform);
            leftControllerHelper.RayHelper.gameObject.SetActive(true);
        }

        var hands = _cameraRig.GetComponentsInChildren<OVRHand>();
        foreach (var hand in hands)
        {
            hand.RayHelper =
                Instantiate(RayHelper, Vector3.zero, Quaternion.identity, _cameraRig.trackingSpace);
            hand.RayHelper.gameObject.SetActive(true);
        }
    }

    private Ray GetControllerRay()
    {
        Vector3 rayOrigin;
        Vector3 rayDirection;
        if (OVRInput.activeControllerType == OVRInput.Controller.Touch
            || OVRInput.activeControllerType == OVRInput.Controller.RTouch)
        {
            rayOrigin = _cameraRig.rightHandOnControllerAnchor.position;
            rayDirection = _cameraRig.rightHandOnControllerAnchor.forward;
        }
        else if (OVRInput.activeControllerType == OVRInput.Controller.LTouch)
        {
            rayOrigin = _cameraRig.leftHandOnControllerAnchor.position;
            rayDirection = _cameraRig.leftHandOnControllerAnchor.forward;
        }
        else // hands
        {
            var rightHand = _cameraRig.rightHandAnchor.GetComponentInChildren<OVRHand>();
            // can be null if running in Editor with Meta Linq app and the headset is put off
            if (rightHand != null)
            {
                rayOrigin = rightHand.PointerPose.position;
                rayDirection = rightHand.PointerPose.forward;
            }
            else
            {
                rayOrigin = _cameraRig.centerEyeAnchor.position;
                rayDirection = _cameraRig.centerEyeAnchor.forward;
            }
        }

        return new Ray(rayOrigin, rayDirection);
    }
}

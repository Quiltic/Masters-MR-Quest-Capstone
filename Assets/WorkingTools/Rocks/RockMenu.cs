using PurrNet;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class RockMenu : MonoBehaviour
{

    private const float SpawnDistanceFromCamera = 0.75f;
    [Tooltip("The Headset")]
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

    [Tooltip("The 'folder' that holds a person's rocks")]
    public GameObject holdMyRocks;
    [Tooltip("The current player")]
    public PlayerID player;

    [Tooltip("The Log Window")]
    public GameObject Log;
    [Tooltip("The Help Window")]
    public GameObject Help;


    public Action<GameObject> lastCommand;
    public GameObject lastGameObj;
    [Tooltip("What state the Rocks should be in")]
    public int rockState = 0; // 0 is kinematic, 1 is not kinematic (asteroids), and 2 is with gravity

    [Tooltip("The default template for the rock.")]
    public GameObject rockTemplate;

    public MultiplayerManager multiplayerManager;


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

    public void CallLastCommand(Action<GameObject> command, GameObject gameObj)
    {
        command.Invoke(gameObj);
    }

    public void TellEveryoneAboutChange()
    {
        multiplayerManager.dataToSend.calledAction = "Change Rock States";
        multiplayerManager.dataToSend.state = rockState;
        //multiplayerManager.dataToSend.lastCommand = lastCommand;
        //multiplayerManager.dataToSend.lastGameObj = lastGameObj;
        multiplayerManager.SendDataMessage();
    }


    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Start))
        {
            ToggleMenu();
        }
        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            multiplayerManager.print($"Repeating last action!");
            CallLastCommand(lastCommand, lastGameObj);
            //TellEveryoneAboutChange();
        }


        if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            ChangeAllRockStates(holdMyRocks);
            //UpdateLastData(ChangeAllRockStates, holdMyRocks);
            //TellEveryoneAboutChange();
        }
        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            DestroyAllMyRocks(holdMyRocks);
            //TellEveryoneAboutChange();
        }
        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            ToggleLogAndHelp(null);
        }
        

        Billboard();
    }

    void UpdateLastData(Action<GameObject> command, GameObject gameObj)
    {
        if (lastCommand != command || lastGameObj != gameObj)
        {
            lastCommand = command;
            lastGameObj = gameObj;
        }
    }

    /// <summary>
    /// Make a Rock based on the Game Object For them
    /// </summary>
    /// <param name="rock"></param>
    public void CreateRock(GameObject rock)
    {
        multiplayerManager.print($"Creating a {rock.name}!");
        GameObject newRock = Instantiate(rock);
        newRock.transform.SetParent(holdMyRocks.transform);
        newRock.transform.position = _cameraRig.rightControllerAnchor.gameObject.transform.position;
        newRock.transform.rotation = _cameraRig.rightControllerAnchor.gameObject.transform.rotation;
        ChangeRockState(newRock);

        UpdateLastData(CreateRock, rock);
    }

    /// <summary>
    /// Destroy all Rocks owned by a Person
    /// </summary>
    /// <param name="rockHolder"></param>
    public void DestroyAllMyRocks(GameObject rockHolder)
    {
        multiplayerManager.print($"Destroying all Rocks!");
        int childLength = holdMyRocks.transform.childCount;
        GameObject[] children = new GameObject[childLength];

        multiplayerManager.print($"Number of rocks to del: {childLength} ");

        for (int i = 0; i < childLength; i++)
        {
            children[i] = holdMyRocks.transform.GetChild(i).gameObject;
        }

        foreach (GameObject child in children)
        {
            Destroy(child);
        }
        
        UpdateLastData(DestroyAllMyRocks, rockHolder);
    }

    /// <summary>
    /// Change the Rock State for each person's Rocks
    /// </summary>
    /// <param name="rockHolder"></param>
    public void ChangeAllRockStates(GameObject rockHolder)
    {
        multiplayerManager.print($"Switching rock states to: {((rockState == 0) ? "Static" : (rockState == 2) ? "Gravity" : "Astroids") } ");
        rockState++;
        rockState = rockState % 3; // loop back to 0

        foreach (Transform rock in rockHolder.transform)
        {
            ChangeRockState(rock.gameObject);
        }

        UpdateLastData(ChangeAllRockStates, rockHolder);
        TellEveryoneAboutChange();
    }

    /// <summary>
    /// Update the state of the Rocks between: Static, Asteroid, and Gravity
    /// </summary>
    /// <param name="rock"></param>
    public void ChangeRockState(GameObject rock)
    {
        Rigidbody rockBody = rock.GetComponentInChildren<Rigidbody>();
        rockBody.isKinematic = rockState<1; // 0 is kin (static)
        // 1 is no grav no kin (asteroids)
        rockBody.useGravity = rockState == 2; // 2 is grav
    }




    /// <summary>
    /// Toggle the Help and Log window (they are kinda intrusive)
    /// </summary>
    /// <param name="Dummy"></param>
    void ToggleLogAndHelp(GameObject Dummy)
    {
        Log.SetActive(!Log.activeInHierarchy);
        Help.SetActive(!Help.activeInHierarchy);

        if (lastCommand != ToggleLogAndHelp || lastGameObj != null)
        {
            lastCommand = ToggleLogAndHelp;
            lastGameObj = null;
        }
    }


    //////////////////////////////////////////////////////////////
    /// Below Is all the stuff for the Menu and controling it. ///
    //////////////////////////////////////////////////////////////

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

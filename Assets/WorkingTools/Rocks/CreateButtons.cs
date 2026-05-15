using PurrNet;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityMeshSimplifier;

public class CreateButtons : MonoBehaviour
{
    // Taken from https://github.com/Whinarn/UnityMeshSimplifier/wiki/LOD-Generator-API
    [SerializeField, Tooltip("The simplification options.")]
    private SimplificationOptions simplificationOptions = SimplificationOptions.Default;
    [SerializeField, Tooltip("If renderers should be automatically collected, otherwise they must be manually applied for each level.")]
    private bool autoCollectRenderers = true;
    [SerializeField, Tooltip("The LOD levels.")]
    private LODLevel[] levels = new LODLevel[]
        {
            new LODLevel(0.75f, 1f)
            {
                CombineMeshes = false,
                CombineSubMeshes = false,
                SkinQuality = SkinQuality.Auto,
                ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                ReceiveShadows = true,
                SkinnedMotionVectors = true,
                LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes,
                ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes,
            },
            new LODLevel(0.45f, 0.45f)
            {
                CombineMeshes = true,
                CombineSubMeshes = false,
                SkinQuality = SkinQuality.Auto,
                ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                ReceiveShadows = true,
                SkinnedMotionVectors = true,
                LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes,
                ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Simple
            },
            new LODLevel(0.02f, 0.25f)
            {
                CombineMeshes = true,
                CombineSubMeshes = true,
                SkinQuality = SkinQuality.Bone2,
                ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                ReceiveShadows = false,
                SkinnedMotionVectors = false,
                LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off,
                ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off
            }
        };


    [Tooltip("The Menu holder for the buttons.")]
    public GameObject rockButtonHolder;
    [Tooltip("The default template for the buttons.")]
    public GameObject rockButtonTemp;



    [Tooltip("The scale for the rocks.")]
    public float rockScale = 0.20f;
    [Tooltip("The default template for the rock.")]
    public GameObject rockTemplate;
    [Tooltip("The default template for the rock.")]
    public Action<GameObject> CreateRockAction;


    private GameObject[] rocksToPrint;



    public void CreateMenu()
    {
        string folderPath = Application.dataPath + "/Resources/Rocks";

        if (Directory.Exists(folderPath))
        {
            //string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly); // Fetch all files
            string[] folders = Directory.GetDirectories(folderPath, "*", SearchOption.TopDirectoryOnly); // Fetch all folders
            rocksToPrint = new GameObject[folders.Length];

            int indx = 0;
            foreach (string folder in folders)
            {
                string folderName = folder.Replace(folderPath + "\\", ""); // lazy way of doing this
                folderName = folderName.Substring(0, 1).ToUpper() + folderName.Substring(1); // make the folder name capital
                Debug.Log("Folder: " + folder);
                //Resources.Load(folder); // If we do this method you need to put everything inside of the Resources folder

                

                Sprite image = Resources.Load<Sprite>($"Rocks/{folderName}/image");
                //Debug.LogError($"{image.name}: Rocks/{folderName}/image");
                CreateButton(folderName, image, indx);

                GameObject rockOBJ = Resources.Load<GameObject>($"Rocks/{folderName}/mesh");
                //Debug.LogError($"{rockOBJ.transform.Find("default").gameObject}: Rocks/{folderName}/mesh"); // we need its baby since that has all the stuff I want

                rocksToPrint[indx] = CreateRockToPrint(rockOBJ.transform.Find("default").gameObject, folderName);

                indx++;
            }
        }
        else
        {
            Debug.LogError("Folder does not exist: " + folderPath);
        }
    }

    private void CreateButton(string folderName, Sprite image, int index)
    {
        GameObject newButton = UnityProxy.InstantiateDirectly(rockButtonTemp);
        newButton.name = folderName;

        newButton.transform.SetParent(rockButtonHolder.transform, false);
        newButton.transform.localPosition = new Vector3(0, 34.8f - 10*(index), 0);


        TMP_Text text = newButton.GetComponentInChildren<TMPro.TMP_Text>();
        text.text = folderName;

        GameObject imageHolder = newButton.transform.Find("Image").gameObject;
        imageHolder.GetComponentInChildren<Image>().sprite = image;


        newButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            //Debug.LogError($"RockToPrint: {rocksToPrint[index]}");
            CreateRockAction(rocksToPrint[index]);
        });

    }



    private GameObject CreateRockToPrint(GameObject rockOBJ, string _name)
    {
        GameObject rock = UnityProxy.InstantiateDirectly(rockTemplate);

        // center the rock apropreatly based on mesh center (because some meshes arnt centered and im tired of doing it manualy)
        Mesh rockMesh = rockOBJ.GetComponent<MeshFilter>().sharedMesh;
        GameObject visual = rock.transform.Find("visual").gameObject;


        visual.transform.position = -1 * rockMesh.bounds.center;
        visual.GetComponent<MeshFilter>().mesh = rockMesh;
        visual.GetComponent<MeshRenderer>().sharedMaterial = rockOBJ.GetComponent<MeshRenderer>().sharedMaterial;
        visual.GetComponent<MeshCollider>().sharedMesh = rockMesh;

        // for roughly equal size rocks
        Vector3 rescale = rockMesh.bounds.size;
        float scale = Mathf.Max(Mathf.Max(rescale.x, rescale.y), rescale.z); // find the largest dimention (because some rocks are long not tall
        scale = 1 + (1 - scale);// get distance from 1
        rescale = new Vector3(scale,scale,scale); // new size
        rescale *= rockScale; // adjust for hand size

        rock.transform.localScale = rescale;
        

        LODGenerator.GenerateLODs(visual, levels, autoCollectRenderers, simplificationOptions);


        rock.name = _name;
        rock.transform.position = Vector3.one * 10000; // way beond the cull mesh so they take little to no render memory
        return (rock);
    }
}

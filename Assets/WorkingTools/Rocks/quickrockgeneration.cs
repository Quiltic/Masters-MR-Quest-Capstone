using Unity.VisualScripting;
using UnityEngine;
using UnityMeshSimplifier;

public class quickrockgeneration : MonoBehaviour
{

    public GameObject rockTemplate;
    public GameObject rockOBJ;

    private Mesh rockMesh;


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


    public GameObject rockToPrint;

    private GameObject CreateRockToPrint()
    {
        GameObject rock = Instantiate(rockTemplate);

        // center the rock apropreatly based on mesh center (because some meshes arnt centered and im tired of doing it manualy)
        rockMesh = rockOBJ.GetComponent<MeshFilter>().sharedMesh;
        GameObject visual = rock.transform.Find("visual").gameObject;


        visual.transform.position = -1 * rockMesh.bounds.center;
        visual.GetComponent<MeshFilter>().mesh = rockMesh;
        visual.GetComponent<MeshRenderer>().sharedMaterial = rockOBJ.GetComponent<MeshRenderer>().sharedMaterial;
        visual.GetComponent<MeshCollider>().sharedMesh = rockMesh;

        LODGenerator.GenerateLODs(visual, levels, autoCollectRenderers, simplificationOptions);


        rock.name = "ROCK!";

        rockToPrint = rock;
        return(rock);
    }

    void Start()
    {
        GameObject rock = CreateRockToPrint();
        rock.transform.position = new Vector3(0,12,0);
        Debug.Log(rock);
    }

    


}

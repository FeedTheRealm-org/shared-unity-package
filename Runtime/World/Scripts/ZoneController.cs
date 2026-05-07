using FTRShared.Runtime.Models;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ZoneController : MonoBehaviour
{
    [Header("World Settings")]
    [SerializeField]
    private float worldSize = 100f;

    [SerializeField]
    private float worldHeight = 10f;

    [Header("Floor Material Settings")]
    [SerializeField]
    private Material defaultMaterial = null;

    [SerializeField]
    private float defaultGranularity = 100f;
    public ZoneAreaData Data { get; set; } = new ZoneAreaData();

    private Renderer worldRenderer;

    void OnEnable()
    {
        worldRenderer = GetComponent<Renderer>();

        if (string.IsNullOrEmpty(Data.zoneMaterialId))
        {
            if (defaultMaterial != null)
            {
                worldRenderer.material = defaultMaterial;
                Data.zoneMaterialId = string.Empty;
                Data.textureGranularity = defaultGranularity;
            }
            ApplyTextureScale();
        }
    }

    void OnValidate()
    {
        worldRenderer = GetComponent<Renderer>();
        transform.localScale = new Vector3(worldSize, worldHeight, worldSize);
    }

    public void SetSkyboxMaterial(Material material, string materialId)
    {
        RenderSettings.skybox = material;
        DynamicGI.UpdateEnvironment();
        Data.skyboxMaterialId = materialId;
    }

    public void ChangeMaterial(Material material, string materialId)
    {
        if (material == null)
            return;
        worldRenderer.material = material;
        Data.zoneMaterialId = materialId;
        Debug.Log($"[ZoneController] Material set to '{Data.zoneMaterialId}'");
        ApplyTextureScale();
    }

    public void ApplyTextureGranularity(float granularity = 100f)
    {
        worldRenderer ??= GetComponent<Renderer>();
        defaultGranularity = granularity < 0 ? defaultGranularity : granularity;
        Data.textureGranularity = defaultGranularity;
        ApplyTextureScale();
    }

    private void ApplyTextureScale()
    {
        if (worldRenderer == null || worldRenderer.material == null)
            return;
        worldRenderer.material.mainTextureScale = new Vector2(
            defaultGranularity,
            defaultGranularity
        );
    }
}

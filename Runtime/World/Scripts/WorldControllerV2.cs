using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class WorldControllerV2 : MonoBehaviour
{
    [Header("World Settings")]
    [SerializeField]
    private float worldSize = 100f;

    [SerializeField]
    private float worldHeight = 10f;

    [SerializeField]
    private float textureGranularity = 4;

    private Renderer _renderer;

    void OnEnable()
    {
        if (_renderer == null)
            _renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        ApplyTextureScale();
    }

    void OnValidate()
    {
        if (_renderer == null)
            _renderer = GetComponent<Renderer>();

        transform.localScale = new Vector3(worldSize, worldHeight, worldSize);
    }

    public void OnFloorMaterialChanged(Material newMat, float granularity = -1f)
    {
        Debug.Log($"Changing floor material to: {newMat.name}");
        if (_renderer != null && newMat != null)
        {
            _renderer.material = newMat;

            if (granularity > 0f)
                textureGranularity = granularity;

            ApplyTextureScale();
        }
    }

    public void ApplyTextureGranularity(float granularity)
    {
        textureGranularity = granularity;
        ApplyTextureScale();
    }

    private void ApplyTextureScale()
    {
        if (_renderer == null)
            return;

#if UNITY_EDITOR
        if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
        {
            if (_renderer.sharedMaterial != null)
                _renderer.sharedMaterial.mainTextureScale = new Vector2(
                    textureGranularity,
                    textureGranularity
                );
            return;
        }
#endif

        if (_renderer.material != null)
        {
            _renderer.material.mainTextureScale = new Vector2(
                textureGranularity,
                textureGranularity
            );
        }
    }
}

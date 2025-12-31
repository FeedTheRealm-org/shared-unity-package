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

    void OnValidate()
    {
        if (_renderer == null)
            _renderer = GetComponent<Renderer>();

        if (_renderer != null)
        {
            _renderer.sharedMaterial.mainTextureScale = new Vector2(
                textureGranularity,
                textureGranularity
            );
        }

        transform.localScale = new Vector3(worldSize, worldHeight, worldSize);
    }
}

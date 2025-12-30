using System;
using UnityEngine;

[Serializable]
public class SceneReference
{
    [SerializeField]
    private string sceneName;

#if UNITY_EDITOR
    [SerializeField]
    private UnityEditor.SceneAsset sceneAsset;
#endif

    public string SceneName => sceneName;

#if UNITY_EDITOR
    public UnityEditor.SceneAsset SceneAsset
    {
        get => sceneAsset;
        set
        {
            sceneAsset = value;
            sceneName =
                value != null
                    ? System.IO.Path.GetFileNameWithoutExtension(
                        UnityEditor.AssetDatabase.GetAssetPath(value)
                    )
                    : "";
        }
    }
#endif
}

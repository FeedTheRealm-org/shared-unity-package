#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferenceDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        var sceneAssetProp = property.FindPropertyRelative("sceneAsset");
        var sceneNameProp = property.FindPropertyRelative("sceneName");

        EditorGUI.BeginChangeCheck();
        var newScene = (SceneAsset)EditorGUI.ObjectField(position, label, sceneAssetProp.objectReferenceValue, typeof(SceneAsset), false);
        if (EditorGUI.EndChangeCheck()) {
            sceneAssetProp.objectReferenceValue = newScene;
            sceneNameProp.stringValue = newScene != null
                ? System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(newScene))
                : "";
        }

        EditorGUI.EndProperty();
    }
}
#endif


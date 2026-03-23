using UnityEditor;
using UnityEngine;

public class TagSceneCamera : Editor {
    [MenuItem("Tools/Tag Scene Camera")]
    public static void TagIt() {
        if (SceneView.lastActiveSceneView != null) {
            // Give the hidden Scene Camera the "MainCamera" tag (or any tag you have)
            SceneView.lastActiveSceneView.camera.tag = "SceneCamera";
            Debug.Log("Scene Camera tagged as SceneCamera!");
        }
    }
}
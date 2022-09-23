#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public sealed class SceneMenu
{
    [MenuItem("Tools/Scene/InitScene 시작 _F2", false, 1)]
    private static void StartInitScene()
    {
        if (Application.isPlaying == false)
        {
            if (EditorSceneManager.GetActiveScene().isDirty)
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            EditorSceneManager.OpenScene("Assets/Scenes/InitScene.unity");
            EditorApplication.isPlaying = true;
        }
    }

    [MenuItem("Tools/Scene/현재 화면 스크린샷 _F4", false, 2)]
    private static void CaptureScreenShot()
    {
        ScreenCapture.CaptureScreenshot(string.Format("Capture_{0}.png", System.DateTime.Now.ToFileTime()));
        Debug.Log("[ Capture ] : " + System.DateTime.Now + " Complete.");
    }
}

public sealed class ToolMenu
{
    [MenuItem("Tools/Tool/내부 데이터 삭제", false, 1)]
    private static void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log(string.Format("[ Tool ][ ClearAllPlayerPrefs ] : Clear Complete."));
    }

    [MenuItem("Tools/Tool/메쉬 콜라이더 제거", false, 2)]
    private static void RemoveMeshCollider()
    {
        foreach (GameObject obj in Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets))
        {
            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(obj) as GameObject;

            MeshCollider[] meshes = prefabInstance.GetComponentsInChildren<MeshCollider>(true);
            foreach (var mesh in meshes)
                Object.DestroyImmediate(mesh);

            PrefabUtility.ApplyPrefabInstance(prefabInstance.gameObject, InteractionMode.UserAction);
            AssetDatabase.SaveAssets();
            Object.DestroyImmediate(prefabInstance.gameObject);
        }
    }

    [MenuItem("Tools/Tool/메쉬 콜라이더 확인", false, 3)]
    private static void CheckMeshCollider()
    {
        foreach (GameObject obj in Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets))
        {
            MeshCollider[] meshes = obj.GetComponentsInChildren<MeshCollider>(true);
            if (meshes.Length > 0)
                Debug.Log(string.Format("[ MeshCollider ][ {0} ] : {1}", obj.name, meshes.Length));
        }
    }
}

#endif
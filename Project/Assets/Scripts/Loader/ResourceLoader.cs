using UnityEngine;
using UnityEngine.SceneManagement;

public interface IResourceLoader
{
    TextAsset LoadData(string assetName);
    AudioClip LoadSoundBGM(string assetName);
    AudioClip LoadSoundEffect(string assetName);
    T LoadUIPrefab<T>(string assetName) where T : Object;
    T LoadTexture<T>(string assetName) where T : Object;
    void LoadModel(string bundleName, string assetName, System.Action<Object> callback);
    void LoadEffect(string assetName, System.Action<Object> callback);

    void Preload();

    /// <summary> 모든 에셋 번들을 언로드합니다. </summary>
    void UnloadAllAssetBundle(bool isForce);
    /// <summary> 프리로드된 에셋 번들을 제외한 모든 에셋 번들을 언로드합니다. </summary>
    void UnloadLoadedAssetBundle(bool isForce);

    void ClearPool();
}

public class ResourceLoader
{
    #region VALUES
    
    public enum LoadType
    {
        Editor,
        Stream,
        Download,
        Binary,
    }

    private static IResourceLoader Loader = null;

    #endregion

    #region PROPERTY

    public static LoadType LoaderType { get; set; } = LoadType.Editor;

    private static IResourceLoader GetLoader
    {
        get {
            if (Loader != null)
                return Loader;
            else
            {
                switch (LoaderType)
                {
#if UNITY_EDITOR
                    case LoadType.Editor: return Loader = new EditorLoader();
#endif
                    case LoadType.Stream: return Loader = new StreamLoader();
                    case LoadType.Download: return Loader = new DownloadLoader();
                    case LoadType.Binary: return Loader = new BinaryLoader();
                    default: return null;
                }
            }
        }
    }
    
    #endregion

    #region PUBLIC METHOD

    public static void LoadLevel(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public static AsyncOperation LoadLevelAsync(string sceneName)
    {
        return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
    }

    public static AsyncOperation LoadLevelAsyncAdditive(string sceneName)
    {
        return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public static TextAsset LoadData(string assetName)
    {
        return GetLoader.LoadData(assetName);
    }

    public static AudioClip LoadSoundBGM(string assetName)
    {
        return GetLoader.LoadSoundBGM(assetName);
    }

    public static AudioClip LoadSoundEffect(string assetName)
    {
        return GetLoader.LoadSoundEffect(assetName);
    }

    public static T LoadUIPrefab<T>(string assetName) where T : Object
    {
        return GetLoader.LoadUIPrefab<T>(assetName);
    }

    public static T LoadTexture<T>(string assetName) where T : Object
    {
        return GetLoader.LoadTexture<T>(assetName);
    }

    public static void LoadModel(string bundleName, string assetName, System.Action<Object> callback)
    {
        GetLoader.LoadModel(bundleName, assetName, callback);
    }

    public static void LoadEffect(string assetName, System.Action<Object> callback)
    {
        GetLoader.LoadEffect(assetName, callback);
    }

    public static void Preload()
    {
        GetLoader.Preload();
    }

    public static void UnloadAssetBundle(bool isForce)
    {
        GetLoader.UnloadAllAssetBundle(isForce);
    }
    
    public static void UnloadLoadedAssetBundle(bool isForce)
    {
        GetLoader.UnloadLoadedAssetBundle(isForce);
    }
    
    public static void ClearPool()
    {
        GetLoader.ClearPool();
    }

    public static void ReleaseLoader()
    {
        Loader = null;
    }

    #endregion
}

#if UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EditorLoader : IResourceLoader
{
    #region Values

    private Dictionary<string, Object> Assets = null;

    #endregion

    public EditorLoader()
    {
        Assets = new Dictionary<string, Object>();
    }

    ~EditorLoader()
    {
        Assets = null;
    }

    #region INTERFACE
    
    public TextAsset LoadData(string assetName)
    {
        return LoadResource<TextAsset>("bytes", assetName);
    }

    public AudioClip LoadSoundBGM(string assetName)
    {
        return LoadResource<AudioClip>("sounds/bgm", assetName);
    }

    public AudioClip LoadSoundEffect(string assetName)
    {
        return LoadResource<AudioClip>("sounds/soundeffect", assetName);
    }

    public T LoadUIPrefab<T>(string assetName) where T : Object
    {
        return LoadResource<T>("ui", assetName);
    }

    public T LoadTexture<T>(string assetName) where T : Object
    {
        return LoadResource<T>("textures", assetName);
    }
    
    public void LoadModel(string bundleName, string assetName, System.Action<Object> callback)
    {
        callback?.Invoke(LoadResource<Object>(bundleName, assetName));
    }

    public void LoadEffect(string assetName, System.Action<Object> callback)
    {
        callback?.Invoke(LoadResource<Object>("effects", assetName));
    }

    public void Preload()
    {
        AppEventManager.Instance.PushAppEvent(new AppEvent(EAppEventType.BundlePreloadComplete));
    }

    public void UnloadAllAssetBundle(bool isForce)
    {
        Resources.UnloadUnusedAssets();
    }

    public void UnloadLoadedAssetBundle(bool isForce)
    {
        ClearPool();
        Resources.UnloadUnusedAssets();
    }

    public void ClearPool()
    {
        if (Assets != null)
        {
            List<string> keyList = new List<string>(Assets.Keys);
            for (int i = 0; i < keyList.Count; i++)
                Assets[keyList[i]] = null;

            Assets.Clear();
        }
    }

    #endregion

    #region PRIVATE METHOD

    private T LoadResource<T>(string bundleName, string assetName) where T : Object
    {
        if (string.IsNullOrEmpty(bundleName)) return null;
        if (string.IsNullOrEmpty(assetName)) return null;

        Object asset = null;
        string fullName = string.Format("{0}.assetbundle/{1}", bundleName, assetName);

        if (Assets.TryGetValue(fullName, out asset) == false)
        {
            string[] assetPath = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(string.Format("{0}.assetbundle", bundleName), assetName);
            if (assetPath.Length > 0)
            {
                asset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetPath[0]);
                if (asset != null)
                    Assets.Add(fullName, asset);
            }
        }

        return (T)asset;
    }

    #endregion
}

#endif

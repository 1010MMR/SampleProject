using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DownloadLoader : IResourceLoader
{
    #region Values

    private Dictionary<string, AssetBundle> Bundles = null;
    private Dictionary<string, Object> Assets = null;

    #endregion

    public DownloadLoader()
    {
        Bundles = new Dictionary<string, AssetBundle>();
        Assets = new Dictionary<string, Object>();
    }

    ~DownloadLoader()
    {
        Bundles = null;
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
    }

    public void LoadEffect(string assetName, System.Action<Object> callback)
    {
    }

    public void Preload()
    {
        AppEventManager.Instance.PushAppEvent(new AppEvent(EAppEventType.BundlePreloadComplete));
    }

    public void UnloadAllAssetBundle(bool isForce)
    {
        if (Bundles != null)
        {
            List<string> keyList = new List<string>(Bundles.Keys);
            for (int i = 0; i < keyList.Count; i++)
                Bundles[keyList[i]].Unload(isForce);

            Bundles.Clear();
        }

        ClearPool();
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
        AssetBundle bundle = LoadBundle(bundleName);

        string fullName = string.Format("{0}.assetbundle/{1}", bundleName, assetName);

        if (Assets.TryGetValue(fullName, out asset) == false)
        {
            if (bundle != null)
            {
                asset = bundle.LoadAsset(assetName);
                if (asset != null)
                    Assets.Add(fullName, asset);
            }
        }

        return (T)asset;
    }

    private AssetBundle LoadBundle(string bundleName)
    {
        if (string.IsNullOrEmpty(bundleName)) return null;

        WWW www = null;
        AssetBundle bundle = null;

        //string bundlePath = string.Format("{0}/AssetBundle/{1}", Application.streamingAssetsPath, bundleName);

        //if (m_Bundles.TryGetValue(bundleName, out bundle) == false)
        //{
        //    if (File.Exists(bundlePath))
        //    {
        //        bundle = AssetBundle.LoadFromFile(bundlePath);
        //        if (bundle != null)
        //            m_Bundles.Add(bundleName, bundle);
        //    }
        //}

        return bundle;
    }

    #endregion
}

using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

public class StreamLoader : IResourceLoader
{
    #region VALUES

    private readonly string[] PRELOAD_ASSETBUNDLE_NAMES = { "bytes", "sounds/bgm", "sounds/soundeffect", "ui" };

    private Dictionary<string, AssetBundle> Bundles = null;
    private Dictionary<string, Object> Assets = null;

    private IEnumerator UpdateLoadResource = null;
    private Queue<IEnumerator> LoadResourceQueue = null;

    #endregion

    #region PROPERTY

    private string StreamingAssetsPath {
        get {
#if UNITY_ANDROID
            return Application.streamingAssetsPath;
#elif UNITY_IOS
            return string.Format("file://{0}", Application.streamingAssetsPath);
#else
            return Application.streamingAssetsPath;
#endif
        }
    }

    #endregion

    public StreamLoader()
    {
        Bundles = new Dictionary<string, AssetBundle>();
        Assets = new Dictionary<string, Object>();

        LoadResourceQueue = new Queue<IEnumerator>();
        Util.StartCoroutine(UpdateLoadResource = IEUpdateLoadResource());
    }

    ~StreamLoader()
    {
        Bundles = null;
        Assets = null;

        if (UpdateLoadResource != null)
            Util.StopCoroutine(UpdateLoadResource);
        UpdateLoadResource = null;
        LoadResourceQueue = null;
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
        LoadResourceQueue.Enqueue(IELoadResource(bundleName, assetName, callback));
    }

    public void LoadEffect(string assetName, System.Action<Object> callback)
    {
        LoadResourceQueue.Enqueue(IELoadResource("effects", assetName, callback));
    }

    public void Preload()
    {
        Util.StartCoroutine(PreloadAssetBundle());
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
        if (Bundles != null)
        {
            List<string> preloads = new List<string>(PRELOAD_ASSETBUNDLE_NAMES);
            List<string> keyList = new List<string>(Bundles.Keys);

            for (int i = 0; i < keyList.Count; i++)
            {
                if (preloads.Contains(keyList[i]) == false)
                {
                    Bundles[keyList[i]].Unload(isForce);

                    Bundles[keyList[i]] = null;
                    Bundles.Remove(keyList[i]);
                }
            }
        }

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

    private IEnumerator IEUpdateLoadResource()
    {
        while (true)
        {
            if (LoadResourceQueue == null) yield break;

            if (LoadResourceQueue.Count.Equals(0)) yield return null;
            else yield return LoadResourceQueue.Dequeue();
        }
    }

    private IEnumerator PreloadAssetBundle()
    {
        float addValue = 1.0f / (float)PRELOAD_ASSETBUNDLE_NAMES.Length;

        for (int i = 0; i < PRELOAD_ASSETBUNDLE_NAMES.Length; i++)
        {
            yield return IELoadBundle(PRELOAD_ASSETBUNDLE_NAMES[i]);
            AppEventManager.Instance.PushAppEvent(new AppEvent(EAppEventType.BundlePreloading, addValue * (i + 1)));
        }

        AppEventManager.Instance.PushAppEvent(new AppEvent(EAppEventType.BundlePreloadComplete));
    }

    private T LoadResource<T>(string bundleName, string assetName) where T : Object
    {
        if (string.IsNullOrEmpty(bundleName)) return null;
        if (string.IsNullOrEmpty(assetName)) return null;

        Object asset = null;
        AssetBundle bundle = LoadBundle(bundleName);

        string fullName = string.Format("{0}/{1}", bundleName, assetName);

        if (Assets.ContainsKey(fullName) == false || Assets[fullName] == null)
        {
            if (bundle != null)
            {
                asset = bundle.LoadAsset(assetName);
                if (asset != null)
                {
                    if (Assets.ContainsKey(fullName)) Assets[fullName] = asset;
                    else Assets.Add(fullName, asset);
                }
            }
        }
        else
            Assets.TryGetValue(fullName, out asset);

        return (T)asset;
    }

    private IEnumerator IELoadResource<T>(string bundleName, string assetName, System.Action<T> callback) where T : Object
    {
        if (string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(assetName)) { callback?.Invoke(null); yield break; }

        yield return IELoadBundle(bundleName);

        Object asset = null;
        string fullName = string.Format("{0}/{1}", bundleName, assetName);

        if (Assets.ContainsKey(fullName) == false || Assets[fullName] == null)
        {
            AssetBundle bundle = LoadBundle(bundleName);
            if (bundle != null)
            {
                asset = bundle.LoadAsset(assetName);
                if (asset != null)
                {
                    if (Assets.ContainsKey(fullName)) Assets[fullName] = asset;
                    else Assets.Add(fullName, asset);
                }
            }
        }
        else
            Assets.TryGetValue(fullName, out asset);

        callback?.Invoke((T)asset);
    }

    private AssetBundle LoadBundle(string bundleName)
    {
        return Bundles.ContainsKey(bundleName) ? Bundles[bundleName] : null;
    }

    private IEnumerator IELoadBundle(string bundleName)
    {
        if (Bundles.ContainsKey(bundleName) == false || Bundles[bundleName] == null)
        {
            string bundlePath = string.Format("{0}/AssetBundle/{1}.assetbundle", StreamingAssetsPath, bundleName);
            UnityWebRequest bundleRequset = UnityWebRequestAssetBundle.GetAssetBundle(bundlePath);

            yield return bundleRequset.SendWebRequest();
            while (bundleRequset.isDone == false)
                yield return null;

            if (string.IsNullOrEmpty(bundleRequset.error))
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(bundleRequset);
                if (bundle != null)
                {
                    if (Bundles.ContainsKey(bundleName)) Bundles[bundleName] = bundle;
                    else Bundles.Add(bundleName, bundle);
                }
            }
        }
    }

    #endregion
}

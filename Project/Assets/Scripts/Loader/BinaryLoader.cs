using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryLoader : IResourceLoader
{
    #region VALUES

    private Dictionary<string, Object> Assets = null;

    #endregion

    public BinaryLoader()
    {
        Assets = new Dictionary<string, Object>();
    }

    ~BinaryLoader()
    {
        Assets = null;
    }

    #region INTERFACE

    public TextAsset LoadData(string assetName)
    {
        return LoadResource<TextAsset>("Bytes", assetName);
    }

    public AudioClip LoadSoundBGM(string assetName)
    {
        return LoadResource<AudioClip>("Sounds/BGM", assetName);
    }

    public AudioClip LoadSoundEffect(string assetName)
    {
        return LoadResource<AudioClip>("Sounds/SoundEffect", assetName);
    }

    public T LoadUIPrefab<T>(string assetName) where T : Object
    {
        return LoadResource<T>("UI", assetName);
    }

    public T LoadTexture<T>(string assetName) where T : Object
    {
        return LoadResource<T>("Textures", assetName);
    }

    public void LoadModel(string bundleName, string assetName, System.Action<Object> callback)
    {
        callback?.Invoke(LoadResource<Object>(bundleName, assetName));
    }

    public void LoadEffect(string assetName, System.Action<Object> callback)
    {
        callback?.Invoke(LoadResource<Object>("Effects", assetName));
    }

    public void Preload()
    {
    }

    public void UnloadAllAssetBundle(bool isForce)
    {
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

    private T LoadResource<T>(string path, string assetName) where T : Object
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (string.IsNullOrEmpty(assetName)) return null;

        Object asset = null;
        string fullPath = string.Format("{0}/{1}", path, assetName);

        if (Assets.TryGetValue(fullPath, out asset) == false)
        {
            asset = Resources.Load<T>(fullPath);
            if (asset != null)
                Assets.Add(fullPath, asset);
        }

        return (T)asset;
    }

    #endregion
}

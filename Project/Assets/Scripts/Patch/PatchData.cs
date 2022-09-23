using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPatchData
{
    public sealed class PatchData
    {
        #region VALUES

        private string name = string.Empty;
        private string url = string.Empty;
        private long size = 0L;
        private string hash = string.Empty;

        #endregion

        #region PROPERTY

        public string GetAssetBundleName { get { return name; } }
        public string GetURL { get { return url; } }
        public long GetFileSize { get { return size; } }
        public Hash128 GetHash128 { get { return Hash128.Parse(hash); } }

        #endregion

        public PatchData(string bundleName, AssetBundleManifest manifest, string outputPath)
        {
            FileInfo fileInfo = new FileInfo(string.Format("{0}/{1}", outputPath, bundleName));

            name = bundleName;
            url = string.Empty;
            size = fileInfo.Length;
            hash = manifest.GetAssetBundleHash(bundleName).ToString();
        }

        public PatchData(string _name, string _hash, long _size)
        {
            name = _name;
            url = string.Empty;
            size = _size;
            hash = _hash;
        }

        public PatchData(string json)
        {
            PatchData data = JsonUtility.FromJson<PatchData>(json);
            if (data != null)
            {
                name = data.name;
                url = data.url;
                size = data.size;
                hash = data.hash;
            }
        }
    }
}
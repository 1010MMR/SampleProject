using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

using System.Linq;

namespace UnityPatchData
{
    public sealed class PatchList
    {
        #region PROPERTY

        public Dictionary<string, PatchData> PatchDataTable { get; private set; } = null;

        #endregion
        
        public PatchList()
        {
            PatchDataTable = new Dictionary<string, PatchData>();
        }

        public PatchList(string json)
        {
            if (string.IsNullOrEmpty(json) == false)
            {
                try { PatchDataTable = JsonUtility.FromJson<Dictionary<string, PatchData>>(json); }
                catch (System.Exception e) { Debug.LogError(e); }
            }
        }

        #region PUBLIC METHOD

        public string GetPatchListJsonString()
        {
            return PatchDataTable != null ? JsonUtility.ToJson(PatchDataTable) : string.Empty;
        }

        public void AddPatchData(PatchData data)
        {
            if (PatchDataTable != null)
            {
                if (PatchDataTable.ContainsKey(data.GetAssetBundleName))
                    PatchDataTable[data.GetAssetBundleName] = data;
                else
                    PatchDataTable.Add(data.GetAssetBundleName, data);
            }
        }

        #endregion
    }
}

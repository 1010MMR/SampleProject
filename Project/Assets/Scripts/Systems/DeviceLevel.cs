using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceLevel
{
    #region VALUES

    private const string CSV = "DeviceLevel";

    public class RowData
    {
        public int Index { get; private set; } = 0;

        public string Description { get; private set; } = string.Empty;
        public string DeviceName { get; private set; } = string.Empty;
        public string GPUName { get; private set; } = string.Empty;

        public int GPULevel { get; private set; } = 0;
        public int Quality { get; private set; } = 0;
        public int DeviceTier { get; private set; } = 0;

        public RowData()
        {
            Index = 0;

            Description = string.Empty;
            DeviceName = string.Empty;
            GPUName = string.Empty;

            GPULevel = 0;
            Quality = 0;
            DeviceTier = 0;
        }

        public RowData(string[] datas)
        {
            if (datas.Length.Equals(7))
            {
                Index = int.Parse(datas[0]);

                Description = datas[1];
                DeviceName = datas[2];
                GPUName = datas[3];

                GPULevel = int.Parse(datas[4]);
                Quality = int.Parse(datas[5]);
                DeviceTier = int.Parse(datas[6]);
            }
        }
    }

    private List<RowData> DataList = null;

    #endregion

    public DeviceLevel()
    {
        DataList = new List<RowData>();

        TextAsset asset = ResourceLoader.LoadData(CSV);
        string[] lines = asset.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] datas = lines[i].Split(',');
            DataList.Add(new RowData(datas));
        }
    }

    ~DeviceLevel()
    {
        DataList = null;
    }

    /// <summary>
    /// 디바이스의 성능에 따라 퀄리티 레벨을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public int CheckDeviceLevel()
    {
        string modelName= SystemInfo.deviceModel;
        string graphicsName = SystemInfo.graphicsDeviceName;

        int quality = -1;

#if UNITY_EDITOR
        quality = 1;
#else
        for (int i = 0; i < DataList.Count; i++)
        {
            RowData data = DataList[i];

            if (string.IsNullOrEmpty(data.DeviceName) == false && modelName.StartsWith(data.DeviceName)) { quality = data.Quality; break; }
            if (string.IsNullOrEmpty(data.GPUName) == false && graphicsName.StartsWith(data.GPUName)) { quality = data.Quality; break; }
        }

        if (quality < 0)
        {
#if UNITY_ANDROID
            quality = 1;

            int freq = SystemInfo.processorFrequency;
            if (freq > 0 && freq < 1700)
                quality = 0;
            if (SystemInfo.SupportsTextureFormat(TextureFormat.ASTC_6x6) == false)
                quality = 0;
            if (SystemInfo.SupportsTextureFormat(TextureFormat.ETC2_RGBA8) == false)
                quality = 0;
#elif UNITY_IOS
            quality = 0;

            UnityEngine.iOS.DeviceGeneration generation = UnityEngine.iOS.Device.generation;
            if (UnityEngine.iOS.DeviceGeneration.iPhone5S <= generation && generation <= UnityEngine.iOS.DeviceGeneration.iPadPro10Inch1Gen)
                quality = 0;
            else if (UnityEngine.iOS.DeviceGeneration.iPhone7 <= generation)
                quality = 1;
#endif
        }
#endif

        return quality;
    }
}

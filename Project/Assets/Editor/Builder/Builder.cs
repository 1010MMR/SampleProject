#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityPatchData;

namespace AssetBuilder
{
    #region AssetExceptionInfo

    public class AssetExceptionInfo
    {
        public enum EExceptionType
        {
            NONE = -1,

            TOPDIRECTORY, /// <summary> 최상위 디렉토리. </summary>
            SUBTOPDIRECTORY, /// <summary> 서브 중 최상위 디렉토리. </summary>
            SUBSUBTOPDIRECTORY, /// <summary> 서브의 서브 디렉토리 중 최상위 디렉토리. </summary>
            EACHOBJECT, /// <summary> 개별 오브젝트. </summary>
        }

        #region PROPERTY

        public EExceptionType Type { get; private set; } = EExceptionType.NONE;
        public string FolderName { get; private set; } = string.Empty;

        #endregion

        #region VALUES

        private bool UseFolderName = false;

        private string SubFolderName = string.Empty;
        private bool UseSubFolderName = false;

        #endregion

        public AssetExceptionInfo(EExceptionType type, string name, string subName = "", bool useName = true, bool useSubName = false)
        {
            Type = type;
            FolderName = name;
            SubFolderName = subName;
            UseFolderName = useName;
            UseSubFolderName = useSubName;
        }

        #region PUBLIC METHOD

        public string GetFolderName(string resourceRootPath)
        {
            return string.Format("{0}/{1}{2}", resourceRootPath, FolderName, (string.IsNullOrEmpty(SubFolderName) ? "" : string.Format("/{0}", SubFolderName)));
        }

        public string GetBundleName(string addName)
        {
            StringBuilder sBuilder = new StringBuilder();

            if (UseFolderName) { sBuilder.Append(FolderName); }
            if (UseSubFolderName) { sBuilder.Append("/"); sBuilder.Append(SubFolderName); }
            if (string.IsNullOrEmpty(addName) == false) { if (sBuilder.Length > 0) sBuilder.Append("/"); sBuilder.Append(addName); }

            sBuilder.Append(".assetbundle");

            return sBuilder.ToString();
        }

        #endregion
    }

    #endregion

    #region AssetInfo

    public class AssetInfo
    {
        public string Path { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;

        public AssetInfo(string path, string name)
        {
            Path = path;
            Name = name;
        }
    }

    #endregion

    public sealed class Builder
    {
        #region VALUES

        private const string BUNDLE_OUTPUT_PATH = @"Assets\StreamingAssets\AssetBundle";
        private const string PATCHLIST_OUTPUT_PATH = @"Assets\StreamingAssets\PatchList";

        private const string PATCHLIST_FILE_NAME = "patchlist.pat";

        private static List<AssetExceptionInfo> AssetExceptionInfos = new List<AssetExceptionInfo> {
            new AssetExceptionInfo(AssetExceptionInfo.EExceptionType.TOPDIRECTORY, "bytes"),
            new AssetExceptionInfo(AssetExceptionInfo.EExceptionType.TOPDIRECTORY, "effects"),
            new AssetExceptionInfo(AssetExceptionInfo.EExceptionType.TOPDIRECTORY, "models", "enemy", true, true),
            new AssetExceptionInfo(AssetExceptionInfo.EExceptionType.TOPDIRECTORY, "models", "map", true, true),
            new AssetExceptionInfo(AssetExceptionInfo.EExceptionType.TOPDIRECTORY, "models", "tower", true, true),
            new AssetExceptionInfo(AssetExceptionInfo.EExceptionType.TOPDIRECTORY, "sounds", "bgm", true, true),
            new AssetExceptionInfo(AssetExceptionInfo.EExceptionType.TOPDIRECTORY, "sounds", "soundeffect", true, true),
            new AssetExceptionInfo(AssetExceptionInfo.EExceptionType.TOPDIRECTORY, "textures"),
            new AssetExceptionInfo(AssetExceptionInfo.EExceptionType.TOPDIRECTORY, "ui"),
        };

        #endregion

        #region PUBLIC METHOD

        /// <summary>
        /// 번들 경로에 번들 이름을 지정합니다.
        /// </summary>
        /// <param name="rootPath"></param>
        public void AssignAssetBundleName(string rootPath)
        {
            List<string> allBundles = new List<string>(AssetDatabase.GetAllAssetBundleNames());

            List<AssetInfo> assetInfoList = null;
            if (SetAssetInfoList(rootPath, AssetExceptionInfos, out assetInfoList))
            {
                for (int i = 0; i < assetInfoList.Count; i++)
                {
                    AssetImporter importer = AssetImporter.GetAtPath(assetInfoList[i].Path);
                    if (importer.assetBundleName.Equals(assetInfoList[i].Name) == false)
                    {
                        importer.assetBundleName = assetInfoList[i].Name;
                        importer.SaveAndReimport();
                    }

                    allBundles.Remove(assetInfoList[i].Name);
                }

                for (int i = 0; i < allBundles.Count; i++)
                    AssetDatabase.RemoveAssetBundleName(allBundles[i], true);
            }
        }

        /// <summary>
        /// 번들을 생성합니다.
        /// </summary>
        /// <param name="rootPath"></param>
        public void MakeBundles(string rootPath)
        {
            if (Directory.Exists(BUNDLE_OUTPUT_PATH) == false) Directory.CreateDirectory(BUNDLE_OUTPUT_PATH);
            if (Directory.Exists(PATCHLIST_OUTPUT_PATH) == false) Directory.CreateDirectory(PATCHLIST_OUTPUT_PATH);

            try
            {
                List<AssetInfo> assetInfoList = null;
                if (SetAssetInfoList(rootPath, AssetExceptionInfos, out assetInfoList))
                {
                    AssetBundleManifest manifest = RunAssetBundleBuild(BUNDLE_OUTPUT_PATH, assetInfoList, BuildAssetBundleOptions.None);
                    if (manifest != null)
                        WritePatchListFile(manifest);

                    string singleManifestFilePath = BUNDLE_OUTPUT_PATH + "/" + Path.GetFileName(BUNDLE_OUTPUT_PATH);

                    File.Delete(singleManifestFilePath);
                    File.Delete(string.Format("{0}.manifest", singleManifestFilePath));
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(string.Format("[ Builder ][ MakeBundles ][ Exception ] : {0}", e.ToString()));
            }
        }

        #endregion

        #region PRIVATE METHOD

        private bool SetAssetInfoList(string rootPath, List<AssetExceptionInfo> exceptionInfos, out List<AssetInfo> assetInfoList)
        {
            assetInfoList = new List<AssetInfo>();

            //if (Directory.Exists(rootPath))
            //{
            //    string[] paths = Directory.GetDirectories(rootPath, "*", SearchOption.TopDirectoryOnly);
            //    for (int i = 0; i < paths.Length; i++)
            //    {
            //        bool isExists = exceptionInfos.Exists((info) => { return info.FolderName.Equals(Path.GetFileNameWithoutExtension(paths[i])); });
            //        if (isExists == false)
            //        {
            //            if (HashFileInPath(paths[i]))
            //            {
            //                string path = paths[i].Replace('\\', '/');
            //                string name = string.Format("{0}.assetbundle", Path.GetFileNameWithoutExtension(paths[i]));

            //                assetInfoList.Add(new AssetInfo(path, name));
            //            }
            //        }
            //    }
            //}

            for (int i = 0; i < exceptionInfos.Count; i++)
            {
                string folderName = exceptionInfos[i].GetFolderName(rootPath);
                if (Directory.Exists(folderName))
                {
                    switch (exceptionInfos[i].Type)
                    {
                        case AssetExceptionInfo.EExceptionType.TOPDIRECTORY:
                            {
                                if (HashFileInPath(folderName))
                                    assetInfoList.Add(new AssetInfo(folderName, exceptionInfos[i].GetBundleName(string.Empty)));
                            }
                            break;
                        case AssetExceptionInfo.EExceptionType.SUBTOPDIRECTORY:
                            {
                                string[] subPaths = Directory.GetDirectories(folderName, "*", SearchOption.TopDirectoryOnly);

                                foreach (string subPath in subPaths)
                                {
                                    if (HashFileInPath(subPath))
                                    {
                                        string path = subPath.Replace('\\', '/');
                                        assetInfoList.Add(new AssetInfo(path, exceptionInfos[i].GetBundleName(Path.GetFileNameWithoutExtension(subPath))));
                                    }
                                }
                            }
                            break;
                        case AssetExceptionInfo.EExceptionType.SUBSUBTOPDIRECTORY:
                            {
                                string[] topPaths = Directory.GetDirectories(folderName, "*", SearchOption.TopDirectoryOnly);
                                string[] subPaths = null;

                                foreach (string topPath in topPaths)
                                {
                                    subPaths = Directory.GetDirectories(topPath, "*", SearchOption.TopDirectoryOnly);

                                    foreach (string subPath in subPaths)
                                    {
                                        if (HashFileInPath(subPath))
                                        {
                                            string path = subPath.Replace('\\', '/');
                                            assetInfoList.Add(new AssetInfo(path, exceptionInfos[i].GetBundleName(Path.GetFileNameWithoutExtension(subPath))));
                                        }
                                    }
                                }
                            }
                            break;
                        case AssetExceptionInfo.EExceptionType.EACHOBJECT:
                            {
                                string[] objectPaths = Directory.GetFiles(folderName, "*.prefab", SearchOption.AllDirectories);

                                foreach (string objPath in objectPaths)
                                {
                                    string path = objPath.Replace('\\', '/');
                                    assetInfoList.Add(new AssetInfo(path, exceptionInfos[i].GetBundleName(Path.GetFileNameWithoutExtension(objPath))));
                                }
                            }
                            break;
                    }
                }
            }

            return assetInfoList.Count > 0;
        }

        /// <summary>
        /// 에셋 번들을 빌드합니다.
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="assetInfoList"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        private AssetBundleManifest RunAssetBundleBuild(string outputPath, List<AssetInfo> assetInfoList, BuildAssetBundleOptions option)
        {
            AssetBundleBuild[] builds = new AssetBundleBuild[assetInfoList.Count];

            for (int i = 0; i < assetInfoList.Count; i++)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = assetInfoList[i].Name;
                build.assetNames = new string[] { assetInfoList[i].Path };

                builds[i] = build;
            }

            return BuildPipeline.BuildAssetBundles(outputPath, builds, option, GetBuildTarget);
        }

        private bool HashFileInPath(string path)
        {
            return Directory.GetFiles(path).Length > 0;
        }

        private BuildTarget GetBuildTarget
        {
            get {
#if UNITY_ANDROID
                return BuildTarget.Android;
#elif UNITY_IPHONE
                return BuildTarget.iOS;
#else
                return BuildTarget.Android;
#endif
            }
        }

        /// <summary>
        /// 패치리스트를 작성합니다.
        /// </summary>
        /// <param name="manifest"></param>
        private void WritePatchListFile(AssetBundleManifest manifest)
        {
            try
            {
                List<string> prevAssetNames = new List<string>();

                string[] allBundles = manifest.GetAllAssetBundles();
                string patchListFilePath = string.Format("{0}/{1}", PATCHLIST_OUTPUT_PATH, PATCHLIST_FILE_NAME);

                PatchList patchList = null;

                #region Get Previous AssetNames
                if (File.Exists(patchListFilePath))
                {
                    string patchListJsonString = File.ReadAllText(patchListFilePath);
                    patchList = new PatchList(patchListJsonString);

                    prevAssetNames = patchList.PatchDataTable.Keys.ToList();
                }
                #endregion
                #region Remove Deleted PatchFile
                for (int i = 0; i < allBundles.Length; i++)
                {
                    if (prevAssetNames.Contains(allBundles[i]))
                        prevAssetNames.Remove(allBundles[i]);
                }

                for (int i = 0; i < prevAssetNames.Count; i++)
                {
                    string path = string.Format("{0}/{1}", BUNDLE_OUTPUT_PATH, prevAssetNames[i]);

                    try
                    {
                        File.Delete(path);
                        File.Delete(string.Format("{0}.manifest", path));
                    }
                    catch (System.Exception e) { Debug.Log(string.Format("[ Builder ][ WritePatchListFile ][ Exception ] : {0}", e.ToString())); }
                }

                string[] outputSubPaths = Directory.GetDirectories(BUNDLE_OUTPUT_PATH, "*.*", SearchOption.AllDirectories);
                for (int i = 0; i < outputSubPaths.Length; i++)
                {
                    if (HashFileInPath(outputSubPaths[i]))
                    {
                        try { Directory.Delete(outputSubPaths[i]); }
                        catch (System.Exception e) { Debug.Log(string.Format("[ Builder ][ WritePatchListFile ][ Exception ] : {0}", e.ToString())); }
                    }
                }
                #endregion
                #region Write PatchList
                patchList = new PatchList();

                for (int i = 0; i < allBundles.Length; i++)
                    patchList.AddPatchData(new PatchData(allBundles[i], manifest, BUNDLE_OUTPUT_PATH));

                File.WriteAllText(patchListFilePath, patchList.GetPatchListJsonString());
                #endregion
            }
            catch (System.Exception e)
            {
                Debug.Log(string.Format("[ Builder ][ WritePatchListFile ][ Exception ] : {0}", e.ToString()));
            }
        }

        #endregion
    }
}

#endif
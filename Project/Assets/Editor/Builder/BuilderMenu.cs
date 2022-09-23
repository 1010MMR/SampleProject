#if UNITY_EDITOR

using UnityEditor;

namespace AssetBuilder
{
    public sealed class BuilderMenu
    {
        #region VALUES

        private const string ROOT_PATH = "Assets/Bundles";

        private static Builder AssetBuilder = new Builder();

        #endregion

        [MenuItem("Tools/AssetBundle/에셋 번들 경로 지정하기", false, 1)]
        private static void AssignAssetBundleName()
        {
            AssetBuilder.AssignAssetBundleName(ROOT_PATH);
            EditorUtility.DisplayDialog("Tools", "에셋 번들 경로가 지정되었습니다.", "확인");
        }

        [MenuItem("Tools/AssetBundle/에셋 번들 빌드하기", false, 1)]
        private static void MakeAssetBundles()
        {
            AssetBuilder.MakeBundles(ROOT_PATH);
            EditorUtility.DisplayDialog("Tools", "에셋 번들 빌드가 완료되었습니다.", "확인");
        }
    }
}

#endif
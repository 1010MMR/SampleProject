using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppRelease : MonoBehaviour
{
    private void Awake()
    {
        #region StopAllCoroutines
        Util.StopAllCoroutines();
        #endregion
        #region Singleton
        SingletonManager.InvokeAllSingletonClearAction();
        #endregion
        #region Assets
        ResourceLoader.UnloadAssetBundle(true);
        ResourceLoader.ReleaseLoader();
        #endregion

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("InitScene");
    }
}

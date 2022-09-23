using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 씬과 UI를 관리합니다.
/// </summary>
public class SceneManager : MonoSingleton<SceneManager>
{
    #region VALUES

    public enum ESceneType
    {
        INIT,
        GAME,

        End,
    }
    
    public delegate void SceneChangeCallback();

    #endregion

    #region PROPERTY

    public ESceneType SceneType { get; private set; } = ESceneType.INIT;
    public SceneRoot SceneRoot { get; private set; } = null;

    /// <summary> InitScene 내에서 SceneRoot를 호출합니다. (약식) </summary>
    public InitScene InitScene { get { return SceneRoot as InitScene; } }

    #endregion

    public override void ExplicitCall()
    {
    }

    protected override void Init()
    {
        if (System.Enum.TryParse(Util.GetCurrentSceneName, out ESceneType eSceneType) == false)
            eSceneType = ESceneType.INIT;

        SceneType = eSceneType;
        SceneRoot = GetCurrentSceneRoot();
    }

    protected override void Release()
    {
        SceneRoot = null;
    }

    protected override void Clear()
    {
        Release();
        base.Clear();
    }

    #region PUBLIC METHOD

    #region Scene

    /// <summary>
    /// 씬을 생성합니다.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="callback"></param>
    public void LoadScene(ESceneType type, SceneChangeCallback callback = null)
    {
        if (SceneType.Equals(type)) callback?.Invoke();
        else Util.StartCoroutine(IELoadScene(type, callback));
    }

    /// <summary>
    /// 터치 영역을 막는 스크린 커버를 On/Off합니다.
    /// </summary>
    /// <param name="isActive"></param>
    public void ScreenCoverActive(bool isActive)
    {
        SceneRoot?.ScreenCoverActive(isActive);
    }

    #endregion

    #endregion

    #region PRIVATE METHOD

    #region Scene

    private SceneRoot GetCurrentSceneRoot()
    {
        SceneRoot root = FindObjectOfType<SceneRoot>();

        if (root != null) return root;
        else return null;
    }

    private IEnumerator IELoadScene(ESceneType type, SceneChangeCallback callback = null)
    {
        AsyncOperation async = ResourceLoader.LoadLevelAsync(type.ToString());
        while (async.isDone == false)
            yield return null;

        SceneType = type;
        SceneRoot = GetCurrentSceneRoot();

        callback?.Invoke();

        switch (type)
        {
            case ESceneType.INIT: break;
            case ESceneType.GAME: SequenceManager.Instance.SetSequence(new SequenceModule(type)); break;
        }
    }

    #endregion

    #endregion
}

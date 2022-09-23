using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 앱이 실행되는 Init Scene에서 스타팅. 시퀀스 시작을 위해 필요합니다.
/// </summary>
public class AppStarting : MonoBehaviour
{
    private void Awake()
    {
        Util.VersionCheck();
        Util.MakeMonoStaticObject();

        ResourceLoader.LoaderType = ResourceLoader.LoadType.Binary;

        AppEventManager.Instance.ExplicitCall();
        SoundManager.Instance.ExplicitCall();
        ScreenManager.Instance.ExplicitCall();
        TimeManager.Instance.ExplicitCall();
        OptionManager.Instance.ExplicitCall();

        SequenceManager.Instance.SetSequence(new SequenceModule(SceneManager.ESceneType.INIT));
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사운드를 관리합니다.
/// </summary>
public class SoundManager : MonoSingleton<SoundManager>
{
    #region VALUES

    public const string POOL_OBJECT_NAME = "SoundPool";

    private const int SOUND_POOL_COUNT = 15;

    private GameObject PoolParent = null;

    private SoundSource BgmSource = null;
    private CircularQueue<SoundSource> AudioQueue = null;

    #endregion

    #region PROPERTY

    #endregion

    #region OVERRIDE METHOD

    public override void ExplicitCall()
    {   
    }

    protected override void Init()
    {
        PoolParent = new GameObject(POOL_OBJECT_NAME);
        Util.DontDestroyOnLoad(PoolParent);
        
        BgmSource = new SoundSource();
        AudioQueue = new CircularQueue<SoundSource>(SOUND_POOL_COUNT);
    }

    protected override void Release()
    {
        if (BgmSource != null)
            BgmSource = null;
        if (AudioQueue != null)
            AudioQueue = null;

        if (PoolParent != null)
        {
            Util.Destroy(PoolParent);
            PoolParent = null;
        }
    }

    protected override void Clear()
    {
        Release();
        base.Clear();
    }

    #endregion

    #region PUBLIC METHOD

    /// <summary>
    /// BGM을 재생합니다.
    /// </summary>
    /// <param name="type"></param>
    public void PlayBGM(ESOUNDBGM type)
    {
        SoundBGMTable table = TableManager.Instance.GetTableRow<SoundBGMTable>(TableEnums.SoundBGM, (int)type);

        if (BgmSource.AudioClipName.Equals(table.Name) == false)
            BgmSource.SetAudioClip(type, ResourceLoader.LoadSoundBGM(table.Name), true);
    }

    /// <summary>
    /// 이펙트 사운드를 재생합니다.
    /// </summary>
    /// <param name="type"></param>
    public void PlaySound(ESOUNDEFFECT type, bool isLoop = false)
    {
        SoundEffectTable table = TableManager.Instance.GetTableRow<SoundEffectTable>(TableEnums.SoundEffect, (int)type);
        AudioClip clip = ResourceLoader.LoadSoundEffect(table.Name);

        AudioQueue.Dequeue().SetAudioClip(type, clip, isLoop);
    }

    /// <summary>
    /// BGM을 정지합니다.
    /// </summary>
    public void StopBGM()
    {
        BgmSource.Stop();
    }

    /// <summary>
    /// 이펙트 사운드를 정지합니다.
    /// </summary>
    /// <param name="type"></param>
    public void StopSound(ESOUNDEFFECT type)
    {
        for (int i = 0; i < AudioQueue.ListCount; i++)
        {
            if (AudioQueue.Get(i).SoundType.Equals(type))
                AudioQueue.Get(i).Stop();
        }
    }

	/// <summary>
	/// 모든 소리를 정지합니다.
	/// </summary>
	public void StopAllSound()
    {
        BgmSource.Stop();

        for (int i = 0; i < AudioQueue.ListCount; i++)
            AudioQueue.Get(i).Stop();
    }

    /// <summary>
    /// BGM을 음소거합니다.
    /// </summary>
    /// <param name="mute"></param>
	public void MuteBGM(bool mute)
	{
		BgmSource.MuteAudioClip(mute);
	}

    /// <summary>
    /// 이펙트 사운드를 음소거합니다.
    /// </summary>
    /// <param name="mute"></param>
    public void MuteSound(bool mute)
    {
        for (int i = 0; i < AudioQueue.ListCount; i++)
        {
            AudioQueue.Get(i).MuteAudioClip(mute);
        }
    }

    #endregion

    #region PRIVATE METHOD

    #endregion
}

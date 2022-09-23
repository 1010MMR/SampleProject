using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 개별 사운드를 컨트롤할 클래스.
/// </summary>
public class SoundSource
{
    #region VALUES

    private const string OBJECT_NAME = "SoundModule";

    private GameObject Obj = null;
    private AudioSource AudioSource = null;

    #endregion

    #region PROPERTY

    /// <summary> 오디오 클립의 이름을 반환합니다. </summary>
    public string AudioClipName { get { return (AudioSource != null && AudioSource.clip != null) ? AudioSource.clip.name : ""; } }

    public ESOUNDBGM BGMType { get; private set; } = ESOUNDBGM.BGM_TITLE;
    public ESOUNDEFFECT SoundType { get; private set; } = ESOUNDEFFECT.NONE;

    #endregion

    public SoundSource()
    {
        Obj = new GameObject(OBJECT_NAME);
        Obj.transform.parent = GameObject.Find(SoundManager.POOL_OBJECT_NAME).transform;
        Obj.transform.localPosition = Vector3.zero;
        Obj.transform.localScale = Vector3.zero;

        AudioSource = Obj.AddComponent<AudioSource>();
    }
    
    ~SoundSource()
    {
        if (AudioSource != null)
            AudioSource.clip = null;
        if (Obj != null)
            Util.Destroy(Obj);

        Obj = null;
        AudioSource = null;
    }

    #region PUBLIC METHOD

    /// <summary>
    /// ESOUNDBGM 타입의 AudioClip을 재생합니다. 기존 재생이 있을 경우 제거합니다.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="clip"></param>
    /// <param name="isLoop"></param>
    public void SetAudioClip(ESOUNDBGM type, AudioClip clip, bool isLoop = false)
    {
        SetAudioClip(clip, OptionManager.Instance.BGMEnable, isLoop);
        BGMType = type;
    }

    /// <summary>
    /// ESOUNDEFFECT 타입의 AudioClip을 재생합니다. 기존 재생이 있을 경우 제거합니다.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="clip"></param>
    /// <param name="isLoop"></param>
    public void SetAudioClip(ESOUNDEFFECT type, AudioClip clip, bool isLoop = false)
    {
        SetAudioClip(clip, OptionManager.Instance.SoundEnable, isLoop);
        SoundType = type;
    }

    /// <summary>
    /// AudioClip을 정지합니다.
    /// </summary>
    public void Stop()
    {
        BGMType = 0;
        SoundType = 0;

        AudioSource.clip = null;
        AudioSource.loop = false;

        AudioSource.Stop();
    }

	/// <summary>
	/// AudioClip을 음소거합니다.
	/// </summary>
	public void MuteAudioClip(bool mute)
    {
        if (AudioSource.clip != null)
            AudioSource.mute = mute;
    }

	#endregion

	#region PRIVATE METHOD

	/// <summary>
	/// AudioClip을 재생합니다. 기존 재생이 있을 경우 제거합니다.
	/// </summary>
	/// <param name="clip"></param>
	private void SetAudioClip(AudioClip clip, bool isEnable, bool isLoop = false)
    {
        if (AudioSource.clip != null)
            Stop();

        AudioSource.clip = clip;
        AudioSource.loop = isLoop;
        AudioSource.mute = !isEnable;

        AudioSource.Play();
    }

	#endregion
}

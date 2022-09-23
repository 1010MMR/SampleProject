using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 내 옵션을 관리합니다.
/// </summary>
public class OptionManager : MonoSingleton<OptionManager>
{
    #region VALUES

    private const string GRAPHIC_LEVEL_PREFS = "GRAPHIC_LEVEL";

    private const string BGM_OPTION_PREFS = "BGM_OPTION_PREFS";
    private const string SOUND_OPTION_PREFS = "SOUND_OPTION_PREFS";

    private const string PUSH_OPTION_PREFS = "PUSH_OPTION_PREFS";

    #endregion

    #region PROPERTY

    public int GraphicLevel { get; private set; } = 0;

    public bool BGMEnable { get; private set; } = true;
    public bool SoundEnable { get; private set; } = true;

    public bool PushEnable { get; private set; } = true;

    #endregion

    #region OVERRIDE METHOD

    public override void ExplicitCall()
    {
    }

    protected override void Init()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        #region Graphic
        if (Util.CheckPrefsKey(GRAPHIC_LEVEL_PREFS) == false) Util.SetPrefsInt(GRAPHIC_LEVEL_PREFS, GraphicLevel = new DeviceLevel().CheckDeviceLevel());
        else GraphicLevel = Util.GetPrefsInt(GRAPHIC_LEVEL_PREFS, 0);
        #endregion
        #region BGM
        if (Util.CheckPrefsKey(BGM_OPTION_PREFS) == false) { BGMEnable = true; Util.SetPrefsInt(BGM_OPTION_PREFS, 1); }
        else BGMEnable = Util.GetPrefsInt(BGM_OPTION_PREFS, 1).Equals(1);
        #endregion
        #region Sound
        if (Util.CheckPrefsKey(SOUND_OPTION_PREFS) == false) { SoundEnable = true; Util.SetPrefsInt(SOUND_OPTION_PREFS, 1); }
        else SoundEnable = Util.GetPrefsInt(SOUND_OPTION_PREFS, 1).Equals(1);
        #endregion
        #region Push
        if (Util.CheckPrefsKey(PUSH_OPTION_PREFS) == false) { PushEnable = true; Util.SetPrefsInt(PUSH_OPTION_PREFS, 1); }
        else PushEnable = Util.GetPrefsInt(PUSH_OPTION_PREFS, 1).Equals(1);
        #endregion

        QualitySettings.SetQualityLevel(GraphicLevel);
    }

    protected override void Release()
    {
    }

    #endregion

    #region PUBLIC METHOD

    /// <summary>
    /// 그래픽 퀄리티를 조정합니다.
    /// </summary>
    /// <param name="level"></param>
    public void UpdateGraphicLevel(int level)
    {
        GraphicLevel = level;

        Util.SetPrefsInt(GRAPHIC_LEVEL_PREFS, GraphicLevel);
        QualitySettings.SetQualityLevel(GraphicLevel);
    }

    /// <summary>
    /// BGM의 ON/OFF를 조정합니다.
    /// </summary>
    /// <param name="isEnable"></param>
    public void UpdateBGMEnable(bool isEnable)
    {
        BGMEnable = isEnable;

        Util.SetPrefsInt(BGM_OPTION_PREFS, isEnable ? 1 : 0);
        SoundManager.Instance.MuteBGM(!BGMEnable);
    }

    /// <summary>
    /// 이펙트 사운드의 ON/OFF를 조정합니다.
    /// </summary>
    /// <param name="isEnable"></param>
    public void UpdateSoundEnable(bool isEnable)
    {
        SoundEnable = isEnable;

        Util.SetPrefsInt(SOUND_OPTION_PREFS, isEnable ? 1 : 0);
        SoundManager.Instance.MuteSound(!SoundEnable);
    }

    /// <summary>
    /// 푸시의 ON/OFF를 조정합니다.
    /// </summary>
    /// <param name="isEnable"></param>
    public void UpdatePushEnable(bool isEnable)
    {
        PushEnable = isEnable;

        Util.SetPrefsInt(PUSH_OPTION_PREFS, isEnable ? 1 : 0);
        if (isEnable == false)
            NotificationManager.Instance.CancelAllNotification();
    }

    #endregion

    #region PRIVATE MEHOD
    
    #endregion
}
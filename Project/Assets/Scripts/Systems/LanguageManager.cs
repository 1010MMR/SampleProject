using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 언어 출력을 관리합니다.
/// </summary>
public class LanguageManager : MonoSingleton<LanguageManager>
{
    #region VALUES

    private const string LANGUAGE_TYPE_KEY = "LANGUAGE_TYPE_KEY";

    #endregion

    #region PROPERTY

    public LANGUAGE_TYPE SelectLanguage { get; private set; } = LANGUAGE_TYPE.ENGLISH;

    #endregion

    #region OVERRIDE METHOD

    public override void ExplicitCall()
    {
    }

    protected override void Init()
    {
        SelectLanguage = (LANGUAGE_TYPE)Util.GetPrefsInt(LANGUAGE_TYPE_KEY, (int)GetApplicationSystemLanguage());
        if (Util.CheckPrefsKey(LANGUAGE_TYPE_KEY) == false)
            Util.SetPrefsInt(LANGUAGE_TYPE_KEY, (int)SelectLanguage);
    }

    protected override void Release()
    {
    }

    #endregion

    #region PUBLIC MEHOD

    /// <summary>
    /// 선택한 언어를 업데이트합니다.
    /// </summary>
    /// <param name="type"></param>
    public void UpdateLanguageType(LANGUAGE_TYPE type)
    {
        SelectLanguage = type;
        Util.SetPrefsInt(LANGUAGE_TYPE_KEY, (int)SelectLanguage);
    }

    public string GetLanguageString(LANGUAGE_TYPE type)
    {
        switch (type)
        {
            case LANGUAGE_TYPE.ENGLISH:
                return Util.GetString(ESTRING.Language_ENG);
            case LANGUAGE_TYPE.KOREAN:
                return Util.GetString(ESTRING.Language_KOR);
            default:
                return Util.GetString(ESTRING.Language_ENG);
        }
    }

    #endregion

    #region PRIVATE METHOD

    private static LANGUAGE_TYPE GetApplicationSystemLanguage()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Korean: return LANGUAGE_TYPE.KOREAN;
            default: return LANGUAGE_TYPE.ENGLISH;
        }
    }

    #endregion
}

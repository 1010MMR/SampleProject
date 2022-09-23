using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public partial class Util
{
    #region VALUES

    public const string TAG_UI = "UI";

    public const string MONO_SINGLETON = "MonoSingleton";
    private const string MONO_STATIC_OBJECT = "MonoStaticObject";

    private const string VERSION_CODE = "VERSION_CODE";

#if UNITY_EDITOR
    public const string GAME_ID = "3153629";
#elif UNITY_ANDROID
    public const string GAME_ID = "3153629";
#elif UNITY_IOS
    public const string GAME_ID = "3153628";
#endif

    private static MonoStaticObject MonoStaticObject = null;
    private static List<Object> DontDestroyOnLoadObjects = null;

    #endregion

    #region Scene

    public static string GetCurrentSceneName { get { return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToUpper(); } }

    #endregion

    #region Mono Static Object

    public static void MakeMonoStaticObject()
    {
        MonoStaticObject monoStatic = Object.FindObjectOfType<MonoStaticObject>();

        if (monoStatic != null)
            MonoStaticObject = monoStatic;
        else
        {
            GameObject obj = new GameObject(MONO_STATIC_OBJECT);
            Object.DontDestroyOnLoad(obj);

            monoStatic = obj.AddComponent<MonoStaticObject>();
            MonoStaticObject = monoStatic;
        }
    }

    #endregion

    #region Application End Process

    public static bool ApplicationEndProcess { get; set; } = false;

    #endregion

    #region Object

    public static T Instantiate<T>(T obj) where T : Object
    {
        T instance = Object.Instantiate(obj);
        if (instance != null)
            instance.name = obj.name;
        return instance;
    }

    public static T Instantiate<T>(T obj, string name) where T : Object
    {
        T instance = Object.Instantiate(obj);
        if (instance != null)
            instance.name = name;
        return instance;
    }

    public static void Destroy(Object obj)
    {
        if (obj != null)
            Object.DestroyImmediate(obj);
    }

    public static void DontDestroyOnLoad(Object obj)
    {
        if (DontDestroyOnLoadObjects == null)
            DontDestroyOnLoadObjects = new List<Object>();

        if (obj != null)
        {
            Object.DontDestroyOnLoad(obj);
            DontDestroyOnLoadObjects.Add(obj);
        }
    }

    /// <summary>
    /// 모든 DontDestroyOnLoad 객체를 제거합니다.
    /// </summary>
    public static void ReleaseAllDontDestroyOnLoadObjects()
    {
        if (DontDestroyOnLoadObjects != null)
        {
            for (int i = 0; i < DontDestroyOnLoadObjects.Count; i++)
            {
                Destroy(DontDestroyOnLoadObjects[i]);
                DontDestroyOnLoadObjects[i] = null;
            }
        }

        DontDestroyOnLoadObjects = null;
    }

    #endregion

    #region Layer

    /// <summary>
    /// 하위 레이어까지 전부 변경합니다.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="layer"></param>
    public static void ChangeLayer(GameObject obj, string layer)
    {
        ChangeLayersRecursively(obj.transform, layer);
    }

    private static void ChangeLayersRecursively(Transform trans, string layer)
    {
        trans.gameObject.layer = LayerMask.NameToLayer(layer);
        foreach (Transform child in trans)
        {
            ChangeLayersRecursively(child, layer);
        }
    }

    #endregion

    #region List

    /// <summary>
    /// 배열을 리스트로 변환합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static List<T> ToList<T>(T[] array)
    {
        List<T> list = new List<T>();
        if (array != null)
        {
            for (int i = 0; i < array.Length; i++)
                list.Add(array[i]);
        }

        return list;
    }

    #endregion

    #region Coroutine

    /// <summary>
    /// Starts a coroutine.
    /// </summary>
    /// <param name="routine"></param>
    /// <returns></returns>
    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        if (MonoStaticObject == null) MakeMonoStaticObject();
        return MonoStaticObject.StartCoroutine(routine);
    }

    /// <summary>
    /// Stops the first coroutine named methodName, or the coroutine stored in routine running on this behaviour.
    /// </summary>
    /// <param name="routine"></param>
    public static void StopCoroutine(IEnumerator routine)
    {
        if (MonoStaticObject == null) MakeMonoStaticObject();
        MonoStaticObject.StopCoroutine(routine);
    }

    /// <summary>
    /// Stops all coroutines running on this behaviour.
    /// </summary>
    public static void StopAllCoroutines()
    {
        if (MonoStaticObject == null) MakeMonoStaticObject();
        MonoStaticObject.StopAllCoroutines();
    }

    #endregion

    #region Player Prefs

    public static void SetPrefsJson(string key, object objs)
    {
        string value = "";
        try { value = JsonUtility.ToJson(objs); }
        catch { value = string.Empty; }

        PlayerPrefs.SetString(key, value);
    }

    public static T GetPrefsJson<T>(string key)
    {
        if (CheckPrefsKey(key) == false) return default;

        string prefs = PlayerPrefs.GetString(key);

        try { return JsonUtility.FromJson<T>(prefs); }
        catch { return default; }
    }

    public static bool CheckPrefsKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public static void SetPrefsInt(string key, int value) { PlayerPrefs.SetInt(key, value); }
    public static void SetPrefsFloat(string key, float value) { PlayerPrefs.SetFloat(key, value); }
    public static void SetPrefsString(string key, string value) { PlayerPrefs.SetString(key, value); }

    public static int GetPrefsInt(string key, int defaultValue = 0) { return PlayerPrefs.GetInt(key, defaultValue); }
    public static float GetPrefsFloat(string key, float defaultValue = 0) { return PlayerPrefs.GetFloat(key, defaultValue); }
    public static string GetPrefsString(string key, string defaultValue = "") { return PlayerPrefs.GetString(key, defaultValue); }

    public static void DeletePrefs(string key)
    {
        if (CheckPrefsKey(key))
            PlayerPrefs.DeleteKey(key);
    }

    public static void DeleteAllPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    #endregion

    #region Unique Value

    /// <summary>
    /// 프로젝트 내에서 한정적인 유니크 값을 만듭니다.
    /// </summary>
    public static uint UniqueValue
    {
        get {
            byte[] bytes = new byte[4];
            RandomNumberGenerator random = RandomNumberGenerator.Create();
            random.GetBytes(bytes);

            return System.BitConverter.ToUInt32(bytes, 0) % 100000000;
        }
    }

    #endregion

    #region Clamp

    /// <summary>
    /// ulong 타입 Clamp.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static ulong Clamp(ulong value, ulong min, ulong max)
    {
        if (value > max) return max;
        else if (value < min) return min;
        else return value;
    }

    /// <summary>
    /// long 타입 Clamp.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static long Clamp(long value, long min, long max)
    {
        if (value > max) return max;
        else if (value < min) return min;
        else return value;
    }

    /// <summary>
    /// int 타입 Clamp.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Clamp(int value, int min, int max)
    {
        return Mathf.Clamp(value, min, max);
    }

    /// <summary>
    /// float 타입 Clamp.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float Clamp(float value, float min, float max)
    {
        return Mathf.Clamp(value, min, max);
    }

    #endregion

    #region MinMax
    
    /// <summary>
    /// ulong 타입 Min.
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static ulong Min(ulong value1, ulong value2)
    {
        if (value1 > value2) return value2;
        else return value1;
    }

    /// <summary>
    /// ulong 타입 Max.
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static ulong Max(ulong value1, ulong value2)
    {
        if (value1 > value2) return value1;
        else return value2;
    }

    /// <summary>
    /// long 타입 Min.
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static long Min(long value1, long value2)
    {
        if (value1 > value2) return value2;
        else return value1;
    }

    /// <summary>
    /// long 타입 Max.
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static long Max(long value1, long value2)
    {
        if (value1 > value2) return value1;
        else return value2;
    }

    #endregion

    #region UTC

    /// <summary>
    /// ISO 8601 형식의 날짜/시간정보 문자열을 DateTime으로 반환합니다.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static System.DateTime DateTimeFromISO8601(string time) => System.DateTime.Parse(time, null, System.Globalization.DateTimeStyles.RoundtripKind);

    /// <summary>
    ///  현재 UNIX Time 을 반환합니다.
    /// </summary>
    public static long UnixTimeSeconds { get { return (long)(System.DateTime.Now - new System.DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds; } }

    /// <summary>
    /// DateTime을 UnixTime으로 변환합니다.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static long DateTimeToUnixTime(System.DateTime time)
    {
        System.TimeSpan timeSpan = time - new System.DateTime(1970, 1, 1, 0, 0, 0);
        return (long)timeSpan.TotalSeconds;
    }

    /// <summary>
    /// DateTime을 UnixTime으로 변환합니다.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static long DateTimeToUnixTime(string time)
    {
        return DateTimeToUnixTime(StringToDateTime(time));
    }

    /// <summary>
    /// 문자열을 DateTime으로 변환합니다.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static System.DateTime StringToDateTime(string time)
    {
        if (System.DateTime.TryParse(time, out System.DateTime dateTime)) return dateTime;
        else return System.DateTime.Now;
    }

    #endregion

    #region Time Value

    /// <summary>
    /// 시간 데이터를 이용해 추가값을 계산합니다.
    /// </summary>
    /// <param name="lastChargeTime"></param>
    /// <param name="chargeTime"></param>
    /// <param name="chargeValue"></param>
    /// <param name="curValue"></param>
    /// <param name="maxValue"></param>
    /// <param name="count"></param>
    /// <param name="resultTime"></param>
    public static void UpdateTimeValue(long lastChargeTime, long chargeTime, int chargeValue, int curValue, int maxValue, out int value, out long resultTime)
    {
        long unixTime = UnixTimeSeconds;

        value = 0;
        resultTime = 0;

        if (curValue < maxValue && lastChargeTime + chargeTime <= unixTime)
        {
            long lakTime = unixTime - lastChargeTime;
            int charge = (int)(lakTime / chargeTime) * chargeValue;

            value = Clamp(curValue + charge, 0, maxValue);
            resultTime = lastChargeTime + (charge * chargeTime);
        }

        else
        {
            value = curValue;
            resultTime = lastChargeTime;
        }
    }

    /// <summary>
    /// 시간 데이터를 이용해 추가값을 계산합니다.
    /// </summary>
    /// <param name="lastChargeTime"></param>
    /// <param name="chargeTime"></param>
    /// <param name="chargeValue"></param>
    /// <param name="curValue"></param>
    /// <param name="stackValue"></param>
    /// <param name="maxValue"></param>
    /// <param name="value"></param>
    /// <param name="resultTime"></param>
    public static void UpdateTimeValue(long lastChargeTime, long chargeTime, int chargeValue, int curValue, int stackValue, int maxValue, out int value, out long resultTime)
    {
        long unixTime = UnixTimeSeconds;

        value = 0;
        resultTime = 0;

        // 현재 값이 쌓을 수 있는 스택보다 작고, 마지막 충전 시간 + 충전 시간이 현재 시간보다 작을 때
        if (curValue < stackValue && lastChargeTime + chargeTime <= unixTime)
        {
            long lakTime = unixTime - lastChargeTime;
            int charge = (int)(lakTime / chargeTime) * chargeValue;

            value = Clamp(curValue + charge, 0, maxValue);
            resultTime = lastChargeTime + (charge * chargeTime);
        }

        else
        {
            value = curValue;
            resultTime = lastChargeTime;
        }
    }

    /// <summary>
    /// 시간 데이터를 이용해 획득할 수 있는 누적 횟수를 계산합니다.
    /// </summary>
    /// <param name="remainTime"></param>
    /// <param name="chargeTime"></param>
    /// <param name="maxCount"></param>
    /// <param name="count"></param>
    /// <param name="resultTime"></param>
    public static void GetTimeCount(long remainTime, long chargeTime, int maxCount, out int count, out long resultTime)
    {
        count = 0;
        resultTime = 0;

        if (remainTime > 0 && UnixTimeSeconds >= remainTime)
        {
            long lakTIme = UnixTimeSeconds - remainTime;
            long lakRemainTime = lakTIme % chargeTime;

            count = 1 + (int)(lakTIme / chargeTime);
            resultTime = count >= maxCount ? 0 : UnixTimeSeconds + (lakRemainTime > 0 ? lakRemainTime : chargeTime);
        }

        else
        {
            count = 0;
            resultTime = remainTime;
        }
    }
    #endregion

    #region Shuffle

    public static List<T> Shuffle<T>(List<T> list)
    {
        List<T> result = new List<T>();

        System.Random random = new System.Random();
        int randomIndex = 0;

        while (list.Count > 0)
        {
            randomIndex = random.Next(0, list.Count);

            result.Add(list[randomIndex]);
            list.RemoveAt(randomIndex);
        }

        return result;
    }

    #endregion

    #region Version

    public static void VersionCheck()
    {
        string appVersion = Application.version;
        string version = CheckPrefsKey(VERSION_CODE) ? GetPrefsString(VERSION_CODE) : "";

        /// <summary>
        /// 버전 정보가 맞지 않을 경우, 기존의 모든 내부 데이터를 삭제합니다.
        /// </summary>
        if (version.Equals(appVersion) == false)
            DeleteAllPrefs();

        SetPrefsString(VERSION_CODE, appVersion);
    }

    #endregion

    #region Table

    /// <summary>
    /// 테이블에서 [,]로 나뉜 벡터를 Parse합니다.
    /// </summary>
    /// <param name="temp"></param>
    /// <returns></returns>
    public static Vector3 TableVectorParse(string temp)
    {
        string[] splits = temp.Split(',');

        if (splits.Length.Equals(2)) return new Vector3(float.Parse(splits[0]), float.Parse(splits[1]));
        else if (splits.Length.Equals(3)) return new Vector3(float.Parse(splits[0]), float.Parse(splits[1]), float.Parse(splits[2]));
        else return Vector3.zero;
    }

    #endregion

    #region Restart

    /// <summary>
    /// 앱을 재시작합니다.
    /// </summary>
    public static void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("EmptyScene");
    }

    #endregion

    #region Quit

    public static void AppQuit()
    {
        Application.Quit();
    }

    #endregion

    #region String

    /// <summary>
    /// 테이블에 적용된 스트링을 불러옵니다.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetString(ESTRING key)
    {
        StringTable table = TableManager.Instance.GetTableRow<StringTable>(TableEnums.String, (int)key);
        switch (LanguageManager.Instance.SelectLanguage)
        {
            case LANGUAGE_TYPE.ENGLISH: return table.ENG;
            case LANGUAGE_TYPE.KOREAN: return table.KOR;
            default: return table.ENG;
        }
    }

    /// <summary>
    /// 테이블에 적용된 스트링을 불러옵니다.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetString(string key)
    {
        if (System.Enum.TryParse(key, out ESTRING strKey)) return GetString(strKey);
        else return string.Empty;
    }

    #endregion

    #region Country Code

    /// <summary>
    /// 국가 코드를 리턴합니다. (임시)
    /// </summary>
    /// <returns></returns>
    public static string GetCountryCode()
    {
        return "kr";
    }

    #endregion

    #region Thumbnail

    /// <summary>
    /// 썸네일에 테두리 마스크를 씌웁니다.
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="isSoft"></param>
    /// <returns></returns>
    public static Material GetThumbnailMaterial(Texture texture, bool isSoft = false)
    {
        string materialPath = isSoft ? "mat_softmask_thumbnail" : "mat_nonemask_thumbnail";
        Material material = Instantiate(ResourceLoader.LoadTexture<Material>(materialPath));

        if (material != null)
            material.SetTexture("_MainTex", texture);

        return material;
    }

    #endregion

    #region Quality

    #endregion

    #region Social

    /// <summary>
    /// 앱 리뷰를 요청합니다.
    /// </summary>
    public static void ShowReview()
    {
#if UNITY_ANDROID
        Application.OpenURL(string.Format("market://details?id={0}", Application.identifier));
#elif UNITY_IOS
        UnityEngine.iOS.Device.RequestStoreReview();
#endif
    }

    /// <summary>
    /// 약관을 요청합니다.
    /// </summary>
    public static void ShowPolicy()
    {
        Application.OpenURL("https://www.20f-studio.com/privacy-policy.html");
    }

    #endregion
}
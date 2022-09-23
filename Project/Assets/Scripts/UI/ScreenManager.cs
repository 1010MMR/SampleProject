using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 기기의 화면에 뿌려지는 스크린 영역을 관리합니다.
/// </summary>
public class ScreenManager : MonoSingleton<ScreenManager>
{
    #region VALUES

    private const float BASE_WIDTH = 1080.0f;
    private const float BASE_HEIGHT = 1920.0f;

    private const float BASE_RATIO = 1.77f;
    private const float BASE_VALUE = 1.0f;

    private const int BASE_RESOLUTION = 720;

    private Rect LastSafeArea;

    #endregion

    #region PROPERTY

    public int ScreenWidth { get; private set; } = -1;
    public int ScreenHeight { get; private set; } = -1;

    public Vector2 ScreenSize { get { return new Vector2(ScreenWidth, ScreenHeight); } }
    public float GetScreenRatio { get { return (float)ScreenWidth / (float)ScreenHeight; } }

    public Rect SafeArea {
        get {
            if (LastSafeArea != Screen.safeArea)
                LastSafeArea = Screen.safeArea;

            return LastSafeArea;
        }
    }

    public static bool CheckSafeArea {
        get {
#if UNITY_EDITOR
            return false;
#else
            Rect safe = Instance.SafeArea;
            bool isSafe = Mathf.RoundToInt(safe.width) != Instance.ScreenWidth || Mathf.RoundToInt(safe.height) != Instance.ScreenHeight;

            return isSafe;
#endif
        }
    }

    #endregion

    #region OVERRIDE METHOD

    public override void ExplicitCall()
    {
    }

    protected override void Init()
    {
        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;

#if SCREEN_RESOLUTION
        SetResolution();
#endif
    }

    protected override void Release()
    {
    }

    #endregion

    #region UNITY METHOD

    private void Update()
    {

    }

    #endregion

    #region PUBLIC METHOD

    public void SetFixedResolution(GameObject obj)
    {
#if SCREEN_RESOLUTION
        if (obj != null)
            obj.transform.localScale = Vector3.one * GetFixedScreenRatio();
#endif
    }

    /// <summary>
    /// SafeArea의 상단과 하단의 공간을 반환합니다.
    /// </summary>
    /// <param name="safeTop"></param>
    /// <param name="safeBottom"></param>
    public void GetSafeArea(out float safeTop, out float safeBottom)
    {
        Rect safe = SafeArea;

        safeTop = safe.y;
#if UNITY_ANDROID
        safeBottom = ScreenHeight - safe.height;
#elif UNITY_IOS
        safeBottom = ScreenHeight - (safe.y + safe.height);
#else
        safeBottom = ScreenHeight - safe.height;
#endif
    }

    #endregion

    #region PRIVATE METHOD

#if SCREEN_RESOLUTION
    private float GetFixedScreenRatio()
    {
        float ratio = (float)(System.Math.Truncate(GetScreenRatio * 100) / 100);
        if (BASE_RATIO >= ratio)
        {
            if (CheckSafeArea)
            {
                Rect safe = SafeArea;
                float minValue = Mathf.Min(safe.width / (float)Screen.width, safe.height / (float)Screen.height);

                return minValue;
            }

            else
                return BASE_VALUE;
        }

        else
            return (float)(System.Math.Truncate((BASE_RATIO / ratio) * 100) / 100);
    }

    private void SetResolution()
    {
        float screenRate = (float)ScreenHeight / (float)ScreenWidth;
        float resWidth = (BASE_RESOLUTION > ScreenWidth) ? ScreenWidth : BASE_RESOLUTION;
        float resHeight = resWidth * screenRate;

        Screen.SetResolution((int)resWidth, (int)resHeight, true);
    }
#endif

    #endregion
}
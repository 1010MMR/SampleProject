using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 내 이벤트 시간, 실제 동작 Time을 관리합니다.
/// </summary>
public class TimeManager : MonoSingleton<TimeManager>
{
    #region VALUES

    private const float ONE_SECOND = 1.0f;

    private IEnumerator Timer = null;
    private List<TimerCallback> TimerCallbackList = null;

    /// <summary>
    /// TimeManager 시작 후 수집된 값을 전달하는 Callback.
    /// </summary>
    /// <param name="timeSinceStartup"></param>
    public delegate void TimerCallback(float timeSinceStartup);

    #endregion

    #region PROPERTY

    public float TimeSinceStartup { get; private set; } = 0f;
    public bool TimeEnable { get; private set; } = true;

    #endregion

    #region OVERRIDE METHOD

    public override void ExplicitCall()
    {
    }

    protected override void Init()
    {
        TimeSinceStartup = 0f;
        TimeEnable = true;

        TimerCallbackList = new List<TimerCallback>();

        Util.StartCoroutine(Timer = IETimer());
    }

    protected override void Release()
    {
        if (Timer != null)
            Util.StopCoroutine(Timer);

        Timer = null;
        TimerCallbackList = null;
    }

    protected override void Clear()
    {
        Release();
        base.Clear();
    }

    #endregion

    #region PUBLIC METHOD

    /// <summary>
    /// TimeManager의 시간 측정을 시작합니다.
    /// </summary>
    public void StartTimer()
    {
        TimeEnable = true;
    }

    /// <summary>
    /// TimeManager의 시간 측정을 중단합니다.
    /// </summary>
    public void StopTimer()
    {
        TimeEnable = false;
    }

    public void AddTimerCallback(TimerCallback callback)
    {
#if ENABLE_LOG
        Debug.Log(string.Format("[ TimeManager ][ AddTimerCallback ] > {0}", callback));
#endif

        TimerCallbackList.Add(callback);
    }

    public void RemoveTimeCallback(TimerCallback callback)
    {
#if ENABLE_LOG
        Debug.Log(string.Format("[ TimeManager ][ RemoveTimeCallback ] > {0}", callback));
#endif

        int index = TimerCallbackList.FindIndex((cb) => { return cb.Equals(callback); });
        if (index > -1)
            TimerCallbackList.RemoveAt(index);
    }

    #endregion

    #region PRIVATE METHOD

    private IEnumerator IETimer()
    {
        while (true)
        {
            if (TimeEnable)
            {
                TimeSinceStartup += ONE_SECOND;

                for (int i = TimerCallbackList.Count - 1; i > -1; --i)
                {
                    if (TimerCallbackList[i] == null) TimerCallbackList.RemoveAt(i);
                    else TimerCallbackList[i].Invoke(TimeSinceStartup);
                }
            }

            yield return new WaitForSeconds(ONE_SECOND);
        }
    }

    #endregion

}

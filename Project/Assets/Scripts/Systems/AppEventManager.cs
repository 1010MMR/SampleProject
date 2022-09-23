using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 어플리케이션에서 발생하는 AppEvent를 관리합니다.
/// </summary>
public class AppEventManager : MonoSingleton<AppEventManager>
{
    #region VALUES

    public delegate void OnAppEventReceive(AppEvent appEvent);

    private List<OnAppEventReceive> EventReceives = null;

    #endregion

    #region OVERRIDE METHOD

    public override void ExplicitCall()
    {
    }

    protected override void Init()
    {
        EventReceives = new List<OnAppEventReceive>();
    }

    protected override void Release()
    {
        if (EventReceives != null)
        {
            for (int i = 0; i < EventReceives.Count; i++)
                EventReceives[i] = null;
            EventReceives = null;
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
    /// AppEvent를 보냅니다. AppEvent는 각 Scene의 Sequence의 조건을 수행합니다.
    /// </summary>
    /// <param name="appEvent"></param>
    public void PushAppEvent(AppEvent appEvent)
    {
        if (EventReceives != null)
        {
            for (int i = 0; i < EventReceives.Count; i++)
                EventReceives[i]?.Invoke(appEvent);
        }
    }
    
    public void AddEventReceive(OnAppEventReceive callback)
    {
        if (EventReceives == null) EventReceives = new List<OnAppEventReceive>();
        EventReceives.Add(callback);
    }

    public void RemoveEventReceive(OnAppEventReceive callback)
    {
        if (EventReceives != null)
        {
            int index = EventReceives.FindIndex((item) => { return item.Equals(callback); });
            if (index > -1)
                EventReceives.RemoveAt(index);
        }
    }

    #endregion

    #region PRIVATE METHOD

    #endregion
}

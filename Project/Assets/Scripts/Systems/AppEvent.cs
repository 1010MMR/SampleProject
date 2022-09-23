using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAppEventType
{
    None = -1,

    BundlePreloading,
    BundlePreloadComplete,

    ClassInitializeComplete,
    PlatformLoginComplete,
    ServerSignInComplete,
    InitSceneComplete,
    PurchaseInitComplete,
    
    SequenceComplete,

    CreatePanel,
    DestroyPanel,

    CreatePopup,
    DestroyPopup,

    MainPanelOpen,
    
    Tutorial_Desc,

    End,
}

public class AppEvent
{
    #region VALUES

    private List<object> Parameter = null;

    #endregion

    #region PROPERTY

    public EAppEventType EventType { get; private set; } = EAppEventType.None;

    #endregion

    public AppEvent(EAppEventType type)
    {
        EventType = type;
    }

    public AppEvent(EAppEventType type, object param) : this(type)
    {
        Parameter = new List<object>();
        Parameter.Add(param);
    }

    public AppEvent(EAppEventType type, object[] param) : this(type)
    {
        Parameter = new List<object>(param);
    }

    #region PUBLIC METHOD

    public object GetParam(int index)
    {
        return (Parameter != null && Parameter.Count > 0) ? Parameter[Util.Clamp(index, 0, Parameter.Count - 1)] : null;
    }

    #endregion
}

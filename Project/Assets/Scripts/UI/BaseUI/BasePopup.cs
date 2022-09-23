using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePopup : BaseUI
{
    #region VALUES

    #endregion

    #region PROPERTY

    /// <summary> 팝업의 타입. </summary>
    public EPopup PopupType { get; private set; } = EPopup.None;
    /// <summary> 팝업이 만들어진 순번. </summary>
    public int PopupCount { get; private set; } = 0;

    #endregion

    #region OVERRIDE METHOD

    protected override void Close()
    {
        PopupManager.Instance.DestroyPopup(PopupType);
    }

    public override void BackButtonAction()
    {
        Close();
    }

    #endregion

    #region PUBLIC METHOD

    public void SetBasePopup(EPopup type, int popupCount, System.Action destroyCb)
    {
        PopupType = type;
        PopupCount = popupCount;

        DestroyCallback += destroyCb;
    }

    #endregion
}
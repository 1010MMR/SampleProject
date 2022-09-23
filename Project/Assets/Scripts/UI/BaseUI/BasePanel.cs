using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : BaseUI
{
    #region VALUES

    #endregion

    #region PROPERTY

    /// <summary> 패널의 타입. </summary>
    public EPanel PanelType { get; private set; } = EPanel.None;
    /// <summary> 패널이 만들어진 순번. </summary>
    public int PanelCount { get; private set; } = 0;

    #endregion

    #region OVERRIDE METHOD

    protected override void Close()
    {
        PanelManager.Instance.DestroyPanel(PanelType);
    }

    public override void BackButtonAction()
    {
        PopupManager.Instance.CreateAppQuitPopup();
    }

    #endregion

    #region PUBLIC METHOD

    public void SetBasePanel(EPanel type, int panelCount, System.Action destroyCb)
    {
        PanelType = type;
        PanelCount = panelCount;

        DestroyCallback += destroyCb;
    }

    #endregion
}

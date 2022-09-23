using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPopup
{
    None = -1,

    CommonOK,
    CommonYN,
    GoodsYN,

    CreateNickname,
    
    End,
}

/// <summary>
/// UI 중 팝업을 생성, 소멸, 관리합니다.
/// </summary>
public class PopupManager : MonoSingleton<PopupManager>
{
    #region VALUES

    private Dictionary<EPopup, BasePopup> PopupTable = null;

    private int PopupCount = 0;

    #endregion

    #region OVERRIDE METHOD

    public override void ExplicitCall()
    {
    }

    protected override void Init()
    {
        PopupTable = new Dictionary<EPopup, BasePopup>();
        PopupCount = 0;
    }

    protected override void Release()
    {
        PopupTable = null;
        PopupCount = 0;
    }

    protected override void Clear()
    {
        Release();
        base.Clear();
    }

    #endregion

    #region PUBLIC METHOD

    public T GetPopup<T>(EPopup type) where T : BasePopup
    {
        return PopupTable.ContainsKey(type) ? PopupTable[type] as T : null;
    }

    public T CreatePopup<T>(EPopup type, System.Action<T> createCb = null, System.Action destroyCb = null) where T : BasePopup
    {
        if (PopupTable.ContainsKey(type)) return null;

#if ENABLE_LOG
        Debug.Log(string.Format("[ PopupManager ][ CreatePopup ] : {0}", type));
#endif

        BasePopup popup = null;
        GameObject obj = ResourceLoader.LoadUIPrefab<GameObject>(string.Format("Popup_{0}", type.ToString()));

        if (obj != null)
        {
            GameObject createObj = Util.Instantiate(obj);
            SceneManager.Instance.SceneRoot.AttachUIRoot(createObj);

            popup = createObj.GetComponent<BasePopup>();
            popup.SetBasePopup(type, PopupCount, destroyCb);

            PopupTable.Add(type, popup);
            PopupCount++;

            createCb?.Invoke((T)popup);

            AppEventManager.Instance.PushAppEvent(new AppEvent(EAppEventType.CreatePopup, type));
        }

        return (T)popup;
    }

    public bool IsExists(EPopup type)
    {
        return PopupTable.ContainsKey(type);
    }

    /// <summary>
    /// 모든 팝업의 UI를 업데이트합니다. (UpdateUI 호출)
    /// </summary>
    public void UpdatePopupUI()
    {
#if ENABLE_LOG
        Debug.Log(string.Format("[ PopupManager ][ UpdatePopupUI ]"));
#endif

        if (PopupTable != null)
        {
            foreach (BasePopup popup in PopupTable.Values)
            {
                if (popup != null)
                    popup.UpdateUI();
            }
        }
    }

    /// <summary>
    /// 가장 최상위 팝업의 BackButtonAction을 호출합니다.
    /// </summary>
    /// <returns></returns>
    public bool BackButtonAction()
    {
        if (PopupTable != null)
        {
            List<BasePopup> popupList = new List<BasePopup>(PopupTable.Values);
            if (popupList.Count > 0)
            {
                popupList.Sort((a, b) => { return b.PopupCount.CompareTo(a.PopupCount); });
                popupList[0].BackButtonAction();

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 팝업을 제거합니다. 외부에서 팝업 제거 호출 시 사용하세요.
    /// </summary>
    /// <param name="type"></param>
    public void DestroyPopup(EPopup type)
    {
        if (PopupTable.ContainsKey(type))
        {
            if (PopupTable[type] != null)
            {
#if ENABLE_LOG
                Debug.Log(string.Format("[ PopupManager ][ DestroyPopup ] : {0}", type));
#endif
                
                Util.Destroy(PopupTable[type].gameObject);
            }

            PopupTable.Remove(type);

            AppEventManager.Instance.PushAppEvent(new AppEvent(EAppEventType.DestroyPopup, type));
        }
    }

    /// <summary>
    /// 모든 팝업을 제거합니다. 외부에서 팝업 제거 호출 시 사용하세요.
    /// </summary>
    public void DestroyAllPopup()
    {
        if (PopupTable != null)
        {
            List<EPopup> keys = new List<EPopup>(PopupTable.Keys);
            for (int i = 0; i < keys.Count; i++)
                DestroyPopup(keys[i]);
        }
    }

    #endregion

    #region COMMON

    /// <summary>
    /// 확인, 취소 버튼이 있는 일반 텍스트 팝업을 생성합니다.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="okCb"></param>
    /// <param name="cancelCb"></param>
    public void CreateCommonPopup(string text, System.Action okCb, System.Action cancelCb)
    {
        //CreatePopup<CommonYNPopup>(EPopup.CommonYN, (popup) => { popup.Open(text, okCb, cancelCb); });
    }

    /// <summary>
    /// 확인, 취소 버튼 텍스트를 지정하는 일반 텍스트 팝업을 생성합니다.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="ok"></param>
    /// <param name="cancel"></param>
    /// <param name="okCb"></param>
    /// <param name="cancelCb"></param>
    public void CreateCommonPopup(string text, string ok, string cancel, System.Action okCb, System.Action cancelCb)
    {
        //CreatePopup<CommonYNPopup>(EPopup.CommonYN, (popup) => { popup.Open(text, ok, cancel, okCb, cancelCb); });
    }

    /// <summary>
    /// 확인 버튼이 있는 일반 텍스트 팝업을 생성합니다.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="okCb"></param>
    public void CreateCommonPopup(string text, System.Action okCb = null)
    {
        //CreatePopup<CommonOKPopup>(EPopup.CommonOK, (popup) => { popup.Open(text, okCb); });
    }

    /// <summary>
    /// 확인 버튼 텍스트를 지정하는 일반 텍스트 팝업을 생성합니다.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="ok"></param>
    /// <param name="okCb"></param>
    public void CreateCommonPopup(string text, string ok, System.Action okCb = null)
    {
        //CreatePopup<CommonOKPopup>(EPopup.CommonOK, (popup) => { popup.Open(text, ok, okCb); });
    }

    /// <summary>
    /// 확인, 취소 버튼이 있는 재화 취급 텍스트 팝업을 생성합니다. 확인 버튼에는 재화값이 들어갑니다.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="value"></param>
    /// <param name="okCb"></param>
    /// <param name="cancelCb"></param>
    public void CreateGoodsPopup(string text, ulong value, System.Action okCb, System.Action cancelCb)
    {
        //CreatePopup<GoodsYNPopup>(EPopup.GoodsYN, (popup) => { popup.Open(text, value, okCb, cancelCb); });
    }

    /// <summary>
    /// 확인, 취소 버튼 텍스트를 지정하는 재화 취급 텍스트 팝업을 생성합니다. 확인 버튼에는 재화값이 들어갑니다.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="value"></param>
    /// <param name="cancel"></param>
    /// <param name="okCb"></param>
    /// <param name="cancelCb"></param>
    public void CreateGoodsPopup(string text, ulong value, string cancel, System.Action okCb, System.Action cancelCb)
    {
        //CreatePopup<GoodsYNPopup>(EPopup.GoodsYN, (popup) => { popup.Open(text, value, cancel, okCb, cancelCb); });
    }

    /// <summary>
    /// 앱 종료 팝업을 생성합니다.
    /// </summary>
    public void CreateAppQuitPopup()
    {
        //CreatePopup<CommonYNPopup>(EPopup.CommonYN, (popup) => { popup.Open(Util.GetString(ESTRING.App_Exit_Popup_Desc), () => { Util.AppQuit(); }, () => { }); });
    }

    #endregion
}
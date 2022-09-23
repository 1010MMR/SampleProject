using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPanel
{
    None = -1,

    Init,
    CreateAccount,

    Main,
    
    End,
}

/// <summary>
/// UI 중 패널을 생성, 소멸, 관리합니다.
/// </summary>
public class PanelManager : MonoSingleton<PanelManager>
{
    #region VALUES

    private Dictionary<EPanel, BasePanel> PanelTable = null;

    private int PanelCount = 0;

    #endregion

    #region OVERRIDE METHOD

    public override void ExplicitCall()
    {
    }

    protected override void Init()
    {
        PanelTable = new Dictionary<EPanel, BasePanel>();
        PanelCount = 0;
    }

    protected override void Release()
    {
        PanelTable = null;
        PanelCount = 0;
    }

    protected override void Clear()
    {
        Release();
        base.Clear();
    }

    #endregion

    #region PUBLIC MEHOD

    public T GetPanel<T>(EPanel type) where T : BasePanel
    {
        return PanelTable.ContainsKey(type) ? PanelTable[type] as T : null;
    }

    public T CreatePanel<T>(EPanel type, System.Action<T> createCb = null, System.Action destroyCb = null) where T : BasePanel
    {
        if (PanelTable.ContainsKey(type)) return null;

#if ENABLE_LOG
        Debug.Log(string.Format("[ PanelManager ][ CreatePanel ] : {0}", type));
#endif

        BasePanel panel = null;
        GameObject obj = ResourceLoader.LoadUIPrefab<GameObject>(string.Format("Panel_{0}", type.ToString()));

        if (obj != null)
        {
            GameObject createObj = Util.Instantiate(obj);
            SceneManager.Instance.SceneRoot.AttachUIRoot(createObj);

            panel = createObj.GetComponent<BasePanel>();
            panel.SetBasePanel(type, PanelCount, destroyCb);

            PanelTable.Add(type, panel);
            PanelCount++;

            createCb?.Invoke((T)panel);

            AppEventManager.Instance.PushAppEvent(new AppEvent(EAppEventType.CreatePanel, type));
        }

        return (T)panel;
    }

    public bool IsExists(EPanel type)
    {
        return PanelTable.ContainsKey(type);
    }

    /// <summary>
    /// 모든 패널의 UI를 업데이트합니다. (UpdateUI 호출)
    /// </summary>
    public void UpdatePanelUI()
    {
#if ENABLE_LOG
        Debug.Log(string.Format("[ PanelManager ][ UpdatePanelUI ]"));
#endif

        if (PanelTable != null)
        {
            foreach (BasePanel panel in PanelTable.Values)
            {
                if (panel != null)
                    panel.UpdateUI();
            }
        }
    }

    /// <summary>
    /// 가장 최상위 패널의 BackButtonAction을 호출합니다.
    /// </summary>
    /// <returns></returns>
    public bool BackButtonAction()
    {
        if (PanelTable != null)
        {
            List<BasePanel> panelList = new List<BasePanel>(PanelTable.Values);
            if (panelList.Count > 0)
            {
                panelList.Sort((a, b) => { return b.PanelCount.CompareTo(a.PanelCount); });
                panelList[0].BackButtonAction();

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 패널을 제거합니다. 외부에서 패널 제거 호출 시 사용하세요.
    /// </summary>
    /// <param name="type"></param>
    public void DestroyPanel(EPanel type)
    {
        if (PanelTable.ContainsKey(type))
        {
            if (PanelTable[type] != null)
            {
#if ENABLE_LOG
                Debug.Log(string.Format("[ PanelManager ][ DestroyPanel ] : {0}", type));
#endif
                
                Util.Destroy(PanelTable[type].gameObject);
            }

            PanelTable.Remove(type);

            AppEventManager.Instance.PushAppEvent(new AppEvent(EAppEventType.DestroyPanel, type));
        }
    }

    /// <summary>
    /// 모든 패널을 제거합니다. 외부에서 패널 제거 호출 시 사용하세요.
    /// </summary>
    public void DestroyAllPanel()
    {
        if (PanelTable != null)
        {
            List<EPanel> keys = new List<EPanel>(PanelTable.Keys);
            for (int i = 0; i < keys.Count; i++)
                DestroyPanel(keys[i]);
        }
    }

    #endregion
}

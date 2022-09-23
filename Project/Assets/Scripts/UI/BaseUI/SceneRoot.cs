using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneRoot : MonoBehaviour
{
    #region OBJECTS

    [SerializeField] private GameObject UIRoot = null;
    [SerializeField] private Camera Camera = null;

    #endregion

    #region VALUES

    private ScreenCover ScreenCover = null;

    #endregion

    #region VIRTUAL METHOD

    protected virtual void Awake() { }

    protected virtual void Start() { }

    protected virtual void Update() { BackButtonAction(); }

    protected virtual void OnEnable() { }
    protected virtual void OnDisable() { }

    protected virtual void OnDestroy() { }

    protected virtual void OnApplicationPause(bool isPause) { }

    #endregion

    #region PUBLIC METHOD

    /// <summary>
    /// UIRoot 밑에 오브젝트를 세팅합니다.
    /// </summary>
    /// <param name="obj"></param>
    public void AttachUIRoot(GameObject obj)
    {
        if (obj != null)
        {
            obj.transform.parent = UIRoot.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.offsetMax = Vector2.zero;
            rect.offsetMin = Vector2.zero;
        }
    }

    /// <summary>
    /// 터치 영역을 막는 스크린 커버를 On/Off합니다.
    /// </summary>
    /// <param name="isActive"></param>
    public void ScreenCoverActive(bool isActive)
    {
        if (ScreenCover == null)
        {
            GameObject obj = ResourceLoader.LoadUIPrefab<GameObject>("ScreenCover");
            if (obj != null)
            {
                GameObject createObj = Util.Instantiate(obj);
                AttachUIRoot(createObj);

                ScreenCover = createObj.GetComponent<ScreenCover>();
            }
        }

        ScreenCover.SetActive(isActive);
    }

    #endregion

    #region PRIVATE MEHOD

    private void BackButtonAction()
    {
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PopupManager.Instance.BackButtonAction()) return;
            else if (PanelManager.Instance.BackButtonAction()) return;
        }
#endif
    }

    #endregion
}
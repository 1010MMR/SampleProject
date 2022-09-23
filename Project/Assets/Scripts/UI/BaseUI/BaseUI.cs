using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    #region VALUES
    
    protected System.Action DestroyCallback = null;

    #endregion

    #region UNITY METHOD

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        DestroyCallback?.Invoke();
    }

    #endregion

    #region PUBLIC METHOD
    
    #endregion

    #region PRIVATE METHOD
    
    #endregion

    #region ABSTRACT METHOD

    /// <summary>
    /// UI를 종료합니다.
    /// </summary>
    protected abstract void Close();

    /// <summary>
    /// UI의 구성 요소를 업데이트합니다.
    /// </summary>
    public virtual void UpdateUI() { }

    /// <summary>
    /// 백 버튼을 눌렀을 때 동작입니다.
    /// </summary>
    public abstract void BackButtonAction();

    #endregion
}

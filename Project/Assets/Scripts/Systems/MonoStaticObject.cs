using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 앱 실행 중 Mono 관련 기능을 통합 수행할 오브젝트.
/// </summary>
public class MonoStaticObject : MonoBehaviour
{
    #region UNITY METHOD

    private void OnApplicationQuit()
    {
        Util.ApplicationEndProcess = true;
    }
    
    #endregion

    #region Coroutine

    public new Coroutine StartCoroutine(IEnumerator routine)
    {
        return base.StartCoroutine(routine);
    }

    public new void StopCoroutine(IEnumerator routine)
    {
        base.StopCoroutine(routine);
    }

    public new void StopAllCoroutines()
    {
        base.StopAllCoroutines();
    }

    #endregion
}

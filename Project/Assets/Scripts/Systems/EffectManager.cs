using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoSingleton<EffectManager>
{
    #region VALUES

    #endregion

    #region OVERRIDE METHOD

    public override void ExplicitCall()
    {
    }

    protected override void Init()
    {
    }

    protected override void Release()
    {
    }

    #endregion

    #region PUBLIC METHOD

    /// <summary>
    /// 이펙트를 생성합니다.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parent"></param>
    /// <param name="callback"></param>
    /// <param name="factor"></param>
    public void MakeEffect(EEFFECT type, Transform parent, System.Action<GameObject> callback, float factor = 1.0f)
    {
        MakeEffect(TableManager.Instance.GetTableRow<EffectTable>(TableEnums.Effect, (int)type), parent, callback, factor);
    }

    /// <summary>
    /// 이펙트를 생성합니다.
    /// </summary>
    /// <param name="table"></param>
    /// <param name="parent"></param>
    /// <param name="callback"></param>
    /// <param name="factor"></param>
    public void MakeEffect(EffectTable table, Transform parent, System.Action<GameObject> callback, float factor = 1.0f)
    {
        ResourceLoader.LoadEffect(table.Name, (result) => {
            if (result != null)
            {
                GameObject obj = result as GameObject;
                GameObject createObj = Util.Instantiate(obj);
                createObj.transform.parent = parent;
                createObj.transform.localScale = obj.transform.localScale * factor;
                createObj.transform.localPosition = obj.transform.localPosition;

                Util.ChangeLayer(createObj, table.EffectLayerName);

                callback?.Invoke(createObj);
            }
        });
    }

    /// <summary>
    /// 이펙트를 생성합니다.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public void MakeEffect(EffectTable table, Transform parent, Vector3 position, System.Action<GameObject> callback, float factor = 1.0f)
    {
        ResourceLoader.LoadEffect(table.Name, (result) => {
            if (result != null)
            {
                GameObject obj = result as GameObject;
                GameObject createObj = Util.Instantiate(obj);
                createObj.transform.parent = parent;
                createObj.transform.localScale = obj.transform.localScale * factor;
                createObj.transform.localPosition = position;

                Util.ChangeLayer(createObj, table.EffectLayerName);

                callback?.Invoke(createObj);
            }
        });
    }
    
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Singleton 오브젝트를 저장하고 관리합니다.
/// </summary>
public class SingletonManager
{
    #region VALUES
    
    private static Transform ParentTransform = null;
    private static Dictionary<string, GameObject> SingletonTable = null;

    private static Dictionary<string, Action> SingletonClearActionTable = null;

    #endregion

    public SingletonManager()
    {
    }

    ~SingletonManager()
    {
        ParentTransform = null;
        SingletonTable = null;

        SingletonClearActionTable = null;
    }

    #region PUBLIC METHOD

    /// <summary>
    /// 싱글톤 오브젝트를 생성합니다.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject MakeSingletonObject(string name)
    {
        if (ParentTransform == null) MakeParent();
        if (SingletonTable == null) SingletonTable = new Dictionary<string, GameObject>();

        GameObject obj = null;
        if (SingletonTable.TryGetValue(name, out obj) == false)
        {
            obj = new GameObject(name);
            obj.transform.parent = ParentTransform;
            
            SingletonTable.Add(name, obj);
        }

        return obj;
    }

    /// <summary>
    /// 싱글톤의 Clear 액션을 취할 수 있도록 리스트에 추가합니다.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public static void AddSingletonClearAction(string name, Action action)
    {
        if (SingletonClearActionTable == null)
            SingletonClearActionTable = new Dictionary<string, Action>();
        SingletonClearActionTable.Add(name, action);
    }

    /// <summary>
    /// 모든 싱글톤 객체의 Clear 액션을 실행합니다.
    /// </summary>
    public static void InvokeAllSingletonClearAction()
    {
        if (SingletonClearActionTable != null)
        {
            foreach (Action action in SingletonClearActionTable.Values)
                action?.Invoke();
        }
    }

    /// <summary>
    /// 싱글톤 객체의 Clear 액션을 실행합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void InvokeSingletonClearAction<T>()
    {
        string name = typeof(T).ToString();
        if (SingletonClearActionTable != null && SingletonClearActionTable.ContainsKey(name))
            SingletonClearActionTable[name]?.Invoke();
    }

    /// <summary>
    /// 모든 싱글톤 객체를 삭제합니다.
    /// </summary>
    public static void DestroyAllSingleton()
    {
        if (SingletonTable != null)
        {
            List<string> keyList = new List<string>(SingletonTable.Keys);
            for (int i = 0; i < keyList.Count; i++)
            {
                Util.Destroy(SingletonTable[keyList[i]]);

                SingletonTable[keyList[i]] = null;
                SingletonTable.Remove(keyList[i]);
            }

            SingletonTable.Clear();
        }
    }

    /// <summary>
    /// 싱글톤 객체를 삭제합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void DestroySingleton<T>()
    {
        if (SingletonTable != null)
        {
            string name = typeof(T).ToString();
            if (SingletonTable.ContainsKey(name))
            {
                Util.Destroy(SingletonTable[name]);

                SingletonTable[name] = null;
                SingletonTable.Remove(name);
            }
        }
    }

    #endregion

    #region PRIVATE METHOD

    private static void MakeParent()
    {
        GameObject obj = new GameObject(Util.MONO_SINGLETON);
        Util.DontDestroyOnLoad(obj);

        ParentTransform = obj.transform;
    }

    #endregion
}

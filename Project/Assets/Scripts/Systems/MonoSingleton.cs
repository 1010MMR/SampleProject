using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour을 상속받아 Singleton을 구성할 클래스.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    #region VALUES

    private static T StaticInstance = null;
    
    #endregion

    #region PROPERTY

    public static T Instance { get { return GetInstance(); } }
    public static bool CheckExists { get { return StaticInstance != null; } }

    protected bool InitializeComplete { get; set; } = false;

    #endregion

    public MonoSingleton()
    {
        StaticInstance = null;
    }

    ~MonoSingleton()
    {
        StaticInstance = null;
    }

    #region ABSTRACT METHOD

    /// <summary>
    /// 인스턴스가 생성되는 시점에 호출됩니다. 명시적으로 사용하지 않습니다.
    /// </summary>
    protected abstract void Init();

    /// <summary>
    /// 인스턴스가 사라지는 시점에 호출됩니다. 명시적으로 사용하지 않습니다.
    /// </summary>
    protected abstract void Release();

    /// <summary>
    /// 명시적으로 인스턴스를 호출할 필요가 있을 때 사용합니다.
    /// </summary>
    public abstract void ExplicitCall();

    /// <summary>
    /// 인스턴스 내의 오브젝트를 초기화합니다. 이후에 다시 인스턴스를 가져올 때, Init()을 호출합니다. 명시적으로 사용하지 않습니다.
    /// </summary>
    protected virtual void Clear() { InitializeComplete = false; }

    #endregion

    #region PUBLIC METHOD

    public void Destroy()
    {
        if (Util.ApplicationEndProcess == false)
            Release();

        SingletonManager.DestroySingleton<T>();
    }

    #endregion

    #region PRIVATE METHOD

    private static T GetInstance()
    {
        if (StaticInstance == null)
        {
            StaticInstance = FindObjectOfType(typeof(T)) as T;
            if (StaticInstance == null)
            {
                GameObject obj = SingletonManager.MakeSingletonObject(typeof(T).ToString());

                StaticInstance = obj.AddComponent<T>();
                StaticInstance.InitializeComplete = true;
                StaticInstance.Init();

                SingletonManager.AddSingletonClearAction(typeof(T).ToString(), () => { StaticInstance.Clear(); });
            }
        }

        else
        {
            if (StaticInstance.InitializeComplete == false)
            {
                StaticInstance.InitializeComplete = true;
                StaticInstance.Init();
            }
        }

        return StaticInstance;
    }

    #endregion
}

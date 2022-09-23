using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 수행될 시퀀스 인터페이스. namespace로 감싸주시되, namespace 이름은 Name+Sequence 형식으로 
/// </summary>
public interface ISequence
{
    /// <summary>
    /// 시퀀스의 시작점.
    /// </summary>
    void Init();
    /// <summary>
    /// 시퀀스의 Update. Mono의 Update와 같은 주기.
    /// </summary>
    void Update();
    /// <summary>
    /// 시퀀스의 종료점.
    /// </summary>
    void Release();
    /// <summary>
    /// 시퀀스가 성공하는 요건 체크. 요건이 맞을 경우, 다음 시퀀스로 넘어갑니다.
    /// </summary>
    /// <param name="appEvent"></param>
    /// <returns></returns>
    bool SequenceComplete(AppEvent appEvent);
}

public class SequenceModule
{
    #region VALUES

    protected ISequence[] Sequences = null;
    protected int SequenceIdx = -1;

    #endregion

    #region PROPERTY

    private bool CheckSequenceStep { get { return (Sequences != null && Sequences.Length > SequenceIdx && SequenceIdx > -1); } }

    #endregion

    public SequenceModule()
    {
        AppEventManager.Instance.AddEventReceive(OnAppEventReceive);
    }

    public SequenceModule(SceneManager.ESceneType type) : this()
    {
        InitSequenceModule(string.Format("{0}Sequence", type.ToString()));
    }

    public SequenceModule(string name) : this()
    {
        InitSequenceModule(string.Format("{0}Sequence", name));
    }

    ~SequenceModule()
    {
        if (Util.ApplicationEndProcess == false)
            AppEventManager.Instance.RemoveEventReceive(OnAppEventReceive);
        
        Sequences = null;
        SequenceIdx = -1;
    }
    
    #region PUBLC METHOD

    public void Update()
    {
        if (CheckSequenceStep)
            Sequences[SequenceIdx].Update();
    }

    #endregion

    #region PRIVATE METHOD

    private void InitSequenceModule(string nameSpace)
    {
        System.Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        List<ISequence> sequences = new List<ISequence>();

        for (int i = 0; i < types.Length; i++)
        {
            if (string.IsNullOrEmpty(types[i].Namespace) == false && types[i].Namespace.Equals(nameSpace))
            {
                ISequence sequence = System.Activator.CreateInstance(types[i]) as ISequence;
                if (sequence != null)
                    sequences.Add(sequence);
            }
        }

        SequenceIdx = 0;

        Sequences = sequences.ToArray();
        Sequences[SequenceIdx].Init();
    }

    private void OnAppEventReceive(AppEvent appEvent)
    {
        if (CheckSequenceStep)
        {
            bool isComplete = Sequences[SequenceIdx].SequenceComplete(appEvent);
            if (isComplete)
            {
#if ENABLE_LOG
                Debug.Log(string.Format("[ SequenceModule ][ Sequence ] : {0}", Sequences[SequenceIdx].GetType().ToString()));
#endif

                Sequences[SequenceIdx].Release();
                SequenceIdx++;

                if (Sequences.Length > SequenceIdx)
                    Sequences[SequenceIdx].Init();
            }
        }
    }

    #endregion
}

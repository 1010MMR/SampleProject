using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각 씬의 Sequence를 관리합니다.
/// </summary>
public class SequenceManager : MonoSingleton<SequenceManager>
{
    #region VALUES

    private SequenceModule Sequence = null;

    #endregion

    #region UNITY METHOD

    private void Update()
    {
        if (Sequence != null)
            Sequence.Update();
    }

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
        Sequence = null;
    }

    protected override void Clear()
    {
        Release();
        base.Clear();
    }

    #endregion

    #region PUBLIC METHOD

    public void SetSequence(SequenceModule sequence)
    {
        if (Sequence != null)
            Sequence = null;
        Sequence = sequence;
    }

    public void ReleaseSceneSequence()
    {
        if (Sequence != null)
            Sequence = null;
    }

    #endregion
}

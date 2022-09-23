#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(BaseUI), true)]
public class BaseUITool : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        
        GUILayout.BeginVertical();
        {
            if (GUILayout.Button("Initialize BaseUI"))
            {
                Transform transform = (target as BaseUI).transform;
                List<Transform> childs = new List<Transform>();
                
                childs.Add(transform);
                GetTransformChild(transform, ref childs);

                for (int i = 0; i < childs.Count; i++)
                {
                    if (childs[i] != null)
                        childs[i].tag = "UI";
                }
                
                EditorUtility.SetDirty(target);
            }
        }
        GUILayout.EndVertical();
    }

    private void GetTransformChild(Transform target, ref List<Transform> childs)
    {
        int count = target.childCount;
        for (int i = 0; i < target.childCount; i++)
        {
            Transform tr = target.GetChild(i);
            childs.Add(tr);

            if (tr.childCount > 0)
                GetTransformChild(tr, ref childs);
        }
    }
}

#endif
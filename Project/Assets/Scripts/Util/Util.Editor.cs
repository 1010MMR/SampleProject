#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEditor;

public partial class Util
{
    #region EDITOR VIEW

    public static void ShowDrawSeperator()
    {
        EditorGUILayout.Separator();
        EditorGUILayout.Space();

        Texture2D tex = new Texture2D(1, 1);
        GUI.color = Color.gray;

        float y = GUILayoutUtility.GetLastRect().yMax;
        GUI.DrawTexture(new Rect(0.0f, y, Screen.width, 1.0f), tex);

        tex.hideFlags = HideFlags.DontSave;
        GUI.color = Color.white;

        EditorGUILayout.Space();
        EditorGUILayout.Separator();
    }

    public static void ShowDrawEditorTitle(string title)
    {
        ShowDrawSeperator();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUILayout.Label(title, EditorStyles.boldLabel);

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        ShowDrawSeperator();
    }

    #endregion
}

#endif

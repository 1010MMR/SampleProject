using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorNotificationPlatform : INotificationPlatform
{
    public void Initialize(System.Action<bool> callback)
    {
        callback?.Invoke(true);
    }

    public void SendNotification(string title, string text, long time)
    {
        Debug.Log(new System.DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(time));
    }

    public void CancelAllNotification()
    {
    }
}

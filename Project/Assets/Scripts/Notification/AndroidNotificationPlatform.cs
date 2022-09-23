#if UNITY_ANDROID

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;

public class AndroidNotificationPlatform : INotificationPlatform
{
    #region VALUES

    public const string DEFAULT_CNANNEL = "default_channel";

    #endregion

    #region INTERFACE

    public void Initialize(System.Action<bool> callback)
    {
        AndroidNotificationChannel channel = new AndroidNotificationChannel() {
            Id = DEFAULT_CNANNEL,
            Name = Util.GetString(ESTRING.Notification_Channel_Name),
            Description = Util.GetString(ESTRING.Notification_Channel_Desc),

            Importance = Importance.High,
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        callback?.Invoke(true);
    }

    public void SendNotification(string title, string text, long time)
    {
        AndroidNotification noti = new AndroidNotification() {
            Title = title,
            Text = text,

            FireTime = new System.DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(time),
        };
        
        AndroidNotificationCenter.SendNotification(noti, DEFAULT_CNANNEL);
    }

    public void CancelAllNotification()
    {
        AndroidNotificationCenter.CancelAllNotifications();
    }

    #endregion
}

#endif
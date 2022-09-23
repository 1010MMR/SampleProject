#if UNITY_IOS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.iOS;

public class iOSNotificationPlatform : INotificationPlatform
{
    #region INTERFACE

    public void Initialize(System.Action<bool> callback)
    {
        Util.StartCoroutine(IERequestAuthorization(callback));
    }

    public void SendNotification(string title, string text, long time)
    {
        iOSNotification noti = new iOSNotification() {
            Title = title,
            Body = text,
            Subtitle = title,

            ShowInForeground = false,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),

            Trigger = new iOSNotificationTimeIntervalTrigger() {
                TimeInterval = new System.DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(time) - System.DateTime.Now,
                Repeats = false,
            },
        };

        iOSNotificationCenter.ScheduleNotification(noti);
    }

    public void CancelAllNotification()
    {
        iOSNotificationCenter.RemoveAllScheduledNotifications();
    }

    #endregion

    #region PRIVATE MEHOD

    private IEnumerator IERequestAuthorization(System.Action<bool> callback)
    {
        using (AuthorizationRequest request = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true))
        {
            while (request.IsFinished == false)
                yield return null;

            callback?.Invoke(request.Granted);
        }
    }

    #endregion
}

#endif
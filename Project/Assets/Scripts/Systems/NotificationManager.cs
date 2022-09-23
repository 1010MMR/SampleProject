using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ENABLE_FIREBASE
using Firebase.DynamicLinks;
#endif

public interface INotificationPlatform
{
    /// <summary>
    /// 알람 플랫폼을 초기화합니다.
    /// </summary>
    /// <param name="callback"></param>
    void Initialize(System.Action<bool> callback);

    /// <summary>
    /// 로컬 알람을 전송합니다.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="text"></param>
    /// <param name="time"></param>
    void SendNotification(string title, string text, long time);

    /// <summary>
    /// 모든 알람을 제거합니다.
    /// </summary>
    void CancelAllNotification();
}

/// <summary>
/// 앱 알람, 게임 내 알람을 관리합니다.
/// </summary>
public class NotificationManager : MonoSingleton<NotificationManager>
{
    #region VALUES

    private INotificationPlatform Notification = null;

    #endregion

    #region OVERRIDE METHOD

    public override void ExplicitCall()
    {
    }

    protected override void Init()
    {
#if UNITY_EDITOR
        Notification = new EditorNotificationPlatform();
#elif UNITY_ANDROID
        Notification = new AndroidNotificationPlatform();
#elif UNITY_IOS
        Notification = new iOSNotificationPlatform();
#endif
        Notification.Initialize((result) => {
            AppEventManager.Instance.PushAppEvent(new AppEvent(EAppEventType.ClassInitializeComplete));
        });

#if ENABLE_FIREBASE
        DynamicLinks.DynamicLinkReceived += OnDynamicLink;
#endif
    }

    protected override void Release()
    {
#if ENABLE_FIREBASE
        if (Util.ApplicationEndProcess == false)
            DynamicLinks.DynamicLinkReceived -= OnDynamicLink;
#endif

        Notification = null;
    }

    #endregion

    #region PUBLIC METHOD

    /// <summary>
    /// 로컬 알람을 전송합니다.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="text"></param>
    /// <param name="time"></param>
    public void SendNotification(string title, string text, long time)
    {
        Notification.SendNotification(title, text, time);
    }

    /// <summary>
    /// 모든 알람을 제거합니다.
    /// </summary>
    public void CancelAllNotification()
    {
        Notification.CancelAllNotification();
    }

    #endregion

    #region CALLBACK

#if ENABLE_FIREBASE
    private void OnDynamicLink(object sender, ReceivedDynamicLinkEventArgs args)
    {
#if ENABLE_LOG
        Debug.Log(string.Format("[ NotificationManager ][ OnDynamicLink ] : {0}", args.ReceivedDynamicLink.Url));
#endif
    }
#endif

    #endregion
}

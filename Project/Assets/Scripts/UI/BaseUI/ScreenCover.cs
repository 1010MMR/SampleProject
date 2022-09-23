using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCover : MonoBehaviour
{
    #region OBJECTS
    
    #endregion

    #region VALUES
    
    #endregion

    #region UNITY METHOD

    private void Awake()
    {
    }

    #endregion

    #region PUBLIC MEHOD

    public void SetActive(bool isActive)
    {
#if ENABLE_LOG
        Debug.Log(string.Format("[ ScreenCover ][ Active ] : {0}", isActive));
#endif

        gameObject.SetActive(isActive);
    }

    #endregion
}

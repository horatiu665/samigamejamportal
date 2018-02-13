#if OCULUS
using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.XR;

public class OculusEntitlementCheck : MonoBehaviour
{
    // default: selfietennis appid
    public string appId = "1233145293403213";

    private void OnEnable()
    {
        var req = Oculus.Platform.Core.AsyncInitialize(appId);
        req.OnComplete(OnAsyncInitializeCompleted);
        Request.RunCallbacks();
    }

    private void OnAsyncInitializeCompleted(Message<PlatformInitialize> message)
    {
        if (message.Data.Result == PlatformInitializeResult.Success)
        {
            Debug.Log("Platform initialize = success. Checking entitlement LOL");
            Oculus.Platform.Entitlements.IsUserEntitledToApplication().OnComplete(UserEntitledCallback);
        }
        else
        {
            Debug.Log("FAIL INITIALIZE PLATFORM");
            UnityEngine.Application.Quit();
        }
    }

    private void UserEntitledCallback(Message message)
    {
        if (message.Type == Message.MessageType.Entitlement_GetIsViewerEntitled)
        {
            if (message.IsError)
            {
                Debug.Log("Entitlement failed. YOU FAILED. Quitting...");
                UnityEngine.Application.Quit();
            }
            else
            {
                Debug.Log("Thank you for playing #SelfieTennis!");
            }
        }
        else
        {
            Debug.Log("Weird message: " + message);
        }
    }
}
#endif
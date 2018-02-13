using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class OculusCameraFix : MonoBehaviour
{
    private static OculusCameraFix _instance;
    public static OculusCameraFix instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<OculusCameraFix>();
            }
            return _instance;
        }
    }

    [SerializeField]
    private Camera _vrcam;
    public Camera vrcam
    {
        get
        {
            if (_vrcam == null)
            {
                _vrcam = GetComponent<Camera>();
            }
            return _vrcam;
        }
    }

    // oculus doesn't like idsableing the vr cam
    public bool disableVRCam = false;
    public Camera nonVRCam;

    bool state;

    private IEnumerator Start()
    {
        yield return 0;
        // make sure init works
        state = false;
        SetCamera(true);

        yield break;
    }

    private void OnApplicationFocus(bool focus)
    {
        SetCamera(focus);
    }

    private void LateUpdate()
    {
        if (XRDevice.userPresence == UserPresenceState.Present && (OculusPauseDetect.instance != null && !OculusPauseDetect.instance.isPaused))
        {
            SetCamera(true);
        }
        else
        {
            SetCamera(false);
        }
    }

    public void SetCamera(bool vrActive)
    {
        if (vrActive != state)
        {
            state = vrActive;
        }
        else
        {
            return;
        }

        // when there is an external camera use solution #1
        if (nonVRCam != null)
        {
            nonVRCam.fieldOfView = vrcam.fieldOfView * 0.75f;
            nonVRCam.enabled = !vrActive;
            if (disableVRCam)
            {
                vrcam.enabled = vrActive;
            }
        }
        // else use solution #2
        else
        {
            vrcam.stereoTargetEye = vrActive ? StereoTargetEyeMask.Both : StereoTargetEyeMask.None;
        }
    }
}

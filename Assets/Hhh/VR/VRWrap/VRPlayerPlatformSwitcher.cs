using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class VRPlayerPlatformSwitcher : MonoBehaviour
{
    [SerializeField]
    private VRPlayerComponent _vrPlayer;
    public VRPlayerComponent vrPlayer
    {
        get
        {
            if (_vrPlayer == null)
            {
                _vrPlayer = GetComponent<VRPlayerComponent>();
            }
            return _vrPlayer;
        }
    }

    [SerializeField]
    private HhhVRGrabber.VRPlayerGrabSystem _vrGrabSystem;
    public HhhVRGrabber.VRPlayerGrabSystem vrGrabSystem
    {
        get
        {
            if (_vrGrabSystem == null)
            {
                _vrGrabSystem = GetComponent<HhhVRGrabber.VRPlayerGrabSystem>();
            }
            return _vrGrabSystem;
        }
    }

    [Header("SteamVR")]
    public GameObject steamVRRoot;
    public SteamVR_Camera steamVrHead;
    public GameObject steamVrLeft, steamVrRight;

    [Header("Oculus")]
    public GameObject oculusRoot;
    public GameObject oculusHead;
    public GameObject oculusLeft, oculusRight;

    private void Reset()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        GetReferencesOculus();
        GetReferencesSteamVR();

        ValidateGrabSystem();
    }

    private void OnEnable()
    {
        if (InputVR.controllerType == InputVR.VRControllerType.Oculus)
        {
            SwitchToOculus();
        }
        else
        {
            SwitchToSteamVR();
        }
    }

    [DebugButton]
    public void SwitchToOculus()
    {
        GetReferencesOculus();

        oculusRoot.SetActive(true);
        if (steamVRRoot != null)
        {
            steamVRRoot.SetActive(false);
        }
        vrPlayer.head = oculusHead.transform;
        vrPlayer.leftHand = oculusLeft.transform;
        vrPlayer.rightHand = oculusRight.transform;

        ValidateGrabSystem();
    }

    private void GetReferencesOculus()
    {
#if OCULUS
        OVRCameraRig oCr;
        if (oculusRight == null || oculusLeft == null || oculusRoot == null || oculusHead == null)
        {
            oCr = GetComponentInChildren<OVRCameraRig>();

            if (oculusRoot == null)
            {
                oculusRoot = oCr.gameObject;
            }
            if (oculusHead == null)
            {
                oculusHead = oCr.centerEyeAnchor.gameObject;
            }
            if (oculusLeft == null)
            {
                oculusLeft = oCr.leftHandAnchor.gameObject;
                if (oculusLeft.transform.childCount > 0)
                {
                    oculusLeft = oculusLeft.transform.GetChild(0).gameObject;
                }
            }
            if (oculusRight == null)
            {
                oculusRight = oCr.rightHandAnchor.gameObject;
                if (oculusRight.transform.childCount > 0)
                {
                    oculusRight = oculusRight.transform.GetChild(0).gameObject;
                }
            }
        }

#endif
    }

    [DebugButton]
    public void SwitchToSteamVR()
    {
        GetReferencesSteamVR();

        if (oculusRoot != null)
        {
            oculusRoot.SetActive(false);
        }
        steamVRRoot.SetActive(true);
        vrPlayer.head = steamVrHead.transform;
        vrPlayer.leftHand = steamVrLeft.transform;
        vrPlayer.rightHand = steamVrRight.transform;

        ValidateGrabSystem();
    }

    private void GetReferencesSteamVR()
    {
        var scm = GetComponentInChildren<SteamVR_ControllerManager>(true);
        steamVRRoot = scm.gameObject;
        steamVrHead = GetComponentInChildren<SteamVR_Camera>(true);
        steamVrLeft = scm.left;
        steamVrRight = scm.right;
    }

    private void ValidateGrabSystem()
    {
        if (vrGrabSystem != null)
        {
            vrGrabSystem.OnValidate();
        }
    }

}
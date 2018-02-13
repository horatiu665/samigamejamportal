using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif
using Random = UnityEngine.Random;

/// <summary>
/// Get inputs from VR controllers, using the InputManager after it has been setup by Horatiu's InputManagerTool
/// </summary>
public class InputVR : MonoBehaviour
{
    private static InputVR _instance;
    public static InputVR instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InputVR>();
            }
            if (_instance == null)
            {
                _instance = new GameObject("[InputVR]", typeof(InputVR)).GetComponent<InputVR>();
            }
            return _instance;
        }
    }

    // refs
    [SerializeField]
    private InputVR_BeforeUpdate _inputVrBeforeUpdate;
    public InputVR_BeforeUpdate inputVrBeforeUpdate
    {
        get
        {
            if (_inputVrBeforeUpdate == null)
            {
                _inputVrBeforeUpdate = GetComponent<InputVR_BeforeUpdate>();
                if (_inputVrBeforeUpdate == null)
                    _inputVrBeforeUpdate = gameObject.AddComponent<InputVR_BeforeUpdate>();
            }
            return _inputVrBeforeUpdate;
        }
    }

    public static Action OnBeforeUpdate;

    public static List<XRNodeState> curNodeStates = new List<XRNodeState>();

#if UNITY_WSA
    public static InteractionSourceState[] winSourceStates = null;
#endif

    // head fixz
    private static Vector3 prevHeadPos = Vector3.zero;
    private Transform _headMagicCenter;
    public Transform headMagicCenter
    {
        get
        {
            if (_headMagicCenter == null)
            {
                _headMagicCenter = new GameObject("[Head Magic Center] sets a more reliable tracked status on the hmd based on the measured movement").transform;
                _headMagicCenter.transform.SetParent(InputVR.VRCamera.transform);
                _headMagicCenter.transform.localPosition = new Vector3(0, -0.08f, -0.08f);
                _headMagicCenter.transform.localRotation = Quaternion.identity;
                _headMagicCenter.transform.localScale = Vector3.one;
            }
            return _headMagicCenter;
        }
    }

    // must refactor this code so it does not use trashy Camera.main
    private static Camera _vrCamera;
    public static Camera VRCamera
    {
        get
        {
            if (_vrCamera == null || !_vrCamera.gameObject.activeInHierarchy)
            {
                _vrCamera = Camera.main;
            }
            return _vrCamera;
        }
    }

    public float headPosTrackingEpsilon = 1e-06f;

    public bool lockFixedDeltaTimeToFpsLikeSteamVR = true;

    [Header("Input settings")]
    [SerializeField]
    private VRControllerType _controllerType = VRControllerType.None;
    public static VRControllerType controllerType
    {
        get
        {
            if (instance != null)
            {
                if (instance._controllerType == VRControllerType.None)
                {
                    instance._controllerType = GetControllerType();
                }
                return instance._controllerType;
            }
            return VRControllerType.SteamVR;
        }
    }

    /// <summary>
    /// Auto sets VR controller type based on selected platform.
    /// </summary>
    /// <returns></returns>
    public static VRControllerType GetControllerType()
    {
        var playing = Application.isPlaying;
        if (!playing)
        {
            //Debug.Log("[InputVR] Warning! Controller Type polled outside play mode. Type returned is best guess based on the supported devices! Not the actual 100% sure loaded device");
            if (XRSettings.supportedDevices.Contains("OpenVR"))
            {
                return VRControllerType.SteamVR;
            }
            else if (XRSettings.supportedDevices.Contains("Oculus"))
            {
                return VRControllerType.Oculus;
            }
            else if (XRSettings.supportedDevices.Contains("WindowsMR"))
            {
                return VRControllerType.Windows;
            }
            else
            {
                return VRControllerType.None;
            }
        }


        var cit = VRControllerType.None;
        // set up controller based on VR type
        // check if SteamVR
        if (XRSettings.supportedDevices.Contains("OpenVR") && XRSettings.loadedDeviceName == "OpenVR")
        {
            // in SteamVR there can be many devices, so let that one take care of it ;)
            cit = VRControllerType.SteamVR;
        }
        // else check if Oculus native
        else if (XRSettings.supportedDevices.Contains("Oculus") && XRSettings.loadedDeviceName == "Oculus")
        {
            cit = VRControllerType.Oculus;
        }
        // else if WindowsMR
        else if (XRSettings.supportedDevices.Contains("WindowsMR") && XRSettings.loadedDeviceName == "WindowsMR")
        {
            cit = VRControllerType.Windows;
        }
        else
        {
            Debug.Log("[InputVR] VR Controller Type not standard! Good luck my friend!");
            cit = VRControllerType.None;
        }

        return cit;

    }

    [SerializeField]
    private bool _defaultViveAnalogStickToTouchpad = true;
    public static bool defaultViveAnalogStickToTouchpad
    {
        get
        {
            if (instance != null)
            {
                return instance._defaultViveAnalogStickToTouchpad;
            }
            return true;
        }
    }

    [SerializeField]
    private bool _defaultOculusTouchpadToAnalogStick = true;
    public static bool defaultOculusTouchpadToAnalogStick
    {
        get
        {
            if (instance != null)
            {
                return instance._defaultOculusTouchpadToAnalogStick;
            }
            return true;
        }
    }

    private static Dictionary<ButtonMask, Vector2> prevAxisValuesLeft = new Dictionary<ButtonMask, Vector2>();
    private static Dictionary<ButtonMask, Vector2> prevAxisValuesRight = new Dictionary<ButtonMask, Vector2>();

    //public int debugQueueSize = 0;
    //Queue<GameObject> debugs = new Queue<GameObject>();


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("[InputVR] There can only be one InputVR object in the scene! Deleting", instance.gameObject);
            Destroy(gameObject);
        }

        if (controllerType == VRControllerType.None)
        {
            Debug.Log("Uhhhh. No VR detecteD);");
        }
    }

    private void OnValidate()
    {
        if (controllerType != VRControllerType.None)
        {
            // just checking.
        }

        if (inputVrBeforeUpdate != null)
        {
        }
        if (!Application.isPlaying && curNodeStates.Count > 0)
        {
            curNodeStates.Clear();
        }
    }

    private void OnEnable()
    {
        inputVrBeforeUpdate.OnUpdateBeforeRegular += UpdateBeforeRegular;

        // preload dictionaries to prevent errors when accessing, and also nullchecks which might cause underperformance when overdone
        foreach (var bmv in buttonMaskEnumValues)
        {
            var buttonMask = (ButtonMask)bmv;
            prevAxisValuesLeft[buttonMask] = Vector2.zero;
            prevAxisValuesRight[buttonMask] = Vector2.zero;
        }

        //if (debugQueueSize > 0)
        //{
        //    DoQueue(debugQueueSize);
        //}

#if UNITY_WSA
        UnityEngine.XR.WSA.Input.InteractionManager.InteractionSourceReleased += WinInteractionManager_InteractionSourceReleased;
#endif
    }

    private void OnDisable()
    {
        inputVrBeforeUpdate.OnUpdateBeforeRegular -= UpdateBeforeRegular;

#if UNITY_WSA
        UnityEngine.XR.WSA.Input.InteractionManager.InteractionSourceReleased -= WinInteractionManager_InteractionSourceReleased;
#endif
    }

    /// <summary>
    /// BEFORE regular Update. Called by <see cref="InputVR_BeforeUpdate"/>, with ExecutionOrder set to -9999
    /// </summary>
    private void UpdateBeforeRegular()
    {
        UpdateBefore_ComputeNodeStates();

        UpdateBefore_StupidMRVelocity();

        UpdateBefore_FixHeadTrackedIssue();

        if (OnBeforeUpdate != null)
        {
            OnBeforeUpdate();
        }

        if (controllerType != VRControllerType.SteamVR)
        {
            if (lockFixedDeltaTimeToFpsLikeSteamVR)
            {
                // fixed at 90 fps
                Time.fixedDeltaTime = Time.timeScale / Mathf.Clamp(XRDevice.refreshRate, 60, 120);
            }
        }

    }

#if UNITY_WSA
    // fix velocity and angular velocity on windows controller release.
    private void WinInteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs obj)
    {
        if (obj.pressType == InteractionSourcePressType.Select && obj.state.source.kind == InteractionSourceKind.Controller)
        {
            var left = (obj.state.source.handedness == InteractionSourceHandedness.Left);

            Vector3 vel;
            if (obj.state.sourcePose.TryGetVelocity(out vel))
            {
                if (float.IsNaN(vel.x) || float.IsNaN(vel.y) || float.IsNaN(vel.z))
                {
                    vel = Vector3.zero;
                }
                else
                {

                    for (int i = 0; i < curNodeStates.Count; i++)
                    {
                        if ((curNodeStates[i].nodeType == XRNode.LeftHand && left) || (curNodeStates[i].nodeType == XRNode.RightHand && !left))
                        {
                            var cns = curNodeStates[i];
                            cns.velocity = vel;
                            curNodeStates[i] = cns;
                        }
                    }
                }
            }

            Vector3 av;
            if (obj.state.sourcePose.TryGetAngularVelocity(out av))
            {
                if (float.IsNaN(av.x) || float.IsNaN(av.y) || float.IsNaN(av.z))
                {
                    av = Vector3.zero;
                }
                else
                {

                    for (int i = 0; i < curNodeStates.Count; i++)
                    {
                        if ((curNodeStates[i].nodeType == XRNode.LeftHand && left) || (curNodeStates[i].nodeType == XRNode.RightHand && !left))
                        {
                            var cns = curNodeStates[i];
                            cns.angularVelocity = av;
                            curNodeStates[i] = cns;
                        }
                    }
                }
            }
        }
    }
#endif

    // fixes lack of velocity info on curNodeStates after simple Unity API call.
    private static void UpdateBefore_StupidMRVelocity()
    {
#if UNITY_WSA
        if (controllerType != VRControllerType.Windows)
        {
            return;
        }

        // get reading, much like the unity API
        if (winSourceStates == null || winSourceStates.Length != curNodeStates.Count)
        {
            winSourceStates = new InteractionSourceState[curNodeStates.Count];
        }
        UnityEngine.XR.WSA.Input.InteractionManager.GetCurrentReading(winSourceStates);

        for (int j = 0; j < winSourceStates.Length; j++)
        {
            var ws = winSourceStates[j];
            if (ws.source.kind == InteractionSourceKind.Controller)
            {
                var left = ws.source.handedness == InteractionSourceHandedness.Left;
                Vector3 vel;
                if (ws.sourcePose.TryGetVelocity(out vel))
                {
                    for (int i = 0; i < curNodeStates.Count; i++)
                    {
                        if ((curNodeStates[i].nodeType == XRNode.LeftHand && left) || (curNodeStates[i].nodeType == XRNode.RightHand && !left))
                        {
                            var cns = curNodeStates[i];
                            cns.velocity = vel;
                            curNodeStates[i] = cns;

                        }
                    }

                    if (instance.debugQueueSize > 0)
                    {
                        if (!InputVR.GetTouch(false, ButtonMask.Touchpad))
                        {
                            Vector3 fckPos;
                            if (ws.sourcePose.TryGetPosition(out fckPos))
                            {
                                fckPos += TennisPlayerRoot.inst.transform.position;
                                DebugInGame.DrawVisibleGizmo(instance.GetDebug(), fckPos, vel);
                            }
                        }

                    }
                }
            }
        }
#endif
    }

    /// <summary>
    /// Uses <see cref="InputTracking.GetNodeStates(List{XRNodeState})"/> to store node states for next frame, and handles tracked issues with TryGetPosition returning NaNs and shit. 
    /// </summary>
    private static void UpdateBefore_ComputeNodeStates()
    {
        InputTracking.GetNodeStates(curNodeStates);
        for (int i = 0; i < curNodeStates.Count; i++)
        {
            if (curNodeStates[i].tracked)
            {
                var cn = curNodeStates[i];
                // try fix NaNs for all fields
                Vector3 temp;
                if (curNodeStates[i].TryGetPosition(out temp))
                {
                    if (float.IsNaN(temp.x) || float.IsNaN(temp.y) || float.IsNaN(temp.z))
                    {
                        cn.position = Vector3.zero;
                        cn.tracked = false;
                    }
                }
                if (curNodeStates[i].TryGetVelocity(out temp))
                {
                    if (float.IsNaN(temp.x) || float.IsNaN(temp.y) || float.IsNaN(temp.z))
                    {
                        cn.velocity = Vector3.zero;
                        cn.tracked = false;
                    }
                }
                if (curNodeStates[i].TryGetAngularVelocity(out temp))
                {
                    if (float.IsNaN(temp.x) || float.IsNaN(temp.y) || float.IsNaN(temp.z))
                    {
                        cn.angularVelocity = Vector3.zero;
                        cn.tracked = false;
                    }
                }
                if (curNodeStates[i].TryGetAcceleration(out temp))
                {
                    if (float.IsNaN(temp.x) || float.IsNaN(temp.y) || float.IsNaN(temp.z))
                    {
                        cn.acceleration = Vector3.zero;
                        cn.tracked = false;
                    }
                }
                if (curNodeStates[i].TryGetAngularAcceleration(out temp))
                {
                    if (float.IsNaN(temp.x) || float.IsNaN(temp.y) || float.IsNaN(temp.z))
                    {
                        cn.angularAcceleration = Vector3.zero;
                        cn.tracked = false;
                    }
                }
                Quaternion temp2;
                if (curNodeStates[i].TryGetRotation(out temp2))
                {
                    if (float.IsNaN(temp2.x) || float.IsNaN(temp2.y) || float.IsNaN(temp2.z) || float.IsNaN(temp2.w))
                    {
                        cn.rotation = Quaternion.identity;
                        cn.tracked = false;
                    }
                }
                if (!cn.tracked)
                {
                    Debug.Log("Not tracked " + cn.nodeType);
                }
                curNodeStates[i] = cn;

            }

        }
    }

    /// <summary>
    /// Handles XRNodeState.tracked for the head using MAGIC ;) because simple Unity API call is not enough so we have to use tricks
    /// Algorithm:
    ///     if head tracking is exactly the same over 2 frames, no supra-epsilon (1e-06 meter) fluctuation, it means:
    ///     1. user is a ninja and perfectly still
    ///     2. user put the headset on a perfectly still tripod in perfect lighting conditions
    ///     3. headset lost tracking
    ///     (guess which one is more likely to be true)
    ///     in any case, mark as not tracked until it regains tracking and moves more than epsilon (ninja better be blind to function)
    /// </summary>
    private static void UpdateBefore_FixHeadTrackedIssue()
    {
        for (int i = 0; i < curNodeStates.Count; i++)
        {
            // handle head tracking
            if (curNodeStates[i].nodeType == XRNode.Head)
            {
                var headProblems = 0;

                Vector3 headPos = instance.headMagicCenter.position;

                // shows the actual position of the magic head position. found a small offset of ~1e-08 on the Y axis... so it's a bit hard to be sure about the epsilon. 10 micrometers should be fine
                //Debug.Log("HP: " + headPos.x.ToString("F9") + ", " + headPos.y.ToString("F9") + ", " + headPos.z.ToString("F9"));

                //problems when identical pos over 2 frames
                if ((prevHeadPos - headPos).magnitude <= instance.headPosTrackingEpsilon)
                {
                    headProblems = 1;
                }
                // prev pos after head...
                prevHeadPos = headPos;

                if (headProblems != 0)
                {
                    var cn = curNodeStates[i];
                    cn.tracked = false;
                    curNodeStates[i] = cn;
                }

            }
        }

        // would like an option to fade out graphics when head is not tracked. to prevent motion sickness 
        // if (!tracked) FADETHATSHIT else un-fade.
    }

    // AFTER regular Update() ♥
    /// <summary>
    /// Updates the prevValues dictionaries with current frame values, for use in the next frame.
    /// InputVR editor script makes sure the InputVR script is executed AFTER all other scripts.
    /// this ensures the Up and Down Get() functions work without a frame delay for all scripts.
    /// </summary>
    private void Update()
    {
        // update input axis states, for the GetDown and GetUp touch and press methods, where not supported by unity InputManager.
        foreach (var bmv in buttonMaskEnumValues)
        {
            var buttonMask = (ButtonMask)bmv;
            prevAxisValuesLeft[buttonMask] = GetAxis(true, buttonMask);
            prevAxisValuesRight[buttonMask] = GetAxis(false, buttonMask);
        }
    }


    /// <summary>
    /// Get input vector for axis ButtonMask. If it supports 2D input like touchpad, returns x and y. If only 1D like trigger, it returns x as trigger and y as zero.
    /// </summary>
    public static Vector2 GetAxis(bool left, ButtonMask buttonMask)
    {
        if (controllerType == VRControllerType.SteamVR)
        {
            if (defaultViveAnalogStickToTouchpad && buttonMask == ButtonMask.AnalogStick)
            {
                buttonMask = ButtonMask.Touchpad;
            }
            return Vive.GetAxis(left, (ViveButtonMask)buttonMask);
        }
        else if (controllerType == VRControllerType.Oculus)
        {
            if (defaultOculusTouchpadToAnalogStick && buttonMask == ButtonMask.Touchpad)
            {
                buttonMask = ButtonMask.AnalogStick;
            }
            return Oculus.GetAxis(left, (OculusButtonMask)buttonMask);
        }
        else if (controllerType == VRControllerType.Windows)
        {
            return Windows.GetAxis(left, (WindowsButtonMask)buttonMask);
        }

        return Vector2.zero;
    }

    public static bool GetTouchDown(bool left, ButtonMask buttonMask)
    {
        if (controllerType == VRControllerType.SteamVR)
        {
            if (defaultViveAnalogStickToTouchpad && buttonMask == ButtonMask.AnalogStick)
            {
                buttonMask = ButtonMask.Touchpad;
            }
            return Vive.GetTouchDown(left, (ViveButtonMask)buttonMask);
        }
        else if (controllerType == VRControllerType.Oculus)
        {
            if (defaultOculusTouchpadToAnalogStick && buttonMask == ButtonMask.Touchpad)
            {
                buttonMask = ButtonMask.AnalogStick;
            }
            return Oculus.GetTouchDown(left, (OculusButtonMask)buttonMask);
        }
        else if (controllerType == VRControllerType.Windows)
        {
            return Windows.GetTouchDown(left, (WindowsButtonMask)buttonMask);
        }

        return false;
    }

    public static bool GetTouchUp(bool left, ButtonMask buttonMask)
    {
        if (controllerType == VRControllerType.SteamVR)
        {
            if (defaultViveAnalogStickToTouchpad && buttonMask == ButtonMask.AnalogStick)
            {
                buttonMask = ButtonMask.Touchpad;
            }
            return Vive.GetTouchUp(left, (ViveButtonMask)buttonMask);
        }
        else if (controllerType == VRControllerType.Oculus)
        {
            if (defaultOculusTouchpadToAnalogStick && buttonMask == ButtonMask.Touchpad)
            {
                buttonMask = ButtonMask.AnalogStick;
            }
            return Oculus.GetTouchUp(left, (OculusButtonMask)buttonMask);
        }
        else if (controllerType == VRControllerType.Windows)
        {
            return Windows.GetTouchUp(left, (WindowsButtonMask)buttonMask);
        }

        return false;
    }

    public static bool GetTouch(bool left, ButtonMask buttonMask)
    {
        if (controllerType == VRControllerType.SteamVR)
        {
            if (defaultViveAnalogStickToTouchpad && buttonMask == ButtonMask.AnalogStick)
            {
                buttonMask = ButtonMask.Touchpad;
            }
            return Vive.GetTouch(left, (ViveButtonMask)buttonMask);
        }
        else if (controllerType == VRControllerType.Oculus)
        {
            if (defaultOculusTouchpadToAnalogStick && buttonMask == ButtonMask.Touchpad)
            {
                buttonMask = ButtonMask.AnalogStick;
            }
            return Oculus.GetTouch(left, (OculusButtonMask)buttonMask);
        }
        else if (controllerType == VRControllerType.Windows)
        {
            return Windows.GetTouch(left, (WindowsButtonMask)buttonMask);
        }

        return false;
    }

    public static bool GetPressDown(bool left, ButtonMask buttonMask)
    {
        if (controllerType == VRControllerType.SteamVR)
        {
            if (defaultViveAnalogStickToTouchpad && buttonMask == ButtonMask.AnalogStick)
            {
                buttonMask = ButtonMask.Touchpad;
            }
            return Vive.GetPressDown(left, (ViveButtonMask)buttonMask);
        }
        else if (controllerType == VRControllerType.Oculus)
        {
            if (defaultOculusTouchpadToAnalogStick && buttonMask == ButtonMask.Touchpad)
            {
                buttonMask = ButtonMask.AnalogStick;
            }
            return Oculus.GetPressDown(left, (OculusButtonMask)buttonMask);
        }
        else if (controllerType == VRControllerType.Windows)
        {
            return Windows.GetPressDown(left, (WindowsButtonMask)buttonMask);
        }

        return false;
    }

    public static bool GetPressUp(bool left, ButtonMask buttonMask)
    {
        if (controllerType == VRControllerType.SteamVR)
        {
            if (defaultViveAnalogStickToTouchpad && buttonMask == ButtonMask.AnalogStick)
            {
                buttonMask = ButtonMask.Touchpad;
            }
            return Vive.GetPressUp(left, (ViveButtonMask)buttonMask);
        }
        else if (controllerType == VRControllerType.Oculus)
        {
            if (defaultOculusTouchpadToAnalogStick && buttonMask == ButtonMask.Touchpad)
            {
                buttonMask = ButtonMask.AnalogStick;
            }
            return Oculus.GetPressUp(left, (OculusButtonMask)buttonMask);
        }
        else if (controllerType == VRControllerType.Windows)
        {
            return Windows.GetPressUp(left, (WindowsButtonMask)buttonMask);
        }

        return false;
    }

    public static bool GetPress(bool left, ButtonMask buttonMask)
    {
        if (controllerType == VRControllerType.SteamVR)
        {
            if (defaultViveAnalogStickToTouchpad && buttonMask == ButtonMask.AnalogStick)
            {
                buttonMask = ButtonMask.Touchpad;
            }
            return Vive.GetPress(left, (ViveButtonMask)buttonMask);
        }
        else if (controllerType == VRControllerType.Oculus)
        {
            if (defaultOculusTouchpadToAnalogStick && buttonMask == ButtonMask.Touchpad)
            {
                buttonMask = ButtonMask.AnalogStick;
            }
            return Oculus.GetPress(left, (OculusButtonMask)buttonMask);
        }
        else if (controllerType == VRControllerType.Windows)
        {
            return Windows.GetPress(left, (WindowsButtonMask)buttonMask);
        }

        return false;
    }

    public static Vector3 GetLocalPosition(bool left)
    {
        return GetLocalPosition(left ? XRNode.LeftHand : XRNode.RightHand);
    }

    public static Vector3 GetLocalPosition(XRNode node)
    {
        for (int i = 0; i < curNodeStates.Count; i++)
        {
            if (curNodeStates[i].tracked && curNodeStates[i].nodeType == node)
            {
                Vector3 pos;
                if (curNodeStates[i].TryGetPosition(out pos))
                {
                    return pos;
                }
            }
        }
        return Vector3.zero;
    }

    public static Quaternion GetLocalRotation(bool left)
    {
        return GetLocalRotation(left ? XRNode.LeftHand : XRNode.RightHand);
    }

    public static Quaternion GetLocalRotation(XRNode node)
    {
        for (int i = 0; i < curNodeStates.Count; i++)
        {
            if (curNodeStates[i].tracked && curNodeStates[i].nodeType == node)
            {
                Quaternion rot;
                if (curNodeStates[i].TryGetRotation(out rot))
                {
                    return rot;
                }
            }
        }
        return Quaternion.identity;
    }

    public static Vector3 GetVelocity(bool left)
    {
        return GetVelocity(left ? XRNode.LeftHand : XRNode.RightHand);
    }

    public static Vector3 GetVelocity(XRNode node)
    {
        for (int i = 0; i < curNodeStates.Count; i++)
        {
            if (curNodeStates[i].tracked && curNodeStates[i].nodeType == node)
            {
                Vector3 vel;
                if (curNodeStates[i].TryGetVelocity(out vel))
                {
                    return vel;
                }
            }
        }
        return Vector3.zero;
    }

    public static Vector3 GetAngularVelocity(bool left)
    {
        return GetAngularVelocity(left ? XRNode.LeftHand : XRNode.RightHand);
    }

    public static Vector3 GetAngularVelocity(XRNode node)
    {
        for (int i = 0; i < curNodeStates.Count; i++)
        {
            if (curNodeStates[i].tracked && curNodeStates[i].nodeType == node)
            {
                Vector3 av;
                if (curNodeStates[i].TryGetAngularVelocity(out av))
                {
                    return av;
                }
            }
        }
        return Vector3.zero;
    }


    public static bool IsTracked(bool left)
    {
        return IsTracked(left ? XRNode.LeftHand : XRNode.RightHand);
    }

    public static bool IsTracked(XRNode node)
    {
        return curNodeStates.Any(xr => xr.nodeType == node && xr.tracked);
    }

    /// <summary>
    /// NOT IMPLEMENTED
    /// </summary>
    /// <param name="left"></param>
    /// <param name="intensity01"></param>
    public static void SetVibration(bool left, float intensity01)
    {
        // perhaps implement through steamVR, oculus and MR separately.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get inputs specifically for Vive. It is recommended to use InputVR.Get() methods instead.
    /// </summary>
    public static class Vive
    {
        private static bool isOculusInSteamVR
        {
            get
            {
                return
#if !UNITY_WSA
                SteamVR.instance.hmd_TrackingSystemName.Contains("oculus")
                ||
#endif
                    false
                ;
            }
        }

        public static float GetTrigger(bool left)
        {
            return Input.GetAxis(left ? InputVRConst.TriggerLeft : InputVRConst.TriggerRight);
        }

        public static float GetGrip(bool left)
        {
            return Input.GetAxis(left ? InputVRConst.GripLeft : InputVRConst.GripRight);
        }

        public static Vector2 GetTouchpad(bool left)
        {
            return new Vector2(
                Input.GetAxis(left ? InputVRConst.ViveTouchpadLeftX : InputVRConst.ViveTouchpadRightX),
                Input.GetAxis(left ? InputVRConst.ViveTouchpadLeftY : InputVRConst.ViveTouchpadRightY)
                );
        }

        public static Vector2 GetAxis(bool left, ViveButtonMask viveButtonMask)
        {
            switch (viveButtonMask)
            {
            case ViveButtonMask.Axis1:
            case ViveButtonMask.Trigger:
                return new Vector2(Vive.GetTrigger(left), 0);

            case ViveButtonMask.Grip:
                return new Vector2(Vive.GetGrip(left), 0);

            case ViveButtonMask.Axis0:
            case ViveButtonMask.Touchpad:
                return Vive.GetTouchpad(left);

            default:
                return Vector2.zero;
            }
        }

        public static bool GetTouch(bool left, ViveButtonMask viveButtonMask)
        {
            switch (viveButtonMask)
            {
            case ViveButtonMask.Axis1:
            case ViveButtonMask.Trigger:
                return Input.GetButton(left ? InputVRConst.TriggerLeftTouch : InputVRConst.TriggerRightTouch);

            case ViveButtonMask.Grip:
                return GetGrip(left) > 0;

            case ViveButtonMask.Axis0:
            case ViveButtonMask.Touchpad:
                return Input.GetButton(left ? InputVRConst.ViveTouchpadLeftTouch : InputVRConst.ViveTouchpadRightTouch);

            case ViveButtonMask.ApplicationMenu:
                return Input.GetButton(left ? InputVRConst.ViveApplicationMenuLeft : InputVRConst.ViveApplicationMenuRight);

            case ViveButtonMask.Axis2:
            case ViveButtonMask.Axis3:
            case ViveButtonMask.Axis4:
            // use steamvr? ifk.
            default:
                break;
            }
            return false;
        }

        public static bool GetTouchDown(bool left, ViveButtonMask viveButtonMask)
        {
            var button = (ButtonMask)viveButtonMask;
            switch (viveButtonMask)
            {
            case ViveButtonMask.Axis1:
            case ViveButtonMask.Trigger:
                return Input.GetButtonDown(left ? InputVRConst.TriggerLeftTouch : InputVRConst.TriggerRightTouch);

            case ViveButtonMask.Grip:
                // there is no Down/Up button for this one, so we have to use the axis.
                var prevValue = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
                return GetGrip(left) > 0 && prevValue.x == 0;

            case ViveButtonMask.Axis0:
            case ViveButtonMask.Touchpad:
                return Input.GetButtonDown(left ? InputVRConst.ViveTouchpadLeftTouch : InputVRConst.ViveTouchpadRightTouch);

            case ViveButtonMask.ApplicationMenu:
                return Input.GetButtonDown(left ? InputVRConst.ViveApplicationMenuLeft : InputVRConst.ViveApplicationMenuRight);

            case ViveButtonMask.Axis2:
            case ViveButtonMask.Axis3:
            case ViveButtonMask.Axis4:
            // use steamvr? ifk.
            default:
                break;
            }
            return false;
        }

        public static bool GetTouchUp(bool left, ViveButtonMask viveButtonMask)
        {
            var button = (ButtonMask)viveButtonMask;
            switch (viveButtonMask)
            {
            case ViveButtonMask.Axis1:
            case ViveButtonMask.Trigger:
                return Input.GetButtonUp(left ? InputVRConst.TriggerLeftTouch : InputVRConst.TriggerRightTouch);

            case ViveButtonMask.Grip:
                // there is no Down/Up button for this one, so we have to use the axis.
                var prevValue = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
                return GetGrip(left) == 0 && prevValue.x > 0;

            case ViveButtonMask.Axis0:
            case ViveButtonMask.Touchpad:
                return Input.GetButtonUp(left ? InputVRConst.ViveTouchpadLeftTouch : InputVRConst.ViveTouchpadRightTouch);

            case ViveButtonMask.ApplicationMenu:
                return Input.GetButtonUp(left ? InputVRConst.ViveApplicationMenuLeft : InputVRConst.ViveApplicationMenuRight);

            case ViveButtonMask.Axis2:
            case ViveButtonMask.Axis3:
            case ViveButtonMask.Axis4:
            // use steamvr? ifk.
            default:
                break;
            }
            return false;
        }

        public static bool GetPress(bool left, ViveButtonMask buttonMask)
        {
            switch (buttonMask)
            {
            case ViveButtonMask.Axis1:
            case ViveButtonMask.Trigger:
                // this only because oculus in steamVR is fucking stupid
                if (isOculusInSteamVR)
                {
                    return Input.GetAxis(left ? InputVRConst.TriggerLeft : InputVRConst.TriggerRight) > 0.5f;
                }
                else
                {
                    return Input.GetAxis(left ? InputVRConst.TriggerLeft : InputVRConst.TriggerRight) == 1f;
                }
            case ViveButtonMask.Grip:
                return GetGrip(left) == 1;

            case ViveButtonMask.Axis0:
            case ViveButtonMask.Touchpad:
                return Input.GetButton(left ? InputVRConst.ViveTouchpadLeftPress : InputVRConst.ViveTouchpadRightPress);

            case ViveButtonMask.ApplicationMenu:
                return Input.GetButton(left ? InputVRConst.ViveApplicationMenuLeft : InputVRConst.ViveApplicationMenuRight);

            case ViveButtonMask.Axis2:
            case ViveButtonMask.Axis3:
            case ViveButtonMask.Axis4:
            // use steamvr? ifk.
            default:
                break;
            }
            return false;
        }

        public static bool GetPressDown(bool left, ViveButtonMask viveButtonMask)
        {
            var button = (ButtonMask)viveButtonMask;
            switch (viveButtonMask)
            {
            case ViveButtonMask.Axis1:
            case ViveButtonMask.Trigger:
                // press down is when the prev frame is NOT 1, but the current is 1.
                var prevFrame = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
                if (isOculusInSteamVR)
                {
                    return Input.GetAxis(left ? InputVRConst.TriggerLeft : InputVRConst.TriggerRight) > 0.5f && prevFrame.x <= 0.5f;
                }
                else
                {
                    return Input.GetAxis(left ? InputVRConst.TriggerLeft : InputVRConst.TriggerRight) == 1f && prevFrame.x != 1f;
                }
            case ViveButtonMask.Grip:
                // there is no Down/Up button for this one, so we have to use the axis.
                var prevValue = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
                return GetGrip(left) == 1f && prevValue.x != 1f;

            case ViveButtonMask.Axis0:
            case ViveButtonMask.Touchpad:
                return Input.GetButtonDown(left ? InputVRConst.ViveTouchpadLeftPress : InputVRConst.ViveTouchpadRightPress);

            case ViveButtonMask.ApplicationMenu:
                return Input.GetButtonDown(left ? InputVRConst.ViveApplicationMenuLeft : InputVRConst.ViveApplicationMenuRight);

            case ViveButtonMask.Axis2:
            case ViveButtonMask.Axis3:
            case ViveButtonMask.Axis4:
            // use steamvr? ifk.
            default:
                break;
            }
            return false;
        }

        public static bool GetPressUp(bool left, ViveButtonMask viveButtonMask)
        {
            var button = (ButtonMask)viveButtonMask;
            switch (viveButtonMask)
            {
            case ViveButtonMask.Axis1:
            case ViveButtonMask.Trigger:
                // press up is when the prev frame is 1, but the current is not 1.
                var prevFrame = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
                if (isOculusInSteamVR)
                {
                    return Input.GetAxis(left ? InputVRConst.TriggerLeft : InputVRConst.TriggerRight) <= 0.5f && prevFrame.x > 0.5f;
                }
                else
                {
                    return Input.GetAxis(left ? InputVRConst.TriggerLeft : InputVRConst.TriggerRight) != 1f && prevFrame.x == 1f;
                }
            case ViveButtonMask.Grip:
                // there is no Down/Up button for this one, so we have to use the axis.
                var prevValue = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
                return GetGrip(left) != 1f && prevValue.x == 1f;

            case ViveButtonMask.Axis0:
            case ViveButtonMask.Touchpad:
                return Input.GetButtonUp(left ? InputVRConst.ViveTouchpadLeftPress : InputVRConst.ViveTouchpadRightPress);

            case ViveButtonMask.ApplicationMenu:
                return Input.GetButtonUp(left ? InputVRConst.ViveApplicationMenuLeft : InputVRConst.ViveApplicationMenuRight);

            case ViveButtonMask.Axis2:
            case ViveButtonMask.Axis3:
            case ViveButtonMask.Axis4:
            // use steamvr? ifk.
            default:
                break;
            }
            return false;
        }

    }

    /// <summary>
    /// Get inputs specifically for Oculus. It is recommended to use InputVR.Get() methods instead.
    /// </summary>
    public static class Oculus
    {
#pragma warning disable 0162
        public static float GetTrigger(bool left)
        {
            return Input.GetAxis(left ? InputVRConst.TriggerLeft : InputVRConst.TriggerRight);
        }

        public static float GetGrip(bool left)
        {
            return Input.GetAxis(left ? InputVRConst.GripLeft : InputVRConst.GripRight);
        }

        public static Vector2 GetAnalogStick(bool left)
        {
            return new Vector2(
                Input.GetAxis(left ? InputVRConst.AnalogStickLeftX : InputVRConst.AnalogStickRightX),
                Input.GetAxis(left ? InputVRConst.AnalogStickLeftY : InputVRConst.AnalogStickRightY)
                );
        }

        public static Vector2 GetAxis(bool left, OculusButtonMask oculusButtonMask)
        {
            switch (oculusButtonMask)
            {
            case OculusButtonMask.Trigger:
                return new Vector2(GetTrigger(left), 0);

            case OculusButtonMask.Grip:
                return new Vector2(GetGrip(left), 0);

            case OculusButtonMask.AnalogStick:
                return GetAnalogStick(left);

            default:
                return Vector2.zero;
            }
        }

        public static bool GetTouch(bool left, OculusButtonMask oculusButtonMask)
        {
            switch (oculusButtonMask)
            {
            case OculusButtonMask.Trigger:
#if OCULUS
                return OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
#endif
                return Input.GetButton(left ? InputVRConst.TriggerLeftTouch : InputVRConst.TriggerRightTouch);

            case OculusButtonMask.Grip:
                return GetGrip(left) > 0;

            case OculusButtonMask.AnalogStick:
                return Input.GetButton(left ? InputVRConst.OculusAnalogStickLeftTouch : InputVRConst.OculusAnalogStickRightTouch);

            case OculusButtonMask.ApplicationMenu:
                return left ? Input.GetButton(InputVRConst.OculusApplicationMenuLeft) : false;

            case OculusButtonMask.OculusA:
                return Input.GetButton(left ? InputVRConst.OculusXTouch : InputVRConst.OculusATouch);

            case OculusButtonMask.OculusB:
                return Input.GetButton(left ? InputVRConst.OculusYTouch : InputVRConst.OculusBTouch);

            default:
                break;
            }
            return false;
        }

        public static bool GetTouchDown(bool left, OculusButtonMask oculusButtonMask)
        {
            var button = (ButtonMask)oculusButtonMask;
            switch (oculusButtonMask)
            {
            case OculusButtonMask.Trigger:
#if OCULUS
                return OVRInput.GetDown(OVRInput.Touch.PrimaryIndexTrigger, left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
#endif
                return Input.GetButtonDown(left ? InputVRConst.TriggerLeftTouch : InputVRConst.TriggerRightTouch);

            case OculusButtonMask.Grip:
                // there is no Down/Up button for this one, so we have to use the axis.
                var prevValue = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
                return GetGrip(left) > 0 && prevValue.x == 0;

            case OculusButtonMask.AnalogStick:
                return Input.GetButtonDown(left ? InputVRConst.OculusAnalogStickLeftTouch : InputVRConst.OculusAnalogStickRightTouch);

            case OculusButtonMask.ApplicationMenu:
                return left ? Input.GetButtonDown(InputVRConst.OculusApplicationMenuLeft) : false;

            case OculusButtonMask.OculusA:
                return Input.GetButtonDown(left ? InputVRConst.OculusXTouch : InputVRConst.OculusATouch);

            case OculusButtonMask.OculusB:
                return Input.GetButtonDown(left ? InputVRConst.OculusYTouch : InputVRConst.OculusBTouch);

            default:
                break;
            }
            return false;
        }

        public static bool GetTouchUp(bool left, OculusButtonMask oculusButtonMask)
        {
            var button = (ButtonMask)oculusButtonMask;
            switch (oculusButtonMask)
            {
            case OculusButtonMask.Trigger:
#if OCULUS
                return OVRInput.GetUp(OVRInput.Touch.PrimaryIndexTrigger, left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
#endif
                return Input.GetButtonUp(left ? InputVRConst.TriggerLeftTouch : InputVRConst.TriggerRightTouch);

            case OculusButtonMask.Grip:
                // there is no Down/Up button for this one, so we have to use the axis.
                var prevValue = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
                return GetGrip(left) == 0 && prevValue.x > 0;

            case OculusButtonMask.AnalogStick:
                return Input.GetButtonUp(left ? InputVRConst.OculusAnalogStickLeftTouch : InputVRConst.OculusAnalogStickRightTouch);

            case OculusButtonMask.ApplicationMenu:
                return left ? Input.GetButtonUp(InputVRConst.OculusApplicationMenuLeft) : false;

            case OculusButtonMask.OculusA:
                return Input.GetButtonUp(left ? InputVRConst.OculusXTouch : InputVRConst.OculusATouch);

            case OculusButtonMask.OculusB:
                return Input.GetButtonUp(left ? InputVRConst.OculusYTouch : InputVRConst.OculusBTouch);

            default:
                break;
            }
            return false;
        }

        public static bool GetPress(bool left, OculusButtonMask oculusButtonMask)
        {
            switch (oculusButtonMask)
            {
            case OculusButtonMask.Trigger:
#if OCULUS
                return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
#endif
                return Input.GetAxis(left ? InputVRConst.TriggerLeft : InputVRConst.TriggerRight) == 1f;

            case OculusButtonMask.Grip:
                return GetGrip(left) == 1;

            case OculusButtonMask.AnalogStick:
                return Input.GetButton(left ? InputVRConst.OculusAnalogStickLeftPress : InputVRConst.OculusAnalogStickRightPress);

            case OculusButtonMask.ApplicationMenu:
                return left ? Input.GetButton(InputVRConst.OculusApplicationMenuLeft) : false;

            case OculusButtonMask.OculusA:
                return Input.GetButton(left ? InputVRConst.OculusXPress : InputVRConst.OculusAPress);

            case OculusButtonMask.OculusB:
                return Input.GetButton(left ? InputVRConst.OculusYPress : InputVRConst.OculusBPress);

            default:
                break;
            }
            return false;
        }

        public static bool GetPressDown(bool left, OculusButtonMask oculusButtonMask)
        {
            var button = (ButtonMask)oculusButtonMask;
            switch (oculusButtonMask)
            {
            case OculusButtonMask.Trigger:
                // press down is when the prev frame is NOT 1, but the current is 1.
                var prevFrame = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
#if OCULUS
                return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
#endif
                return Input.GetAxis(left ? InputVRConst.TriggerLeft : InputVRConst.TriggerRight) == 1f && prevFrame.x != 1f;

            case OculusButtonMask.Grip:
                // there is no Down/Up button for this one, so we have to use the axis.
                var prevValue = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
                return GetGrip(left) == 1f && prevValue.x != 1f;

            case OculusButtonMask.AnalogStick:
                return Input.GetButtonDown(left ? InputVRConst.OculusAnalogStickLeftPress : InputVRConst.OculusAnalogStickRightPress);

            case OculusButtonMask.ApplicationMenu:
                return left ? Input.GetButtonDown(InputVRConst.OculusApplicationMenuLeft) : false;

            case OculusButtonMask.OculusA:
                return Input.GetButtonDown(left ? InputVRConst.OculusXPress : InputVRConst.OculusAPress);

            case OculusButtonMask.OculusB:
                return Input.GetButtonDown(left ? InputVRConst.OculusYPress : InputVRConst.OculusBPress);

            default:
                break;
            }
            return false;
        }

        public static bool GetPressUp(bool left, OculusButtonMask oculusButtonMask)
        {
            var button = (ButtonMask)oculusButtonMask;
            switch (oculusButtonMask)
            {
            case OculusButtonMask.Trigger:
                // press up is when the prev frame is 1, but the current is not 1.
                var prevFrame = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
#if OCULUS
                return OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
#endif
                return Input.GetAxis(left ? InputVRConst.TriggerLeft : InputVRConst.TriggerRight) != 1f && prevFrame.x == 1f;

            case OculusButtonMask.Grip:
                // there is no Down/Up button for this one, so we have to use the axis.
                var prevValue = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
                return GetGrip(left) != 1f && prevValue.x == 1f;

            case OculusButtonMask.AnalogStick:
                return Input.GetButtonUp(left ? InputVRConst.OculusAnalogStickLeftPress : InputVRConst.OculusAnalogStickRightPress);

            case OculusButtonMask.ApplicationMenu:
                return left ? Input.GetButtonUp(InputVRConst.OculusApplicationMenuLeft) : false;

            case OculusButtonMask.OculusA:
                return Input.GetButtonUp(left ? InputVRConst.OculusXPress : InputVRConst.OculusAPress);

            case OculusButtonMask.OculusB:
                return Input.GetButtonUp(left ? InputVRConst.OculusYPress : InputVRConst.OculusBPress);

            default:
                break;
            }
            return false;
        }

#pragma warning restore 0162
    }

    /// <summary>
    /// Get inputs specifically for Microsoft MR devices. It is recommended to use InputVR.Get() methods instead.
    /// </summary>
    public static class Windows
    {
        public static float GetTrigger(bool left)
        {
            return Input.GetAxis(left ? InputVRConst.TriggerLeft : InputVRConst.TriggerRight);
        }

        public static float GetGrip(bool left)
        {
            return Input.GetAxis(left ? InputVRConst.GripLeft : InputVRConst.GripRight);
        }

        public static Vector2 GetTouchpad(bool left)
        {
            return new Vector2(
                Input.GetAxis(left ? InputVRConst.WindowsTouchpadLeftX : InputVRConst.WindowsTouchpadRightX),
                Input.GetAxis(left ? InputVRConst.WindowsTouchpadLeftY : InputVRConst.WindowsTouchpadRightY)
                );
        }

        public static Vector2 GetAnalogStick(bool left)
        {
            return new Vector2(
                Input.GetAxis(left ? InputVRConst.AnalogStickLeftX : InputVRConst.AnalogStickRightX),
                Input.GetAxis(left ? InputVRConst.AnalogStickLeftY : InputVRConst.AnalogStickRightY)
                );
        }

        public static Vector2 GetAxis(bool left, WindowsButtonMask windowsButtonMask)
        {
            switch (windowsButtonMask)
            {
            case WindowsButtonMask.Trigger:
                return new Vector2(GetTrigger(left), 0);

            case WindowsButtonMask.Grip:
                return new Vector2(GetGrip(left), 0);

            case WindowsButtonMask.AnalogStick:
                return GetAnalogStick(left);

            case WindowsButtonMask.Touchpad:
                return GetTouchpad(left);

            default:
                return Vector2.zero;
            }
        }

        public static bool GetTouch(bool left, WindowsButtonMask windowsButtonMask)
        {
            switch (windowsButtonMask)
            {
            case WindowsButtonMask.Trigger:
                return Input.GetButton(left ? InputVRConst.TriggerLeftTouch : InputVRConst.TriggerRightTouch);

            case WindowsButtonMask.Grip:
                return GetGrip(left) > 0;

            case WindowsButtonMask.Touchpad:
                return Input.GetButton(left ? InputVRConst.WindowsTouchpadLeftTouch : InputVRConst.WindowsTouchpadRightTouch);

            case WindowsButtonMask.AnalogStick:
                return Input.GetButton(left ? InputVRConst.WindowsAnalogStickLeftPress : InputVRConst.WindowsAnalogStickRightPress);

            case WindowsButtonMask.ApplicationMenu:
                return Input.GetButton(left ? InputVRConst.WindowsApplicationMenuLeft : InputVRConst.WindowsApplicationMenuRight);

            default:
                break;
            }
            return false;
        }

        public static bool GetTouchDown(bool left, WindowsButtonMask windowsButtonMask)
        {
            var button = (ButtonMask)windowsButtonMask;
            switch (windowsButtonMask)
            {
            case WindowsButtonMask.Trigger:
                return Input.GetButtonDown(left ? InputVRConst.TriggerLeftTouch : InputVRConst.TriggerRightTouch);

            case WindowsButtonMask.Grip:
                // there is no Down/Up button for this one, so we have to use the axis.
                var prevValue = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
                return GetGrip(left) > 0 && prevValue.x == 0;

            case WindowsButtonMask.Touchpad:
                return Input.GetButtonDown(left ? InputVRConst.WindowsTouchpadLeftTouch : InputVRConst.WindowsTouchpadRightTouch);

            case WindowsButtonMask.AnalogStick:
                return Input.GetButtonDown(left ? InputVRConst.WindowsAnalogStickLeftPress : InputVRConst.WindowsAnalogStickRightPress);

            case WindowsButtonMask.ApplicationMenu:
                return Input.GetButtonDown(left ? InputVRConst.WindowsApplicationMenuLeft : InputVRConst.WindowsApplicationMenuRight);

            default:
                break;
            }
            return false;
        }

        public static bool GetTouchUp(bool left, WindowsButtonMask windowsButtonMask)
        {
            var button = (ButtonMask)windowsButtonMask;
            switch (windowsButtonMask)
            {
            case WindowsButtonMask.Trigger:
                return Input.GetButtonUp(left ? InputVRConst.TriggerLeftTouch : InputVRConst.TriggerRightTouch);

            case WindowsButtonMask.Grip:
                // there is no Down/Up button for this one, so we have to use the axis.
                var prevValue = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
                return GetGrip(left) == 0 && prevValue.x > 0;

            case WindowsButtonMask.Touchpad:
                return Input.GetButtonUp(left ? InputVRConst.WindowsTouchpadLeftTouch : InputVRConst.WindowsTouchpadRightTouch);

            case WindowsButtonMask.AnalogStick:
                return Input.GetButtonUp(left ? InputVRConst.WindowsAnalogStickLeftPress : InputVRConst.WindowsAnalogStickRightPress);

            case WindowsButtonMask.ApplicationMenu:
                return Input.GetButtonUp(left ? InputVRConst.WindowsApplicationMenuLeft : InputVRConst.WindowsApplicationMenuRight);

            default:
                break;
            }
            return false;
        }

        public static bool GetPress(bool left, WindowsButtonMask windowsButtonMask)
        {
            switch (windowsButtonMask)
            {
            case WindowsButtonMask.Trigger:
                return Input.GetAxis(left ? InputVRConst.TriggerLeft : InputVRConst.TriggerRight) == 1f;

            case WindowsButtonMask.Grip:
                return GetGrip(left) == 1;

            case WindowsButtonMask.Touchpad:
                return Input.GetButton(left ? InputVRConst.WindowsTouchpadLeftPress : InputVRConst.WindowsTouchpadRightPress);

            case WindowsButtonMask.AnalogStick:
                return Input.GetButton(left ? InputVRConst.WindowsAnalogStickLeftPress : InputVRConst.WindowsAnalogStickRightPress);

            case WindowsButtonMask.ApplicationMenu:
                return Input.GetButton(left ? InputVRConst.WindowsApplicationMenuLeft : InputVRConst.WindowsApplicationMenuRight);

            default:
                break;
            }
            return false;
        }

        public static bool GetPressDown(bool left, WindowsButtonMask windowsButtonMask)
        {
            var button = (ButtonMask)windowsButtonMask;
            switch (windowsButtonMask)
            {
            case WindowsButtonMask.Trigger:

                // press down is when the prev frame is NOT 1, but the current is 1.
                var prevFrame = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
                var curAxis = Input.GetAxis(left ? InputVRConst.TriggerLeft : InputVRConst.TriggerRight);
                return curAxis == 1f && prevFrame.x != 1f;

            case WindowsButtonMask.Grip:
                // there is no Down/Up button for this one, so we have to use the axis.
                var prevValue = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
                return GetGrip(left) == 1f && prevValue.x != 1f;

            case WindowsButtonMask.Touchpad:
                return Input.GetButtonDown(left ? InputVRConst.WindowsTouchpadLeftPress : InputVRConst.WindowsTouchpadRightPress);

            case WindowsButtonMask.AnalogStick:
                return Input.GetButtonDown(left ? InputVRConst.WindowsAnalogStickLeftPress : InputVRConst.WindowsAnalogStickRightPress);

            case WindowsButtonMask.ApplicationMenu:
                return Input.GetButtonDown(left ? InputVRConst.WindowsApplicationMenuLeft : InputVRConst.WindowsApplicationMenuRight);

            default:
                break;
            }
            return false;
        }

        public static bool GetPressUp(bool left, WindowsButtonMask windowsButtonMask)
        {
            var button = (ButtonMask)windowsButtonMask;
            switch (windowsButtonMask)
            {
            case WindowsButtonMask.Trigger:
                // press up is when the prev frame is 1, but the current is not 1.
                var prevFrame = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
                return Input.GetAxis(left ? InputVRConst.TriggerLeft : InputVRConst.TriggerRight) != 1f && prevFrame.x == 1f;

            case WindowsButtonMask.Grip:
                // there is no Down/Up button for this one, so we have to use the axis.
                var prevValue = left ? prevAxisValuesLeft[button] : prevAxisValuesRight[button];
                return GetGrip(left) != 1f && prevValue.x == 1f;

            case WindowsButtonMask.Touchpad:
                return Input.GetButtonUp(left ? InputVRConst.WindowsTouchpadLeftPress : InputVRConst.WindowsTouchpadRightPress);

            case WindowsButtonMask.AnalogStick:
                return Input.GetButtonUp(left ? InputVRConst.WindowsAnalogStickLeftPress : InputVRConst.WindowsAnalogStickRightPress);

            case WindowsButtonMask.ApplicationMenu:
                return Input.GetButtonUp(left ? InputVRConst.WindowsApplicationMenuLeft : InputVRConst.WindowsApplicationMenuRight);

            default:
                break;
            }
            return false;
        }

    }


    #region Private Constants and ButtonMask enums

    /// <summary>
    /// Full, cross-platform button mask. Some buttons unavailable and potentially remapped due to devices being different.
    /// </summary>
    private static Array _buttonMaskEnumValues;

    private static Array buttonMaskEnumValues
    {
        get
        {
            if (_buttonMaskEnumValues == null)
            {
                _buttonMaskEnumValues = Enum.GetValues(typeof(ButtonMask));
            }
            return _buttonMaskEnumValues;
        }
    }

    /// <summary>
    /// Cross platform ButtonMask, representing all buttons of all platforms, even though perhaps not supported by some devices. In which case the GetPress() and GetAxis() return default
    /// </summary>
    public enum ButtonMask
    {
        Trigger = 1,
        Grip = 2,
        // only on vive and windows. oculus can default to AnalogStick
        Touchpad = 3,
        // only on oculus and windows. vive can default to Touchpad
        AnalogStick = 4,

        ApplicationMenu = 5,

        // SteamVR axes for Vive Knuckles sep. fingers....
        Axis0 = 6,
        Axis1 = 7,
        Axis2 = 8,
        Axis3 = 9,
        Axis4 = 10,

        OculusA = 11,       // THESE VALUES ARE MIRRORED FOR THE OCULUSX AND Y FOR THE LEFT/RIGHT HAND.
        OculusB = 12,       // THESE VALUES ARE MIRRORED FOR THE OCULUSX AND Y FOR THE LEFT/RIGHT HAND.
    }

    /// <summary>
    /// Enum meant to limit the buttons available on the Vive. It always has the same int values as the full <see cref="ButtonMask"/> except unavailable buttons
    /// </summary>
    public enum ViveButtonMask
    {
        Trigger = 1,
        Grip = 2,
        Touchpad = 3,
        //AnalogStick = 4,
        ApplicationMenu = 5,
        Axis0 = 6, // because SteamVR and knuckles controllers
        Axis1 = 7,
        Axis2 = 8,
        Axis3 = 9,
        Axis4 = 10,
        //OculusA = 11,
        //OculusB = 12,
        //OculusX = 13,
        //OculusY = 14,
    }

    /// <summary>
    /// Enum meant to limit the buttons available on the Oculus. It always has the same int values as the full <see cref="ButtonMask"/> except unavailable buttons
    /// </summary>
    public enum OculusButtonMask
    {
        Trigger = 1,
        Grip = 2,
        //Touchpad = 3,
        AnalogStick = 4,
        ApplicationMenu = 5,
        //Axis0 = 6,
        //Axis1 = 7,
        //Axis2 = 8,
        //Axis3 = 9,
        //Axis4 = 10,
        OculusA = 11,
        OculusB = 12,
    }

    /// <summary>
    /// Enum meant to limit the buttons available on the WindowsMR. It always has the same int values as the full <see cref="ButtonMask"/> except unavailable buttons
    /// </summary>
    public enum WindowsButtonMask
    {
        Trigger = 1,
        Grip = 2,
        Touchpad = 3,
        AnalogStick = 4,
        ApplicationMenu = 5,
        //Axis0 = 6,
        //Axis1 = 7,
        //Axis2 = 8,
        //Axis3 = 9,
        //Axis4 = 10,
        //OculusA = 11,
        //OculusB = 12,
        //OculusX = 13,
        //OculusY = 14,
    }

    public enum VRControllerType
    {
        None = -1,
        SteamVR,
        Oculus,
        Windows,
        // PSVR,
        // GOOGLEPLEASEMAKEAMOBILEVRWITHPOSITIONALTRACKING,
    }

    // this is what valve's SteamVR buttons look like
    //public enum EVRButtonId
    //{
    //    k_EButton_System = 0,
    //    k_EButton_ApplicationMenu = 1,
    //    k_EButton_Grip = 2,
    //    k_EButton_DPad_Left = 3,
    //    k_EButton_DPad_Up = 4,
    //    k_EButton_DPad_Right = 5,
    //    k_EButton_DPad_Down = 6,
    //    k_EButton_A = 7,
    //    k_EButton_ProximitySensor = 31,
    //    k_EButton_Axis0 = 32,
    //    k_EButton_Axis1 = 33,
    //    k_EButton_Axis2 = 34,
    //    k_EButton_Axis3 = 35,
    //    k_EButton_Axis4 = 36,
    //    k_EButton_SteamVR_Touchpad = 32,
    //    k_EButton_SteamVR_Trigger = 33,
    //    k_EButton_Dashboard_Back = 2,
    //    k_EButton_Max = 64,
    //}


    #endregion
}

/// <summary>
/// Holds constants for the Input axes and buttons used in the InputVR system. Used by editor for generating axes as well as by InputVR for using them.
/// Should not be referenced by user code except for extreme cases. Better alternative than using those directly, is extending InputVR with the new func in a more generic robust way.
/// </summary>
public static class InputVRConst
{
    // axes
    public const string GripLeft = "Grip Left";
    public const string GripRight = "Grip Right";

    // all triggers
    public const string TriggerLeft = "Trigger Left";
    public const string TriggerRight = "Trigger Right";

    // vive touchpad
    public const string ViveTouchpadLeftX = "ViveTouchpad Left X";
    public const string ViveTouchpadLeftY = "ViveTouchpad Left Y";
    public const string ViveTouchpadRightX = "ViveTouchpad Right X";
    public const string ViveTouchpadRightY = "ViveTouchpad Right Y";

    // oculus, MR and xbox controller
    public const string AnalogStickLeftX = "AnalogStick Left X";
    public const string AnalogStickLeftY = "AnalogStick Left Y";
    public const string AnalogStickRightX = "AnalogStick Right X";
    public const string AnalogStickRightY = "AnalogStick Right Y";

    // windows touchpad
    public const string WindowsTouchpadLeftX = "WindowsTouchpad Left X";
    public const string WindowsTouchpadLeftY = "WindowsTouchpad Left Y";
    public const string WindowsTouchpadRightX = "WindowsTouchpad Right X";
    public const string WindowsTouchpadRightY = "WindowsTouchpad Right Y";

    // oculus remote (lol)
    public const string OculusRemoteTouchpadX = "OculusRemoteTouchpad X";
    public const string OculusRemoteTouchpadY = "OculusRemoteTouchpad Y";

    // buttons
    public const string TriggerLeftTouch = "Trigger Left Touch";
    public const string TriggerRightTouch = "Trigger Right Touch";

    // vive
    public const string ViveTouchpadLeftTouch = "ViveTouchpad Left Touch";
    public const string ViveTouchpadRightTouch = "ViveTouchpad Right Touch";
    public const string ViveTouchpadLeftPress = "ViveTouchpad Left Press";
    public const string ViveTouchpadRightPress = "ViveTouchpad Right Press";

    // windows touchpad
    public const string WindowsTouchpadLeftPress = "WindowsTouchpad Left Press";
    public const string WindowsTouchpadRightPress = "WindowsTouchpad Right Press";
    public const string WindowsTouchpadLeftTouch = "WindowsTouchpad Left Touch";
    public const string WindowsTouchpadRightTouch = "WindowsTouchpad Right Touch";

    // windows anal stick (no touch, just press)
    public const string WindowsAnalogStickLeftPress = "WindowsAnalogStick Left Press";
    public const string WindowsAnalogStickRightPress = "WindowsAnalogStick Right Press";

    // oculus anal stick
    public const string OculusAnalogStickLeftTouch = "OculusAnalogStick Left Touch";
    public const string OculusAnalogStickRightTouch = "OculusAnalogStick Right Touch";
    public const string OculusAnalogStickLeftPress = "OculusAnalogStick Left Press";
    public const string OculusAnalogStickRightPress = "OculusAnalogStick Right Press";

    // application menus for all
    public const string WindowsApplicationMenuLeft = "WindowsApplicationMenu Left";
    public const string WindowsApplicationMenuRight = "WindowsApplicationMenu Right";
    public const string OculusApplicationMenuLeft = "OculusApplicationMenu Left";
    public const string ViveApplicationMenuLeft = "ViveApplicationMenu Left";
    public const string ViveApplicationMenuRight = "ViveApplicationMenu Right";

    // oculus A, B, X, Y buttons
    public const string OculusAPress = "Oculus A Press";
    public const string OculusBPress = "Oculus B Press";
    public const string OculusXPress = "Oculus X Press";
    public const string OculusYPress = "Oculus Y Press";
    public const string OculusATouch = "Oculus A Touch";
    public const string OculusBTouch = "Oculus B Touch";
    public const string OculusXTouch = "Oculus X Touch";
    public const string OculusYTouch = "Oculus Y Touch";

}
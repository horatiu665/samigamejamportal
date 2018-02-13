using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class OculusPauseDetect : MonoBehaviour
{
    private static OculusPauseDetect _instance;
    public static OculusPauseDetect instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<OculusPauseDetect>();
            }
            return _instance;
        }
    }

    public bool isPaused { get; private set; }

    public bool showDebug = false;

    private void OnEnable()
    {
        SetPause(false);
    }

    private void OnDisable()
    {
        SetPause(false);
    }

    private void Update()
    {
#if OCULUS
        if (OVRManager.hasInputFocus)
        {
            SetPause(false);
        }
        else
        {
            SetPause(true);
        }
#endif
    }

    public void SetPause(bool pause)
    {
        if (this.isPaused != pause)
        {
            this.isPaused = pause;
            if (showDebug)
            {
                Debug.Log("<color=#00ff00>Setting pause " + pause + "</color>");
            }
            Time.timeScale = pause ? 0 : 1;

            // kill all inputs if paused
            //ControllerWrapper.pauseAllInput = pause;

            //for (int i = 0; i < ControllerWrapper.allControllers.Count; i++)
            //{
            //    ControllerWrapper.allControllers[i].SetControllerGraphics(this, !pause);
            //}
        }
    }

}
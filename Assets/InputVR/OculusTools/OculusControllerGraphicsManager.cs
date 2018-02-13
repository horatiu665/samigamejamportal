using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusControllerGraphicsManager : MonoBehaviour
{
    private static OculusControllerGraphicsManager _instance;
    public static OculusControllerGraphicsManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<OculusControllerGraphicsManager>();
            }
            return _instance;
        }
    }

#if OCULUS
    [SerializeField]
    private OvrAvatar _ovrAvatar;
    public OvrAvatar ovrAvatar
    {
        get
        {
            if (_ovrAvatar == null)
            {
                _ovrAvatar = GetComponentInChildren<OvrAvatar>();
            }
            return _ovrAvatar;
        }
    }

    public void SetControllersVisible(bool isLeft, bool visible)
    {
        if (isLeft)
        {
            ovrAvatar.ShowLeftController(visible);
        }
        else
        {
            ovrAvatar.ShowRightController(visible);
        }
    }
#endif

}

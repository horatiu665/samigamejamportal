using System;
using UnityEngine;

public class InputVR_BeforeUpdate : MonoBehaviour
{
    public event Action OnUpdateBeforeRegular;

    // Happens BEFORE regular Update() ♥
    /// <summary>
    /// Updates InputNodes so they are ready for use ahead of time
    /// </summary>
    private void Update()
    {
        if (OnUpdateBeforeRegular != null)
        {
            OnUpdateBeforeRegular();
        }
    }
}

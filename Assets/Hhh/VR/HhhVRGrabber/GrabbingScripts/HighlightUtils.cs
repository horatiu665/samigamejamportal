namespace HhhVRGrabber
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    public static class HighlightUtils
    {
        private static Material _highlightMat;
        public static Material highlightMat
        {
            get
            {
                if (_highlightMat == null)
                {
                    _highlightMat = Resources.Load<Material>("GrabHighlightMat");
                }
                return _highlightMat;
            }
        }

        public static void Highlight(GrabbableBase obj, HighlightMethod highlightMethod)
        {
            if (highlightMethod == HighlightMethod.AddSecondMaterial)
            {
                foreach (var r in obj.highlightRenderers)
                {
                    if (r.sharedMaterials.Length == 1)
                    {
                        r.sharedMaterials = new Material[]
                        {
                            r.sharedMaterials[0],
                            highlightMat,
                        };
                    }
                }
            }
            else
            {
                foreach (var r in obj.highlightRenderers)
                {
                    r.enabled = !r.enabled;
                }
            }
        }

        public static void Unhighlight(GrabbableBase obj, HighlightMethod highlightMethod)
        {
            if (highlightMethod == HighlightMethod.AddSecondMaterial)
            {
                foreach (var r in obj.highlightRenderers)
                {
                    if (r.sharedMaterials.Length == 2)
                    {
                        r.sharedMaterials = new Material[]
                        {
                            r.sharedMaterials[0]
                        };
                    }
                }
            }
            else
            {
                foreach (var r in obj.highlightRenderers)
                {
                    r.enabled = !r.enabled;
                }
            }
        }
    }

    public enum HighlightMethod
    {
        /// <summary>
        /// Adds the outline mat to each renderer as a second mat (must be only 1 material per renderer for this to work)
        /// </summary>
        AddSecondMaterial,

        /// <summary>
        /// toggles all renderers, so enabled becomes disabled and viceversa. can be used to have 2 meshes to switch between, or even 3.
        /// </summary>
        ToggleRenderers,
    }

}

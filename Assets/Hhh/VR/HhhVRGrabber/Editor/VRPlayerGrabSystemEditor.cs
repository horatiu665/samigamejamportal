namespace HhhVRGrabber.Editor
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using Random = UnityEngine.Random;

    [CustomEditor(typeof(VRPlayerGrabSystem))]
    public class VRPlayerGrabSystemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var grabSystem = target as VRPlayerGrabSystem;
            if (grabSystem != null)
            {

                // feedback shit
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Debug/Feedback", EditorStyles.boldLabel);

                for (int i = 0; i < grabSystem.controllers.Count; i++)
                {
                    var c = grabSystem.controllers[i];
                    EditorGUILayout.LabelField(c.isLeft ? "Left" : "Right");
                    EditorGUILayout.Toggle("Is Grabbing", c.isGrabbing);
                    EditorGUILayout.Toggle("Is Highlighting", c.isHighlighting);
                    EditorGUILayout.ObjectField("Cur Grabbed", c.curGrabbed as Object, typeof(IHandleGrabbing), true);

                }
            }
        }
    }
}
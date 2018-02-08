using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

[CustomEditor(typeof(InputVR))]
public class InputVREditor : Editor
{
    private void OnEnable()
    {
        var monoScript = MonoScript.FromMonoBehaviour(target as MonoBehaviour);
        var eo = MonoImporter.GetExecutionOrder(monoScript);
        if (eo == 0)
        {
            Debug.Log("Changed Execution Order of [InputVR] to " + eo + " -> " + 9999);
            MonoImporter.SetExecutionOrder(monoScript, 9999);
        }
        var bu = (target as InputVR).GetComponent<InputVR_BeforeUpdate>(); 
        if (bu != null)
        {
            var monoScriptIVRBU = MonoScript.FromMonoBehaviour(bu);
            var eor = MonoImporter.GetExecutionOrder(monoScriptIVRBU);
            if (eor == 0)
            {
                Debug.Log("Changed Execution Order of [InputVR] to " + eo + " -> " + -9999);
                MonoImporter.SetExecutionOrder(monoScriptIVRBU, -9999);
            }
        }

    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("This script is used for the Update() of the XR.NodeStates", MessageType.Info);

        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.LabelField("Node States");
        if (InputVR.curNodeStates.Count == 0)
        {
            EditorGUI.indentLevel++;
            GUI.enabled = false;
            EditorGUILayout.LabelField("none");
            GUI.enabled = true;
            EditorGUI.indentLevel--;
        }
        for (int i = 0; i < InputVR.curNodeStates.Count; i++)
        {
            var ns = InputVR.curNodeStates[i];
            EditorGUILayout.LabelField("Node " + i);
            EditorGUI.indentLevel++;
            {
                var type = ns.nodeType;
                EditorGUILayout.EnumPopup("Node Type", type);
                EditorGUILayout.TextField("Unique ID", ns.uniqueID.ToString());
                EditorGUILayout.Toggle("Tracked", ns.tracked);
            }
            EditorGUI.indentLevel--;

        }

        serializedObject.ApplyModifiedProperties();
    }
}

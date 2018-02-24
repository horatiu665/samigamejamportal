using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AmbienceDesigner))]
public class AmbienceDesignerEditor : Editor
{
    private AmbienceDesigner ad;
    private void OnEnable()
    {
        ad = target as AmbienceDesigner;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        for (int i = 0; i < ad.ambiences.Count; i++)
        {
            var a = ad.ambiences[i];
            if (i <= ad.level)
                GUI.backgroundColor = Color.green;
            else
            {
                GUI.backgroundColor = Color.white;
            }
            a.volume = EditorGUILayout.Slider(a.name,a.volume, 0, 1);
            GUI.backgroundColor = Color.white;

        }


        if(GUILayout.Button("Play"))
            ad.Play();
        if(GUILayout.Button("Stop"))
            ad.Stop();
        if(GUILayout.Button("Next"))
            ad.Next();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;
using System;

[CustomEditor(typeof(MetaStarmap)), CanEditMultipleObjects]
public class MetaStarmapEditor : Editor
{
    private string saveAllMeshesPath = "Assets/STARGAZNIG/piano stars";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        var ms = target as MetaStarmap;

        EditorGUILayout.BeginHorizontal();

        saveAllMeshesPath = EditorGUILayout.TextField("Path", saveAllMeshesPath);
        if (GUILayout.Button("SaveAllMeshes"))
        {
            SaveAllMeshes(ms, saveAllMeshesPath);
        }

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    private void SaveAllMeshes(MetaStarmap ms, string saveAllMeshesPath)
    {
        var meshes = ms.GetComponentsInChildren<MeshFilter>();
        foreach (var m in meshes)
        {
            AssetDatabase.CreateAsset(m.mesh, saveAllMeshesPath + "/" + m.gameObject.name + ".asset");
        }

        AssetDatabase.Refresh();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class ClearDirectoriesTool : MonoBehaviour
{
    public bool doIt;

    private void Update()
    {
        if (doIt)
        {
            doIt = false;
            DoIt();
        }
    }

    private void DoIt()
    {
        var projectPath = Application.dataPath;
        foreach (var dir in Directory.GetDirectories(projectPath))
        {
            ClearAllEmptyFolders(dir);
        }
    }

    private void ClearAllEmptyFolders(string dir)
    {
        if (Directory.GetFiles(dir).Length == 0 && Directory.GetDirectories(dir).Length == 0)
        {
            dir = dir.Replace('\\', Path.AltDirectorySeparatorChar);
            dir = dir.Replace('/', Path.AltDirectorySeparatorChar);
            Directory.Delete(dir);
            Debug.Log("Deleted folder " + dir);
        }
        else
        {
            foreach (var dir2 in Directory.GetDirectories(dir))
            {
                ClearAllEmptyFolders(dir2);
            }
        }
    }
}
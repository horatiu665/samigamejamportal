using HhhPrefabManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Random = UnityEngine.Random;

public class ReplacePrefabs : MonoBehaviour
{
    public string nameContains = "Cube";

    public GameObject prefabToReplaceWith;

    public bool keepPos = true, keepRot = true, keepLocalScale = true;

#if UNITY_EDITOR
    [DebugButton]
    public void Replace()
    {
        List<GameObject> spawned = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var t = transform.GetChild(i);
            if (t.name.Contains(nameContains))
            {
                GameObject s = PrefabUtility.InstantiatePrefab(prefabToReplaceWith) as GameObject;
                if (keepPos)
                    s.transform.position = t.position;
                if (keepRot)
                    s.transform.rotation = t.rotation;
                if (keepLocalScale)
                    s.transform.localScale = t.localScale;
                spawned.Add(s);
                this.DestroySafe(t.gameObject);
                i--;
            }
        }

        for (int i = 0; i < spawned.Count; i++)
        {
            spawned[i].transform.SetParent(transform);
        }
    }
#endif
}
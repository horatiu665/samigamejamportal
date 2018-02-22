using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaStarmap : MonoBehaviour
{
    public int starCount = 120000;
    public int[] counts = new int[]
    {
        50,
        100,
        200,

    };

    public int maxStars = 63000 / 4;

    public TextAsset starDatabase;

    public Material material;

    public bool android;

    public float magnitudeLimit = 12;

    public float sizeMultiplier = 1f;

    List<Starmap> spawned = new List<Starmap>();

    [Header("Meshes or gameObjects?")]
    public bool generateMeshes = true;

    public GameObject starPrefab;

    void OnEnable()
    {
        Clear();
        Spawn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Clear();
            Spawn();
        }
    }

    public void Clear()
    {
        for (int i = 0; i < spawned.Count; i++)
        {
            if (spawned[i] != null)
            {
                Destroy(spawned[i].gameObject);
            }
        }
        spawned.Clear();
    }

    public void Spawn()
    {
        int curSkip = 0;
        var starCount = this.starCount;
        int i = 0;
        while (starCount > 0)
        {
            var count = i >= counts.Length ? maxStars : counts[i];
            var go = new GameObject("starmap " + i);
            go.transform.SetParent(transform);
            var sm = go.AddComponent<Starmap>();
            sm.maxStars = count;
            sm.lineSkip = curSkip;
            sm.android = this.android;
            sm.starDatabase = starDatabase;
            sm.material = material;
            sm.magnitudeLimit = magnitudeLimit;
            sm.sizeMultiplier = sizeMultiplier;
            sm.generateMeshes = generateMeshes;
            sm.starPrefab = this.starPrefab;
            spawned.Add(sm);

            curSkip += count;
            starCount -= count;
            i++;
        }
    }
    
}
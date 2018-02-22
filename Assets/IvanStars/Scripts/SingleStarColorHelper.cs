using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SingleStarColorHelper : MonoBehaviour
{
    public float temperature = 6000;
    public bool validateTempToColor = true;

    public Gradient tempGradPreview = new Gradient();

    [SerializeField, HideInInspector]
    float temperatureValidate;

    public Color color;

    private void OnValidate()
    {
        if (validateTempToColor)
        {
            if (temperature != temperatureValidate)
            {
                color = Starmap.GetColorFromTemperature(temperature);
            }

            if (tempGradPreview.colorKeys.Length <= 1 || temperature != temperatureValidate)
            {
                var gcks = new GradientColorKey[8];
                for (int i = 0; i < gcks.Length; i++)
                {
                    gcks[i].time = i / (float)(gcks.Length - 1);
                    gcks[i].color = Starmap.GetColorFromTemperature(temperature * gcks[i].time);
                }
                tempGradPreview = new Gradient();
                tempGradPreview.colorKeys = gcks;
            }

            temperatureValidate = temperature;
        }
    }

    private void OnEnable()
    {
        SetColor();
    }

    [DebugButton]
    public void SetColor()
    {
        SetColor(color);
    }

    public void SetColor(Color c)
    {
        var meshFilter = GetComponent<MeshFilter>();
        Color[] colors = new Color[meshFilter.mesh.vertices.Length];
        for (int i = 0; i < meshFilter.mesh.vertices.Length; i++)
        {
            colors[i] = c;
        }

        meshFilter.mesh.colors = colors;
    }
}
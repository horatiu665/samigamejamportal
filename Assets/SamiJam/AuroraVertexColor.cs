using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AuroraVertexColor : MonoBehaviour
{
    public Color allColor = Color.green;

    [SerializeField]
    private Renderer _renderer;
    public new Renderer renderer
    {
        get
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();
            }
            return _renderer;
        }
    }

    public bool useSharedMat = false;

    private void OnEnable()
    {
        OnValidate();
    }

    void OnValidate()
    {
        if (useSharedMat)
        {
            renderer.sharedMaterial.SetColor("_TintColor", allColor);
        }
        else
        {
            renderer.material.SetColor("_TintColor", allColor);
        }
    }
}
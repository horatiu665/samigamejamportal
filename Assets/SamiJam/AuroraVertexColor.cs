using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AuroraVertexColor : MonoBehaviour
{
    public Color allColor = Color.green;
    public Vector4 vector = new Vector4(113.235f, 217.61346f, 79.1324123f, 94.8917f);

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

    [DebugButton]
    private void GetVectorFromMaterial()
    {
        vector = renderer.sharedMaterial.GetVector("_RandomWobbleVector");
    }

    private void OnEnable()
    {
        OnValidate();
    }

    void OnValidate()
    {
        if (!Application.isPlaying || useSharedMat)
        {
            if (vector.x == vector.y && vector.y == vector.z && vector.z == vector.w && vector.z == 0)
            {
                GetVectorFromMaterial();
            }
            renderer.sharedMaterial.SetColor("_TintColor", allColor);
            renderer.sharedMaterial.SetVector("_RandomWobbleVector", vector);
        }
        else
        {
            renderer.material.SetColor("_TintColor", allColor);
            renderer.material.SetVector("_RandomWobbleVector", vector);
        }
    }
}
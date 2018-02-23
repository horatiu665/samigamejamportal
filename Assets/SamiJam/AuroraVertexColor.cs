using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AuroraVertexColor : MonoBehaviour
{
    public Color allColor = Color.green;
    public Vector4 vector = new Vector4(113.235f, 217.61346f, 79.1324123f, 94.8917f);
    public Vector4 randomizeVector = new Vector4(10, 10, 10, 10);
    public Vector4 vector2 = new Vector4(113.235f, 217.61346f, 79.1324123f, 94.8917f);
    public Vector4 randomizeVector2 = new Vector4(10, 10, 10, 10);

    public Vector4 _FlickerLuminosity = new Vector4();

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
        vector = renderer.sharedMaterial.GetVector("_RandomWobbleVector1");
        vector2 = renderer.sharedMaterial.GetVector("_RandomWobbleVector2");
    }

    private void OnEnable()
    {
        randomizeVector.x *= Random.Range(-1f, 1f);
        randomizeVector.y *= Random.Range(-1f, 1f);
        randomizeVector.z *= Random.Range(-1f, 1f);
        randomizeVector.w *= Random.Range(-1f, 1f);
        vector += randomizeVector;
        randomizeVector2.x *= Random.Range(-1f, 1f);
        randomizeVector2.y *= Random.Range(-1f, 1f);
        randomizeVector2.z *= Random.Range(-1f, 1f);
        randomizeVector2.w *= Random.Range(-1f, 1f);
        vector2 += randomizeVector2;
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
            renderer.sharedMaterial.SetVector("_RandomWobbleVector1", vector);
            renderer.sharedMaterial.SetVector("_RandomWobbleVector2", vector2);
            renderer.sharedMaterial.SetVector("_FlickerLuminosity", _FlickerLuminosity);

        }
        else
        {
            renderer.material.SetColor("_TintColor", allColor);
            renderer.material.SetVector("_RandomWobbleVector1", vector);
            renderer.material.SetVector("_RandomWobbleVector2", vector2);
            renderer.material.SetVector("_FlickerLuminosity", _FlickerLuminosity);
        }
    }
}
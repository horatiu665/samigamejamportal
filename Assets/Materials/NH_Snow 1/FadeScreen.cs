using System.Collections;
using Kaae;
using UnityEngine;

public class FadeScreen : Singleton<FadeScreen>
{
    public Cubemap cubemap;

    public bool Colliding;

    private Material _mat;
    private Renderer _renderer;
    private Coroutine _fadeRoutine;
    private Color _color = Color.clear;
    private Color _collideColor = Color.black;
    public bool _covered = false;
    public bool fadeInOnEnable;

    private void OnEnable()
    {
        _renderer = GetComponent<Renderer>();
        _mat = _renderer.material;
        if (fadeInOnEnable)
        {
            _mat.color = Color.black;
            Fade(Color.clear,5);
        }

        if (cubemap != null)
        {
            _mat.SetInt("_UseTexture", 1);
            _mat.SetTexture("_CubeTex", cubemap);
        }

        // automatically parent under main camera
        transform.parent = Camera.main.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Fade(Color color, float time = 1f, Texture texture = null)
    {

        if (texture)
        {
            _mat.SetTexture("_CubeTex", texture);
            _mat.SetInt("_UseTexture", 1);
        }

        _mat.SetInt("_UseTexture", _mat.GetTexture("_CubeTex") == null ? 0 : 1);

        if (_mat.color.a < 0.001f)
        {
            var c = color;
            c.a = 0;
            _mat.color = c;
        }

        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(FadeFromTo(_mat.color, color, time));
    }

    IEnumerator FadeFromTo(Color from, Color to, float time)
    {
        var fade = true;
        var d = 1 / time;
        var t = 0f;
        var startTime = Time.time;

        var color = from;
        _covered = false;

        while (fade)
        {
            yield return null;

            t = d * (Time.time - startTime);

            if (t > 0.999f)
            {
                t = 1f;
                fade = false;
                color = to;
            }
            else
            {
                color = Color.Lerp(from, to, Mathf.SmoothStep(0.0f, 1.0f, t));
            }


            // if fading out (black)
            if (Colliding)
            {
                if (from.a < to.a)
                {
                    var a = color.a > _collideColor.a ? color.a : _collideColor.a;
                    color = Color.Lerp(_collideColor, color, t);
                    color.a = a;
                }
                // else fading in (clear)
                else
                {
                    var a = color.a < _collideColor.a ? _collideColor.a : color.a;
                    color = Color.Lerp(_collideColor, color, t);
                    color.a = a;
                }
            }

            _mat.color = color;
        }

        _covered = to.a > 0.99f;

        _fadeRoutine = null;
    }

    public void Clear()
    {
        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        var color = _mat.color;
        color.a = 0;
        _mat.color = color;

        _covered = false;
    }

    public void Cover(Color? color = null)
    {
        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _covered = true;

        _mat.color = color ?? Color.black;
    }

    public void Collide(Color color)
    {
        // only fixed when not already fading
        if (_fadeRoutine == null && !_covered)
        {
            var a = color.a;
            var c = _mat.color;
            c = Color.Lerp(c, color, 0.5f * Time.deltaTime);
            c.a = a;
            _mat.color = c;
        }
        // otherwise can only be tweaked to less
        else
        {
            var a = color.a;
            _collideColor = Color.Lerp(_mat.color, color, 0.5f * Time.deltaTime);
            _collideColor.a = a;
        }
    }

    [DebugButton]
    private void TestFadeBlack() { Fade(Color.black, 5f); }

    [DebugButton]
    private void TestFadeClear() { Fade(Color.clear, 5f); }

    [DebugButton]
    private void TestClear() { Clear();}

    [DebugButton]
    private void TestCover() { Cover();}

}
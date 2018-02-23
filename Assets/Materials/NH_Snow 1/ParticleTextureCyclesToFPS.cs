using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTextureCyclesToFPS : MonoBehaviour
{
    public enum ParticleFps
    {
        FPS8 = 8,
        FPS12 = 12,
        FPS16 = 16,
        FPS24 = 24
    }

    public ParticleFps Fps = ParticleFps.FPS16;

    private ParticleSystem _ps;
    private float _lifetime;
    private int _sprites;
    private int _cycles;

    private void Start()
    {
        CalculateCycles();
    }

    private void OnEnable()
    {
        CalculateCycles();
    }

    private void CalculateCycles()
    {
        _ps = GetComponent<ParticleSystem>();

        if (_ps == null)
            return;

        _lifetime = _ps.main.startLifetime.constantMax / _ps.main.simulationSpeed;

        var tex = _ps.textureSheetAnimation;
        _sprites = tex.numTilesX * tex.numTilesY;

        _cycles = (int) Mathf.Round(_lifetime * (int)Fps / _sprites);

        tex.cycleCount = _cycles;
    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleUpdate : MonoBehaviour
{
    public int fps = 24;

    private ParticleSystem _ps;
    private ParticleSystem.MainModule _main;
    private float _t = 0;
    private int _skips;

    public void Start()
    {
        _ps = GetComponent<ParticleSystem>();
        _main = _ps.main;
        _ps.Pause(false);
    }

    private void OnEnable()
    {
        _ps = GetComponent<ParticleSystem>();
        _main = _ps.main;
        _ps.Pause(false);
    }

    private void Update()
    {
        _t += Time.deltaTime;

        if (_t > (1f / fps))
        {
            _main.simulationSpeed = _skips;
            _ps.Play(false);
            _t = 0;
            _skips = 0;
        }
        else
        {
            _ps.Pause(false);
            _skips++;
        }
    }
}
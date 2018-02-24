using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PortalShrinkTimer : MonoBehaviour
{
    private static PortalShrinkTimer _instance;
    public static PortalShrinkTimer instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PortalShrinkTimer>();
            }
            return _instance;
        }
    }

    [SerializeField]
    private Portal _portal;
    public Portal portal
    {
        get
        {
            if (_portal == null)
            {
                _portal = GetComponentInChildren<Portal>();
            }
            return _portal;
        }
    }

    [SerializeField]
    private Transform _playerHead;
    public Transform playerHead
    {
        get
        {
            if (_playerHead == null)
            {
                _playerHead = FindObjectOfType<SteamVR_Camera>().transform;
            }
            return _playerHead;
        }
    }

    [SerializeField]
    private ParticleSystem _funky;
    public ParticleSystem funky
    {
        get
        {
            if (_funky == null)
            {
                _funky = GetComponentsInChildren<ParticleSystem>().First(n => n.name.Contains("funky"));
            }
            return _funky;
        }
    }


    [Header("Params")]
    public bool shrinkAfterFirstPassthrough = true;

    public float maxShrinkSpeed = 1f;

    [Space]
    public Vector2 minDistXZForShrink = new Vector2(0.2f, 0.5f);
    public float maxLookAngleForShrink = 45f;
    public float maxLookAngleUpDown = 30f;

    [Header("State")]
    public bool startedShrinking = false;

    public float curShrinkSpeed = 0f;
    public float shrinkAcceleration = 0.1f;
    public float shrinkDeceleration = 0.6f;

    [Header("WhatToScale")]
    public Transform whatToScale;
    private Vector3 initScale;

    [Header("Last bit")]
    public float lastBitScale = 0.25f;
    public float lastBitDuration = 0.2f;
    public AnimationCurve lastBitCurve = new AnimationCurve() { keys = new Keyframe[] { new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 0, 0) } };

    Vector3 initPos;

    private void OnEnable()
    {
        initPos = transform.position;
        initScale = whatToScale.transform.localScale;
        Portal.OnSwitchedDimensions += Portal_OnSwitchedDimensions;
    }

    private void OnDisable()
    {
        Portal.OnSwitchedDimensions -= Portal_OnSwitchedDimensions;
    }

    private void Portal_OnSwitchedDimensions(string oldDimension)
    {
        startedShrinking = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetPortal();
        }

        if (!startedShrinking)
        {
            return;
        }

        var headToPortal = transform.position - playerHead.transform.position;
        headToPortal.y = 0;
        // when closer to 1, shrink faster
        var distBasedSpeedParam = Mathf.InverseLerp(minDistXZForShrink.x, minDistXZForShrink.y, headToPortal.magnitude);

        var upAngle = Vector3.Angle(playerHead.transform.forward, Vector3.up);
        var downAngle = Vector3.Angle(playerHead.transform.forward, Vector3.down);
        var minUpDownAngle = Mathf.Min(upAngle, downAngle);
        var upDownAngleBasedSpeedParam = Mathf.InverseLerp(maxLookAngleUpDown, 0f, minUpDownAngle);

        var angle = Vector3.Angle(headToPortal, playerHead.transform.forward);
        // when closer to 1, shrink faster
        var angleBasedSpeedParam = Mathf.InverseLerp(maxLookAngleForShrink, 0f, angle);

        // distance to portal can cockblock the shrinking (if you are too close, don't shrink)
        // angle can cockblock if you are looking straight at it, bu tif u look down/up, that angle param will be bigger than the straight at it, so we shrink faster when u look down  AND are close to it...
        var targetSpeed = Mathf.Max(distBasedSpeedParam * angleBasedSpeedParam, (1 - distBasedSpeedParam) * upDownAngleBasedSpeedParam);

        // if the target speed is increased, accelerate with one speed (Faster). if target speed is reduced (zero), decelerate
        curShrinkSpeed = Mathf.MoveTowards(curShrinkSpeed, targetSpeed, targetSpeed > curShrinkSpeed ? shrinkAcceleration : shrinkDeceleration);
        curShrinkSpeed = Mathf.Clamp(curShrinkSpeed, 0, 1f);

        // last bit happens only when u look at it
        if (curShrinkSpeed > 0)
        {
            if (lastBitScale >= whatToScale.transform.localScale.x)
            {
                LastBit();
            }
        }

        Shrink();
    }

    private void LastBit()
    {
        startedShrinking = false;
        var lastInitS = whatToScale.transform.localScale;
        StartCoroutine(pTween.To(lastBitDuration, t =>
        {
            var ls = new Vector3(lastInitS.x * lastBitCurve.Evaluate(t), lastInitS.y, lastInitS.z);
            whatToScale.transform.localScale = ls;
            if (t == 1)
            {
                transform.position = Vector3.one * 1000000;
                funky.transform.position = initPos;
                funky.Play();
            }
        }));

    }

    private void Shrink()
    {
        var ls = whatToScale.transform.localScale;
        ls.x -= curShrinkSpeed * maxShrinkSpeed * Time.deltaTime;
        ls.x = Mathf.Clamp(ls.x, 0, float.MaxValue);
        whatToScale.transform.localScale = ls;
    }

    [DebugButton]
    public void ResetPortal()
    {
        startedShrinking = false;
        whatToScale.transform.localScale = initScale;
        transform.position = initPos;
    }

    /// <summary>
    /// 0 is no shrink. 1 is full shrink, just before lastBit clamping animation. Use startedShrinking to see if the portal is shrinking.
    /// </summary>
    /// <returns></returns>
    public float GetShrink01()
    {
        return Mathf.InverseLerp(this.initScale.x, lastBitScale, whatToScale.localScale.x);
    }

}
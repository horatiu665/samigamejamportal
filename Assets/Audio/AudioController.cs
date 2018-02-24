using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{


	public Dimension samiLand, cityLand;
	public float transitionTime =.2f;
	public AudioMixerSnapshot samiSnapshot, citySnapshot;
	public AudioSource audioSource;
	public AudioClip portalTransitionSound;
	public AmbienceDesigner ambienceDesigner;


	private void Start()
	{
		ambienceDesigner.Play();
		if (samiLand.initialWorld)
		{
			samiSnapshot.TransitionTo(0);
		}
		else if (cityLand.initialWorld)
		{
			citySnapshot.TransitionTo(0);
		}
	}

	private void OnEnable()
	{
		Portal.OnSwitchedDimensions += PortalOnOnSwitchedDimensions;
	}

	private void OnDisable()
	{
		Portal.OnSwitchedDimensions -= PortalOnOnSwitchedDimensions;
	}

	private void PortalOnOnSwitchedDimensions(string dimensionName)
	{
		audioSource.PlayOneShot(portalTransitionSound);
		if (dimensionName == cityLand.name)
		{
			samiSnapshot.TransitionTo(transitionTime);
		}
		else if (dimensionName == samiLand.name)
		{
			citySnapshot.TransitionTo(transitionTime);	
		}
	}

	
}

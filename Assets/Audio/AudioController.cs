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


	private void Start()
	{
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
		if (dimensionName == samiLand.name)
		{
			samiSnapshot.TransitionTo(transitionTime);
		}
		else if (dimensionName == cityLand.name)
		{
			citySnapshot.TransitionTo(transitionTime);	
		}
	}

	
}

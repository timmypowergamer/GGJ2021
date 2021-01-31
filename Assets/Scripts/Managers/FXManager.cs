using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FXManager : MonoBehaviour
{
	[SerializeField] VolumeProfile NearSightedEffect;
	[SerializeField] VolumeProfile FarSightedEffect;

	[SerializeField] Volume PostProcessVolume;

	public static FXManager Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	public void SetPostEffect(bool IsPredator)
	{
		PostProcessVolume.profile = IsPredator ? FarSightedEffect : NearSightedEffect;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLocation : MonoBehaviour
{
	[SerializeField] private Outline outline;

	public void SetOutlineEffect(bool enable)
	{
		outline.enabled = enable;
	}
}

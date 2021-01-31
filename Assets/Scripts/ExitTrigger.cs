using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
	private  List<PlayerController> _playersInRange = new List<PlayerController>();

	public delegate void OnWinConditionMet();
	public OnWinConditionMet OnGameWon;

    // Start is called before the first frame update
    void Start()
    {
		LIBGameManager.Instance.SetUpExit(this);
    }

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("OnTriggerEnter " + other.gameObject.name);
		if(other.gameObject.CompareTag("Lover"))
		{
			PlayerController player = other.GetComponentInParent<PlayerController>();
			if(!_playersInRange.Contains(player))
			{
				_playersInRange.Add(player);
			}
			if(CheckWinCondition())
			{
				OnGameWon?.Invoke();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Lover"))
		{
			PlayerController player = other.GetComponentInParent<PlayerController>();
			if (_playersInRange.Contains(player))
			{
				_playersInRange.Remove(player);
			}
		}
	}

	public bool CheckWinCondition()
	{
		if(_playersInRange.Count >= 1)
		{
			return true;
		}
		return false;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Slider healthslider;
	public Sprite[] heartSpriteList;
	public GameObject loveState;
	
	public void SetMaxHealth(int health)
	{
		healthslider.maxValue = health;
		healthslider.value = health;
	}
	
	public void SetHealth(int health)
	{
		healthslider.value = health;
	}
	
	public void SetLove(bool love)
	{
		if (love == true)
			loveState.GetComponent<Image> ().sprite = heartSpriteList[1];
		else
			loveState.GetComponent<Image> ().sprite = heartSpriteList[0];
	}
	
	
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Slider healthslider;
	public Sprite[] heartSpriteList;
	public Sprite[] LoverEndScreen;
	public Sprite[] PredatorEndScreen;
	public GameObject loveState;

	public Image _predatorEndImage;
	public Image _loverEndImage;

	public GameObject PredatorHUD;
	public GameObject LoverHUD;

	private bool IsPredator = false;
	

	public static PlayerHUD Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		//gameObject.SetActive(false);
	}


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

	public void SetOwner(bool isPredator)
	{
		PredatorHUD.SetActive(isPredator);
		LoverHUD.SetActive(!isPredator);
		_predatorEndImage.gameObject.SetActive(false);
		_loverEndImage.gameObject.SetActive(false);
		IsPredator = isPredator;
		SetLove(false);
	}

	public void SetPredatorWon()
	{
		_predatorEndImage.sprite = PredatorEndScreen[1];
		_predatorEndImage.gameObject.SetActive(true);
	}

	public void SetPredatorLost()
	{
		_predatorEndImage.sprite = PredatorEndScreen[0];
		_predatorEndImage.gameObject.SetActive(true);
	}

	public void SetIsDead()
	{
		_loverEndImage.sprite = LoverEndScreen[0];
		_loverEndImage.gameObject.SetActive(true);
	}

	public void SetLoverDead()
	{
		_loverEndImage.sprite = LoverEndScreen[1];
		_loverEndImage.gameObject.SetActive(true);
	}

	public void SetLoversWon()
	{
		_loverEndImage.sprite = LoverEndScreen[2];
		_loverEndImage.gameObject.SetActive(true);
	}
	
	
}

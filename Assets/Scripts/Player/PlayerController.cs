using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Linq;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	public enum PlayerState
	{
		WAITING,
		PLAYING
	}

	public enum GroundState
	{
		GROUNDED,
		JUMPING,
		FALLING
	}

	public PlayerState CurrentPlayerState { get; private set; }
	public GroundState CurrentGroundState { get; private set; }


	#region Serialized Fields
	[SerializeField] private CharacterController Controller;
	[SerializeField] private Camera Camera;

	[SerializeField] private InputActionReference _inputActionReferenceLook;

	[Header("Sounds")]
	[SerializeField] private AudioSource MetallicSFX;
	[SerializeField] private AudioSource OuchSFX;

	[Header("Movement")]
	[SerializeField] private float MoveSpeed = 5;
	[SerializeField] private float LookSensitivity = 3;
	[SerializeField] private float MaxPositiveVerticalLook = 80f;
	[SerializeField] private float MaxNegativeVerticalLook = -80f;
	[SerializeField] private float Gravity = -9.81f;
	[SerializeField] private float GroundCheckDistance = 0.4f;
	[SerializeField] private LayerMask GroundMask;
	[SerializeField] private float JumpVelocity = 10f;
	[SerializeField] private Transform GroundCheckLocation;


	[Header("Predator Stuff")]
	[SerializeField] private bool _isPredator = false;
	public bool IsPredator { get { return _isPredator; } }
	[SerializeField] private Transform AttackCheckLocation;
	[SerializeField] private float AttackCheckDistance;
	[SerializeField] private LayerMask PreyMask;
	[SerializeField] private GameObject FP_WeaponModel;

	[Header("Lover Stuff")]
	[SerializeField] private GameObject PlayerHUDObject;
	[SerializeField] private Outline OutlineEffect;
	[SerializeField] private float LoveRadius = 10f;

	#endregion

	private Vector3 cameraPos;
	private Quaternion cameraRot;

	//movement
	private Vector2 MoveInputValue;
	private Vector2 LookInputValue;
	private Vector2 lookRotation;
	private Vector3 Velocity;
	private bool jumpPressed = false;

	//Networking
	private PhotonView _photonView;


	public PlayerInput Input { get; private set; }
	private Transform spawnPoint;

	private Animator animator;

	public static PlayerController LocalPlayerInstance;
	public int PlayerIndex { get; private set; }
	public PlayerController Partner { get; set; }
	private bool _lastPartnerCheck = false;

	public PlayerHUD HUD { get { return PlayerHUD.Instance; } }
	private const int HitDamage = 10;
	private const int MaxHealth = 100;
	private int _health = MaxHealth;

	private void Awake()
	{
		_photonView = GetComponent<PhotonView>();
		if (PhotonNetwork.IsConnected)
		{
			if (_photonView.IsMine)
			{
				LocalPlayerInstance = this;
			}
			else
			{
				Camera.gameObject.SetActive(false);
				transform.SetLayerRecursively("OtherPlayers");
				GetComponent<PlayerInput>().enabled = false;
				if (FP_WeaponModel != null)
				{
					FP_WeaponModel.SetActive(false);
				}
			}
			PlayerIndex = (int)_photonView.Owner.CustomProperties["player_index"];
			LIBGameManager.Instance.PlayerSpawned(this);
		}
	}

	private void OnEnable()
	{
        animator = GetComponentInChildren<Animator>();

		CurrentPlayerState = PlayerState.WAITING;
		CurrentGroundState = GroundState.GROUNDED;
		Controller.enabled = false;		
		animator.SetFloat("Forward", 0);

    }

	private void Start()
	{
		if(_photonView.IsMine && PhotonNetwork.IsConnected)
		{
			FXManager.Instance.SetPostEffect(IsPredator);
		}
		//for offline testing
		if(!PhotonNetwork.IsConnected)
		{
			FXManager.Instance.SetPostEffect(IsPredator);
			StartGame(LIBGameManager.Instance.SpawnGroups[0].GetSpawnPoint(IsPredator ? LIBGameManager.SpawnGroup.SpawnType.PREDATOR : LIBGameManager.SpawnGroup.SpawnType.LOVER, 0));
		}
	}

	private void OnDisable()
	{
	}

	public void Move(InputAction.CallbackContext context)
	{
		if(CurrentPlayerState == PlayerState.PLAYING)
			MoveInputValue = context.ReadValue<Vector2>();		
	}

	public void Jump(InputAction.CallbackContext context)
	{
		if (CurrentPlayerState == PlayerState.PLAYING)
			jumpPressed = true;
	}
	
	public void Look(InputAction.CallbackContext context)
	{
		//if (CurrentPlayerState == PlayerState.PLAYING)
		//{
		//	LookInputValue = context.ReadValue<Vector2>();
		//	Debug.Log("Look input value = " + LookInputValue.ToString());
		//}
	}

	public void Attack(InputAction.CallbackContext context)
    {
		if (!IsPredator || context.phase != InputActionPhase.Started)
        {
			return;
        }

		animator.SetTrigger("Attack");

		if (Physics.CheckSphere(AttackCheckLocation.position, AttackCheckDistance, GroundMask))
        {
			MetallicSFX.Play();
        }
		else
        {
			var colliders = Physics.OverlapSphere(AttackCheckLocation.position, AttackCheckDistance, PreyMask);
			if (colliders.Any())
            {
				var hit = colliders.OrderBy(c => (c.transform.position - AttackCheckLocation.position).sqrMagnitude).First();
				var preyGameObject = hit.gameObject;
				var preyController = preyGameObject.GetComponentInParent<PlayerController>();
				preyController.TakeDamage(HitDamage);
			}
		}
    }

	public void TakeDamage(int damage)
    {
		_health -= damage;
		HUD.SetHealth(_health);
		OuchSFX.Play();
    }

	private void Update()
	{
		if (CurrentPlayerState == PlayerState.PLAYING)
		{
			//look
			LookInputValue = _inputActionReferenceLook.action.ReadValue<Vector2>();
			lookRotation.y += LookInputValue.x * LookSensitivity;
			lookRotation.x += LookInputValue.y * LookSensitivity;
			lookRotation.x = Mathf.Clamp(lookRotation.x, MaxNegativeVerticalLook, MaxPositiveVerticalLook);
			Controller.transform.localRotation = Quaternion.Euler(0, lookRotation.y, 0);
			Camera.transform.localRotation = Quaternion.Euler(lookRotation.x, 0, 0);
		}

		//checking range
		if (!IsPredator)
		{
			bool partnerCheck = IsInRangeOfPartner();
			if (partnerCheck == true && _lastPartnerCheck == false)
			{
				OnEnteredRange();
			}
			else if (partnerCheck == false && _lastPartnerCheck == true)
			{
				OnLeftRange();
			}
			_lastPartnerCheck = partnerCheck;
		}
	}

	public bool IsInRangeOfPartner()
	{
		if (!IsPredator && Partner != null)
		{
			if ((Partner.transform.position - transform.position).magnitude <= LoveRadius)
			{
				return true;
			}
		}
		return false;
	}


	private void FixedUpdate()
	{
		if (!_photonView.IsMine && PhotonNetwork.IsConnected)
		{
			return;
		}

		if (CurrentPlayerState == PlayerState.PLAYING)
		{
			//Ground check
			if (CurrentGroundState != GroundState.JUMPING)
			{
				if (Physics.CheckSphere(GroundCheckLocation.position, GroundCheckDistance, GroundMask))
				{
					//Debug.Log("Grounded");
					CurrentGroundState = GroundState.GROUNDED;
				}
				else
				{
					CurrentGroundState = GroundState.FALLING;
				}
			}
			else if(Velocity.y <= 0)
			{
				CurrentGroundState = GroundState.FALLING;
			}

			//move
			Vector3 movement = (Controller.transform.right * MoveInputValue.x + Controller.transform.forward * MoveInputValue.y) * MoveSpeed;
			Velocity.x = movement.x;
			Velocity.z = movement.z;
			//gravity
			Velocity.y += Gravity * Time.fixedDeltaTime;

			//jump
			if(CurrentGroundState == GroundState.GROUNDED && jumpPressed)
			{
				Velocity.y = JumpVelocity;
				//Velocity.y += JumpVelocity;
				CurrentGroundState = GroundState.JUMPING;
			}
			Controller.Move(Velocity * Time.fixedDeltaTime);
			jumpPressed = false;

            animator.SetFloat("Forward", MoveInputValue.y);
			animator.SetFloat("Strafe", MoveInputValue.x);
		}		
	}

	public void StartGame(Transform startPos)
	{
		if (PhotonNetwork.IsConnected && !_photonView.IsMine) return;

		spawnPoint = startPos;
		Controller.enabled = false;
		transform.SetPositionAndRotation(startPos.position, startPos.rotation);
		CurrentPlayerState = PlayerState.PLAYING;
		CurrentGroundState = GroundState.FALLING;
		Controller.enabled = true;
		HUD.SetMaxHealth(MaxHealth);
		HUD.SetHealth(_health);
		HUD.gameObject.SetActive(true);

		if (PhotonNetwork.IsConnected)
		{
			LIBGameManager.Instance.Lovers[0].Partner = LIBGameManager.Instance.Lovers[1];
			LIBGameManager.Instance.Lovers[1].Partner = LIBGameManager.Instance.Lovers[0];
		}
	}

	public void OnEnteredRange()
	{
		if (LocalPlayerInstance.IsPredator)
		{
			OutlineEffect.enabled = true;
		}
		else
		{
			//Play SFX and stuff here?
			HUD.SetLove(true);
			if (LIBGameManager.Instance.Exit != null)
			{
				LIBGameManager.Instance.Exit.SetOutlineEffect(true);
			}
		}
	}

	public void OnLeftRange()
	{
		if (LocalPlayerInstance.IsPredator)
		{
			OutlineEffect.enabled = false;
		}
		else
		{
			//Play SFX and stuff here?
			HUD.SetLove(false);
			if (LIBGameManager.Instance.Exit != null)
			{
				LIBGameManager.Instance.Exit.SetOutlineEffect(false);
			}
		}
	}
}

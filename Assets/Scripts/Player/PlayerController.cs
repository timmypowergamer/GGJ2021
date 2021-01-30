using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	public enum PlayerState
	{
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

	[SerializeField] private CharacterController Controller;
	[SerializeField] private Camera Camera;
	[SerializeField] private Transform GroundCheckLocation;

	[SerializeField] private InputActionReference _inputActionReferenceLook;


	[SerializeField] private float MoveSpeed = 5;
	[SerializeField] private float LookSensitivity = 3;
	[SerializeField] private float MaxPositiveVerticalLook = 80f;
	[SerializeField] private float MaxNegativeVerticalLook = -80f;
	[SerializeField] private float Gravity = -9.81f;
	[SerializeField] private float GroundCheckDistance = 0.4f;
	[SerializeField] private LayerMask GroundMask;
	[SerializeField] private float JumpVelocity = 10f;

	private Vector3 cameraPos;
	private Quaternion cameraRot;

	//movement
	private Vector2 MoveInputValue;
	private Vector2 LookInputValue;
	private Vector2 lookRotation;
	private Vector3 Velocity;
	private bool jumpPressed = false;


	public PlayerInput Input { get; private set; }
	private Transform spawnPoint;

    private Animator animator;

	private void OnEnable()
	{		
        animator = GetComponentInChildren<Animator>();

		CurrentPlayerState = PlayerState.PLAYING;
		//Controller.enabled = false;		
		animator.SetFloat("Forward", 0);
    }

    private void OnDisable()
	{
	}

	public void Move(InputAction.CallbackContext context)
	{
		if(CurrentPlayerState == PlayerState.PLAYING)
			MoveInputValue = context.ReadValue<Vector2>();		
	}
	
	public void Look(InputAction.CallbackContext context)
	{
		//if (CurrentPlayerState == PlayerState.PLAYING)
		//{
		//	LookInputValue = context.ReadValue<Vector2>();
		//	Debug.Log("Look input value = " + LookInputValue.ToString());
		//}
	}

	private void Update()
	{
		//look
		LookInputValue = _inputActionReferenceLook.action.ReadValue<Vector2>();
		lookRotation.y += LookInputValue.x * LookSensitivity;
		lookRotation.x += LookInputValue.y * LookSensitivity;
		lookRotation.x = Mathf.Clamp(lookRotation.x, MaxNegativeVerticalLook, MaxPositiveVerticalLook);
		Controller.transform.localRotation = Quaternion.Euler(0, lookRotation.y, 0);
		Camera.transform.localRotation = Quaternion.Euler(lookRotation.x, 0, 0);
	}


	private void FixedUpdate()
	{
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
		}		
	}
}

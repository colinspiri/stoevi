using System;
using DG.Tweening;
using SpookuleleAudio;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(CharacterController))]

public class FirstPersonMovement : MonoBehaviour
{
	#region Components
	public static FirstPersonMovement Instance;
	public PlayerInput playerInput;
	private CharacterController controller;
	private InputActions inputActions;
	public IntReference mapEnabled;
	#endregion
	
	#region Public Constants
	[Header("Movement")]
	public float walkSpeed = 1f;
	public float runSpeed = 1f;
	public float crouchSpeed = 1f;
	public float interactingSpeed = 1f;
	public float backwardSlowFactor = 1f;
	public float mapSpeed = 1f;
	[Tooltip("Acceleration and deceleration")]
	public float speedChangeRate = 10.0f;
	
	[Header("Crouching")]
	// public float crouchStaminaMultiplier = 1f;
	public ASoundContainer player_inhale;
	public ASoundContainer player_exhale;

	[Header("Sounds")] 
	public FloatReference walkLoudness;
	public FloatReference runLoudness;

	[Header("Player Grounded")]
	[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
	public float gravity = -15.0f;
	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	public float fallTimeout = 0.15f;
	[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
	public bool grounded = true;
	[Tooltip("Useful for rough ground")]
	public float groundedOffset = -0.14f;
	[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
	public float groundedRadius = 0.5f;
	[Tooltip("What layers the character uses as ground")]
	public LayerMask groundLayers;

	[Header("Camera")]
	[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
	public GameObject cameraTarget;
	[Tooltip("How far in degrees can you move the camera up")]
	public float topClamp = 90.0f;
	[Tooltip("How far in degrees can you move the camera down")]
	public float bottomClamp = -90.0f;
	public float rotationSpeed = 1.0f;
	public float crouchHeight;
	public float bobFrequency;
	public float bobMagnitude;
	public float bobMagnitudeCrouching;
	public float peekUpDistance;
	public float peekSideDistance;
	public float peekRotation;
	
	private const float threshold = 0.01f;
	#endregion
	
	#region State Variables
	// camera
	public bool crouching { get; private set; }
	private float cameraTargetPitch;
	private float normalHeight;
	private float bobCycle;
	private Vector3 cameraTargetPosition;
	private Tween cameraPositionLerpTween;
	private bool cameraPositionLerping;
	private float cameraTargetRoll;
	private Tween cameraRotationLerpTween;

	// player
	private float currentSpeed;
	private float rotationVelocity;
	private float verticalVelocity;
	private float terminalVelocity = 53.0f;
	public enum MoveState { Still, Walking, Running, CrouchWalking }
	public MoveState moveState { get; private set; }
	private enum PeekState { None, PeekCenter, PeekLeft, PeekRight, PeekUp }
	private PeekState peekState;

	// timeout deltatime
	private float fallTimeoutDelta;
	private bool IsCurrentDeviceMouse => playerInput.currentControlScheme == "KeyboardMouse";
	#endregion
	
	#region Event Functions
	private void Awake() {
		Instance = this;
		controller = GetComponent<CharacterController>();
	}

	private void OnEnable() {
		inputActions = new InputActions();
		inputActions.Enable();
	}

	private void Start() {
		// initialize states
		moveState = MoveState.Still;
		peekState = PeekState.None;
		
		// reset our timeouts on start
		fallTimeoutDelta = fallTimeout;
		
		// camera
		normalHeight = cameraTarget.transform.position.y - transform.position.y;
	}

	private void Update() {
		JumpAndGravity();
		GroundedCheck();
		Move();
		
		// consume stamina
		if(moveState == MoveState.Running) StaminaController.Instance.ConsumeStamina();
		else if(crouching) StaminaController.Instance.PauseStaminaRegeneration();
		
		// report sound
		if (TorbalanHearing.Instance != null) {
			if(moveState == MoveState.Running) TorbalanHearing.Instance.ReportSound(transform.position, runLoudness);
			else if(moveState == MoveState.Walking) TorbalanHearing.Instance.ReportSound(transform.position, walkLoudness);
		}
	}

	private void LateUpdate() {
		CameraMovement();
	}
	#endregion

	private void JumpAndGravity()
	{ 
		if (grounded)
		{
			// reset the fall timeout timer
			fallTimeoutDelta = fallTimeout;

			// stop our velocity dropping infinitely when grounded
			if (verticalVelocity < 0.0f)
			{
				verticalVelocity = -2f;
			}
		}
		else
		{
			// fall timeout
			if (fallTimeoutDelta >= 0.0f)
			{
				fallTimeoutDelta -= Time.deltaTime;
			}
		}

		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (verticalVelocity < terminalVelocity)
		{
			verticalVelocity += gravity * Time.deltaTime;
		}
	}

	private void GroundedCheck() {
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
		grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
	}
	
	private void Move() {
		// if crouching
		if (InputHandler.Instance.crouching && StaminaController.Instance.HasStamina()) {
			if (!crouching) {
				// start crouching
				player_inhale.Play();
			}
			crouching = true;
		}
		else {
			if (crouching) {
				// stop crouching
				player_exhale.Play();
			}
			crouching = false;
			InputHandler.Instance.ResetCrouchInput();
		}
		
		// if there is no input, set the target speed to 0
		float targetSpeed;
		var move = inputActions.Gameplay.Move.ReadValue<Vector2>();
		if (move == Vector2.zero) {
			targetSpeed = 0.0f;
			moveState = MoveState.Still;
		}
		// set target speed and and move state if player is moving
		else if (InputHandler.Instance.peek) {
			targetSpeed = 0;
			moveState = MoveState.Still;
		}
		else if (InteractableManager.Instance.interactionState == InteractableManager.InteractionState.Interacting) {
			targetSpeed = interactingSpeed;
			moveState = MoveState.Walking;
		}
		else if (crouching) {
			targetSpeed = crouchSpeed;
			moveState = MoveState.CrouchWalking;
		}
		else if (InputHandler.Instance.run && StaminaController.Instance.HasStamina()) {
			targetSpeed = runSpeed;
			moveState = MoveState.Running;
		}
		else {
			targetSpeed = walkSpeed;
			moveState = MoveState.Walking;
		}
		// if moving backward
		if (move.y < 0) {
			targetSpeed *= backwardSlowFactor;
		}
		// if map is up
		if (mapEnabled.Value == 1 && targetSpeed > interactingSpeed) {
			targetSpeed = interactingSpeed;
		}

		// a reference to the players current horizontal velocity
		float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

		float speedOffset = 0.1f;
		// analog movement
		bool analogMovement = false;
		float inputMagnitude = analogMovement ? move.magnitude : 1f;

		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			currentSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);

			// round speed to 3 decimal places
			currentSpeed = Mathf.Round(currentSpeed * 1000f) / 1000f;
		}
		else
		{
			currentSpeed = targetSpeed;
		}

		// normalise input direction
		Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving
		if (move != Vector2.zero)
		{
			// move
			inputDirection = transform.right * move.x + transform.forward * move.y;
		}

		// move the player
		controller.Move(inputDirection.normalized * (currentSpeed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
	}

	private void CameraMovement() {
		// change rotation and pitch based on mouse movement
		var look = InputHandler.Instance.look;
		if (look.sqrMagnitude >= threshold) {
			// Don't multiply mouse input by Time.deltaTime
			float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
			
			cameraTargetPitch += -look.y * rotationSpeed * deltaTimeMultiplier;
			rotationVelocity = look.x * rotationSpeed * deltaTimeMultiplier;

			// clamp our pitch rotation
			cameraTargetPitch = ClampAngle(cameraTargetPitch, bottomClamp, topClamp);

			// Update Cinemachine camera target pitch
			cameraTarget.transform.localRotation = Quaternion.Euler(cameraTargetPitch, 0.0f, 0.0f);

			// rotate the player left and right
			transform.Rotate(Vector3.up * rotationVelocity);
		}
		
		// get base camera height
		Vector3 cameraPos = GetBaseCameraPosition();
		float cameraRot = 0;
		
		// bob head while moving
		if (moveState != MoveState.Still) {
			bobCycle += currentSpeed * bobFrequency * Time.deltaTime;
			bobCycle %= 2 * Mathf.PI;
			var magnitude = crouching ? bobMagnitudeCrouching : bobMagnitude;
			cameraPos.y += Mathf.Sin(bobCycle) * magnitude;
		}
		else bobCycle = 0;
		
		// get peeking direction
		if (InputHandler.Instance.peek) {
			var directionInput = inputActions.Gameplay.Move.ReadValue<Vector2>();
			if (directionInput.y > 0) {
				peekState = PeekState.PeekUp;
			}
			else if (directionInput.x < 0) {
				peekState = PeekState.PeekLeft;
			}
			else if (directionInput.x > 0) {
				peekState = PeekState.PeekRight;
			}
			else peekState = PeekState.PeekCenter;
		}
		else peekState = PeekState.None;

		// if peeking, add offset to camera pos
		if (peekState != PeekState.None) {
			switch (peekState) {
				case PeekState.PeekLeft:
					cameraPos += -peekSideDistance * transform.right;
					cameraRot += peekRotation;
					break;
				case PeekState.PeekRight:
					cameraPos += peekSideDistance * transform.right;
					cameraRot += -peekRotation;
					break;
				case PeekState.PeekUp:
					cameraPos += peekUpDistance * transform.up;
					break;
				case PeekState.PeekCenter:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		LerpCameraPosition(cameraPos, 0.5f);
		LerpCameraRotation(cameraRot, 0.5f);
	}

	public Vector3 GetRaycastTarget() {
		return GetBaseCameraPosition();
	}

	#region Helper Functions

	private Vector3 GetBaseCameraPosition() {
		Vector3 cameraPos = transform.position;
		if (crouching) cameraPos += crouchHeight * transform.up;
		else cameraPos += normalHeight * transform.up;
		return cameraPos;
	}
	private void LerpCameraPosition(Vector3 newCameraPosition, float duration) {
		if (newCameraPosition == cameraTargetPosition) return;
		
		cameraTargetPosition = newCameraPosition;
		
		cameraPositionLerpTween?.Kill();
		cameraPositionLerpTween = cameraTarget.transform.DOMove(cameraTargetPosition, duration);
		
		cameraPositionLerping = true;
		cameraPositionLerpTween.onComplete += () => {
			cameraPositionLerping = false;
			cameraPositionLerpTween = null;
		};
	}
	private void LerpCameraRotation(float newCameraRoll, float duration) {
		if (Math.Abs(newCameraRoll - cameraTargetRoll) <= threshold) return;
		
		cameraTargetRoll = newCameraRoll;
		
		cameraRotationLerpTween?.Kill();
		cameraRotationLerpTween =
			cameraTarget.transform.DOLocalRotate(new Vector3(cameraTarget.transform.eulerAngles.x, 0, cameraTargetRoll),
				duration);
		
		cameraRotationLerpTween.onComplete += () => {
			cameraRotationLerpTween = null;
		};
	}

	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}
	#endregion

	private void OnDrawGizmosSelected() {
		// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
		Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
		if (grounded) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;
		Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundedRadius);
		
		// loudness radius
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, walkLoudness);
		Gizmos.DrawWireSphere(transform.position, runLoudness);
	}
}
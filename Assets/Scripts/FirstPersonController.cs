using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class FirstPersonController : MonoBehaviour
{
	// components
	public static FirstPersonController Instance;
	public PlayerInput playerInput;
	private CharacterController controller;
	private InputActions inputActions;
	
	// public variables
	[Header("Player")]
	public float walkSpeed = 1f;
	public float runSpeed = 1f;
	public float crouchSpeed = 1f;
	[Tooltip("Acceleration and deceleration")]
	public float speedChangeRate = 10.0f;

	[Header("Sounds")] 
	public float walkLoudness;
	public float runLoudness;

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

	// state
	// input
	private bool runInput;
	private bool crouchingLastFrame;
	private bool crouching;

	// camera
	private float cameraTargetPitch;
	private float normalHeight;
	private Tween crouchTransition;
	private bool bobbing;
	private float bobCycle;

	// player
	private float currentSpeed;
	private float rotationVelocity;
	private float verticalVelocity;
	private float terminalVelocity = 53.0f;
	public enum MoveState { Still, Walking, Running, CrouchWalking }
	private MoveState moveState = MoveState.Still;
	public MoveState GetMoveState => moveState;

	// timeout deltatime
	private float fallTimeoutDelta;
	
	private const float threshold = 0.01f;

	private bool IsCurrentDeviceMouse => playerInput.currentControlScheme == "KeyboardMouse";

	private void Awake() {
		Instance = this;
		controller = GetComponent<CharacterController>();
	}

	private void OnEnable() {
		inputActions = new InputActions();
		inputActions.Enable();
	}

	private void Start() {
		// reset our timeouts on start
		fallTimeoutDelta = fallTimeout;
		
		// camera
		normalHeight = cameraTarget.transform.position.y - transform.position.y;
	}

	private void Update() {
		// toggle crouching
		if(crouching && !crouchingLastFrame) {
			crouchTransition = cameraTarget.transform.DOMoveY(transform.position.y + crouchHeight, 0.5f);
			crouchingLastFrame = true;
		}
		if (crouchingLastFrame && !crouching) {
			crouchTransition = cameraTarget.transform.DOMoveY(transform.position.y + normalHeight, 0.5f);
			crouchingLastFrame = false;
		}

		JumpAndGravity();
		GroundedCheck();
		Move();
		
		// report sound
		if (TorbalanSenses.Instance != null) {
			if(moveState == MoveState.Running) TorbalanSenses.Instance.ReportSound(transform.position, runLoudness);
			else if(moveState == MoveState.Walking) TorbalanSenses.Instance.ReportSound(transform.position, walkLoudness);
		}
	}

	private void LateUpdate() {
		CameraMovement();
	}

	private void GroundedCheck() {
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
		grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
	}

	private void CameraMovement() {
		// if there is an input
		var look = inputActions.Gameplay.Look.ReadValue<Vector2>();
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
		
		// bob head
		bool crouchTransitioning = crouchTransition != null && crouchTransition.IsActive() && crouchTransition.IsPlaying();
		if (moveState != MoveState.Still && !crouchTransitioning) {
			bobCycle += currentSpeed * bobFrequency * Time.deltaTime;
			bobCycle %= 2 * Mathf.PI;
			var baseHeight = crouching ? crouchHeight : normalHeight;
			var magnitude = crouching ? bobMagnitudeCrouching : bobMagnitude;
			float newCameraY = transform.position.y + baseHeight + Mathf.Sin(bobCycle) * magnitude;
			cameraTarget.transform.position = new Vector3(cameraTarget.transform.position.x, newCameraY,
				cameraTarget.transform.position.z);
		}
		else bobCycle = 0;
	}

	private void Move() {
		// if interacting with something, stop moving
		if (InteractableManager.Instance.interactionState == InteractableManager.InteractionState.Interacting) {
			moveState = MoveState.Still;
			currentSpeed = 0;
			return;
		}
		
		// set target speed and movestate assuming the player is moving
		float targetSpeed;
		if (crouching) {
			targetSpeed = crouchSpeed;
			moveState = MoveState.CrouchWalking;
		}
		else if (runInput) {
			targetSpeed = runSpeed;
			moveState = MoveState.Running;
		}
		else {
			targetSpeed = walkSpeed;
			moveState = MoveState.Walking;
		}

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon
		// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is no input, set the target speed to 0
		var move = inputActions.Gameplay.Move.ReadValue<Vector2>();
		if (move == Vector2.zero) {
			targetSpeed = 0.0f;
			moveState = MoveState.Still;
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

	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}
	
	public void OnRunInput(InputAction.CallbackContext context) {
		runInput = context.ReadValueAsButton();
	}
	public void OnCrouchInput(InputAction.CallbackContext context) {
		if (!crouchingLastFrame && context.ReadValueAsButton()) {
			crouching = true;
		}
		if (crouchingLastFrame && context.ReadValueAsButton()) {
			crouching = false;
		}
	}

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
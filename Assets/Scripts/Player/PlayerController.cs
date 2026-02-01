using System;
using System.Collections;
using System.Threading;
using NetworkMask.Constants;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(UiController))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] float gravity = -30f;
    // [SerializeField] float maxCameraSpeed = 15f;
    [SerializeField] float maxSpeed = 4f;
    float _realMaxSpeed;
    [SerializeField] float acceleration = 20f;
    [SerializeField] float jumpHeight = 1.6f;
    [SerializeField] float deceleration = 35f;
    [SerializeField] float sprintFactor = 2f;
    [SerializeField] float cameraSensibility = 100f;
    [SerializeField] int coyoteMiliseconds = 100;
    [SerializeField] float interactionDistance = 3f;
    [SerializeField] float normalFOV = 75f;
    [SerializeField] float sprintFOV = 86f;
    [SerializeField] float fovTransitionSpeed = 4f;
    [SerializeField] float minGrabingDistance = 3f;
    public Camera playerCamera;
    public GameObject grabReference;
    UiController _uiController;

    //Distancia que deberá recorrer el jugador para que suene un sonido de pisada
    public float stepDistance = 1f;
    //Clip del sonido de pisada
    public AudioClip stepSound;
    //Contador de distancia recorrida tras el ultimo paso
    float stepDistanceCounter = 0f;
    //Referencia al sonido del personaje al caer
    public AudioClip landingSound;
    //Referencia del sonido del personaje al saltar
    public AudioClip jumpSound;
    public AudioClip changeMaskSound;
    public AudioClip newMaskSound;
    //Para comprobar si se encontrba tocando el suelo en el ciclo anterior
    private bool _previouslyGrounded = true;
    //Referencia al audiosource
    private AudioSource _audioSource;


    // ------ Player State ------------//
    Vector3 _currentSpeed;
    Vector3 _movement;
    float _verticalSpeed;
    Vector2 _cameraMovement;
    bool _coyoteGrounded = true;
    float _airSeconds = 0;
    bool _sprinting = false;

    float _cameraPitch;
    float _playerYaw;
    float _targetFOV;

    MaskColor _currentMask;
    bool[] _masksEnabled;
    public bool HasRedMask => _masksEnabled[1];
    public bool HasBlueMask => _masksEnabled[0];
    public bool HasYellowMask => _masksEnabled[2];
    LayerMask _currentLayer;

    GameObject _holdingObject;
    Collider _holdingObjectCollider;
    Rigidbody _holdingObjectBody;
    //----------------------------------//

    PlayerInput _inputs;
    CharacterController _controller;
    Transform _playerPosition;
    bool _initialiced = false;
    ChangeMaskAnimationController _maskAnimation;
    bool _changeOnGoing = false;
    bool _wasSprintingBeforeJumping = false;
    bool _tpPreparing = false;

    void Initialize()
    {
        _inputs = GetComponent<PlayerInput>();
        _controller = GetComponent<CharacterController>();
        _playerPosition = GetComponent<Transform>();
        _masksEnabled = new bool[] { false, false, false };
        _movement = new Vector3(0f, 0f, 0f);
        _holdingObject = null;
        _realMaxSpeed = maxSpeed * sprintFactor;
        _audioSource = GetComponent<AudioSource>();
        _playerYaw = _playerPosition.localEulerAngles.y;
        if (_inputs == null) throw new NullReferenceException("No player input found");
        if (playerCamera == null) throw new NullReferenceException("No player camera found");
        if (grabReference == null) throw new NullReferenceException("No grab reference found");
        _uiController = GetComponent<UiController>();
        if (_uiController == null) throw new NullReferenceException("No uiController found");
        _maskAnimation = playerCamera.GetComponentInChildren<ChangeMaskAnimationController>();
        if (_maskAnimation == null) throw new NullReferenceException("Animation not foud");
        StartCoroutine(ChangeMask(MaskColor.None));
        _initialiced = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!_initialiced) Initialize();
        Cursor.visible = false;
        // Locks the cursor
        Cursor.lockState = CursorLockMode.Locked;

        // Confines the cursor
        Cursor.lockState = CursorLockMode.Confined;

        // Initialize FOV
        playerCamera.fieldOfView = normalFOV;
        _targetFOV = normalFOV;
    }

    public void OnEnable()
    {
        if (!_initialiced) Initialize();
        // subscribe to the event
        _inputs.onActionTriggered += HandleInput;
    }

    public void OnDisable()
    {
        // unsubscribe to avoid memory leaks
        _inputs.onActionTriggered -= HandleInput;
    }

    public void TpStart()
    {
        _tpPreparing = true;
        DropObject();
    }

    public void TpEnd()
    {
        _tpPreparing = false;
    }

    private void HandleInput(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "Move":
                Vector2 temp = context.ReadValue<Vector2>();
                _movement.x = temp.x;
                _movement.z = temp.y;
                break;
            case "Jump":
                _movement.y = 1;
                break;
            case "Look":
                _cameraMovement = context.ReadValue<Vector2>();
                break;
            case "Sprint":
                _sprinting = context.ReadValueAsButton();
                // if (_coyoteGrounded || !tempSprint)
                // {
                //     _sprinting = tempSprint;
                //     Debug.Log("Sprinting");
                // }
                break;
            case "Interact":
                if (context.ReadValueAsButton()) Interact();
                break;
            case "BlueMask":
                if (_currentMask != MaskColor.Blue && _masksEnabled[0])
                {
                    Debug.Log("BlueMask Action");
                    StartCoroutine(ChangeMask(MaskColor.Blue));
                }
                break;
            case "RedMask":
                if (_currentMask != MaskColor.Red && _masksEnabled[1])
                {
                    Debug.Log("RedMask Action");
                    StartCoroutine(ChangeMask(MaskColor.Red));
                }
                break;
            case "YellowMask":
                if (_currentMask != MaskColor.Yellow && _masksEnabled[2])
                {
                    Debug.Log("YellowMask Action");
                    StartCoroutine(ChangeMask(MaskColor.Yellow));
                }
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!_tpPreparing)
        {
            PlayerMovement();
            CameraMovement();
            UpdateFOV();
            InteractableOnSight();
            Grabing();
        }
    }

    private void PlayerMovement()
    {
        if (!_controller.isGrounded && _airSeconds > coyoteMiliseconds && _coyoteGrounded)
        {
            _coyoteGrounded = false;
        }
        else if (!_controller.isGrounded && _airSeconds < coyoteMiliseconds)
        {
            _airSeconds += Time.deltaTime * 1000;
        }
        else if (_controller.isGrounded && (_airSeconds != 0 || !_coyoteGrounded))
        {
            _coyoteGrounded = true;
            _airSeconds = 0;
            _audioSource.PlayOneShot(landingSound);
        }

        bool shouldSprint = _sprinting && _coyoteGrounded;
        bool mantainSpeed = (_wasSprintingBeforeJumping && !_coyoteGrounded) || shouldSprint;
        Vector3 desiredLocal = new Vector3(_movement.x, 0f, _movement.z);
        desiredLocal = desiredLocal.normalized * (mantainSpeed ? _realMaxSpeed : maxSpeed);

        Vector3 desiredWorld = _playerPosition.TransformDirection(desiredLocal);

        float accel = (desiredWorld.sqrMagnitude > 0.001f) ? (shouldSprint ? acceleration * sprintFactor : acceleration) : (_coyoteGrounded ? deceleration : 0f);
        _currentSpeed = Vector3.MoveTowards(_currentSpeed, desiredWorld, accel * Time.deltaTime);

        // Jump + gravedad
        if (_coyoteGrounded)
        {

            if (_controller.isGrounded) _verticalSpeed = -1f;
            else _verticalSpeed = 0;// mantiene pegado al suelo
            if (_movement.y == 1)
            {
                _verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
                _coyoteGrounded = false;
                _wasSprintingBeforeJumping = _sprinting;
                _audioSource.PlayOneShot(jumpSound);
            }
        }
        else
        {
            var hittedCeiling = Physics.Raycast(transform.position, new Vector3(0, 1, 0), 1.1f, _currentLayer);
            if (hittedCeiling)
            {
                _verticalSpeed = -1f;
            }
        }


        _verticalSpeed += gravity * Time.deltaTime;
        _movement.y = 0;
        _currentSpeed.y = _verticalSpeed;

        _controller.Move(_currentSpeed * Time.deltaTime);

        //Si el player está grounded, el contador se suma
        if (_coyoteGrounded)
        {
            //Distancia que recorrido el jugador
            stepDistanceCounter += new Vector3(_currentSpeed.x, 0f, _currentSpeed.z).magnitude * Time.deltaTime;
        }
        //Si la distancia recorrida es igual o superior a la distancia de una zancada, reseteamos el contador
        if (stepDistanceCounter >= stepDistance)
        {
            stepDistanceCounter = -1f;
            //Le damos un pitch random al audiosource para que el sonido no sea muy repetitivo
            _audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            //Reproducimos el sonido de pisada
            _audioSource.PlayOneShot(stepSound);

        }
    }

    private void CameraMovement()
    {
        _cameraPitch = Mathf.Clamp(_cameraPitch + _cameraMovement.y * cameraSensibility * Time.deltaTime, -80f, 85f);
        _playerYaw += _cameraMovement.x * cameraSensibility * Time.deltaTime;

        playerCamera.transform.localRotation = Quaternion.Euler(_cameraPitch * -1, 0f, 0f);
        _playerPosition.localRotation = Quaternion.Euler(0f, _playerYaw, 0f);
    }

    private void UpdateFOV()
    {
        bool mantainSpeed = (_wasSprintingBeforeJumping && !_coyoteGrounded) || _sprinting && _coyoteGrounded;
        _targetFOV = mantainSpeed ? sprintFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, _targetFOV, fovTransitionSpeed * Time.deltaTime);
    }



    public void EnablePlayerMask(MaskColor mask)
    {
        switch (mask)
        {
            case MaskColor.Blue:
                _masksEnabled[0] = true;
                break;
            case MaskColor.Red:
                _masksEnabled[1] = true;
                break;
            case MaskColor.Yellow:
                _masksEnabled[2] = true;
                break;
        }
        _uiController.MaskEnabled(mask);
        _audioSource.PlayOneShot(newMaskSound);
    }

    public void DisablePlayerMask(MaskColor mask)
    {
        switch (mask)
        {
            case MaskColor.Blue:
                _masksEnabled[0] = false;
                break;
            case MaskColor.Red:
                _masksEnabled[1] = false;
                break;
            case MaskColor.Yellow:
                _masksEnabled[2] = false;
                break;
        }
        _uiController.MaskDisabled(mask);
    }

    public IEnumerator ChangeMask(MaskColor mask)
    {
        if (!_changeOnGoing) _changeOnGoing = true;
        else yield break;

        if (mask != MaskColor.None)
        {
            _maskAnimation.ChangeMaskTransition(mask);
            _audioSource.PlayOneShot(changeMaskSound);
            yield return new WaitForSeconds(_maskAnimation.ChangeMaskTransitionAnimationLength());
        }
        _currentMask = mask;
        _changeOnGoing = false;
        _uiController.ChangeMaskUI(_currentMask);
        switch (mask)
        {
            case MaskColor.None:
                _controller.excludeLayers = LayerMask.GetMask(new string[] { CollitionLayerName.BlueLayer, CollitionLayerName.RedLayer, CollitionLayerName.YellowLayer });
                _currentLayer = LayerMask.GetMask(new string[] { CollitionLayerName.BaseLayer });
                break;
            case MaskColor.Blue:
                _controller.excludeLayers = LayerMask.GetMask(new string[] { CollitionLayerName.RedLayer, CollitionLayerName.YellowLayer });
                _currentLayer = LayerMask.GetMask(new string[] { CollitionLayerName.BaseLayer, CollitionLayerName.BlueLayer });
                break;
            case MaskColor.Red:
                _controller.excludeLayers = LayerMask.GetMask(new string[] { CollitionLayerName.BlueLayer, CollitionLayerName.YellowLayer });
                _currentLayer = LayerMask.GetMask(new string[] { CollitionLayerName.BaseLayer, CollitionLayerName.RedLayer });
                break;
            case MaskColor.Yellow:
                _controller.excludeLayers = LayerMask.GetMask(new string[] { CollitionLayerName.BlueLayer, CollitionLayerName.RedLayer });
                _currentLayer = LayerMask.GetMask(new string[] { CollitionLayerName.BaseLayer, CollitionLayerName.YellowLayer });
                break;
        }
        if (_holdingObject != null) DropObject();
        GameController.ChangeMask(this.gameObject, mask);
        _controller.includeLayers = _currentLayer;
    }

    private void Interact()
    {
        if (_holdingObject != null)
        {
            DropObject();
            return;
        }

        RaycastHit hitted;
        bool collided = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hitted, interactionDistance, _currentLayer, QueryTriggerInteraction.Ignore);
        if (collided)
        {
            try
            {
                var interactable = hitted.collider.gameObject.GetComponent<Interactable>();
                if (interactable) Grab(hitted);
            }
            catch { }
        }
    }

    private void Grab(RaycastHit interactableObject)
    {
        _holdingObject = interactableObject.rigidbody.gameObject;
        _holdingObjectBody = interactableObject.rigidbody;
        _holdingObjectBody.useGravity = false;
        _holdingObjectBody.freezeRotation = true;
        _holdingObjectBody.isKinematic = true;
        _holdingObjectBody.interpolation = RigidbodyInterpolation.Interpolate;
        _holdingObjectCollider = interactableObject.rigidbody.gameObject.GetComponent<Collider>();
        _holdingObjectCollider.enabled = false;
    }

    private void Grabing()
    {
        if (!_holdingObject) return;
        // Debug.Log(CameraUtils.IsVisibleByCamera(_holdingObject, playerCamera,false, _currentLayer));
        if (!CameraUtils.IsVisibleByCamera(_holdingObject, playerCamera, false, _currentLayer))
        {
            DropObject();
            return;
        }

        Vector3 targetPos = grabReference.transform.position;
        Vector3 camPos = playerCamera.transform.position;
        float dist = Vector3.Distance(camPos, targetPos);

        if (dist < minGrabingDistance)
        {
            targetPos = camPos + playerCamera.transform.forward.normalized * minGrabingDistance;
        }

        // Check for collisions between camera and target position using current layer
        Vector3 directionToTarget = (targetPos - camPos).normalized;
        float distanceToTarget = Vector3.Distance(camPos, targetPos);

        // Raycast with current layer to detect walls/obstacles
        if (Physics.Raycast(camPos, directionToTarget, out RaycastHit hit, distanceToTarget, _currentLayer, QueryTriggerInteraction.Ignore))
        {
            // Ignore if the hit object is the grabbed object itself
            if (hit.collider.gameObject != _holdingObject)
            {
                // Place object just before the collision point with proper offset
                float safeDistance = Mathf.Max(hit.distance - 0.5f, minGrabingDistance);
                targetPos = camPos + directionToTarget * safeDistance;
            }
        }

        // Check if object position would be inside a collider using box overlap
        Vector3 boxHalfExtents = _holdingObject.transform.lossyScale / 2f;
        Collider[] collidersAtPosition = Physics.OverlapBox(targetPos, boxHalfExtents, _holdingObject.transform.rotation, _currentLayer, QueryTriggerInteraction.Ignore);

        // If overlapping with a collider, don't move the object (keep previous position)
        bool isOverlapping = false;
        foreach (Collider col in collidersAtPosition)
        {
            if (col.gameObject != _holdingObject)
            {
                isOverlapping = true;
                break;
            }
        }

        if (!isOverlapping)
        {
            _holdingObjectBody.position = targetPos;
        }
    }
    private void DropObject()
    {
        if (_holdingObject == null) return;
        _holdingObjectBody.useGravity = true;
        _holdingObjectBody.freezeRotation = false;
        _holdingObjectCollider.enabled = true;
        _holdingObjectBody.isKinematic = false;
        _holdingObjectBody.interpolation = RigidbodyInterpolation.None;
        _holdingObject = null;
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }

    private void InteractableOnSight()
    {
        RaycastHit hitted;
        bool collided = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hitted, interactionDistance, _currentLayer, QueryTriggerInteraction.Ignore);
        if (collided)
        {
            try
            {
                var interactable = hitted.collider.gameObject.GetComponent<Interactable>();
                if (interactable)
                {
                    _uiController.InteractableOnSight();
                    return;
                }
            }
            catch { }
        }
        _uiController.NoInteractableOnSight();
    }

    public bool HasMask(MaskColor mask)
    {
        switch (mask)
        {
            case MaskColor.Red:
                return _masksEnabled[1];
            case MaskColor.Blue:
                return _masksEnabled[0];
            case MaskColor.Yellow:
                return _masksEnabled[2];
            default:
                return false;
        }
    }
}

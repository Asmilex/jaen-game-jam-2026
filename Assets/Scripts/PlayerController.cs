using System;
using NetworkMask.Constants;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float gravity = -25f;
    // [SerializeField] float maxCameraSpeed = 15f;
    [SerializeField] float maxSpeed = 6f;
    [SerializeField] float acceleration = 20f;
    [SerializeField] float jumpHeight = 1.6f;
    [SerializeField] float deceleration = 25f;
    [SerializeField] float cameraSensibility = 20f;    
    [SerializeField] int coyoteMiliseconds = 20;
    public Camera playerCamera;

    Vector3 _currentSpeed;
    Vector3 _movement;
    float _verticalSpeed;
    Vector2 _cameraMovement;
    Vector3 _currentCameraSpeed;
    float _cameraPitch;
    float _playerYaw;
    bool _sprinting = false;
    MaskColor _currentMask;

    bool _coyoteGrounded = true;
    float _airSeconds = 0;
    PlayerInput _inputs;
    CharacterController _controller;
    Transform _playerPosition;
    bool _initialiced = false;

    void Initialize()
    {
        _inputs = GetComponent<PlayerInput>();
        _controller = GetComponent<CharacterController>();
        _playerPosition = GetComponent<Transform>();
        _movement = new Vector3(0f,0f,0f);
        if (_inputs == null) throw new NullReferenceException("No player input found");  
        if (playerCamera == null) throw new NullReferenceException("No player camera found");
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
    }

    public void OnEnable() {
        if (!_initialiced) Initialize();
        // subscribe to the event
        _inputs.onActionTriggered += HandleInput;
    }

    public void OnDisable() {
        // unsubscribe to avoid memory leaks
        _inputs.onActionTriggered -= HandleInput;
    }

    private void HandleInput(InputAction.CallbackContext context) {
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
                break;
            case "BlueMask":
                if (_currentMask != MaskColor.Blue)
                {
                    _currentMask = MaskColor.Blue;
                } else
                {
                    _currentMask = MaskColor.None;
                }
                GameController.ChangeMask(this.gameObject, _currentMask);
                break;
            case "RedMask":
                if (_currentMask != MaskColor.Red)
                {
                    _currentMask = MaskColor.Red;
                } else
                {
                    _currentMask = MaskColor.None;
                }
                GameController.ChangeMask(this.gameObject, _currentMask);
                break;
            case "GreenMask":
                if (_currentMask != MaskColor.Green)
                {
                    _currentMask = MaskColor.Green;
                } else
                {
                    _currentMask = MaskColor.None;
                }
                GameController.ChangeMask(this.gameObject, _currentMask);
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        CameraMovement();
    }

    private void PlayerMovement()
    {
        Vector3 desiredLocal = new Vector3(_movement.x, 0f, _movement.z);
        desiredLocal = desiredLocal.normalized * (_sprinting ? maxSpeed*3f : maxSpeed);


        Vector3 desiredWorld = _playerPosition.TransformDirection(desiredLocal); 

        float accel = (desiredWorld.sqrMagnitude > 0.001f) ? (_sprinting ? acceleration * 3f : acceleration) : deceleration;
        _currentSpeed = Vector3.MoveTowards(_currentSpeed, desiredWorld, accel * Time.deltaTime);

        if (!_controller.isGrounded && _airSeconds > coyoteMiliseconds && _coyoteGrounded)
        {
            _coyoteGrounded = false;   
            Debug.Log($"End Coyote Time | {_coyoteGrounded}");
        } else if (!_controller.isGrounded && _airSeconds < coyoteMiliseconds)
        {
            _airSeconds += Time.deltaTime*1000;
            Debug.Log($"Coyote Time entered - {_airSeconds}|{coyoteMiliseconds} - {_coyoteGrounded}");
        } else if (_controller.isGrounded && _airSeconds != 0)
        {
            _coyoteGrounded = true;
            _airSeconds = 0;
            Debug.Log($"Grounded | {_coyoteGrounded}");
        }

        // Jump + gravedad
        if (_coyoteGrounded)
        {
            if (_controller.isGrounded)_verticalSpeed = -1f; 
            else _verticalSpeed = 0;// mantiene pegado al suelo
            if (_movement.y == 1) 
            {
                Debug.Log("Jumping");
                _verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
                _coyoteGrounded = false;
            }
        }

        
        _verticalSpeed += gravity * Time.deltaTime;
        _movement.y = 0;
        _currentSpeed.y = _verticalSpeed;

        _controller.Move(_currentSpeed * Time.deltaTime);
    }

    private void CameraMovement()
    {
        _cameraPitch = Mathf.Clamp(_cameraPitch + _cameraMovement.y * cameraSensibility * Time.deltaTime, -80f, 85f);
        _playerYaw += _cameraMovement.x * cameraSensibility * Time.deltaTime;

        playerCamera.transform.localRotation = Quaternion.Euler(_cameraPitch* -1, 0f, 0f);
        _playerPosition.localRotation = Quaternion.Euler(0f, _playerYaw, 0f);
    }
}

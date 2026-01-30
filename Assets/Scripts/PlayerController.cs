using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public Camera playerCamera;

    Vector3 _currentSpeed;
    Vector3 _movement;
    float _verticalSpeed;
    Vector2 _cameraMovement;
    Vector3 _currentCameraSpeed;
    float _cameraPitch;
    float _playerYaw;

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
                Debug.Log(_cameraMovement);
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
        desiredLocal = desiredLocal.normalized * maxSpeed;

        Vector3 desiredWorld = _playerPosition.TransformDirection(desiredLocal); 

        float accel = (desiredWorld.sqrMagnitude > 0.001f) ? acceleration : deceleration;
        _currentSpeed = Vector3.MoveTowards(_currentSpeed, desiredWorld, accel * Time.deltaTime);

        // Jump + gravedad
        if (_controller.isGrounded)
        {
            _verticalSpeed = -1f; // mantiene pegado al suelo
            if (_movement.y == 1)
                _verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        _verticalSpeed += gravity * Time.deltaTime;
        _movement.y = 0;
        _currentSpeed.y = _verticalSpeed;

        _controller.Move(_currentSpeed * Time.deltaTime);
    }

    private void CameraMovement()
    {
        _cameraPitch = Mathf.Clamp(_cameraPitch + _cameraMovement.y * cameraSensibility * Time.deltaTime, -70f, 70f);
        _playerYaw += _cameraMovement.x * cameraSensibility * Time.deltaTime;

        playerCamera.transform.localRotation = Quaternion.Euler(_cameraPitch* -1, 0f, 0f);
        _playerPosition.localRotation = Quaternion.Euler(0f, _playerYaw, 0f);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    PlayerInput _inputs;
    CharacterController _controller;
    bool _initialiced = false;

    void Initialize()
    {
        _inputs = GetComponent<PlayerInput>();
        _controller = GetComponent<CharacterController>();
        if (_inputs is null) throw new UnityException("No player input found");  
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
                Vector2 movement = context.ReadValue<Vector2>();
                _controller.Move(new Vector3(movement.x, 0f, movement.y));
                break;
            case "Look":
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

using NetworkMask.Interactive;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PressurePlate))]
public class Activate : MonoBehaviour
{
    private Animator _animator;
    private PressurePlate _pressurePlate;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _pressurePlate = GetComponent<PressurePlate>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (_pressurePlate.IsActive)
        {
            _animator.speed = 1;
            _animator.SetTrigger("Activate");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (!_pressurePlate.IsActive)
        {
            _animator.speed = -1;
            _animator.SetTrigger("Activate");
        }
    }
}

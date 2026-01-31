using NetworkMask.Interfaces;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LevelDoors : MonoBehaviour, IActivable
{
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void Activate()
    {
        _animator.SetTrigger("Open");
    }

    public void Deactivate()
    {
        _animator.SetTrigger("Close");
    }
}

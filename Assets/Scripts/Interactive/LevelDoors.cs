using NetworkMask.Interfaces;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LevelDoors : MonoBehaviour, IActivable
{
    private Animator _animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip doorAudioClip;

    void Start()
    {
        _animator = this.gameObject.GetComponent<Animator>();
    }

    public void Activate()
    {
        _animator.SetTrigger("Open");
        DoorSound();
    }

    public void Deactivate()
    {
        _animator.SetTrigger("Close");
        DoorSound();
    }

    private void DoorSound()
    {
        audioSource.PlayOneShot(doorAudioClip);
    }
}

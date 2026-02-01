using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Interactable : MonoBehaviour
{

    private AudioClip _metalSound;
    private AudioSource _audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _metalSound = Resources.Load<AudioClip>("Sounds/Cube");
        _audioSource.volume = 0.5f;
        _audioSource.pitch = UnityEngine.Random.Range(0f, 3f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(_metalSound);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

using UnityEngine;
using NetworkMask.Mask;

[RequireComponent(typeof(Collider))]
public class ObjectColorConverter: MonoBehaviour
{
    [Tooltip("The new colored mask script to be used for the converter.")]
    public ColoredObject NewColoredMask;
    private Collider _collider;

    public void Start()
    {
        _collider = GetComponent<Collider>();
        if (NewColoredMask == null)
        {
            Debug.LogError("NewColoredMask is not assigned in ColorConverter on " + gameObject.name);
            _collider.enabled = false; 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var coloredObject = other.GetComponent<ColoredObject>();
        if (coloredObject == null)
        {
            Debug.LogWarning("The object " + other.gameObject.name + " does not have a ColoredObject component.");
            return;
        }
        Destroy(coloredObject);
        other.gameObject.AddComponent(NewColoredMask.GetType());
    }
}
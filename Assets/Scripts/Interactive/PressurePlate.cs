using System.Linq;
using System.Threading.Tasks;
using NetworkMask.Constants;
using NetworkMask.Interfaces;
using NetworkMask.Mask;
using UnityEngine;

namespace NetworkMask.Interactive
{
    [RequireComponent(typeof(Collider))]
    public class PressurePlate : MonoBehaviour
    {
        [Tooltip("The target object to activate/deactivate when the pressure plate is activated.")]
        public GameObject TargetObject = null;

        [Tooltip("If true, the pressure plate will stay activated once triggered.")]
        public bool ActiveOnce = false;

        [Tooltip("The color of the pressure plate. None will affect all colors.")]
        public MaskColor PlateColor = MaskColor.None;

        private bool _isActive = false;
        public bool IsActive => _isActive;

        private ushort _ElementCount = 0;

        void Start()
        {
            if (TargetObject == null)
            {
                Debug.LogWarning("TargetObject is not assigned in PressurePlate on " + gameObject.name);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            _ElementCount++;
            var otherColor = other.GetComponent<ColoredObject>()?.MaskColor ?? MaskColor.None;
            if (otherColor != PlateColor && !other.CompareTag("Player"))
            {
                return;
            }

            if (_isActive)
            {
                return;
            }

            _isActive = true;
            var activables = TargetObject.GetComponents<IActivable>() ?? Enumerable.Empty<IActivable>();
            Parallel.ForEach(activables, c => c.Activate());
        }

        private void OnTriggerExit(Collider other)
        {
            _ElementCount = (ushort)Mathf.Max(0, _ElementCount - 1);
            var otherColor = other.GetComponent<ColoredObject>()?.MaskColor ?? MaskColor.None;
            if (otherColor != PlateColor && !other.CompareTag("Player"))
            {
                return;
            }

            if (ActiveOnce)
            {
                return;
            }

            if (_ElementCount > 0)
            {
                return;
            }

            _isActive = false;
            var activables = TargetObject.GetComponents<IActivable>() ?? Enumerable.Empty<IActivable>();
            Parallel.ForEach(activables, c => c.Deactivate());
        }
    }
}
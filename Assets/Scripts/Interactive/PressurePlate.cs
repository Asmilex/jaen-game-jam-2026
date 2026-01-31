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
        public GameObject TargetObject;

        [Tooltip("If true, the pressure plate will stay activated once triggered.")]
        public bool ActiveOnce = false;

        [Tooltip("The color of the pressure plate. None will affect all colors.")]
        public MaskColor PlateColor = MaskColor.None;

        private bool _isActive = false;

        void Start()
        {
            if (TargetObject == null)
            {
                Debug.LogError("TargetObject is not assigned in PressurePlate on " + gameObject.name);
                enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var otherColor = other.GetComponent<ColoredObject>()?.MaskColor ?? MaskColor.None;
            if (otherColor != PlateColor)
            {
                return;
            }

            if (_isActive)
            {
                return;
            }

            _isActive = true;
            var activables = TargetObject.GetComponents<IActivable>();
            Parallel.ForEach(activables, c => c.Activate());
        }

        private void OnTriggerExit(Collider other)
        {
            var otherColor = other.GetComponent<ColoredObject>()?.MaskColor ?? MaskColor.None;
            if (otherColor != PlateColor)
            {
                return;
            }

            if (ActiveOnce)
            {
                return;
            }

            _isActive = false;
            var activables = TargetObject.GetComponents<IActivable>();
            Parallel.ForEach(activables, c => c.Deactivate());
        }
    }
}

using NetworkMask.Constants;
using NetworkMask.Utils;
using UnityEngine;

namespace NetworkMask.Mask
{
    [RequireComponent(typeof(Collider))]
    public abstract class ColoredObject: MonoBehaviour
    {
        public abstract MaskColor MaskColor { get; }
        private Collider _maskCollider;
        private static LayerMask BaseCollitionLayer = LayerMask.GetMask(CollitionLayerName.BaseLayer, CollitionLayerName.PlayerLayer);

        void Start()
        {
            _maskCollider = GetComponent<Collider>();
            GameController.OnMaskChange += OnColorChange;
        }

        void OnColorChange(GameObject sender, MaskChangeEventArgs args)
        {
            if (sender != gameObject) return;

            if (args.NewColor == MaskColor)
            {
                _maskCollider.includeLayers = BaseCollitionLayer | LayerMask.GetMask(CollitionLayerConverters.GetCollitionLayerNameFromColor(MaskColor));
            }
            else
            {
                _maskCollider.includeLayers = BaseCollitionLayer;
            }
        }

        void OnDestroy()
        {
            GameController.OnMaskChange -= OnColorChange;
        }
    }
}

using NetworkMask.Constants;
using NetworkMask.Utils;
using UnityEngine;

namespace NetworkMask.Mask
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Collider))]
    public abstract class ColoredObject: MonoBehaviour
    {
        public abstract MaskColor MaskColor { get; }
        private MeshRenderer _renderer;
        private Collider _maskCollider;

        void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
            _maskCollider = GetComponent<Collider>();
            GameController.OnMaskChange += OnColorChange;
            _renderer.renderingLayerMask = RenderingLayerMask.GetMask(RenderLayerConverters.GetRenderLayerNameFromColor(MaskColor));
        }

        void OnColorChange(GameObject sender, MaskChangeEventArgs args)
        {
            var mask = CollitionLayerConverters.GetCollitionLayerNameFromColor(MaskColor);
            if (args.NewColor != MaskColor || string.IsNullOrEmpty(mask))
            {
                _maskCollider.includeLayers = BaseLayer.CollitionBaseLayer;
            }
            _maskCollider.includeLayers = BaseLayer.CollitionBaseLayer | LayerMask.GetMask(mask);
        }

        public void ChangeMaterial(Material newMaterial)
        {
            if (_renderer != null && newMaterial != null)
            {
                _renderer.material = newMaterial;
            }
        }

        void OnDestroy()
        {
            GameController.OnMaskChange -= OnColorChange;
        }
    }
}
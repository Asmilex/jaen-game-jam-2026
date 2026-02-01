
using NetworkMask.Constants;
using NetworkMask.Utils;
using UnityEngine;

namespace NetworkMask.Mask
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Collider))]
    public abstract class ColoredObject : MonoBehaviour
    {
        public abstract MaskColor MaskColor { get; }
        private MeshRenderer _renderer;
        private Collider[] _maskCollider;
        private LayerMask _mask;

        void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
            _maskCollider = GetComponents<Collider>();
            GameController.OnMaskChange += OnColorChange;
            _renderer.renderingLayerMask = RenderingLayerMask.GetMask(RenderLayerConverters.GetRenderLayerNameFromColor(MaskColor));
            _mask = LayerMask.GetMask(CollitionLayerConverters.GetCollitionLayerNameFromColor(MaskColor));
            _renderer.material = RenderLayerConverters.GetRenderLayerMaterialFromColor(MaskColor);
            gameObject.layer = CollitionLayerConverters.GetCollitionLayerIndexFromColor(MaskColor);
            var mask = BaseLayer.CollitionBaseLayer | _mask;
            foreach (var collider in _maskCollider)
            {
                collider.includeLayers = mask;
            }

            OnColorChange(this.gameObject, new MaskChangeEventArgs { NewColor = GameController.CurrentMaskColor });
        }

        void OnColorChange(GameObject sender, MaskChangeEventArgs args)
        {
            var mask = BaseLayer.CollitionBaseLayer | _mask;
            if (args.NewColor == MaskColor)
            {
                mask |= BaseLayer.BasePlayerLayer;
            }
            foreach (var collider in _maskCollider)
            {
                collider.includeLayers = mask;
            }
        }

        void OnDestroy()
        {
            GameController.OnMaskChange -= OnColorChange;
        }
    }
}
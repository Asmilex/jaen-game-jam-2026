using UnityEngine;

namespace NetworkMask.Constants
{
    public enum MaskColor
    {
        None = 0,
        Red = 8,
        Blue = 9,
        Green = 10
    }

    public static class CollitionLayerName
    {
        public const string BaseLayer = "Base";
        public const string PlayerLayer = "Player";
        public const string RedLayer = "RedMask";
        public const string BlueLayer = "BlueMask";
        public const string GreenLayer = "GreenMask";
    }

    public enum CollitionLayer
    {
        BaseLayer = 6,
        PlayerLayer = 7,
        RedLayer = 8,
        BlueLayer = 9,
        GreenLayer = 10,
    }

    public static class RenderMaskLayerName
    {
        public const string DefaultMaskLayer = "Default";
        public const string RedMaskLayer = "RedMask";
        public const string BlueMaskLayer = "BlueMask";
        public const string GreenMaskLayer = "GreenMask";
        public const string Animations = "Animations";
    }

    public enum RenderMaskLayer
    {
        DefaultMaskLayer = 0,
        RedMaskLayer = 1,
        BlueMaskLayer = 2,
        GreenMaskLayer = 3,
        Animations = 4,
    }

    public static class RenderMaterialName
    {
        public const string RedColorMaterial = "RedColorMaterial";
        public const string BlueColorMaterial = "BlueColorMaterial";
        public const string GreenColorMaterial = "GreenColorMaterial";
    }
    
    public static class BaseLayer
    {
        public static LayerMask CollitionBaseLayer = LayerMask.GetMask(CollitionLayerName.BaseLayer, CollitionLayerName.PlayerLayer);
        public static LayerMask CullingBaseLayer = LayerMask.GetMask(RenderMaskLayerName.DefaultMaskLayer);
        public static RenderingLayerMask renderingLayerMask = RenderingLayerMask.GetMask(RenderMaskLayerName.DefaultMaskLayer);
    }
}
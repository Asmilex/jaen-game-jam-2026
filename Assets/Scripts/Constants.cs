using UnityEngine;

namespace NetworkMask.Constants
{
    public enum MaskColor
    {
        None = 0,
        Red = 8,
        Blue = 9,
        Yellow = 10
    }

    public static class CollitionLayerName
    {
        public const string BaseLayer = "Base";
        public const string PlayerLayer = "Player";
        public const string RedLayer = "RedMask";
        public const string BlueLayer = "BlueMask";
        public const string YellowLayer = "YellowMask";
    }

    public enum CollitionLayer
    {
        BaseLayer = 6,
        PlayerLayer = 7,
        RedLayer = 8,
        BlueLayer = 9,
        YellowLayer = 10,
    }

    public static class RenderMaskLayerName
    {
        public const string DefaultMaskLayer = "Default";
        public const string RedMaskLayer = "RedMask";
        public const string BlueMaskLayer = "BlueMask";
        public const string YellowMaskLayer = "YellowMask";
        public const string Animations = "Animations";
    }

    public enum RenderMaskLayer
    {
        DefaultMaskLayer = 0,
        RedMaskLayer = 1,
        BlueMaskLayer = 2,
        YellowMaskLayer = 3,
        Animations = 4,
    }

    public static class RenderMaterialName
    {
        public const string RedColorMaterial = "RedColorMaterial";
        public const string BlueColorMaterial = "BlueColorMaterial";
        public const string YellowColorMaterial = "YellowColorMaterial";
    }
    
    public static class BaseLayer
    {
        public static LayerMask CollitionBaseLayer = LayerMask.GetMask(CollitionLayerName.BaseLayer, "Default");
        public static LayerMask BasePlayerLayer = LayerMask.GetMask(CollitionLayerName.BaseLayer, CollitionLayerName.PlayerLayer, "Default");
        public static RenderingLayerMask renderingLayerMask = RenderingLayerMask.GetMask(RenderMaskLayerName.DefaultMaskLayer);
    }
}
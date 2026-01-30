namespace NetworkMask.Constants
{
    public enum MaskColor
    {
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
    
}
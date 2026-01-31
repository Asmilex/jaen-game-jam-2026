using NetworkMask.Constants;
using UnityEngine;

namespace NetworkMask.Utils
{
    public static class CollitionLayerConverters
    {
        public static string GetCollitionLayerNameFromColor(MaskColor color)
        {
            var result = color switch
            {
                MaskColor.Red => CollitionLayerName.RedLayer,
                MaskColor.Blue => CollitionLayerName.BlueLayer,
                MaskColor.Yellow => CollitionLayerName.YellowLayer,
                _ => string.Empty
            };
            return result;
        }

        public static int GetCollitionLayerIndexFromColor(MaskColor color)
        {
            var result = color switch
            {
                MaskColor.Red => CollitionLayer.RedLayer,
                MaskColor.Blue => CollitionLayer.BlueLayer,
                MaskColor.Yellow => CollitionLayer.YellowLayer,
                _ => default
            };
            return (int)result;
        }
    }
}
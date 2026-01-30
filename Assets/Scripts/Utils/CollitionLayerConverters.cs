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
                MaskColor.Green => CollitionLayerName.GreenLayer,
                _ => string.Empty
            };
            return result;
        }
    }
}
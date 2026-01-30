using NetworkMask.Constants;
using UnityEngine;

namespace NetworkMask.Utils
{
    public static class RenderLayerConverters
    {
        public static string GetRenderLayerNameFromColor(MaskColor color)
        {
            var result = color switch
            {
                MaskColor.Red => RenderMaskLayerName.RedMaskLayer,
                MaskColor.Blue => RenderMaskLayerName.BlueMaskLayer,
                MaskColor.Green => RenderMaskLayerName.GreenMaskLayer,
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(result))
            {
                Debug.LogError($"No render layer name found for color {color}");
            }
            return result;
        }
    }
}
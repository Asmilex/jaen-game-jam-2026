using NetworkMask.Constants;
using UnityEngine;

namespace NetworkMask.Utils
{
    public static class RenderLayerConverters
    {
        public const string MaterialColorNameSuffix = "ColorMaterial";

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

        public static Material GetRenderLayerMaterialFromColor(MaskColor color)
        {
            var materialName = color switch
            {
                MaskColor.Red => RenderMaterialName.RedColorMaterial,
                MaskColor.Blue => RenderMaterialName.BlueColorMaterial,
                MaskColor.Green => RenderMaterialName.GreenColorMaterial,
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(materialName))
            {
                Debug.LogError($"No material name found for color {color}");
                return null;
            }

            var material = Resources.Load<Material>($"Materials/Colors/{materialName}");
            return material;
        }
    }
}
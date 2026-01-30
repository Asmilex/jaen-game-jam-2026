using NetworkMask.Constants;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCameraMaskController : MonoBehaviour
{
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = GetComponent<Camera>();
        GameController.OnMaskChange += OnMaskChange;
    }

    void OnMaskChange(GameObject sender, MaskChangeEventArgs args)
    {

        _mainCamera.cullingMask = args.NewColor switch
        {
            MaskColor.Red => BaseLayer.CullingBaseLayer | LayerMask.GetMask(RenderMaskLayerName.RedMaskLayer),
            MaskColor.Blue => BaseLayer.CullingBaseLayer | LayerMask.GetMask(RenderMaskLayerName.BlueMaskLayer),
            MaskColor.Green => BaseLayer.CullingBaseLayer | LayerMask.GetMask(RenderMaskLayerName.GreenMaskLayer),
            _ => BaseLayer.CullingBaseLayer,
        };
    }
}
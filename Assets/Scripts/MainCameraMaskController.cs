using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCameraMaskController : MonoBehaviour
{
    private static LayerMask DefaultMaskLayer = LayerMask.GetMask(NetworkMask.Constants.RenderMaskLayerName.DefaultMaskLayer);
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
            NetworkMask.Constants.MaskColor.Red => DefaultMaskLayer | LayerMask.GetMask(NetworkMask.Constants.RenderMaskLayerName.RedMaskLayer),
            NetworkMask.Constants.MaskColor.Blue => DefaultMaskLayer | LayerMask.GetMask(NetworkMask.Constants.RenderMaskLayerName.BlueMaskLayer),
            NetworkMask.Constants.MaskColor.Green => DefaultMaskLayer | LayerMask.GetMask(NetworkMask.Constants.RenderMaskLayerName.GreenMaskLayer),
            _ => (int)DefaultMaskLayer,
        };
    }
}
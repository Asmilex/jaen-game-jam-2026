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
        _mainCamera.cullingMask = BaseLayer.BasePlayerLayer;
    }

    void OnMaskChange(GameObject sender, MaskChangeEventArgs args)
    {

        _mainCamera.cullingMask = args.NewColor switch
        {
            MaskColor.Red => BaseLayer.BasePlayerLayer | LayerMask.GetMask(CollitionLayerName.RedLayer),
            MaskColor.Blue => BaseLayer.BasePlayerLayer | LayerMask.GetMask(CollitionLayerName.BlueLayer),
            MaskColor.Yellow => BaseLayer.BasePlayerLayer | LayerMask.GetMask(CollitionLayerName.YellowLayer),
            _ => BaseLayer.BasePlayerLayer,
        };
    }
}
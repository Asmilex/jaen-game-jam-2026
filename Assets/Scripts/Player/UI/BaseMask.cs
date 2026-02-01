
using NetworkMask.Constants;
using UnityEngine;

public class BaseMask : MonoBehaviour
{

    protected MaskColor Mask;

    public BaseMask(MaskColor mask)
    {
        Mask = mask;
    }
    public void OnChangeMask(MaskColor mask)
    {
        Debug.Log(mask);
    }
}
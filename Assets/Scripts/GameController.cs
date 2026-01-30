using NetworkMask.Constants;
using UnityEngine;
public record MaskChangeEventArgs
{
    public MaskColor NewColor;
}

public static class GameController
{
    public delegate void MaskChangeHandler(GameObject sender, MaskChangeEventArgs args);
    public static event MaskChangeHandler OnMaskChange;

    public static void ChangeMask(GameObject sender, MaskColor newColor)
    {
        OnMaskChange?.Invoke(sender, new MaskChangeEventArgs { NewColor = newColor });
    }
}

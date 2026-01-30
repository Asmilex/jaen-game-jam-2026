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
}

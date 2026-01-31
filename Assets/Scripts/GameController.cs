using System.Collections.Generic;
using NetworkMask.Constants;
using UnityEngine;

public record MaskChangeEventArgs
{
    public MaskColor NewColor;
}

public record GameObjectStatus
{
    public Transform Transform;
    public bool IsActive;
}

public class GameController : MonoBehaviour
{
    public delegate void MaskChangeHandler(GameObject sender, MaskChangeEventArgs args);
    public static event MaskChangeHandler OnMaskChange;

    public static void ChangeMask(GameObject sender, MaskColor newColor)
    {
        OnMaskChange?.Invoke(sender, new MaskChangeEventArgs { NewColor = newColor });
    }

    private Dictionary<string, GameObjectStatus> gameObjectStatuses = new Dictionary<string, GameObjectStatus>();

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SaveGameObjectStatus(string name, Transform transform, bool isActive)
    {
        gameObjectStatuses[name] = new GameObjectStatus { Transform = transform, IsActive = isActive };
    }
}

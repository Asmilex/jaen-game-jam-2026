using System.Collections.Generic;
using NetworkMask.Constants;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private const string Door01NamePrefix = "Door_";
    public delegate void MaskChangeHandler(GameObject sender, MaskChangeEventArgs args);
    public static event MaskChangeHandler OnMaskChange;
    private static byte LastDoorIndex = 0;
    private static GameObject Player;

    public static void ChangeMask(GameObject sender, MaskColor newColor)
    {
        OnMaskChange?.Invoke(sender, new MaskChangeEventArgs { NewColor = newColor });
    }

    private Dictionary<string, GameObjectStatus> gameObjectStatuses = new Dictionary<string, GameObjectStatus>();

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Player = GameObject.FindWithTag("Player");
        if (Player == null)
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene!");
        }
        DontDestroyOnLoad(Player);
    }

    public static async void ChangeScene(string nextLevel, byte doorIndex)
    {
        //Scene load
        await SceneManager.LoadSceneAsync(nextLevel);

        LastDoorIndex = doorIndex;
        MovePlayerToDoor(doorIndex);
    }

    public static void ResetPlayer()
    {
        MovePlayerToDoor(LastDoorIndex);
    }

    private static void MovePlayerToDoor(byte doorIndex)
    {
        var doorName = Door01NamePrefix + doorIndex.ToString("D2");
        var door = GameObject.Find(doorName);
        if (door == null)
        {
            Debug.LogError($"Door with name {doorName} not found!");
            return;
        }

        Player.transform.SetPositionAndRotation(door.transform.position, door.transform.rotation);
    }

    public void SaveGameObjectStatus(string name, Transform transform, bool isActive)
    {
        gameObjectStatuses[name] = new GameObjectStatus { Transform = transform, IsActive = isActive };
    }
}

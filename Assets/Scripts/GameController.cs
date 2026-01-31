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
        var playerObjects = GameObject.FindGameObjectsWithTag("Player");
        if (playerObjects.Length > 1)
        {
            Debug.LogWarning("Multiple objects with tag 'Player' found in the scene! There should be only one.");
            foreach (var obj in playerObjects)
            {
                if (obj != Player)
                {
                    Destroy(obj);
                }
            }
            return;
        }
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene!");
        }
        DontDestroyOnLoad(Player);
    }

    public static async void ChangeScene(string nextLevel, byte doorIndex, Vector3 spawnPosition = default)
    {
        //Scene load
        await SceneManager.LoadSceneAsync(nextLevel);

        LastDoorIndex = doorIndex;
        MovePlayerToDoor(doorIndex, spawnPosition);
    }

    public static void ResetPlayer()
    {
        MovePlayerToDoor(LastDoorIndex);
    }

    private static void MovePlayerToDoor(byte doorIndex, Vector3 spawnPosition = default)
    {
        var doorName = Door01NamePrefix + doorIndex.ToString("D2");
        var door = GameObject.Find(doorName);
        if (door == null)
        {
            Debug.LogError($"Door with name {doorName} not found!");
            return;
        }

        // Spawn Position is the center of the scene transition prefab in the previous scene
        // We calculate the offset from that position to the player position and apply the same offset to the door position
        // to seamlessly position the player in the new scene
        Vector3 targetPosition = door.transform.position;
        if (spawnPosition != default)
        {
            Vector3 offset = Player.transform.position - spawnPosition;
            targetPosition += offset;
        }
        Player.transform.position = targetPosition;
        // we keep the player's rotation
    }

    public void SaveGameObjectStatus(string name, Transform transform, bool isActive)
    {
        gameObjectStatuses[name] = new GameObjectStatus { Transform = transform, IsActive = isActive };
    }
}

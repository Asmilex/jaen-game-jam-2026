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

[RequireComponent(typeof(AudioSource))]

public class GameController : MonoBehaviour
{
    private const string Door01NamePrefix = "Door_";
    public delegate void MaskChangeHandler(GameObject sender, MaskChangeEventArgs args);
    public static event MaskChangeHandler OnMaskChange;
    private static byte _lastDoorIndex = 0;
    public static byte LastDoorIndex => _lastDoorIndex;
    private static GameObject Player;

    public static void ChangeMask(GameObject sender, MaskColor newColor)
    {
        OnMaskChange?.Invoke(sender, new MaskChangeEventArgs { NewColor = newColor });
    }

    private Dictionary<string, GameObjectStatus> gameObjectStatuses = new Dictionary<string, GameObjectStatus>();

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            Debug.LogError("Player object with tag 'Player' not found in the scene!");
        }
    }

    public static void ChangeScene(string nextLevel, byte doorIndex, Vector3? spawnPosition = null, Vector3? playerOffset = null)
    {
        // Destroy the old player before loading the new scene
        if (Player != null)
        {
            Debug.Log($"[GAME_CONTROLLER] Destroying old Player at position: {Player.transform.position}");
            Destroy(Player);
            Player = null;
        }

        //Scene load
        var operation = SceneManager.LoadSceneAsync(nextLevel);
        operation.completed += (asyncOperation) =>
        {
            // Find the player in the new scene
            Player = GameObject.FindGameObjectWithTag("Player");
            if (Player == null)
            {
                Debug.LogError("[GAME_CONTROLLER] Player not found in new scene!");
                return;
            }
            Debug.Log($"[GAME_CONTROLLER] Found new Player in scene at position: {Player.transform.position}");

            MovePlayerToDoor(doorIndex, spawnPosition, playerOffset);
        };

        _lastDoorIndex = doorIndex;
    }

    public static void ResetPlayer()
    {
        // Destroy the old player before reloading
        if (Player != null)
        {
            Destroy(Player);
            Player = null;
        }

        //Scene reload
        var currentScene = SceneManager.GetActiveScene().name;
        var operation = SceneManager.LoadSceneAsync(currentScene);
        operation.completed += (asyncOperation) =>
        {
            // Find the player in the reloaded scene
            Player = GameObject.FindGameObjectWithTag("Player");
            if (Player == null)
            {
                Debug.LogError("[GAME_CONTROLLER] Player not found after scene reload!");
                return;
            }

            MovePlayerToDoor(_lastDoorIndex);
        };
    }

    private static void MovePlayerToDoor(byte doorIndex, Vector3? spawnPosition = null, Vector3? playerOffset = null)
    {
        var doorName = Door01NamePrefix + doorIndex.ToString("D2");
        Debug.Log($"[GAME_CONTROLLER] Looking for door: {doorName}");
        Debug.Log($"[GAME_CONTROLLER] Player current position BEFORE move: {Player.transform.position}");
        Debug.Log($"[GAME_CONTROLLER] Received spawnPosition: {spawnPosition}");
        Debug.Log($"[GAME_CONTROLLER] Received playerOffset: {playerOffset}");

        var door = GameObject.Find(doorName);
        if (door == null)
        {
            Debug.LogError($"Door with name {doorName} not found!");
            return;
        }

        Debug.Log($"[GAME_CONTROLLER] Found door at position: {door.transform.position}");
        Debug.Log($"[GAME_CONTROLLER] Door GameObject name: {door.name}");

        // Spawn Position is the center of the scene transition prefab in the previous scene
        // playerOffset is the offset from that position to the player position, captured at the moment of transition
        // We apply the same offset to the door position to seamlessly position the player in the new scene
        Vector3 targetPosition = door.transform.position;
        Quaternion targetRotation = Player.transform.rotation;

        Debug.Log($"[GAME_CONTROLLER] Initial target position (door position): {targetPosition}");

        if (spawnPosition == null)
        {
            Debug.Log($"[GAME_CONTROLLER] No spawn position provided, calculating rotation to center");
            Quaternion directionToCenter = Quaternion.LookRotation(Vector3.zero - door.transform.position);
            targetRotation = directionToCenter;
        }
        else if (playerOffset.HasValue)
        {
            Debug.Log($"[GAME_CONTROLLER] Applying player offset: {playerOffset.Value}");
            targetPosition += playerOffset.Value;
            Debug.Log($"[GAME_CONTROLLER] Target position after offset: {targetPosition}");
        }
        else
        {
            Debug.Log($"[GAME_CONTROLLER] Spawn position provided but no player offset");
        }

        Debug.Log($"[GAME_CONTROLLER] Final target position: {targetPosition}");
        Debug.Log($"[GAME_CONTROLLER] Final target rotation: {targetRotation.eulerAngles}");

        Player.transform.SetPositionAndRotation(targetPosition, targetRotation);

        Debug.Log($"[GAME_CONTROLLER] Player position AFTER move: {Player.transform.position}");
    }

    public void SaveGameObjectStatus(string name, Transform transform, bool isActive)
    {
        gameObjectStatuses[name] = new GameObjectStatus { Transform = transform, IsActive = isActive };
    }
}

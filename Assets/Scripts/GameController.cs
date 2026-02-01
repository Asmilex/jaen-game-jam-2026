using System;
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
    private static GameController _instance;
    public static GameController Instance => _instance;
    private const string DoorNamePrefix = "Door_";
    public delegate void MaskChangeHandler(GameObject sender, MaskChangeEventArgs args);
    public static event MaskChangeHandler OnMaskChange;
    private static MaskColor _currentMaskColor = MaskColor.None;
    public static MaskColor CurrentMaskColor => _currentMaskColor;
    private static byte _lastDoorIndex = 0;
    public static byte LastDoorIndex => _lastDoorIndex;
    public GameObject Player;
    private readonly DateTime startTime = DateTime.Now;

    public static void ChangeMask(GameObject sender, MaskColor newColor)
    {
        OnMaskChange?.Invoke(sender, new MaskChangeEventArgs { NewColor = newColor });
        _currentMaskColor = newColor;
    }

    void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(Player);
        DontDestroyOnLoad(gameObject);
        _instance = this;
    }

    public string TimeSpentOnSession()
    {
        TimeSpan timeSpan = DateTime.Now - startTime;
        return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }

    public void ChangeScene(string nextLevel, byte doorIndex)
    {
        // Scene load
        var operation = SceneManager.LoadSceneAsync(nextLevel);
        operation.completed += (asyncOperation) =>
        {
            // Destroy any duplicate players from the new scene
            var allPlayers = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in allPlayers)
            {
                if (Player.GetInstanceID() != player.GetInstanceID())
                {
                    Debug.Log($"[GAME_CONTROLLER] Destroying duplicate Player instance with ID: {player.GetInstanceID()}");
                    Destroy(player);
                }
            }

            MovePlayerToDoor(doorIndex);
        };

        _lastDoorIndex = doorIndex;
    }

    public void ResetPlayer()
    {
        // Scene reload
        var currentScene = SceneManager.GetActiveScene().name;
        var operation = SceneManager.LoadSceneAsync(currentScene);
        operation.completed += (asyncOperation) =>
        {
            // Destroy any duplicate players from the reloaded scene
            var allPlayers = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in allPlayers)
            {
                if (Player.GetInstanceID() != player.GetInstanceID())
                {
                    Debug.Log($"[GAME_CONTROLLER] Destroying duplicate Player instance with ID: {player.GetInstanceID()}");
                    Destroy(player);
                }
            }

            MovePlayerToDoor(_lastDoorIndex);
        };
    }

    private void MovePlayerToDoor(byte doorIndex)
    {
        var doorName = DoorNamePrefix + doorIndex.ToString("D2");
        Debug.Log($"[GAME_CONTROLLER] Looking for door: {doorName}");
        Debug.Log($"[GAME_CONTROLLER] Player current position BEFORE move: {Player.transform.position}");

        // Find the door (which is actually the ChangeSceneSection parent)
        var door = GameObject.Find(doorName);
        if (door == null)
        {
            Debug.LogError($"[GAME_CONTROLLER] Door with name {doorName} not found!");
            return;
        }

        Debug.Log($"[GAME_CONTROLLER] Found door at position: {door.transform.position}");

        // Find the PlayerStart spawn point within the door's hierarchy
        // The ChangeSceneSection prefab has a PlayerStart child that defines where players should spawn
        Transform spawnPoint = FindPlayerStartInHierarchy(door.transform);

        Vector3 targetPosition;
        Quaternion targetRotation;

        if (spawnPoint != null)
        {
            // Use the PlayerStart position and rotation
            targetPosition = spawnPoint.position;
            targetRotation = spawnPoint.rotation;
            Debug.Log($"[GAME_CONTROLLER] Found PlayerStart spawn point at: {targetPosition}");
        }
        else
        {
            // Fallback: Try to find ChangeSceneCorridorController and use its playerStartTransform
            var corridorController = door.GetComponentInChildren<ChangeSceneCorridorController>();
            if (corridorController != null)
            {
                var playerStartTransform = corridorController.GetPlayerStartInCorridorTransform();
                if (playerStartTransform != null)
                {
                    targetPosition = playerStartTransform.position;
                    targetRotation = playerStartTransform.rotation;
                    Debug.Log($"[GAME_CONTROLLER] Using corridor's playerStartTransform at: {targetPosition}");
                }
                else
                {
                    // Last resort fallback: use door position with offset
                    targetPosition = door.transform.position + door.transform.forward * 2f;
                    targetRotation = Quaternion.LookRotation(-door.transform.forward);
                    Debug.LogWarning($"[GAME_CONTROLLER] No playerStartTransform found, using fallback position: {targetPosition}");
                }
            }
            else
            {
                // Last resort fallback: use door position with offset
                targetPosition = door.transform.position + door.transform.forward * 2f;
                targetRotation = Quaternion.LookRotation(-door.transform.forward);
                Debug.LogWarning($"[GAME_CONTROLLER] No ChangeSceneCorridorController found, using fallback position: {targetPosition}");
            }
        }

        Debug.Log($"[GAME_CONTROLLER] Final target position: {targetPosition}");
        Debug.Log($"[GAME_CONTROLLER] Final target rotation: {targetRotation.eulerAngles}");

        // Disable CharacterController temporarily if present (required for teleportation)
        var characterController = Player.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        Player.transform.SetPositionAndRotation(targetPosition, targetRotation);

        // Re-enable CharacterController
        if (characterController != null)
        {
            characterController.enabled = true;
        }

        Debug.Log($"[GAME_CONTROLLER] Player position AFTER move: {Player.transform.position}");
    }

    /// <summary>
    /// Recursively searches for a PlayerStart transform in the door's hierarchy
    /// </summary>
    private Transform FindPlayerStartInHierarchy(Transform parent)
    {
        // Check direct children first
        foreach (Transform child in parent)
        {
            if (child.name == "PlayerStart")
            {
                return child;
            }
        }

        // Then check recursively
        foreach (Transform child in parent)
        {
            var result = FindPlayerStartInHierarchy(child);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    public void SaveGameObjectStatus(string name, Transform transform, bool isActive)
    {
        // Reserved for future use
    }
}

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
    private const string Door01NamePrefix = "Door_";
    public delegate void MaskChangeHandler(GameObject sender, MaskChangeEventArgs args);
    public static event MaskChangeHandler OnMaskChange;
    private static byte _lastDoorIndex = 0;
    public static byte LastDoorIndex => _lastDoorIndex;
    public GameObject Player;

    public static void ChangeMask(GameObject sender, MaskColor newColor)
    {
        OnMaskChange?.Invoke(sender, new MaskChangeEventArgs { NewColor = newColor });
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

    public void ChangeScene(string nextLevel, byte doorIndex, Vector3? spawnPosition = null)
    {
        //Scene load
        var operation = SceneManager.LoadSceneAsync(nextLevel);
        operation.completed += (asyncOperation) =>
        {
            var newPlayer = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in newPlayer)
            {
                if (Player.GetInstanceID() != player.GetInstanceID())
                {
                    Debug.Log($"[GAME_CONTROLLER] Destroying duplicate Player instance with ID: {player.GetInstanceID()}");
                    Destroy(player);
                }
            }
            MovePlayerToDoor(doorIndex, spawnPosition);
        };

        _lastDoorIndex = doorIndex;
    }

    public void ResetPlayer()
    {
        //Scene reload
        var currentScene = SceneManager.GetActiveScene().name;
        var operation = SceneManager.LoadSceneAsync(currentScene);
        operation.completed += (asyncOperation) =>
        {
            var newPlayer = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in newPlayer)
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

    private void MovePlayerToDoor(byte doorIndex, Vector3? spawnPosition = null)
    {
        var doorName = Door01NamePrefix + doorIndex.ToString("D2");
        Debug.Log($"[GAME_CONTROLLER] Looking for door: {doorName}");
        Debug.Log($"[GAME_CONTROLLER] Player current position BEFORE move: {Player.transform.position}");
        Debug.Log($"[GAME_CONTROLLER] Received spawnPosition: {spawnPosition}");

        var door = GameObject.Find(doorName);
        if (door == null)
        {
            Debug.LogError($"Door with name {doorName} not found!");
            return;
        }

        Debug.Log($"[GAME_CONTROLLER] Found door at position: {door.transform.position}");
        Debug.Log($"[GAME_CONTROLLER] Door GameObject name: {door.name}");

        Vector3 targetPosition = door.transform.position;
        Quaternion targetRotation = Player.transform.rotation;

        Debug.Log($"[GAME_CONTROLLER] Initial target position (door position): {targetPosition}");

        if (spawnPosition == null)
        {
            Debug.Log($"[GAME_CONTROLLER] No spawn position provided, calculating rotation to center");
            Quaternion directionToCenter = Quaternion.LookRotation(Vector3.zero - door.transform.position);
            targetRotation = directionToCenter;
        }
        else
        {
            Vector3 playerOffset = spawnPosition.Value - Player.transform.position;
            Debug.Log($"[GAME_CONTROLLER] Applying player offset: {playerOffset}");
            Debug.Log($"[GAME_CONTROLLER] Target position after offset: {targetPosition}");
            targetPosition += playerOffset;
        }

        Debug.Log($"[GAME_CONTROLLER] Final target position: {targetPosition}");
        Debug.Log($"[GAME_CONTROLLER] Final target rotation: {targetRotation.eulerAngles}");

        Player.transform.SetPositionAndRotation(targetPosition, targetRotation);

        Debug.Log($"[GAME_CONTROLLER] Player position AFTER move: {Player.transform.position}");
    }
}

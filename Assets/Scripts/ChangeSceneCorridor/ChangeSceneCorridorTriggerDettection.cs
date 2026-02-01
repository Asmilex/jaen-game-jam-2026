using UnityEngine;

public class ChangeSceneCorridorTriggerDettection : MonoBehaviour
{
    [SerializeField] private ChangeSceneCorridorController changeSceneCorridorController;

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // Calculate the player's offset from the trigger at the moment of entry
        Vector3 playerOffset = other.transform.position - transform.position;

        Debug.Log($"[TRIGGER] Player entered trigger at position: {other.transform.position}");
        Debug.Log($"[TRIGGER] Trigger position: {transform.position}");
        Debug.Log($"[TRIGGER] Calculated player offset: {playerOffset}");
        Debug.Log($"[TRIGGER] Trigger GameObject name: {gameObject.name}");

        changeSceneCorridorController.LoadNextLevel(transform.position, playerOffset);
    }

}

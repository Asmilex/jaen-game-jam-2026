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

        Debug.Log($"[TRIGGER] Player entered trigger at position: {other.transform.position}");
        Debug.Log($"[TRIGGER] Trigger position: {transform.position}");
        Debug.Log($"[TRIGGER] Trigger GameObject name: {gameObject.name}");

        changeSceneCorridorController.LoadNextLevel(transform.position);
    }

}

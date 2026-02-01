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

        Debug.Log($"[TRIGGER] Player entered scene change trigger: {gameObject.name}");

        changeSceneCorridorController.LoadNextLevel();
    }
}

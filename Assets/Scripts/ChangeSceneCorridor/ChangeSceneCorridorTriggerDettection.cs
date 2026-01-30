using UnityEngine;

public class ChangeSceneCorridorTriggerDettection : MonoBehaviour
{
    [SerializeField] private ChangeSceneCorridorController changeSceneCorridorController;

    private void OnTriggerEnter(Collider other)
    {
        changeSceneCorridorController.LoadNextLevel();
    }

}

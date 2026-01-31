using UnityEngine;

public class ChangeSceneCorridorTriggerDettection : MonoBehaviour
{
    [SerializeField] private ChangeSceneCorridorController changeSceneCorridorController;

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Interactuado con player");
        changeSceneCorridorController.LoadNextLevel(transform.position);
    }

}

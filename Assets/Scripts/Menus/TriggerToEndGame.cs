using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerToEndGame : MonoBehaviour
{
    public string sceneToChangeName;

    void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(sceneToChangeName);
    }
}

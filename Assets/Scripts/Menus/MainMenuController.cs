using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayGame(string firstLevelName)
    {
        SceneManager.LoadScene(firstLevelName); // TODO - Poner nombre del primer nivel (Cuando exista)
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

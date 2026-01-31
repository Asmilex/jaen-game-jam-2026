using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame(string firstLevelName)
    {
        SceneManager.LoadScene(firstLevelName); // TODO - Poner nombre del primer nivel (Cuando exista)
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

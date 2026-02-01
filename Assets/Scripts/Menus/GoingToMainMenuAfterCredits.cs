using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoingToMainMenuAfterCredits : MonoBehaviour
{
    public AnimationClip creditsAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(GoingToMainMenuAfterCreditsCoroutine());

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Destroy(player);
        Destroy(GameController.Instance.gameObject);
    }

    IEnumerator GoingToMainMenuAfterCreditsCoroutine()
    {
        yield return new WaitForSeconds(creditsAnimator.length);

        SceneManager.LoadScene("MainMenu");
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GoingToMainMenuAfterCredits : MonoBehaviour
{
    public AnimationClip creditsAnimator;
    public AudioSource musicAudioSource;
    public TMP_Text finalCounterAtCredits;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetPlayedTimeText();

        StartCoroutine(GoingToMainMenuAfterCreditsCoroutine());


        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Destroy(player);
        //Destroy(GameController.Instance.gameObject);
    }

    IEnumerator GoingToMainMenuAfterCreditsCoroutine()
    {
        yield return new WaitForSeconds(creditsAnimator.length);

        SceneManager.LoadScene("MainMenu");

        Destroy(GameController.Instance.gameObject);
    }

    public void SetPlayedTimeText()
    {
        finalCounterAtCredits.text = "Time played: "+ GameController.Instance.gameObject.GetComponent<GameController>().TimeSpentOnSession();
    }
}

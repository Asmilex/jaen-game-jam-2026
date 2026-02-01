using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneCorridorController : MonoBehaviour
{
    [SerializeField] private string levelNameToLoadAfterPlayerEnter;
    [SerializeField] private Transform playerStartTransform;
    [SerializeField] public byte TargetDoorIndex = 1;

    [Header("Doors animations")]
    [SerializeField] private Animator doorsAnimatorController;
    [SerializeField] private AnimationClip openDoorAnimation;
    [SerializeField] private AnimationClip closeDoorAnimation;
    [SerializeField] private float extraTimeBeforeLoadingNextLevel = 2;

    [Header("Door sound effects")]
    [SerializeField] private AudioSource movableDoorAudiosource;
    [SerializeField] private AudioClip movableDoorAudioClip;

    [Header("Trigger Control")]
    [SerializeField] private GameObject changeSceneTrigger;

    private bool _allowedToLoadNextLevel = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(EnterSceneLogic());
        if (SceneManager.GetSceneByName(levelNameToLoadAfterPlayerEnter) == null)
        {
            Debug.LogError("Scene " + levelNameToLoadAfterPlayerEnter + " not found in build settings. Please add it.");
            _allowedToLoadNextLevel = false;
        }
    }

    #region ENTERING SCENE
    [ContextMenu("Enter Scene Logic")]
    private IEnumerator EnterSceneLogic()
    {
        yield return new WaitForSeconds(openDoorAnimation.length + extraTimeBeforeLoadingNextLevel);

        //Door opening
        doorsAnimatorController.SetTrigger("OpenDoor");

        //DoorSFX
        movableDoorAudiosource.PlayOneShot(movableDoorAudioClip);

        //Coroutine to enable trigger to load next level. Disabled at the beggining to avoid player trigger at the very beggining of scene
        //StartCoroutine(EnableTriggerAfterAFewSecondsAfterLoadingScene());
    }

    #endregion

    #region EXITING SCENE
    [ContextMenu("Load Next Level")]
    public void LoadNextLevel(Vector3 spawnPosition = default)
    {
        StartCoroutine(LoadNextLevelLogic(spawnPosition));
    }

    private IEnumerator LoadNextLevelLogic(Vector3 spawnPosition)
    {
        //Trigger deactivation to avoid multiple hits
        changeSceneTrigger.SetActive(false);

        //Door close and wait
        doorsAnimatorController.SetTrigger("CloseDoor");
        //Door SFX
        movableDoorAudiosource.PlayOneShot(movableDoorAudioClip);

        yield return new WaitForSeconds(closeDoorAnimation.length + extraTimeBeforeLoadingNextLevel);

        //Change scene
        if (_allowedToLoadNextLevel)
        {
            Debug.Log($"[CORRIDOR] Loading scene: {levelNameToLoadAfterPlayerEnter}, TargetDoorIndex: {TargetDoorIndex}");
            Debug.Log($"[CORRIDOR] SpawnPosition: {spawnPosition}");
            GameController.Instance.ChangeScene(levelNameToLoadAfterPlayerEnter, TargetDoorIndex, spawnPosition);
        }
    }
    #endregion




    #region MISC
    /*
    private IEnumerator EnableTriggerAfterAFewSecondsAfterLoadingScene()
    {
        changeSceneTrigger.SetActive(false);
        yield return new WaitForSeconds(timeToActivateTriggerAfterLoadingScene);
        changeSceneTrigger.SetActive(true);
    }
    */

    /// <summary>
    /// Player start transform when he goes to another scene through corridor in case we want him to start in this corridor
    /// </summary>
    /// <returns></returns>
    public Transform GetPlayerStartInCorridorTransform()
    {
        return playerStartTransform;
    }
    #endregion
}

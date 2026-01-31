using UnityEngine;

public class ChangeSceneTriggerEnabler : MonoBehaviour
{
    public GameObject changeSceneTrigger;

    void OnTriggerEnter(Collider other)
    {
        changeSceneTrigger.SetActive(true);
    }
}

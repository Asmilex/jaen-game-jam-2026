using UnityEngine;

public class ChangeSceneTriggerEnabler : MonoBehaviour
{
    public GameObject changeSceneTrigger;
    public 

    void OnTriggerEnter(Collider other)
    {
        changeSceneTrigger.SetActive(true);
        Destroy(this.gameObject);
    }
}

using System;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriadDoorSpawn : MonoBehaviour
{
    public GameObject[] DoorsToSpawn = Array.Empty<GameObject>();

    [Tooltip("The door index where the player came from to enable this spawn.")]
    public byte OnlyFromDoorIndex = 0;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (OnlyFromDoorIndex != 0 && OnlyFromDoorIndex != GameController.LastDoorIndex)
        {
            return;
        }

        foreach (var door in DoorsToSpawn)
        {
            door.SetActive(true);
        }
    }
}

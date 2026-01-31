using NetworkMask.Constants;
using System.Collections;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class GrabbableMaskObject : MonoBehaviour
{
    private PlayerController playerController;
    
    [Header("Mask type")]
    [SerializeField] private GrabbableMaskColor maskColor;
    enum GrabbableMaskColor
    {
        BlueMask,
        RedMask,
        YellowMask,
    }

    [Header("Prefab references")]
    [SerializeField] private MeshRenderer maskAnimMeshRenderer;
    [SerializeField] private Light maskPointLight;
    [SerializeField] private Collider maskCollider;
    [SerializeField] private Color maskPointLightColorBlue;
    [SerializeField] private Color maskPointLightColorRed;
    [SerializeField] private Color maskPointLightColorYellow;

    [Header("Materials")]
    [SerializeField] private Material RedMaterial;
    [SerializeField] private Material BlueMaterial;
    [SerializeField] private Material YellowMaterial;

    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetGrabbableMaskState();
    }

    private void SetGrabbableMaskState()
    {
        switch (maskColor)
        {
            case GrabbableMaskColor.BlueMask:
                maskAnimMeshRenderer.material = BlueMaterial;
                maskPointLight.color = maskPointLightColorBlue;
                break;
            case GrabbableMaskColor.RedMask:
                maskAnimMeshRenderer.material = RedMaterial;
                maskPointLight.color = maskPointLightColorRed;
                break;
            case GrabbableMaskColor.YellowMask:
                maskAnimMeshRenderer.material = YellowMaterial;
                maskPointLight.color = maskPointLightColorYellow;
                break;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //PlayerControllerReference
        playerController = other.GetComponent<PlayerController>();
        if (playerController == null)
        {
            return;
        }

        //Call player with color mask
        switch (maskColor)
        {
            case GrabbableMaskColor.BlueMask:
                playerController.EnablePlayerMask(MaskColor.Blue);
                StartCoroutine(playerController.ChangeMask(MaskColor.Blue));
                break;
            case GrabbableMaskColor.RedMask:
                playerController.EnablePlayerMask(MaskColor.Red);
                StartCoroutine(playerController.ChangeMask(MaskColor.Red));
                break;
            case GrabbableMaskColor.YellowMask:
                playerController.EnablePlayerMask(MaskColor.Yellow);
                StartCoroutine(playerController.ChangeMask(MaskColor.Yellow));
                break;
        }

        //Object is destroyed at the end of execution
        maskAnimMeshRenderer.enabled = false;
        maskPointLight.enabled = false;
        maskCollider.enabled = false;

    }


}

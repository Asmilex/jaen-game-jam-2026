using NetworkMask.Constants;
using UnityEditorInternal;
using UnityEngine;

public class ChangeMaskAnimationController : MonoBehaviour
{
    [Header("Gameobjects")]
    [SerializeField] private MeshRenderer ChangeMaskAnimMeshRenderer;

    [Header("Materials")]
    [SerializeField] private Material RedMaterial;
    [SerializeField] private Material BlueMaterial;
    [SerializeField] private Material YellowMaterial;

    [Header("Change mask animator controler")]
    [SerializeField] private Animator changeMaskAnimatorController;
    [SerializeField] private AnimationClip changeMaskAnimation;


    /// <summary>
    /// Triggers change mask animation (Parameter = 1(red), 2(blue), 3(Yellow))
    /// </summary>
    /// <param name="maskNumber"></param>
    public void ChangeMaskTransition(MaskColor mask)
    {
        switch (mask)
        {
            case MaskColor.Red:
                Debug.Log("Trigger transition to red mask");
                ChangeMaskAnimMeshRenderer.material = RedMaterial;
                changeMaskAnimatorController.SetTrigger("ChangeMaskAnimation");
                break;
            case MaskColor.Blue:
                Debug.Log("Trigger transition to blue mask");
                ChangeMaskAnimMeshRenderer.material = BlueMaterial;
                changeMaskAnimatorController.SetTrigger("ChangeMaskAnimation");
                break;
            case MaskColor.Yellow:
                Debug.Log("Trigger transition to Yellow mask");
                ChangeMaskAnimMeshRenderer.material = YellowMaterial;
                changeMaskAnimatorController.SetTrigger("ChangeMaskAnimation");
                break;
        }
    }

    /// <summary>
    /// Returns change mask animation lenght in seconds
    /// </summary>
    /// <returns></returns>
    public float ChangeMaskTransitionAnimationLength()
    {
        return changeMaskAnimation.length - 0.0f;
    }
}

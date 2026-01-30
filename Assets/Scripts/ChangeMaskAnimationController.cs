using UnityEditorInternal;
using UnityEngine;

public class ChangeMaskAnimationController : MonoBehaviour
{

    [Header("Gameobjects")]
    [SerializeField] private MeshRenderer ChangeMaskAnimMeshRenderer;

    [Header("Materials")]
    [SerializeField] private Material BlueMaterial;
    [SerializeField] private Material RedMaterial;
    [SerializeField] private Material GreenMaterial;

    //[Header("Change mask animator controler")]
    //[SerializeField] private AnimatorController changeMaskAnimatorController;
    

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


}

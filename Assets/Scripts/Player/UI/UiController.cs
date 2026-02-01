using System;
using NetworkMask.Constants;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UiController : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject interactUI;
    public GameObject redMask;
    public GameObject redMaskNotSelected;
    public GameObject blueMask;
    public GameObject blueMaskNotSelected;
    public GameObject yellowMask;
    public GameObject yellowMaskNotSelected;
    public GameObject pauseMenu;
    public UnityEngine.UI.Slider cameraSensitivySlider;

    private MaskColor _currentMask;

    public UiController()
    {
        _currentMask = MaskColor.None;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = this.gameObject.GetComponent<PlayerController>();

        Debug.Log("UI Started");
        if (interactUI == null) throw new NullReferenceException("Interact UI Missing");
        interactUI.SetActive(false);
        if (redMask == null) throw new NullReferenceException("RedMask UI Missing");
        redMask.SetActive(false);
        if (blueMask == null) throw new NullReferenceException("BlueMask UI Missing");
        blueMask.SetActive(false);
        if (yellowMask == null) throw new NullReferenceException("YellowMask UI Missing");
        yellowMask.SetActive(false);
    }

    public void MaskEnabled(MaskColor maskColor)
    {
        switch (maskColor)
        {
            case MaskColor.Red:
                redMask.SetActive(true);
                break;
            case MaskColor.Blue:
                blueMask.SetActive(true);
                break;
            case MaskColor.Yellow:
                yellowMask.SetActive(true);
                break;
        }
    }

    public void ChangeMaskUI(MaskColor mask)
    {
        switch (mask)
        {
            case MaskColor.Red:
                redMaskNotSelected.SetActive(false);
                blueMaskNotSelected.SetActive(true);
                yellowMaskNotSelected.SetActive(true);
                break;
            case MaskColor.Blue:
                redMaskNotSelected.SetActive(true);
                blueMaskNotSelected.SetActive(false);
                yellowMaskNotSelected.SetActive(true);
                break;
            case MaskColor.Yellow:
                redMaskNotSelected.SetActive(true);
                blueMaskNotSelected.SetActive(true);
                yellowMaskNotSelected.SetActive(false);
                break;
            case MaskColor.None:
                redMaskNotSelected.SetActive(true);
                blueMaskNotSelected.SetActive(true);
                yellowMaskNotSelected.SetActive(true);
                break;
        }
    }

    public void InteractableOnSight()
    {
        // try
        // {
        interactUI.SetActive(true);
        // }
        // catch { }
    }

    public void NoInteractableOnSight()
    {
        try
        {
            interactUI.SetActive(false);
        }
        catch { }
    }

    public void MaskDisabled(MaskColor maskColor)
    {
        switch (maskColor)
        {
            case MaskColor.Red:
                redMask.SetActive(false);
                break;
            case MaskColor.Blue:
                blueMask.SetActive(false);
                break;
            case MaskColor.Yellow:
                yellowMask.SetActive(false);
                break;
        }
    }

    public void ActivatePauseMenu(bool pauseMenuState)
    {
        pauseMenu.SetActive(pauseMenuState);
        
        if (pauseMenuState == true)
        {
             UnityEngine.Cursor.lockState = CursorLockMode.None;
             UnityEngine.Cursor.visible = true;

        }else
        {
             UnityEngine.Cursor.lockState = CursorLockMode.Locked;
             UnityEngine.Cursor.visible = false;
        }
    }

    public void TogglePauseMenu()
    {
        if(pauseMenu.activeInHierarchy == false)
        {
            pauseMenu.SetActive(true);
        }else
        {
            pauseMenu.SetActive(false);
        }
    }

    public void ContinueButtonPauseMenu()
    {
        pauseMenu.SetActive(false);
        ActivatePauseMenu(false);
    }

    public void MouseSensitivitySlider()
    {
        playerController.cameraSensibility = cameraSensitivySlider.value;
    }

}

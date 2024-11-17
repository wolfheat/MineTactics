using System;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{

    [SerializeField] GameObject settingsButton;
    [SerializeField] GameObject createButton;
    [SerializeField] GameObject randomButton;
    
    [SerializeField] GameObject cancelButton;
    [SerializeField] GameObject nextButton;
    [SerializeField] GameObject submitMenu;


    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject createPanel;
    [SerializeField] Toggle toggle;
    public static bool UsePending { get; private set; }

    public void ToggleSettings()
    {
        if (!settingsPanel.gameObject.activeSelf)
        {
            OpenSettings();
            return;
        }
        ToggleMenuButtons(true);
        settingsPanel.SetActive(false);
    }

    public void UpdateLoadPending()
    {
        UsePending = toggle.isOn;
    }

    public void OpenSettings()
    {
        Debug.Log("Opening Settings");
        ToggleMenuButtons(false);
        settingsPanel.SetActive(true);
    }

    public void OpenCreateMenu()
    {
        ToggleMenuButtons(false);
        createPanel.SetActive(true);
        LevelCreator.Instance.OnToggleCreate();
    }

    public void Next()
    {
        nextButton.gameObject.SetActive(false);
        submitMenu.SetActive(true);
        LevelCreator.Instance.OnCreateNext();
    }
    public void Back()
    {
        BaseMenu();
        LevelCreator.Instance.OnCreateBack();
    }

    private void BaseMenu()
    {
        nextButton.gameObject.SetActive(true);
        submitMenu.SetActive(false);
    }

    public void Cancel()
    {
        BaseMenu();
        ToggleMenuButtons(true);
        createPanel.SetActive(false);
        LevelCreator.Instance.CancelEditMode();

        // Change this for going to Nothing loaded?
        LevelCreator.Instance.RestartGame();
    }
    
    public void Submit()
    {
        LevelCreator.Instance.OnSubmitLevel();
    }
    
    public void Random()
    {
        LevelCreator.Instance.LoadRandomLevel();
    }

    private void ToggleMenuButtons(bool setActive)
    {
        // Disable all Buttons
        settingsButton.SetActive(setActive);
        createButton.SetActive(setActive);
        randomButton.SetActive(setActive);
    }

}

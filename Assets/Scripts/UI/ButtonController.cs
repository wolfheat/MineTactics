using System;
using UnityEngine;

public enum MenuState{Settings,Normal,CreateA,CreateB,Challenge,Test}

public class ButtonController : MonoBehaviour
{
    [SerializeField] IconButton[] mainButtons;

    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject normalButtons;
    [SerializeField] GameObject challengeButtons;
    [SerializeField] GameObject testButtons;

    [SerializeField] GameObject createPanel;
    [SerializeField] GameObject createAButtons;
    [SerializeField] GameObject createBButtons;


    public static ButtonController Instance { get; private set; }

    private MenuState lastState = MenuState.Normal;
    private MenuState stateBeforeSettings = MenuState.Normal;

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    
        ShowButtons(MenuState.Normal);            
    }

    public void ShowButtons(MenuState state)
    {
        if(state == MenuState.Settings && lastState == MenuState.Settings)
        {
            //Close Settings here!
            state = stateBeforeSettings;
        }
        mainButtons[(int)lastState].SetSelected(false);
        switch (lastState)
        {
            case MenuState.Settings:
                settingsPanel.gameObject.SetActive(false);
                //challengeModeButtonPanel.gameObject.SetActive(true);
                break;
            case MenuState.Normal:
                normalButtons.gameObject.SetActive(false);
                //challengeModeButtonPanel.gameObject.SetActive(true);
                break;
            case MenuState.CreateA:
                createAButtons.gameObject.SetActive(false);
                if (state != MenuState.CreateB)
                    createPanel.gameObject.SetActive(false);
                break;
            case MenuState.CreateB:
                createBButtons.gameObject.SetActive(false);
                if (state != MenuState.CreateA)
                    createPanel.gameObject.SetActive(false);
                break;
            case MenuState.Challenge:
                challengeButtons.gameObject.SetActive(false);
                break;
            case MenuState.Test:
                testButtons.gameObject.SetActive(false);
                break;
        }

        mainButtons[(int)state].SetSelected(true);


        switch (state)
        {
            case MenuState.Settings:
                settingsPanel.gameObject.SetActive(true);
                stateBeforeSettings = lastState;
                break;
            case MenuState.Normal:
                normalButtons.gameObject.SetActive(true);
                break;
            case MenuState.CreateA:
                createPanel.gameObject.SetActive(true);
                createAButtons.gameObject.SetActive(true);
                break;
            case MenuState.CreateB:
                createPanel.gameObject.SetActive(true);
                createBButtons.gameObject.SetActive(true);
                break;
            case MenuState.Challenge:
                challengeButtons.gameObject.SetActive(true);
                break;
            case MenuState.Test:
                createPanel.gameObject.SetActive(true);
                testButtons.gameObject.SetActive(true);
                break;
        }
        lastState = state;
    }

    public void ResetShowButtons()
    {
        ShowButtons(stateBeforeSettings);
    }
}

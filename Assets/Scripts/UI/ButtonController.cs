using System;
using UnityEngine;

public enum MenuState{Settings,Normal,CreateA,CreateB,Challenge}

public class ButtonController : MonoBehaviour
{
    [SerializeField] IconButton[] mainButtons;

    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject normalButtons;
    [SerializeField] GameObject challengeButtons;

    [SerializeField] GameObject createPanel;
    [SerializeField] GameObject createAButtons;
    [SerializeField] GameObject createBButtons;


    public static ButtonController Instance { get; private set; }

    private MenuState lastState = MenuState.Normal;

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
        Debug.Log("Show buttons "+state);
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
                createPanel.gameObject.SetActive(false);
                break;
            case MenuState.CreateB:
                createBButtons.gameObject.SetActive(false);
                createPanel.gameObject.SetActive(false);
                break;
            case MenuState.Challenge:
                challengeButtons.gameObject.SetActive(false);
                break;
        }

        mainButtons[(int)state].SetSelected(true);

        switch (state)
        {
            case MenuState.Settings:
                settingsPanel.gameObject.SetActive(true); 
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
        }
        lastState = state;
    }
}

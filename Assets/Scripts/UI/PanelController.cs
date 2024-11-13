using System;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{

    [SerializeField] GameObject settingsButton;
    [SerializeField] GameObject createButton;
    [SerializeField] GameObject randomButton;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] Toggle toggle;
    public static bool UsePending { get; private set; }


    public void ToggleSettings()
    {
        if (!settingsPanel.gameObject.activeSelf)
        {
            OpenSettings();
            return;
        }
        CloseSettings();
    }

    public void UpdateLoadPending()
    {
        UsePending = toggle.isOn;
    }
    
    private void OpenSettings()
    {
        HideAllButtons();
        settingsPanel.SetActive(true);
    }

    private void HideAllButtons()
    {
        // Disable all Buttons
        settingsButton.SetActive(false);
        createButton.SetActive(false);
        randomButton.SetActive(false);
    }

    private void CloseSettings()
    {
        // Enable all buttons
        settingsButton.SetActive(true);
        createButton.SetActive(true);
        randomButton.SetActive(true);
        settingsPanel.SetActive(false);
    }
}

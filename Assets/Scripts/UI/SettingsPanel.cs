using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameSize {S,M,L}

public class SettingsPanel : MonoBehaviour
{
    int enumGameSize = Enum.GetNames(typeof(GameSize)).Length;
    public static int activeGameSize = 0;
    [SerializeField] TextMeshProUGUI boardSizeText;
    [SerializeField] TextMeshProUGUI sensitivity;
    [SerializeField] TextMeshProUGUI mineDens;
    [SerializeField] TextMeshProUGUI winprob;
    [SerializeField] Toggle pendingToggle;
    [SerializeField] Slider slider;
    [SerializeField] Slider sensitivitySlider;

    private float[] winProbs = {30.2f, 1.08f, 1.56f};
    private float[] densities = {15f, 34.38f, 30.02f};

    public static SettingsPanel Instance { get; private set; }

    private void Awake()
    {
        Debug.Log("SETTINGS PANEL AWAKE");
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ConfirmSettings()
    {
        Debug.Log("** ConfirmSettings");
        USerInfo.Instance.BoardSize = (int)slider.value;
        USerInfo.Instance.UsePending = pendingToggle.isOn;
        // Update settings values
        SavingUtility.gameSettingsData.BoardSize = (int)slider.value;
        SavingUtility.gameSettingsData.TouchSensitivity = (int)sensitivitySlider.value;
        SavingUtility.gameSettingsData.UsePending = pendingToggle.isOn;
        SaveSettingsToFile();

        // Apply the size settings
        if (USerInfo.Instance.currentType == GameType.Normal)
            PanelController.Instance.ChangeMode(0); // Forces Update if playing Normal game


    }

    public void SaveSettingsToFile() => SavingUtility.Instance.SaveAllDataToFile();

    public void UpdateSizeText(Slider slider)
    {
        // Read value of slider and update
        int nexValue = (int)slider.value;
        boardSizeText.text = nexValue+"x"+ nexValue;

    }
    public void UpdateSensitivity(Slider slider)
    {
        // Read value of slider and update
        int nexValue = (int)slider.value;
        sensitivity.text = nexValue+" ms";
        USerInfo.Instance.Sensitivity = nexValue;

    }
    public void SetValuesFromLoadedSettings()
    {
        GameSettingsData data = SavingUtility.gameSettingsData;
        sensitivitySlider.value = data.TouchSensitivity;
        slider.value = data.BoardSize;
        pendingToggle.isOn = data.UsePending;
        Debug.Log("Settingspanel set Usepending to " + data.UsePending);
        Debug.Log("Settingspanel set Usepending to " + pendingToggle.isOn);
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

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
    [SerializeField] Toggle expertRotatedToggle;
    [SerializeField] Slider slider;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] TMP_Dropdown dropDown;

    private float[] winProbs = {30.2f, 1.08f, 1.56f};
    private float[] densities = {15f, 34.38f, 30.02f};

    private bool themeChanged = false;
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

    private void OnEnable()
    {
        // Occupy the dropDown
        dropDown.options.Clear();
        foreach (string t in ThemePicker.Instance.GetThemes())
            dropDown.options.Add(new TMP_Dropdown.OptionData() { text = t });
    }

    public void ConfirmSettings()
    {
        Debug.Log("** ConfirmSettings");
        USerInfo.Instance.BoardSize = (int)slider.value;
        USerInfo.Instance.UsePending = pendingToggle.isOn;
        USerInfo.Instance.UseRotatedExpert = expertRotatedToggle.isOn;

        // Update settings values
        SavingUtility.gameSettingsData.BoardSize = (int)slider.value;
        SavingUtility.gameSettingsData.TouchSensitivity = (int)sensitivitySlider.value;
        SavingUtility.gameSettingsData.UsePending = pendingToggle.isOn;
        SavingUtility.gameSettingsData.UseRotatedExpert = pendingToggle.isOn;
        SaveSettingsToFile();

        // Apply the size settings
        /*
        if (USerInfo.Instance.currentType == GameType.Normal)
            PanelController.Instance.ChangeMode(0); // Forces Update if playing Normal game
        */
        // Update theme on current screen 
        if (themeChanged)
            GameAreaMaster.Instance.MainGameArea.UpdateTheme();

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
    public void UpdateThemeChoice(TMP_Dropdown dropdown)
    {
        Debug.Log("Drop down picked "+dropdown.value);
        ThemePicker.Instance.SetTheme(dropdown.value);
        themeChanged = true;
    }

    public void SetValuesFromLoadedSettings()
    {
        GameSettingsData data = SavingUtility.gameSettingsData;
        sensitivitySlider.value = data.TouchSensitivity;
        slider.value = data.BoardSize;
        pendingToggle.isOn = data.UsePending;

        Debug.Log("Settingspanel set expertRotatedToggle to " + data.UseRotatedExpert);
        expertRotatedToggle.isOn = data.UseRotatedExpert;

        Debug.Log("Settingspanel set Usepending to " + data.UsePending);
        Debug.Log("Settingspanel set Usepending to " + pendingToggle.isOn);
    }
}

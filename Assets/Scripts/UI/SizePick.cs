using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SizePick : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI boardSizeText;

    public void Init()
    {
        // Set slider to current Size
        // Clear level and set it to stored size
        Debug.Log("OnEnable SizePick - Sets slider to stored value");
        slider.value = USerInfo.Instance.BoardSize;
    }
    public void ConfirmSettings()
    {
        USerInfo.Instance.BoardSize = (int)slider.value;

        // SAVE THIS??? NO

        // Update settings values
        SavingUtility.gameSettingsData.BoardSize = (int)slider.value;
        // Save to File here
        SavingUtility.Instance.SaveAllDataToFile();

        // Apply the size settings
        LevelCreator.Instance.OnToggleCreate();

    }

    public void UpdateCreateLevelSize()
    {
        // Read value of slider and update
        int nexValue = (int)slider.value;
        boardSizeText.text = nexValue + "x" + nexValue;
        ConfirmSettings();
    }
}

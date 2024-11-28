using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatePanelController : MonoBehaviour
{
    [SerializeField] GameObject[] createPanels;

    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI boardSizeText;

    private void OnEnable()
    {
        // Initiate the Create With start of creat buttons and 
        Debug.Log("Create Panel Controller Enabled - Enables first Panel");

        SetCreateButtonsToLevel(1);
        //UpdateCreateLevelSize();
        SetSliderSize();
        UpdateCreateLevelSize();
    }

    public void Back()
    {
        SetCreateButtonsToLevel(1);
        LevelCreator.Instance.OnCreateBack();
    }
    public void Next()
    {
        SetCreateButtonsToLevel(2);
        LevelCreator.Instance.OnCreateNext();
    }
    
    public void Cancel()
    {
        gameObject.SetActive(false);
        PanelController.Instance.Cancel();
    }

    public void SetCreateButtonsToLevel(int v)
    {
        for (int i = 0; i < createPanels.Length; i++) 
            createPanels[i].SetActive(i==v?true:false);
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

    public void SetSliderSize() => slider.value = USerInfo.Instance.BoardSize;
    public void UpdateCreateLevelSize()
    {
        // Read value of slider and update
        int nexValue = (int)slider.value;
        boardSizeText.text = nexValue + "x" + nexValue;
        ConfirmSettings();
    }
}

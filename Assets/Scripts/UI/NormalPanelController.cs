using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NormalPanelController : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI boardSizeText;

    private void OnEnable()
    {
        // Initiate the Create With start of creat buttons and 
        SetSliderSize();
    }

    public void ConfirmSettings()
    {
        USerInfo.Instance.BoardSize = (int)slider.value;

        LevelCreator.Instance.RestartGame();
    }

    public void UpdateCreateLevelSize()
    {
        // Read value of slider and update
        int nexValue = (int)slider.value;
        boardSizeText.text = nexValue + "x" + nexValue;
        ConfirmSettings();
    }
    public void SetSliderSize() => slider.value = USerInfo.Instance.BoardSize;
}

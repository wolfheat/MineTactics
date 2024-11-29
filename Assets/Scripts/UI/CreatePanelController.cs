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

    public void AddToCollectionList()
    {
        Debug.Log("Add this Level to The Collection List");
        GameArea.Instance.AddLevelToCollection();
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

        int[,] flagged = GameArea.Instance.GetFlaggedArray();
        
        Debug.Log("** Before OnChangeSizeCreate");   
        // Apply the size settings
        LevelCreator.Instance.OnChangeSizeCreate(flagged);
        Debug.Log("** After OnChangeSizeCreate");


        //LevelCreator.Instance.ApplyFlagged(flagged);
        GameArea.Instance.OnCreateBack(flagged);
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

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI index_text;
    [SerializeField] TextMeshProUGUI id_text;
    [SerializeField] Color unMarkedColor;
    [SerializeField] Color markedColor;
    [SerializeField] Image image;
    [SerializeField] GameObject marker;

    private int index = 0;
    private LevelData levelData;

    public int Index { get { return index; }}

    public void OnClick()
    {
        Debug.Log("Clicking List Item "+index);
        if (Inputs.Instance.Controls.UI.Shift.IsPressed() || Inputs.Instance.TimeHeld > 0.15f)
            LocalLevelsPanel.Instance.AddSelectedLevelToList(this,true);
        else
            LocalLevelsPanel.Instance.AddSelectedLevelToList(this);
    }

    public void RequestReplaceLevel()
    {
        Debug.Log("Request Replace Level (ListItem)");
        // CHeck here for valid Level
        bool isValid = GameArea.Instance.ValidateLevel();

        // Open Replace Confirmation screen
        if (isValid) 
            ConfirmPanel.Instance.ShowConfirmationOption("Replace Level?", "Are you sure you want to replace level " + index, ReplaceSelected);
        else
            PanelController.Instance.ShowInfo("this level can not be replaced because the current one is not valid");            
    }
    public void RequestDeleteLevel()
    {
        Debug.Log("Request Delete Level (ListItem)");
        ConfirmPanel.Instance.ShowConfirmationOption("Delete Level?", "Are you sure you want to delete level " + index+"?", DeleteSelected);
    }
    public void DeleteSelected()
    {
        LocalLevelsPanel.Instance.RemoveIndexFromList(index);
    }
    public void ReplaceSelected()
    {
        if (GameArea.Instance.ReplaceLevelToCollection(index))
        {
            LocalLevelsPanel.Instance.UpdateIndexFromCollection(index);
            PanelController.Instance.ShowFadableInfo("Level Replaced");
        }
        else
            PanelController.Instance.ShowInfo("Could not replace this level");
    }

    public void SetAsActive(bool set) => image.color = set ? markedColor : unMarkedColor;
    public void RequestLoadLevel()
    {
        Debug.Log("Request Load Level (ListItem)");
        LocalLevelsPanel.Instance.LoadLevel(levelData,this);
    }

    internal void UpdateData(int i, LevelData newlevelData)
    {
        levelData = newlevelData;
        index_text.text = i.ToString();
        id_text.text    = levelData.LevelId;
        index = i;
    }

    internal void UpdateIndex(int i)
    {
        index_text.text = i.ToString();
        index = i;
    }

    internal void DeMark()
    {
        marker.gameObject.SetActive(false);
    }

    internal void Mark()
    {
        marker.gameObject.SetActive(true);
    }
}

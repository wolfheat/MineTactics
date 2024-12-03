using TMPro;
using UnityEngine;

public class ConfirmReplaceScreen : MonoBehaviour
{
    private int ActiveIndex = -1;
    [SerializeField] TextMeshProUGUI index_text;
    public void SetActiveIndex(int index)
    {
        ActiveIndex = index;
        index_text.text = "Are you sure you want to replace level "+ActiveIndex+"?";
    }

    public void OnAcceptReplace()
    {
        if (GameArea.Instance.ReplaceLevelToCollection(ActiveIndex))
        {
            LocalLevelsPanel.Instance.UpdateIndexFromCollection(ActiveIndex);
            PanelController.Instance.ShowFadableInfo("Level Replaced");
        }
        else
            PanelController.Instance.ShowInfo("Could not replace this level");
        gameObject.SetActive(false);
    }
}

using UnityEngine;

public class ConfirmReplaceScreen : MonoBehaviour
{
    private int ActiveIndex = -1;
    public void SetActiveIndex(int index)
    {
        ActiveIndex = index;
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

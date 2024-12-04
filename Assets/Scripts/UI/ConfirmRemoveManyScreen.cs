using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConfirmRemoveManyScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI index_text;

    private List<LevelData> levelsToRemove;
    public void SetActiveQuery(List<LevelData> query)
    {
        levelsToRemove = query;
        index_text.text = "Are you sure you want to delete "+query.Count+" levels?";
    }

    public void OnAcceptDelete()
    {
        // remove Levels Here
        LocalLevelsPanel.Instance.RemoveQueryFromList(levelsToRemove);
        PanelController.Instance.ShowFadableInfo(levelsToRemove.Count+" Levels Deleted");
        gameObject.SetActive(false);
    }
}

using TMPro;
using UnityEngine;

public class ConfirmRemoveManySelectedScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI index_text;

    private int amount;

    public void OnAcceptDelete()
    {
        // remove Levels Here
        LocalLevelsPanel.Instance.DeleteSelected("");
        PanelController.Instance.ShowFadableInfo(amount + " Levels Deleted");
        gameObject.SetActive(false);
    }

    internal void SetAmt(int amt)
    {
        amount = amt;
        index_text.text = "Do you want to remove these "+ amt.ToString()+" levels?";
    }
}

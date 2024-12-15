using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;     
using UnityEngine.UI;

public class CollectionListItem : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI index_text;
    [SerializeField] TextMeshProUGUI id_text;
    [SerializeField] Color unMarkedColor;
    [SerializeField] Color markedColor;
    [SerializeField] Color unavailableColor;
    [SerializeField] Color normalTextColor;
    [SerializeField] Color selectedManyTextColor;
    [SerializeField] Image image;
    [SerializeField] GameObject marker;
    [SerializeField] GameObject downloadText;

    private int index = 0;
    private LevelData levelData;
    public LevelData Data { get {return levelData; }}
    public string CollectionName { get {return id_text.text; }}

    public int Index { get { return index; }}

    /*
    public void OnClick()
    {
        Debug.Log("Clicking List Item "+index);
        if (Inputs.Instance.Controls.UI.Ctrl.IsPressed() || Inputs.Instance.TimeHeld > 0.15f)
            LocalLevelsPanel.Instance.AddSelectedLevelToList(this,true);
        else if(Inputs.Instance.Controls.UI.Shift.IsPressed())
            LocalLevelsPanel.Instance.AddSelectedLevelToListShift(this);
        else
            LocalLevelsPanel.Instance.AddSelectedLevelToList(this);
    }
    
    */
    public void SetAsActive(bool set)
    {
        image.color = set ? markedColor : unMarkedColor;
    }
    public void ShowAsLocal(bool exists) => downloadText.SetActive(!exists); // Only visible if never downloaded
    public void RequestLoadLevel() => LoadPanel.Instance.ClickingCollection(index);

    public void UpdateData(int i, string collectionName)
    {
        index_text.text = i.ToString();
        id_text.text    = collectionName;
        index = i;
    }

    public void UpdateIndex(int i)
    {
        index_text.text = i.ToString();
        index = i;
    }

    internal void SetAsUnavailable()
    {
        Debug.Log("** NOT AVAILABLE COLLECTION");
        image.color = unavailableColor;
        GetComponent<Button>().interactable = false;
    }

    internal void SetAsDownloaded()
    {
        throw new NotImplementedException();
    }
}

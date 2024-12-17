using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LocalLevelsPanel : MonoBehaviour
{
    [SerializeField] GameObject panel;
    private List<ListItem> listItems = new List<ListItem>();
    [SerializeField] ListItem listItemPrefab;
    [SerializeField] GameObject listItemHolder;
    [SerializeField] SaveCollectionPanel storeCollection;
    [SerializeField] TextMeshProUGUI selectedAmoutButton;
    [SerializeField] LevelInfoPanel levelInfoPanel;


    public static LocalLevelsPanel Instance { get; private set; }
    public int LastLoadedIndex { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        FirestoreManager.OnLevelCollectionListChange += UpdateCollectionSize;
        FirestoreManager.SubmitLevelAttemptSuccess += OnSubmitLevelSuccess;
    }

    public void ShowLevelInfo(LevelData levelData)
    {
        levelInfoPanel.UpdateLevelInfo(levelData);
    }

    public void LeftLevelInfo()
    {
        //Debug.Log("Left ListItem with pointer, show last selected if any");
        if (selectedListItems.Count > 0)
            levelInfoPanel.UpdateLevelInfo(selectedListItems[selectedListItems.Count - 1].Data);
        else
            levelInfoPanel.HideLevelInfo();

    }
    public void HidePanel()
    {
        panel.SetActive(false);
    }
    
    public void ShowPanel()
    {
        panel.SetActive(true);
    }

    private void UpdateCollectionSize(int itemToSelect = -1)
    {
        UpdateList(itemToSelect);
        Debug.Log("LocalLevelsPanel - Updating List of Collection "+FirestoreManager.Instance.LocalCollectionList.Count);
    }

    public void RemoveIndexFromList(int i)
    {
        LastLoadedIndex = -1;
        Debug.Log("Remove index "+i+" from the LocalLevelsPanel list");
        // removing index i
        Debug.Log("List size before " + FirestoreManager.Instance.LocalCollectionList.Count);
        FirestoreManager.Instance.LocalCollectionList.RemoveAt(i);
        Debug.Log("List size after " + FirestoreManager.Instance.LocalCollectionList.Count);

        ListItem itemToDestroy = listItems[i];
        listItems.RemoveAt(i);
        Destroy(itemToDestroy.gameObject);
        UpdateListIndexes(i);
        SetLastLoadedLevelColor();
    }

    public void SelectRecentlyAdded()
    {
        LastLoadedIndex = FirestoreManager.Instance.LocalCollectionList.Count-1;
        SetLastLoadedLevelColor();
    }

    private void UpdateListIndexes(int i_start)
    {
        // Create New ListItems
        List<LevelData> levelDatas = FirestoreManager.Instance.LocalCollectionList;
        for (int i = i_start; i < levelDatas.Count; i++)
        {
            ListItem newListItem = listItems[i];
            newListItem.UpdateIndex(i);
        }
    }

    public void UpdatePanel()
    {
        Debug.Log("Update localLevelPanel");
        UpdateList();
        DeselectAll();
    }

    private void DeselectAll()
    {
        selectedIndexes.Clear();
        selectedListItems.Clear();
        UpdateSelectedAmt();
        levelInfoPanel.HideLevelInfo();
    }

    private void UpdateList(int itemToSelect = -1)
    {
        Debug.Log("Updating List -  Should only be done in the LocalLevels Panel?");
        DestroyOldListItems();

        // Create New ListItems
        List<LevelData> levelDatas = FirestoreManager.Instance.LocalCollectionList;
                
        for (int i = 0; i < levelDatas.Count; i++) 
        {
            LevelData levelData = levelDatas[i];
            ListItem newListItem = Instantiate(listItemPrefab,listItemHolder.transform);
            newListItem.UpdateData(i,levelData);
            listItems.Add(newListItem);
            if (selectedIndexes.Contains(i))
                newListItem.Mark();
        }

        SetLastLoadedLevelColor();

    }

    private void DestroyOldListItems()
    {
        foreach (Transform item in listItemHolder.transform) {
            if (item == this.gameObject)
                continue;
            Destroy(item.gameObject);        
        }
        listItems.Clear();
    }

    public void ClosePanel() => panel.SetActive(false);

    public void RequestClearCollection()
    {
        if(listItems.Count == 0)
        {
            PanelController.Instance.ShowInfo("There is no levels in the Collection!");
            return;
        }
        ConfirmPanel.Instance.ShowConfirmationOption("Clear Level lists?", "Do you want to remove all levels from the list? (This Only affects this local list!)", ClearCollection);
    }

    public void ClearCollection(string info)
    {
        Debug.Log("Clear Collection");

        FirestoreManager.Instance.ClearLocalCollectionList();
        DestroyOldListItems();
        DeselectAll();
    }
    public void DeleteSelected(string info)
    {
        Debug.Log("Delete Selection");

        // removing index i
        Debug.Log("List size before " + FirestoreManager.Instance.LocalCollectionList.Count);
        selectedIndexes.Sort();
        for (int i = selectedIndexes.Count - 1; i >= 0; i--)
        {
            FirestoreManager.Instance.LocalCollectionList.RemoveAt(selectedIndexes[i]);
            ListItem itemToDestroy = listItems[selectedIndexes[i]];
            listItems.RemoveAt(selectedIndexes[i]);
            Destroy(itemToDestroy.gameObject);
        }
        Debug.Log("List size after " + FirestoreManager.Instance.LocalCollectionList.Count);

        selectedIndexes.Clear();
        selectedListItems.Clear();
        UpdateSelectedAmt();

        UpdateListIndexes(0);
        SetLastLoadedLevelColor();
        levelInfoPanel.HideLevelInfo();
    }
    
    public void RequestDeleteSelected()
    {
        if(selectedListItems.Count == 0)
        {
            PanelController.Instance.ShowInfo("No level selected. Can not delete!");
            return;
        }
        ConfirmPanel.Instance.ShowConfirmationOption("Remove Levels?", "Do you want to remove the " + selectedListItems.Count + " selected levels from the list? (This Only affects this local list!)", DeleteSelected);
    }

    bool entireLast = true;
    string lastSavedCollectionName = "";
    private void OnSubmitLevelSuccess()
    {
        Debug.Log("Successfully submitted levels to Collection - Set these levels to this Collection");
        if (entireLast)
            SetEntireCollectionToBelongTo(lastSavedCollectionName);
        else
            SetSelectionOfCollectionToBelongTo(lastSavedCollectionName);
        // Update the list
        UpdateList();
    }

    private void SetSelectionOfCollectionToBelongTo(string lastSavedCollectionName)
    {
        Debug.Log("SELECTION OF COLLECTION");
        FirestoreManager.Instance.SetLocalCollectionListAsFromCollection(lastSavedCollectionName,selectedIndexes);
    }

    private void SetEntireCollectionToBelongTo(string lastSavedCollectionName)
    {
        Debug.Log("ENTIRE COLLECTION");
        FirestoreManager.Instance.SetLocalCollectionListAsFromCollection(lastSavedCollectionName);        
    }

    public void RequestStoreCollection(bool entire)
    {
        entireLast = entire;
        Debug.Log("Store "+(entire?"Entire":"Selected"));
        if (FirestoreManager.Instance.LocalCollectionList.Count == 0)
        {
            PanelController.Instance.ShowInfo("The Collection is Empty, can not save an empty Collection.");
            return;
        }
        if(entire)
            ConfirmInputPanel.Instance.ShowConfirmationOption("Store Collection?", "Storing the entire collection with " + FirestoreManager.Instance.LocalCollectionList.Count + " levels.", StoreCollection);
        else
        {
            if(selectedListItems.Count == 0)
                PanelController.Instance.ShowInfo("The Selection is Empty, can not save an empty Collection.");
            else
                ConfirmInputPanel.Instance.ShowConfirmationOption("Store Collection?", "Storing selected " + selectedListItems.Count + " levels as collection.", StoreSelectionToCollection);        
        }
    }
    public void StoreCollection(string collectionName)
    {
        lastSavedCollectionName = collectionName;
        Debug.Log("StoreCollection");
        FirestoreManager.Instance.StoreLevelCollection(collectionName);
        //FirestoreManager.Instance.StoreLevelCollectionPreset();
    }
    public void StoreSelectionToCollection(string collectionName)
    {
        lastSavedCollectionName = collectionName;
        Debug.Log("StoreSelectionToCollection");
        List<LevelData> levelDataList = (List<LevelData>)selectedListItems.Select(x => x.Data).ToList();
        FirestoreManager.Instance.StoreSelectedLevelCollection(collectionName, levelDataList);
        //FirestoreManager.Instance.StoreLevelCollectionPreset();
    }
    public void RequestLoadCollection()
    {
        ConfirmInputPanel.Instance.ShowConfirmationOption("Load Collection?", "Name the collection you want to load!", LoadCollection);        
    }
    
    public void LoadCollection(string levelName)
    {
        LastLoadedIndex = -1;
        Debug.Log("Request Load Collection");
        FirestoreManager.Instance.LoadLevelCollection(levelName,true);
        SetLastLoadedLevelColor();
    }
    

    public void UpdateIndexFromCollection(int index)
    {
        Debug.Log("** UpdateIndexFromCollection");
        // Unset The previous marked List item
        if(loadedLevelListItem != null)
        {
            Debug.Log("** DeMarking index "+loadedLevelListItem.Index);
            loadedLevelListItem.SetAsLastLoaded(false);
        }
        LastLoadedIndex = index;
        Debug.Log("** UpdateIndexFromCollection");
        LevelData levelData = FirestoreManager.Instance.LocalCollectionList[index];
        listItems[index].UpdateData(index, levelData);
        loadedLevelData = levelData;

        // Set the new List item
        SetLastLoadedLevelColor();
        Debug.Log("** Marking index "+index);
    }

    private void SetLastLoadedLevelColor()
    {
        for (int i = 0; i<listItems.Count; i++)
        {
            listItems[i].SetAsLastLoaded(i==LastLoadedIndex?true:false);
        }
    }

    private ListItem loadedLevelListItem;
    private LevelData loadedLevelData;

    public void LoadLevel(LevelData levelData, ListItem listItem)
    {
        LastLoadedIndex = listItem.Index;
        if (loadedLevelListItem != null)
        {
            Debug.Log("Demark last ListItem " + loadedLevelListItem.Index);
            loadedLevelListItem.SetAsLastLoaded(false);
        }
        loadedLevelListItem = listItem;
        LoadLevel(levelData);
    }
    public void LoadLevel(LevelData levelData)
    {
        Debug.Log("Setting loadedLevelListItem = "+ loadedLevelListItem?.Index);
        loadedLevelData = levelData;

        GameAreaMaster.Instance.MainGameArea.OnLoadLevelComplete(levelData.Level,true);
        // Close Panel and show Load message
        PanelController.Instance.ShowFadableInfo("Level Loaded");

        SetLastLoadedLevelColor();
        ClosePanel();

    }

    List<int> selectedIndexes = new List<int>();
    List<ListItem> selectedListItems = new List<ListItem>();
    public void AddSelectedLevelToList(ListItem item, bool keep = false)
    {
        if(selectedIndexes.Contains(item.Index))
        {
            int indexInList = selectedIndexes.IndexOf(item.Index);
            item.DeMark();
            selectedListItems.RemoveAt(indexInList);
            selectedIndexes.Remove(item.Index);
        }
        else
        {
            if (!keep)
            {
                UnselectAllMarked();
            }

            // Remove if already marked = unmark
            selectedIndexes.Add(item.Index);
            selectedListItems.Add(item);
            item.Mark();

        }
        UpdateSelectedAmt();
    }

    private void UnselectAllMarked()
    {
        foreach (var index in selectedIndexes)
        {
            ListItem listItem = listItems[index];
                listItem?.DeMark();
        }
        selectedListItems.Clear();
        selectedIndexes.Clear();
    }

    public void InvertSelection()
    {
        List<int> newSelectedIndexes = Enumerable.Range(0,listItems.Count).Where(x=>!selectedIndexes.Contains(x)).ToList();
        UnselectAllMarked();
        foreach (var selected in newSelectedIndexes)
        {
            ListItem selectedItem = listItems[selected];
            selectedIndexes.Add(selectedItem.Index);
            selectedListItems.Add(selectedItem);
            selectedItem.Mark();
        }
        UpdateSelectedAmt();
    }

    internal void AddSelectedLevelsToCurrentMousePositionFromLast_UsingShift(ListItem listItem)
    {
        // Mark all items from index min to max
        int minIndex = Math.Min(listItem.Index, selectedListItems[selectedListItems.Count - 1].Index);
        int maxIndex = Math.Max(listItem.Index, selectedListItems[selectedListItems.Count - 1].Index);
        for(int i = minIndex; i<=maxIndex; i++)
        {
            if (selectedIndexes.Contains(i))
                continue;
            selectedIndexes.Add(i);
            selectedListItems.Add(listItems[i]);
            listItems[i].Mark();
        }   
        UpdateSelectedAmt();
    }
    public void AddQueryToSelectedList(List<int> datas)
    {
        foreach (var item in datas)
        {
            if (selectedIndexes.Contains(item))
                continue;
            Debug.Log("Adding Index "+item+" to selected");
            selectedIndexes.Add(item);            
            ListItem listItem = listItems[item];
            selectedListItems.Add(listItem);
            listItems.Add(listItem);
            listItem.Mark();
        }
        UpdateSelectedAmt();
    }

    private void UpdateSelectedAmt() => selectedAmoutButton.text = "Delete (" + selectedIndexes.Count.ToString() + ")";

}

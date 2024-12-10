using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public int SelectedIndex { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        FirestoreManager.OnLevelCollectionListChange += UpdateCollectionSize;
    }


    public void ShowLevelInfo(LevelData levelData)
    {
        levelInfoPanel.UpdateLevelInfo(levelData);
    }

    public void LeftLevelInfo()
    {
        Debug.Log("Left ListItem with pointer, show last selected if any");
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
        Debug.Log("Updating List of Collection "+FirestoreManager.Instance.LocalCollectionList.Count);
    }

    public void RemoveIndexFromList(int i)
    {
        SelectedIndex = -1;
        Debug.Log("Remove index "+i+" from the LocalLevelsPanel list");
        // removing index i
        Debug.Log("List size before " + FirestoreManager.Instance.LocalCollectionList.Count);
        FirestoreManager.Instance.LocalCollectionList.RemoveAt(i);
        Debug.Log("List size after " + FirestoreManager.Instance.LocalCollectionList.Count);

        ListItem itemToDestroy = listItems[i];
        listItems.RemoveAt(i);
        Destroy(itemToDestroy.gameObject);
        UpdateListIndexes(i);
        ActivateSelected();
    }

    public void RemoveQueryFromList(List<LevelData> levelsToRemove)
    {
        Debug.Log("** RemoveQueryFromList");
        FirestoreManager.Instance.RemoveLocalCollectionListQuery(levelsToRemove);
        UpdateCollectionSize();
    }
    public void SelectRecentlyAdded()
    {
        SelectedIndex = FirestoreManager.Instance.LocalCollectionList.Count-1;
        ActivateSelected();
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
    }

    private void UpdateList(int itemToSelect = -1)
    {
        Debug.Log("Updating List");
        DestroyOldListItems();

        // Create New ListItems
        List<LevelData> levelDatas = FirestoreManager.Instance.LocalCollectionList;
                
        for (int i = 0; i < levelDatas.Count; i++) 
        {
            LevelData levelData = levelDatas[i];
            ListItem newListItem = Instantiate(listItemPrefab,listItemHolder.transform);
            newListItem.UpdateData(i,levelData);
            listItems.Add(newListItem);
        }

        ActivateSelected();

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

    public void RequestClearCollection() => ConfirmPanel.Instance.ShowConfirmationOption("Clear Levels?", "Do you want to remove all levels from the list?", ClearCollection);
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
        ActivateSelected();
    }
    
    public void RequestDeleteSelected()
    {
        if(selectedListItems.Count == 0)
        {
            PanelController.Instance.ShowInfo("No level selected. Can not delete!");
            return;
        }
        ConfirmPanel.Instance.ShowConfirmationOption("Remove Levels?", "Do you want to remove the " + selectedListItems.Count + " selected levels from the list?", DeleteSelected);
    }

    public void RequestStoreCollection(bool entire)
    {
        Debug.Log("Store "+(entire?"Entire":"Selected"));
        if (FirestoreManager.Instance.LocalCollectionList.Count == 0)
        {
            PanelController.Instance.ShowInfo("The Collection is Empty, can not save an empty Collection.");
            return;
        }
        if(entire)
            ConfirmInputPanel.Instance.ShowConfirmationOption("Store Collection?", "Name your collection to store the entire Collection with " + FirestoreManager.Instance.LocalCollectionList.Count + " levels.", StoreCollection);        
        else
            ConfirmInputPanel.Instance.ShowConfirmationOption("Store Collection?", "Name your collection to store these " + selectedListItems.Count + " selected levels to!", StoreSelectionToCollection);        
    }
    public void StoreCollection(string collectionName)
    {
        Debug.Log("StoreCollection");
        FirestoreManager.Instance.StoreLevelCollection(collectionName);
        //FirestoreManager.Instance.StoreLevelCollectionPreset();
    }
        public void StoreSelectionToCollection(string collectionName)
    {
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
        SelectedIndex = -1;
        Debug.Log("Request Load Collection");
        FirestoreManager.Instance.LoadLevelCollection(levelName,true);
        ActivateSelected();
    }
    

    public void UpdateIndexFromCollection(int index)
    {
        // Unset The previous marked List item
        if(loadedLevelListItem != null)
        {
            Debug.Log("** DeMarking index "+loadedLevelListItem.Index);
            loadedLevelListItem.SetAsActive(false);
        }
        SelectedIndex = index;
        Debug.Log("** UpdateIndexFromCollection");
        ListItem newListItem = listItems[index];
        LevelData levelData = FirestoreManager.Instance.LocalCollectionList[index];
        loadedLevelData = levelData;
        newListItem.UpdateData(index, levelData);

        // Set the new List item
        ActivateSelected();
        Debug.Log("** Marking index "+index);
    }

    private void ActivateSelected()
    {
        for (int i = 0; i<listItems.Count; i++)
        {
            listItems[i].SetAsActive(i==SelectedIndex?true:false);
        }
    }

    private ListItem loadedLevelListItem;
    private LevelData loadedLevelData;

    public void LoadLevel(LevelData levelData,ListItem listItem)
    {
        SelectedIndex = listItem.Index;
        if(loadedLevelListItem != null)
        {
            Debug.Log("Demark last ListItem "+loadedLevelListItem.Index);
            loadedLevelListItem.SetAsActive(false);
        }
        loadedLevelListItem = listItem;
        Debug.Log("Setting loadedLevelListItem = "+ loadedLevelListItem?.Index);
        loadedLevelData = levelData;

        GameAreaMaster.Instance.MainGameArea.OnLoadLevelComplete(levelData.Level,true);
        // Close Panel and show Load message
        PanelController.Instance.ShowFadableInfo("Level Loaded");

        ActivateSelected();
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
                foreach(var listItem in selectedListItems)
                    if(listItem != null)
                        listItem?.DeMark();
                selectedListItems.Clear();
                selectedIndexes.Clear();
            }

            // Remove if already marked = unmark
            selectedIndexes.Add(item.Index);
            selectedListItems.Add(item);
            item.Mark();

        }
        UpdateSelectedAmt();
    }

    private void UpdateSelectedAmt() => selectedAmoutButton.text = "Delete (" + selectedIndexes.Count.ToString() + ")";

}

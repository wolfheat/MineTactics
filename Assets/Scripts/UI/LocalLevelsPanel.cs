using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class LocalLevelsPanel : MonoBehaviour
{
    private List<ListItem> listItems = new List<ListItem>();
    [SerializeField] ListItem listItemPrefab;
    [SerializeField] GameObject listItemHolder;
    [SerializeField] GameObject storeCollection;


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

    private void UpdateCollectionSize(int itemToSelect = -1)
    {
        UpdateList(itemToSelect);
        Debug.Log("Updating List of Collection "+FirestoreManager.Instance.LocalCollectionList.Count);
    }

    internal void RemoveIndexFromList(int i)
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

    internal void RemoveQueryFromList(List<LevelData> levelsToRemove)
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

    internal void UpdatePanel()
    {
        Debug.Log("Update localLevelPanel");
        UpdateList();
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

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public void ClearCollection()
    {
        Debug.Log("Request Clear Collection");

        FirestoreManager.Instance.ClearLocalCollectionList();
        DestroyOldListItems();

    }
    public void StoreCollection(string collectionName)
    {
        Debug.Log("Request Store COllection");

        FirestoreManager.Instance.StoreLevelCollection(collectionName);
        //FirestoreManager.Instance.StoreLevelCollectionPreset();
    }

    public void LoadCollection(string levelName)
    {
        SelectedIndex = -1;
        Debug.Log("Request Load Collection");
        FirestoreManager.Instance.LoadLevelCollection(levelName,true);
        ActivateSelected();
    }
    
    public void RequestStoreCollection()
    {
        if(FirestoreManager.Instance.LocalCollectionList.Count == 0)
        {
            PanelController.Instance.ShowInfo("The Collection is Empty, can not save an empty Collection.");
            return;
        }
        storeCollection.gameObject.SetActive(true);
    }

    internal void UpdateIndexFromCollection(int index)
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

    internal void LoadLevel(LevelData levelData,ListItem listItem)
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

        GameArea.Instance.OnLoadLevelComplete(levelData.Level,true);
        // Close Panel and show Load message
        PanelController.Instance.ShowFadableInfo("Level Loaded");

        ActivateSelected();
        gameObject.SetActive(false);

    }

}

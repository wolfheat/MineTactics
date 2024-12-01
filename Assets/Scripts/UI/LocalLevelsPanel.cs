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

    private void UpdateCollectionSize()
    {
        UpdateList();
        Debug.Log("Updating List of Collection "+FirestoreManager.Instance.LocalCollectionList.Count);
    }

    internal void RemoveIndexFromList(int i)
    {
        Debug.Log("Remove index "+i+" from the LocalLevelsPanel list");
        // removing index i
        Debug.Log("List size before " + FirestoreManager.Instance.LocalCollectionList.Count);
        FirestoreManager.Instance.LocalCollectionList.RemoveAt(i);
        Debug.Log("List size after " + FirestoreManager.Instance.LocalCollectionList.Count);

        ListItem itemToDestroy = listItems[i];
        listItems.RemoveAt(i);
        Destroy(itemToDestroy.gameObject);
        UpdateListIndexes(i);
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

    private void UpdateList()
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
        Debug.Log("Request Load Collection");
        FirestoreManager.Instance.LoadLevelCollection(levelName,true);
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
        ListItem newListItem = listItems[index];
        LevelData levelData = FirestoreManager.Instance.LocalCollectionList[index];
        newListItem.UpdateData(index, levelData);
    }

    internal void LoadLevel(LevelData levelData)
    {
        GameArea.Instance.OnLoadLevelComplete(levelData.Level,true);
        // Close Panel and show Load message
        PanelController.Instance.ShowFadableInfo("Level Loaded");
        gameObject.SetActive(false);
    }
}

using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class LoadPanel : MonoBehaviour
{
    [SerializeField] CollectionListItem listItemPrefab;
    [SerializeField] GameObject listHolder;
    [SerializeField] CollectionInfoPanel infoPanel;

    private List<CollectionListItem> collectionList = new();
    private List<CollectionListItem> selectedCollections = new();
    public static LoadPanel Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RequestLoadCollection(string collectionToLoad)
    {
        Debug.Log("RequestLoadLevel");
        FirestoreManager.Instance.LoadLevelCollection(collectionToLoad);
    }
    
    public void UpdateCollectionInfoPanel(CollectionListItem collectionToLoad)
    {
        Debug.Log("Update SHown Collection Info Panel");
        infoPanel.UpdateLevelInfo(collectionToLoad);

        //FirestoreManager.Instance.LoadLevelCollection(collectionToLoad);
    }
    
    public void ClickingCollection(int index)
    {
        CollectionListItem listItem = collectionList[index];
        Debug.Log("Clicked collection " + listItem);
        UpdateCollectionInfoPanel(listItem);

        if (selectedCollections.Contains(listItem))
        {
            Debug.Log("Deselect "+listItem.CollectionName);
            selectedCollections.Remove(listItem);
            listItem.SetAsActive(false);
        }
        else
        {
            Debug.Log("Select "+listItem.CollectionName);
            selectedCollections.Add(listItem);
            listItem.SetAsActive(true);
        }


        //FirestoreManager.Instance.LoadLevelCollection(levelToLoad);
    }
    
    public void RequestUpdateCollection(string ColelctionToUpdate)
    {
        Debug.Log("Requesting update level "+ColelctionToUpdate);
        //FirestoreManager.Instance.LoadLevelCollection(levelToLoad);
    }

    public void LoadClicked()
    {
        Debug.Log("Load the selected Collection");
    }

    public void GenerateCollections(List<string> collectionNames)
    {
        for (int i = 0; i < collectionNames.Count; i++)
        {
            string collectionName = collectionNames[i];
            CollectionListItem item = Instantiate(listItemPrefab, listHolder.transform);
            item.UpdateData(i, collectionName);
            collectionList.Add(item);
            item.SetAsActive(false);
        }
    }



    // Maybe not used, just set inactive?
    public void Close() => Debug.Log("Close the Collection Panel");
    public void GetCollectionsClicked()
    {
        Debug.Log("Get Collections Clicked");
        List<string> collectionNames = new List<string>() {"Easy","Basic","Corners","BasicCollection","Selection"};
        GenerateCollections(collectionNames);
    }
}

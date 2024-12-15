using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        FirestoreManager.OnSuccessfulLoadingOfCollection += CollectionLoadedFromFirebase;
        FirestoreManager.OnLevelCollectionLevelsDownloadedFailSendCollection += RecievedUnloadableCollectionNotice;
    }
    private void OnEnable() => GetCollectionsClicked();


    public void RequestUnloadCollection(string collectionToRemove)
    {
        Debug.Log("** RequestUnloadCollection - from Locally");
        Debug.Log("First check if this collection exists stored locally which it should");
        if (SavingUtility.Instance.FileExists(collectionToRemove))
        {
            Debug.Log("File exists locally");
            FirestoreManager.Instance.RemoveCollectionFromChallengeList(collectionToRemove);
        }
        else
        {
            Debug.Log("File does not exist locally, Why?????");
        }
    }
    
    public void RequestLoadCollection(string collectionToLoad)
    {
        Debug.Log("** Requesting to Load a Collection - (from DB or Locally)");
        if (SavingUtility.Instance.FileExists(collectionToLoad))
        {
            Debug.Log(" LOCALLY - File exists locally");
            LevelDataCollection collection = SavingUtility.Instance.LoadCollectionDataFromFile(collectionToLoad);
            if (collection != null)
            {
                Debug.Log("** Successfully loaded locally data with " + collection.Level.Length + " levels.");
                FirestoreManager.Instance.AddCollectionToChallengeList(collection,collectionToLoad);
            }
        }
        else
        {
            Debug.Log(" FIREBASE - File does not exist locally, download it from the Firestore");
            FirestoreManager.Instance.LoadLevelCollection(collectionToLoad);
        }
    }
    
    public void CollectionLoadedFromFirebase(LevelDataCollection collection, string collectionName)
    {
        Debug.Log("CollectionLoadedFromFirebase");

        // Overwrite or write this file locally
        SavingUtility.Instance.SaveCollectionDataToFile(collection, collectionName);
        Debug.Log("Saving the Downloaded Firebase collection as Local collection");

        // Add to active challenge list
        FirestoreManager.Instance.AddCollectionToChallengeList(collection, collectionName);
    }
    
    public void UpdateCollectionInfoPanel(CollectionListItem collectionToLoad)
    {
        Debug.Log("INFO PANEL - Updated to show "+collectionToLoad+" collection.");
        infoPanel.UpdateLevelInfo(collectionToLoad);
    }

    public void RecievedUnloadableCollectionNotice(string collectionName)
    {
        Debug.Log("Looking for Unavailable Collection in the list and setting it correctly in the list");
        foreach (var item in selectedCollections)
        {
            if(item.CollectionName == collectionName)
            {
                item.SetAsUnavailable();
                return;
            }
        }
    }
    public void ClickingCollection(int index)
    {
        CollectionListItem listItem = collectionList[index];
        Debug.Log("Clicked collection " + listItem.CollectionName);
        UpdateCollectionInfoPanel(listItem);

        if (selectedCollections.Contains(listItem))
        {
            Debug.Log("Deselect "+listItem.CollectionName);
            selectedCollections.Remove(listItem);            
            listItem.SetAsActive(false);
            RequestUnloadCollection(listItem.CollectionName);
        }
        else
        {
            Debug.Log("Select "+listItem.CollectionName);
            selectedCollections.Add(listItem);
            listItem.SetAsActive(true);
            RequestLoadCollection(listItem.CollectionName);
        }
        listItem.ShowAsLocal(true);


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
        DeleteAllPresentCollectionListItems();
        for (int i = 0; i < collectionNames.Count; i++)
        {
            string collectionName = collectionNames[i];
            Debug.Log("Checking Collection name "+collectionName);
            CollectionListItem item = Instantiate(listItemPrefab, listHolder.transform);
            item.UpdateData(i, collectionName);
            collectionList.Add(item);
            if (USerInfo.Instance.ActiveCollections.Contains(collectionName))
            {
                Debug.Log("IN ACTIVE");
                selectedCollections.Add(item);
                item.SetAsActive(true);
                item.ShowAsLocal(SavingUtility.Instance.FileExists(collectionName));
            }else if (USerInfo.Instance.InactiveCollections.Contains(collectionName))
            {
                Debug.Log("IN INACTIVE");
                item.ShowAsLocal(true);
            }
        }
    }

    private void DeleteAllPresentCollectionListItems()
    {
        foreach (var collectionItem in collectionList)
        {
            Destroy(collectionItem.gameObject);
        }
        collectionList.Clear();
        selectedCollections.Clear();
    }



    // Maybe not used, just set inactive?
    public void Close() => Debug.Log("Close the Collection Panel");
    public void GetCollectionsClicked()
    {
        Debug.Log("Get Collections Clicked");
        List<string> collectionNames = new List<string>() {"Easy","Basic","Corners","BasicCollection","Selection","Doesnt_Exist"};
        GenerateCollections(collectionNames);
    }
}
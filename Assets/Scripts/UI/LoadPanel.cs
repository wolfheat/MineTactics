using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadPanel : MonoBehaviour
{
    [SerializeField] CollectionListItem listItemPrefab;
    [SerializeField] GameObject listHolder;
    [SerializeField] CollectionInfoPanel collectionInfoPanel;
    [SerializeField] GameObject infoPanel;

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
        FirestoreManager.OnLevelCollectionListItemUpdatedItsCollection += CollectionListChanged;
        FirestoreManager.OnSuccessfulLoadingOfCollection += CollectionLoadedFromFirebase;
        FirestoreManager.OnLevelCollectionLevelsDownloadedFailSendCollection += RecievedUnloadableCollectionNotice;
    }

    private void CollectionListChanged(string collectionThatWasUpdated)
    {
        // Reload entire list to update Collection stored in listitems
        SetCollectionToListItem(FirestoreManager.Instance.LevelDataCollections[collectionThatWasUpdated],collectionThatWasUpdated);
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
    
    public void RequestLoadCollection(string collectionToLoad,bool forceDownload = false)
    {
        Debug.Log("** Requesting to Load a Collection - (from DB or Locally) "+collectionToLoad);

        if (FirestoreManager.Instance.LevelDataCollections.ContainsKey(collectionToLoad))
        {
            Debug.Log(" LOADED - File exists loaded, dont nee to load from file or DB");
            FirestoreManager.Instance.AddCollectionToChallengeList(FirestoreManager.Instance.LevelDataCollections[collectionToLoad], collectionToLoad);
        }


        if (!forceDownload && SavingUtility.Instance.FileExists(collectionToLoad))
        {
            Debug.Log(" LOCALLY - File exists locally");
            LevelDataCollection collection = SavingUtility.Instance.LoadCollectionDataFromFile(collectionToLoad);
            SetCollectionToListItem(collection,collectionToLoad);
            if (collection != null)
            {
                Debug.Log("** Successfully loaded locally data with " + collection.Level.Length + " levels.");
                FirestoreManager.Instance.AddCollectionToChallengeList(collection,collectionToLoad);
            }
        }
        else
        {
            Debug.Log(" FIREBASE - "+(forceDownload?"Forced Download":"File does not exist locally")+", download it from the Firestore");
            FirestoreManager.Instance.LoadLevelCollection(collectionToLoad);
        }
    }

    private void SetCollectionToListItem(LevelDataCollection collection, string collectionToLoad)
    {
        foreach (var item in collectionList) {
            if(item != null && item.CollectionName == collectionToLoad)
            {
                item.UpdateData(item.Index, collection);
                if (infoPanel.gameObject.activeSelf)
                    UpdateCollectionInfoPanel(item);
            }
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
        infoPanel.gameObject.SetActive(true);
        Debug.Log("INFO PANEL - Updated to show "+collectionToLoad+" collection.");
        collectionInfoPanel.SetNewCollectionData(collectionToLoad);
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

    public void RightClickingCollection(int index)
    {
        CollectionListItem listItem = collectionList[index];
        if (listItem.Data == null)
        {
            Debug.Log("RightClickingCollection - NO DATA");
            return;
        }
        Debug.Log("RightClickingCollection");
        UpdateCollectionInfoPanel(listItem);
    }
    public void ClickingCollection(int index)
    {
        CollectionListItem listItem = collectionList[index];
        Debug.Log("Clicked collection " + listItem?.CollectionName);

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
            Debug.Log("Checking Collection name " + collectionName);
            CollectionListItem item = CreateCollectionListItem(i, collectionName);
            if (USerInfo.Instance.ActiveCollections.Contains(collectionName))
            {
                Debug.Log("IN ACTIVE");
                selectedCollections.Add(item);
                item.SetAsActive(true);
                item.ShowAsLocal(SavingUtility.Instance.FileExists(collectionName));
            }
            else if (USerInfo.Instance.InactiveCollections.Contains(collectionName))
            {
                Debug.Log("IN INACTIVE");
                item.ShowAsLocal(true);
            }
        }
    }

    private CollectionListItem CreateCollectionListItem(int i, string collectionName)
    {
        CollectionListItem item = Instantiate(listItemPrefab, listHolder.transform);
        if (FirestoreManager.Instance.LevelDataCollections.ContainsKey(collectionName))
            item.UpdateData(i, FirestoreManager.Instance.LevelDataCollections[collectionName]);
        else
            item.UpdateData(i, collectionName);

        collectionList.Add(item);
        return item;
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
        if(SavingUtility.gameSettingsData.CollectionNames.Count == 0)
            SavingUtility.gameSettingsData.CollectionNames = new List<string>{ "Easy","Basic","Corners","BasicCollection","Selection","Doesnt_Exist"};
        Debug.Log("Get Collections Clicked");
        GenerateCollections(SavingUtility.gameSettingsData.CollectionNames);
    }
    public void AddACollectionByText()
    {
        Debug.Log("Add a collection by text");
        ConfirmInputPanel.Instance.ShowConfirmationOption("Add Collection?", "Type the name of the Collection you want to add to the list!", AddACollectionByText);
    }
    public void AddACollectionByText(string nameOfCollectionToAdd)
    {
        Debug.Log("Add a collection by text: "+nameOfCollectionToAdd);
        if (SavingUtility.gameSettingsData.CollectionNames.Contains(nameOfCollectionToAdd))
        {
            PanelController.Instance.ShowInfo("This Collection name is allready in the list!");
            return;
        }
        SavingUtility.gameSettingsData.CollectionNames.Add(nameOfCollectionToAdd);
        CreateCollectionListItem(collectionList.Count, nameOfCollectionToAdd);
    }
    
    public void RemoveACollectionByText()
    {
        Debug.Log("Remove a collection by text");
        ConfirmInputPanel.Instance.ShowConfirmationOption("Remove Collection?", "Type the name of the Collection you want to remove from the list!", RemoveACollectionByText);
    }
    public void RemoveACollectionByText(string nameOfCollectionToRemove)
    {
        Debug.Log("Remove a collection by text: "+nameOfCollectionToRemove);
        if (SavingUtility.gameSettingsData.ActiveCollections.Contains(nameOfCollectionToRemove))
        {
            PanelController.Instance.ShowInfo("Can not remove active Collection from the list!");
        }else if (SavingUtility.gameSettingsData.CollectionNames.Contains(nameOfCollectionToRemove))
        {
            int index = collectionList.FindIndex(x => x.CollectionName == nameOfCollectionToRemove);
            Debug.Log("index for findindex "+index);
            if (index == -1)
            {
                PanelController.Instance.ShowInfo("This Collection could not be found in the list!");
                return;
            }
            CollectionListItem item = collectionList[index];
            collectionList.RemoveAt(index);
            Destroy(item.gameObject);
            SavingUtility.gameSettingsData.CollectionNames.Remove(nameOfCollectionToRemove);
        }
        else
            PanelController.Instance.ShowInfo("This Collection could not be found in the list!");

    }

}

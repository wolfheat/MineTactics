using System;
using TMPro;
using UnityEngine;

public class CollectionInfoPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI creatorId;
    [SerializeField] TextMeshProUGUI collection;
    [SerializeField] TextMeshProUGUI lastUpdate;

    private LevelData Data;
    public void RequestUpdateLevelInfo() => UpdateCollectionInfo();// FirestoreManager.Instance.LevelData);

    public void LoadSelected()
    {
        Debug.Log("Load Active Collection " + activeCollection);
        //LocalLevelsPanel.Instance.LoadLevel(Data);
    }
    private CollectionListItem activeCollection;
    public void UpdateSelected()
    {
        Debug.Log("Update Active Collection from Database: "+activeCollection.CollectionName);
        LoadPanel.Instance.RequestLoadCollection(activeCollection.CollectionName,true);
    }
    
    public void SetNewCollectionData(CollectionListItem collectionListItem)
    {
        activeCollection = collectionListItem;
        UpdateCollectionInfo();
    }

    public void UpdateCollectionInfo()
    {
        if (activeCollection == null)
            return;
        collection.text = activeCollection.CollectionName;
        /*Data = data;
        if(!panel.activeSelf)
            panel.gameObject.SetActive(true);
        creatorId.text = data.CreatorId.ToString();
        collection.text = data.Collection.ToString();
        lastUpdate.text = data.DifficultyRating.ToString();

        //
        */
    }

}

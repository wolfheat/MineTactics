using System.Collections.Generic;
using UnityEngine;

public class LoadPanel : MonoBehaviour
{
    [SerializeField] CollectionListItem listItemPrefab;
    [SerializeField] GameObject listHolder;


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

    public void RequestLoadLevel(string levelToLoad)
    {
        Debug.Log("RequestLoadLevel");
        FirestoreManager.Instance.LoadLevelCollection(levelToLoad);
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

using System;
using TMPro;
using UnityEngine;

public class LoadCollectionPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        FirestoreManager.OnLevelCollectionLevelsDownloaded += OnSuccessfulLoadLevels;
        FirestoreManager.OnLevelCollectionLevelsDownloadedFail += OnUnsuccessfulLoadLevels;
    }

    private void OnSuccessfulLoadLevels(int amt)
    {
        gameObject.SetActive(false);
        PanelController.Instance.ShowFadableInfo(amt + " Levels Added");
    }
    
    private void OnUnsuccessfulLoadLevels(string info)
    {
        //gameObject.SetActive(false);
        Debug.Log("SHOW HERE");
        PanelController.Instance.ShowInfo(info);
    }

    public void OnRequestGetCollection()
    {
        Debug.Log("Get Collection "+inputField.text);
        LocalLevelsPanel.Instance.LoadCollection(inputField.text);
    }
}

using TMPro;
using UnityEngine;
using WolfheatProductions;

public class LoadCollectionPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    // Start is called before the first frame update

    private const int Number = 2;

    void Start()
    {
        FirestoreManager.OnLevelCollectionLevelsDownloaded += OnSuccessfulLoadLevels;
        FirestoreManager.OnLevelCollectionLevelsDownloadedFail += OnUnsuccessfulLoadLevels;
        inputField.onValueChanged.AddListener(delegate { InputFieldValueChanged(); });
    }
    private void InputFieldValueChanged()
    {
        inputField.text = Converter.RemoveAllNonCharacters(inputField.text);
        //inputField.text = inputField.text.Replace(" ","");
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
        if (inputField.text.Length < 3)
            return;
        LocalLevelsPanel.Instance.LoadCollection(inputField.text);
    }
}

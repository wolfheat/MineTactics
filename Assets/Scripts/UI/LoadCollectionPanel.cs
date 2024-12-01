using TMPro;
using UnityEngine;

public class LoadCollectionPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void OnRequestGetCollection()
    {
        Debug.Log("Get Collection "+inputField.text);
        LocalLevelsPanel.Instance.LoadCollection(inputField.text);
    }
}

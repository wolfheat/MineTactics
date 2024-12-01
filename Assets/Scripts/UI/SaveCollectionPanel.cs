using TMPro;
using UnityEngine;

public class SaveCollectionPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    // Start is called before the first frame update
    void Start()
    {

    }


    public void OnRequestSendCollection()
    {
        Debug.Log("Send Collection with name" + inputField.text);
        LocalLevelsPanel.Instance.StoreCollection(inputField.text);
    }
}

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
        int lengthOfText = inputField.text.Length;
        if(lengthOfText < 4)
        {
            PanelController.Instance.ShowInfo("The name is to short. Needs to be 4+ letters!");
            return;
        }
        Debug.Log("Send Collection with name" + inputField.text);
        LocalLevelsPanel.Instance.StoreCollection(inputField.text);
    }
}

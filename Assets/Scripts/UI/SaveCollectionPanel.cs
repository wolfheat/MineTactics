using TMPro;
using UnityEngine;
using WolfheatProductions;

public class SaveCollectionPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        inputField.onValueChanged.AddListener(delegate { InputFieldValueChanged(); });
    }

    private void InputFieldValueChanged()
    {
        inputField.text = Converter.RemoveAllNonCharacters(inputField.text);
        //inputField.text = inputField.text.Replace(" ","");
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

using System;
using TMPro;
using UnityEngine;
using WolfheatProductions;

public class ConfirmInputPanel : ConfirmPanelBase
{
    [SerializeField] TMP_InputField inputField;

    public static ConfirmInputPanel Instance { get; private set; }

    protected void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        inputField.onValueChanged.AddListener(delegate { InputFieldValueChanged(); });
    }

    private void InputFieldValueChanged() => inputField.text = Converter.RemoveAllNonCharacters(inputField.text);

    public override void YesClicked()
    {
        int lengthOfText = inputField.text.Length;

        if (lengthOfText < 4)
        {
            PanelController.Instance.ShowInfo("The name is to short. Needs to be 4+ letters!");
            return;
        }

        if (!Pascal())
        {
            PanelController.Instance.ShowInfo("Please use a Pascal Name formatting! Xxxxx");
            return;
        }

        Debug.Log("Send Collection with name" + inputField.text);
        Callback?.Invoke(inputField.text);

        // Show loading panel here?
        Debug.Log("LOADING PANEL");
        panel.SetActive(false);
    }

    private bool Pascal()
    {
        string word = inputField.text;
        if (!Char.IsUpper(word[0]))
            return false;
        for (int i = 1; i < word.Length; i++) {
            char c = word[i];
            if (!Char.IsLower(word[i]))
                return false;
        }
        return true;
    }
}

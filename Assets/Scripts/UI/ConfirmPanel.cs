using System;
using TMPro;
using UnityEngine;

public class ConfirmPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI header_text;
    [SerializeField] TextMeshProUGUI info_text;
    [SerializeField] GameObject panel;

    private Action Callback;

    public static ConfirmPanel Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void ShowConfirmationOption(string header, string info, Action CallbackSent)
    {
        panel.SetActive(true);
        Callback = CallbackSent;
        header_text.text = header;
        info_text.text = info;
    }

    public void YesClicked()
    {
        Callback?.Invoke();
        panel.SetActive(false);
    }
}

using System;
using TMPro;
using UnityEngine;
public abstract class ConfirmPanelBase : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI header_text;
    [SerializeField] TextMeshProUGUI info_text;
    [SerializeField] protected GameObject panel;

    protected Action<string> Callback;

    public void ShowConfirmationOption(string header, string info, Action<string> CallbackSent)
    {
        panel.SetActive(true);
        Callback = CallbackSent;
        header_text.text = header;
        info_text.text = info;
    }

    public abstract void YesClicked();
}

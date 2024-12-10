public class ConfirmPanel : ConfirmPanelBase
{
    public static ConfirmPanel Instance { get; private set; }

    protected void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void YesClicked()
    {
        Callback?.Invoke("");
        panel.SetActive(false);
    }

}

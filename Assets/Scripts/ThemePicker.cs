using UnityEngine;

public class ThemePicker : MonoBehaviour
{
    public NumberThemeSO current;
    [SerializeField] NumberThemeSO[] themes;

    public static ThemePicker Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetTheme(int index)
    {
        if (index >= themes.Length) return;

        current = themes[index];
    }
}

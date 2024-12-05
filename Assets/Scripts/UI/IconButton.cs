using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconButton : MonoBehaviour
{
    [SerializeField] Color unselectedIconColor;
    [SerializeField] Color unselectedBackgroundColor;
    [SerializeField] Color selectedIconColor;
    [SerializeField] Color selectedBackgroundColor;
    [SerializeField] Image image;
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI text_field;


    public void SetSelected(bool set)
    {
        text_field.color = set?selectedIconColor:unselectedIconColor;
        image.color = set?selectedIconColor:unselectedIconColor;
        background.color = set?selectedBackgroundColor:unselectedBackgroundColor;
    }
}

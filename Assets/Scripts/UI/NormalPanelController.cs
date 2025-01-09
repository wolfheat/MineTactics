using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NormalPanelController : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI boardSizeText;
    private bool expanded = false;
    private float heigth;

    private void OnEnable()
    {
        // Initiate the Create With start of creat buttons and 
        SetSliderSize();
        heigth = transform.GetComponent<RectTransform>().rect.height;
    }

    public void ExpandMenu()
    {
        Debug.Log("Expand");
        if (expanded)
        {
            transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y - heigth,transform.localPosition.z);
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y + heigth,transform.localPosition.z);
        }
        expanded = !expanded;
    }


    public void ConfirmSettings()
    {
        USerInfo.Instance.BoardType = BoardTypes.Slider;
        USerInfo.Instance.BoardSize = (int)slider.value;
        LevelCreator.Instance.RestartGame(false,true);
    }

    public void RequestStartBeginner()
    {
        Debug.Log("Beginner");
        USerInfo.Instance.BoardType = BoardTypes.Beginner;
        LevelCreator.Instance.RestartGame(false,true);
    }
    public void RequestStartIntermediate()
    {
        Debug.Log("Intermediate");
        USerInfo.Instance.BoardType = BoardTypes.Intermediate;
        LevelCreator.Instance.RestartGame(false, true);
    }
    public void RequestStartExpert()
    {
        Debug.Log("Expert");
        USerInfo.Instance.BoardType = BoardTypes.Expert;
        LevelCreator.Instance.RestartGame(false, true);
    }

    public void UpdateCreateLevelSize()
    {
        // Read value of slider and update
        int nexValue = (int)slider.value;
        boardSizeText.text = nexValue + "x" + nexValue;
        ConfirmSettings();
    }
    public void SetSliderSize() => slider.value = USerInfo.Instance.BoardSize;
}

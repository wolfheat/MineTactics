using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NormalPanelController : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI boardSizeText;
    private bool expanded = true;
    private float heigth;
    private Vector3 panelOpenPosition;
    private Vector3 panelClosedPosition;
    Coroutine activeRoutine;
    Coroutine autoClose;

    const float TotTime = 0.1f;
    const float WaitTime = 4f;
    const float WaitTimeFast = 0.3f;

    private void OnEnable()
    {
        // Initiate the Create With start of creat buttons and 
        SetSliderSize();
        autoClose = StartCoroutine(AutoClose(true));

    }
    private void Start()
    {
        heigth = transform.GetComponent<RectTransform>().rect.height;            
        panelOpenPosition = transform.localPosition;
        panelClosedPosition = transform.localPosition + Vector3.up*heigth;
    }

    public void ExpandMenu()
    {
        if (activeRoutine != null) 
            return;
        Debug.Log("Expand");
        if (expanded)
        {
            //transform.localPosition = panelClosedPosition;
            activeRoutine = StartCoroutine(CloseMenu("close"));
         //   transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y - heigth,transform.localPosition.z);
        }
        else
        {
            //transform.localPosition = panelOpenPosition;
            activeRoutine = StartCoroutine(CloseMenu("open"));
            //transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y + heigth,transform.localPosition.z);
        }
        expanded = !expanded;
    }

    private IEnumerator AutoClose(bool fast = false)
    {
        float t = 0f;
        while (t < (fast? WaitTimeFast:WaitTime))
        {
            t += Time.deltaTime;               
            yield return null;
        }
        if(activeRoutine == null)
            StartCoroutine(CloseMenu("close"));
    }
    private IEnumerator CloseMenu(string type)
    {
        Vector3 start = type=="close"?panelOpenPosition:panelClosedPosition;
        Vector3 end = type == "close"?panelClosedPosition:panelOpenPosition;

        float YPos = transform.localPosition.y;
        float t = 0f;
        while(type=="close"?(YPos < end.y): (YPos > end.y))
        {
            YPos = Mathf.Lerp(start.y, end.y, t/TotTime);
            transform.localPosition = new Vector3(transform.localPosition.x,YPos, transform.localPosition.z);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = end;  
        if(type == "open")
        {
            StartAutoCloseCO();            
        }
        else
            expanded = false;
        activeRoutine = null;
    }

    public void StartAutoCloseCO(bool fast = false)
    {
        if (autoClose != null)
            StopCoroutine(autoClose);
        autoClose = StartCoroutine(AutoClose(fast));
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
        StartAutoCloseCO(true);
    }
    public void RequestStartIntermediate()
    {
        Debug.Log("Intermediate");
        USerInfo.Instance.BoardType = BoardTypes.Intermediate;
        LevelCreator.Instance.RestartGame(false, true);
        StartAutoCloseCO(true);
    }
    public void RequestStartExpert()
    {
        Debug.Log("Expert");
        USerInfo.Instance.BoardType = BoardTypes.Expert;
        LevelCreator.Instance.RestartGame(false, true);
        StartAutoCloseCO(true);
    }

    public void UpdateCreateLevelSize()
    {
        // Read value of slider and update
        int nexValue = (int)slider.value;
        boardSizeText.text = nexValue + "x" + nexValue;
        ConfirmSettings();
        StartAutoCloseCO();
    }
    public void SetSliderSize() => slider.value = USerInfo.Instance.BoardSize;
}

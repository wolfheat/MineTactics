
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandeler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] TextMeshProUGUI info;
    [SerializeField] GameObject ui;
    void Start()
    {
        AuthManager.OnDependenciesSuccess += ShowDependencyResult;
        AuthManager.OnShowInfo += ShowInfo;
    }

    private void ShowInfo(string infoText)
    {
            info.text = infoText+" "+timer;        
    }
    
    private void ShowDependencyResult(string result)
    {
        StartCoroutine(DelayedLoad(result));
    }

    int timer = 2;
    private IEnumerator DelayedLoad(string result)
    {
        resultText.text = result;
        while (timer > 0) { 
            yield return new WaitForSeconds(1);
            timer--;
            resultText.text = result+" "+timer;
        }

        // Remove the start UI 
        ui.SetActive(false);

        Debug.Log("Dependencies success - Load Main");
        // Unload Camera
        Destroy(Camera.main.gameObject);
        Destroy(EventSystemSingleton.Instance.gameObject);
        SceneManager.LoadScene("Main",LoadSceneMode.Additive);
        Debug.Log(" -- Loading Main Scene --");
        Debug.Log(" ");

    }
}

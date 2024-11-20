
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandeler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] GameObject ui;
    void Start()
    {
        AuthManager.OnDependenciesSuccess += ShowDependencyResult;
    }

    private void ShowDependencyResult(string result)
    {
        StartCoroutine(DelayedLoad(result));
    }

    int timer = 10;
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
        SceneManager.LoadScene("Main",LoadSceneMode.Additive);
    }
}

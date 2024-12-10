using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemoveAllFromCollectionPanel : MonoBehaviour
{
    [SerializeField] Button RemoveButton;
    [SerializeField] TMP_Dropdown DropDownField;
    [SerializeField] TMP_Dropdown DropDownExpression;
    [SerializeField] TMP_InputField inputFieldCompareAgainst;
    private List<string> valueStringList = new List<string>{"CreatorId","Downvotes", "Upvotes", "DifficultyRating","PlayCount"};
    private List<LevelData> Query { get; set; }

    private void OnEnable()
    {
        UpdateDropdown();
    }

    private void UpdateDropdown()
    {
        DropDownField.options.Clear();
        foreach (string t in valueStringList)
            DropDownField.options.Add(new TMP_Dropdown.OptionData() { text = t });
    }
    public void OnRemoveClicked()
    {
        Debug.Log("Removed Selection Clicked");
        string option = DropDownField.options[DropDownField.value].text;
        string operatorType = DropDownExpression.options[DropDownExpression.value].text;
        string against = inputFieldCompareAgainst.text;
        Int32.TryParse(against, out int againstInt);

        Debug.Log(option+" "+ operatorType+ " "+against);
        Query = new();
        switch (option)
        {
            case "CreatorId":
                if(operatorType == "!=")
                {
                    Debug.Log("Checking if not created by " + against);
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.CreatorId != against).ToList();
                }
                else if(operatorType == "==")
                {
                    Debug.Log("Checking if Creator Id is " + against);
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.CreatorId == against).ToList();
                }
                else
                    return; 
                break;
            case "Downvotes":
                if (operatorType == ">")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Downvotes > againstInt).ToList();
                    Debug.Log("Checking if having more DownVotes than " + against + " AMT: " + Query?.Count);
                }
                else if (operatorType == "<")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Downvotes < againstInt).ToList();
                    Debug.Log("Checking if having less DownVotes than " + against + " AMT: " + Query?.Count);
                }
                else if (operatorType == "==")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Downvotes == againstInt).ToList();
                    Debug.Log("Checking if having exactly " + against + " DownVotes.  AMT: " + Query?.Count);
                }
                else if (operatorType == "!=")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Downvotes != againstInt).ToList();
                    Debug.Log("Checking if having all but " + against + " DownVotes.  AMT: " + Query?.Count);
                }
                else
                    return;
                break;
            case "Upvotes":
                if (operatorType == ">")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Upvotes > againstInt).ToList();
                    Debug.Log("Checking if having more Upvotes than " + against + " AMT: " + Query?.Count);
                }
                else if (operatorType == "<")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Upvotes < againstInt).ToList();
                    Debug.Log("Checking if having less Upvotes than " + against + " AMT: " + Query?.Count);
                }
                else if (operatorType == "==")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Upvotes == againstInt).ToList();
                    Debug.Log("Checking if having exactly " + against + " Upvotes.  AMT: " + Query?.Count);
                }
                else if (operatorType == "!=")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Upvotes != againstInt).ToList();
                    Debug.Log("Checking if having all but " + against + " Upvotes.  AMT: " + Query?.Count);
                }
                else
                    return;
                break;
            case "DifficultyRating":
                if (operatorType == ">")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.DifficultyRating > againstInt).ToList();
                    Debug.Log("Checking if having more DifficultyRating than " + against+" AMT: "+Query?.Count);
                }
                else if (operatorType == "<")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.DifficultyRating < againstInt).ToList();
                    Debug.Log("Checking if having less DifficultyRating than " + against + " AMT: " + Query?.Count);
                }
                else if (operatorType == "==")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.DifficultyRating == againstInt).ToList();
                    Debug.Log("Checking if having exactly " + against + " DifficultyRating.  AMT: " + Query?.Count);
                }
                else if (operatorType == "!=")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.DifficultyRating != againstInt).ToList();
                    Debug.Log("Checking if having all but " + against + " DifficultyRating.  AMT: " + Query?.Count);
                }
                else
                    return;
                break;
            case "PlayCount":
                if (operatorType == ">")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.PlayCount > againstInt).ToList();
                    Debug.Log("Checking if having more PlayCount than " + against + " AMT: " + Query?.Count);
                }
                else if (operatorType == "<")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.PlayCount < againstInt).ToList();
                    Debug.Log("Checking if having less PlayCount than " + against + " AMT: " + Query?.Count);
                }
                else if (operatorType == "==")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.PlayCount == againstInt).ToList();
                    Debug.Log("Checking if having exactly " + against + " PlayCount.  AMT: " + Query?.Count);
                }
                else if (operatorType == "!=")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.PlayCount != againstInt).ToList();
                    Debug.Log("Checking if having all but " + against + " PlayCount.  AMT: " + Query?.Count);
                }
                else
                    return;
                break;

        }
        if(Query.Count > 0)
        {
            Debug.Log("Remove "+Query.Count+" Items from Collection?");
            ConfirmPanel.Instance.ShowConfirmationOption("Delete Query?","Are you sure you want to delete this Query of "+Query.Count+" levels?", OnAcceptDelete);
            //PanelController.Instance.ShowConfirmRemoveManyScreen(Query);
        }
        else
        {
            PanelController.Instance.ShowFadableInfo("No Levels To Remove!");
        }
    }

    public void OnAcceptDelete(string info)
    {
        // remove Levels Here
        LocalLevelsPanel.Instance.RemoveQueryFromList(Query);
        PanelController.Instance.ShowFadableInfo(Query.Count + " Levels Deleted");
        gameObject.SetActive(false);
    }
}

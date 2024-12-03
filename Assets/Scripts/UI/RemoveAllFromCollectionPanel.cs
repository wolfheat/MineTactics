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
        List<LevelData> query = new();
        switch (option)
        {
            case "CreatorId":
                if(operatorType == "!=")
                {
                    Debug.Log("Checking if not created by " + against);
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.CreatorId != against).ToList();
                }
                else if(operatorType == "==")
                {
                    Debug.Log("Checking if Creator Id is " + against);
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.CreatorId == against).ToList();
                }
                else
                    return; 
                break;
            case "Downvotes":
                if (operatorType == ">")
                {
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Downvotes > againstInt).ToList();
                    Debug.Log("Checking if having more DownVotes than " + against + " AMT: " + query?.Count);
                }
                else if (operatorType == "<")
                {
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Downvotes < againstInt).ToList();
                    Debug.Log("Checking if having less DownVotes than " + against + " AMT: " + query?.Count);
                }
                else if (operatorType == "==")
                {
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Downvotes == againstInt).ToList();
                    Debug.Log("Checking if having exactly " + against + " DownVotes.  AMT: " + query?.Count);
                }
                else if (operatorType == "!=")
                {
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Downvotes != againstInt).ToList();
                    Debug.Log("Checking if having all but " + against + " DownVotes.  AMT: " + query?.Count);
                }
                else
                    return;
                break;
            case "Upvotes":
                if (operatorType == ">")
                {
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Upvotes > againstInt).ToList();
                    Debug.Log("Checking if having more Upvotes than " + against + " AMT: " + query?.Count);
                }
                else if (operatorType == "<")
                {
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Upvotes < againstInt).ToList();
                    Debug.Log("Checking if having less Upvotes than " + against + " AMT: " + query?.Count);
                }
                else if (operatorType == "==")
                {
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Upvotes == againstInt).ToList();
                    Debug.Log("Checking if having exactly " + against + " Upvotes.  AMT: " + query?.Count);
                }
                else if (operatorType == "!=")
                {
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Upvotes != againstInt).ToList();
                    Debug.Log("Checking if having all but " + against + " Upvotes.  AMT: " + query?.Count);
                }
                else
                    return;
                break;
            case "DifficultyRating":
                if (operatorType == ">")
                {
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.DifficultyRating > againstInt).ToList();
                    Debug.Log("Checking if having more DifficultyRating than " + against+" AMT: "+query?.Count);
                }
                else if (operatorType == "<")
                {
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.DifficultyRating < againstInt).ToList();
                    Debug.Log("Checking if having less DifficultyRating than " + against + " AMT: " + query?.Count);
                }
                else if (operatorType == "==")
                {
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.DifficultyRating == againstInt).ToList();
                    Debug.Log("Checking if having exactly " + against + " DifficultyRating.  AMT: " + query?.Count);
                }
                else if (operatorType == "!=")
                {
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.DifficultyRating != againstInt).ToList();
                    Debug.Log("Checking if having all but " + against + " DifficultyRating.  AMT: " + query?.Count);
                }
                else
                    return;
                break;
            case "PlayCount":
                if (operatorType == ">")
                {
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.PlayCount > againstInt).ToList();
                    Debug.Log("Checking if having more PlayCount than " + against + " AMT: " + query?.Count);
                }
                else if (operatorType == "<")
                {
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.PlayCount < againstInt).ToList();
                    Debug.Log("Checking if having less PlayCount than " + against + " AMT: " + query?.Count);
                }
                else if (operatorType == "==")
                {
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.PlayCount == againstInt).ToList();
                    Debug.Log("Checking if having exactly " + against + " PlayCount.  AMT: " + query?.Count);
                }
                else if (operatorType == "!=")
                {
                    query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.PlayCount != againstInt).ToList();
                    Debug.Log("Checking if having all but " + against + " PlayCount.  AMT: " + query?.Count);
                }
                else
                    return;
                break;

        }
        if(query.Count > 0)
        {
            Debug.Log("Remove "+query.Count+" Items from Collection?");
            PanelController.Instance.ShowConfirmRemoveManyScreen(query);
        }
        else
        {
            PanelController.Instance.ShowFadableInfo("No Levels To Remove!");
        }


    }

}

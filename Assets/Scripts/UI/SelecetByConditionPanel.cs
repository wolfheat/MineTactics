using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelecetByConditionPanel : MonoBehaviour
{
    [SerializeField] Button RemoveButton;
    [SerializeField] TMP_Dropdown DropDownField;
    [SerializeField] TMP_Dropdown DropDownExpression;
    [SerializeField] TMP_InputField inputFieldCompareAgainst;
    private List<string> valueStringList = new List<string>{"CreatorId","Collection","Downvotes", "Upvotes", "DifficultyRating","PlayCount"};
    private List<int> Query { get; set; }

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
    public void OnSelectClicked()
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
                if (operatorType == "==")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.CreatorId == against)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else if (operatorType == "!=")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.CreatorId != against)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else
                    return; 
                break;
            case "Collection":
                
                if (operatorType == "==")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.Collection == against)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else if (operatorType == "!=")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.Collection != against)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else
                    return; 
                break;
            case "Downvotes":

                if (operatorType == ">")
                {
                    //Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Downvotes > againstInt).Select(x => x.index).ToList();
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.Downvotes > againstInt)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else if (operatorType == "<")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.Downvotes < againstInt)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else if (operatorType == "==")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.Downvotes == againstInt)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else if (operatorType == "!=")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.Downvotes != againstInt)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else
                    return;
                break;
            case "Upvotes":
                if (operatorType == ">")
                {
                    //Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Downvotes > againstInt).Select(x => x.index).ToList();
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.Upvotes > againstInt)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else if (operatorType == "<")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.Upvotes < againstInt)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else if (operatorType == "==")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.Upvotes == againstInt)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else if (operatorType == "!=")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.Upvotes != againstInt)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else
                    return;
                break;
            case "DifficultyRating":
                if (operatorType == ">")
                {
                    //Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Downvotes > againstInt).Select(x => x.index).ToList();
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.DifficultyRating > againstInt)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else if (operatorType == "<")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.DifficultyRating < againstInt)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else if (operatorType == "==")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.DifficultyRating == againstInt)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else if (operatorType == "!=")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.DifficultyRating != againstInt)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else
                    return;
                break;
            case "PlayCount":
                if (operatorType == ">")
                {
                    //Query = FirestoreManager.Instance.LocalCollectionList?.Where(x => x.Downvotes > againstInt).Select(x => x.index).ToList();
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.PlayCount > againstInt)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else if (operatorType == "<")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.PlayCount < againstInt)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else if (operatorType == "==")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.PlayCount == againstInt)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else if (operatorType == "!=")
                {
                    Query = FirestoreManager.Instance.LocalCollectionList?.Select((item, index) => new { Item = item, Index = index })
                    .Where(x => x.Item.PlayCount != againstInt)                  // Filter based on the condition
                    .Select(x => x.Index)                                       // Select only the indexes
                    .ToList();
                }
                else
                    return;
                break;

        }
        if(Query.Count > 0)
        {
            Debug.Log("Remove "+Query.Count+" Items from Collection?");
            ConfirmPanel.Instance.ShowConfirmationOption("Selection","This will select "+Query.Count+" levels?", OnAcceptSelect);
            //PanelController.Instance.ShowConfirmRemoveManyScreen(Query);
        }
        else
        {
            PanelController.Instance.ShowFadableInfo("No Levels Satisfies the condition!");
        }
    }

    public void OnAcceptSelect(string info)
    {
        // Select Levels Here
        LocalLevelsPanel.Instance.AddQueryToSelectedList(Query);
        PanelController.Instance.ShowFadableInfo(Query.Count + " Levels Selected");
        gameObject.SetActive(false);
    }
}

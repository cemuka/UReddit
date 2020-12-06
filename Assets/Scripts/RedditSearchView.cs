using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RedditSearchView : MonoBehaviour
{
    public TMP_InputField searchField;

    public void Init(Action<string> onSearchEdit)
    {
        searchField.onEndEdit.AddListener(keyword => onSearchEdit(keyword));
    }
}
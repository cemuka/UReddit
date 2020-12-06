using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SearchResultCardView : MonoBehaviour
{
    public TMP_Text subredditText;
    public TMP_Text authorText;
    public TMP_Text titleText;
    public TMP_Text upsText;
    public Image thumbnail;
    public Button selectButton;

    private SearchResultCardInfo _context;

    public void Init(SearchResultCardInfo info, System.Action<string> onClick)
    {
        this._context = info;

        subredditText.text = "r/" + _context.subreddit;
        authorText.text = "u/" + _context.author;
        titleText.text = _context.title;
        upsText.text = _context.ups > 1000 ? (_context.ups / 1000D).ToString("0.#") + "K" : _context.ups.ToString("000");

        if (_context.thumbnail != null)
        {
            this.thumbnail.sprite = _context.thumbnail;
        }        

        selectButton.onClick.AddListener(() => onClick(_context.id));
    }

    public SearchResultCardInfo GetContext()
    {
        return _context;
    }
}

public class SearchResultCardInfo
{
    public string subreddit;    
    public string author;
    public string title;
    public Sprite thumbnail;
    public int ups;
    public string id;
    public string selftext;
}

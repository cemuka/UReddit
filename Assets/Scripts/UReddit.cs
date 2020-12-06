
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UReddit : MonoBehaviour
{
    public RedditSearchView _searchView;
    [Header("Prefabs")]
    public GameObject redditChildPrefab;
    public GameObject redditMainPrefab;

    [Header("References")]
    public Transform sidePanelParent;
    public Transform contentParent;
    public Transform mainParent;

    private string _baseUrl = "https://www.reddit.com/search.json?q=";
    private string _searchKeyword = "home";

    private Dictionary<string, SearchResultCardView> _childs;
    private RedditMainView _mainView;

    private void Start()
    {
        _searchView.Init(OnSearchEdit);

        var mainGO = Instantiate(redditMainPrefab, mainParent);
        _mainView = mainGO.GetComponent<RedditMainView>();
        _mainView.Hide();

        _childs = new Dictionary<string, SearchResultCardView>();

        _searchView.searchField.text = _searchKeyword;
        Populate();
    }

    private async void Populate()
    {
        var response = await FetchRequestAsync(_baseUrl, _searchKeyword);

        for (int i = 0; i < response.data.children.Count; i++)
        {
            var container = new SearchResultCardInfo();
            container.subreddit = response.data.children[i].data.subreddit;
            container.author = response.data.children[i].data.author;
            container.title = response.data.children[i].data.title;
            container.ups = response.data.children[i].data.ups;
            container.id = response.data.children[i].data.id;
            container.selftext = response.data.children[i].data.selftext;


            if (!string.IsNullOrEmpty(response.data.children[i].data.thumbnail) && 
                    response.data.children[i].data.thumbnail != "default" && 
                    response.data.children[i].data.thumbnail != "self" &&
                    response.data.children[i].data.thumbnail != "image")
            {

                // Uri uriResult;
                // bool result = Uri.TryCreate(response.data.children[i].data.thumbnail, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
                
                // Debug.Log(result);
                // if (result)
                // {
                //     var imageBytes = await DownloadImageAsync(uriResult);
                //     container.thumbnail = CreateSprite(imageBytes);      
                // }

                if (Uri.IsWellFormedUriString(response.data.children[i].data.thumbnail, UriKind.Absolute))
                {
                    var imageBytes = await DownloadDataAsync(new Uri(response.data.children[i].data.thumbnail));
                    container.thumbnail = CreateSprite(imageBytes); 
                }
            }

            var childGO = Instantiate(redditChildPrefab, contentParent);
            var childView = childGO.GetComponent<SearchResultCardView>();

            childView.Init(container, OnChildClicked);
            _childs.Add(container.id, childView);
        }
    }

    private void OnChildClicked(string id)
    {
        var child = _childs[id];
        var data = child.GetContext();
        _mainView.Display(data.thumbnail, data.title, data.selftext);
    }

    private void OnSearchEdit(string searchKeyword)
    {
        _searchKeyword = searchKeyword;
        ClearChilds();

        Populate();
    }

    private void ClearChilds()
    {
        foreach (var child in _childs.Values)
        {
            Destroy(child.gameObject);
        }
        _childs.Clear();
    }

    private async Task<Response> FetchRequestAsync(string url, string searchKey)
    {
        using (var httpClient = new HttpClient())
        {
            var result = await httpClient.GetAsync(url + searchKey);
            var response = await result.Content.ReadAsStringAsync();

            Debug.Log(result.StatusCode.ToString());

            return JsonUtility.FromJson<Response>(response);
        }
    }

    private async Task<byte[]> DownloadDataAsync(Uri url)
    {
        using (var httpClient = new HttpClient())
        {
            return await httpClient.GetByteArrayAsync(url);
        }
    }

    private Sprite CreateSprite(byte[] stream)
    {
        Texture2D tex = new Texture2D(100, 100, TextureFormat.RGBA32, false);
        tex.LoadImage(stream);
        tex.Apply();

        var rect = new Rect(0, 0, tex.width, tex.height);
        return Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));
    }
}


[Serializable]
public class RedditContentData
{
    public string subreddit;    
    public string author;
    public string title;
    public string thumbnail;
    public int ups;
    public string id;
    public string selftext;
}

[Serializable]
public class RedditChild
{
    public string kind;
    public RedditContentData data;
}

[Serializable]
public class Data
{
    public List<RedditChild> children;
}

[Serializable]
public class Response
{
    public string kind;
    public Data data;
}



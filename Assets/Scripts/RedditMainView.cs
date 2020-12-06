using UnityEngine;
using UnityEngine.UI;

public class RedditMainView : MonoBehaviour
{
    public GameObject panel;
    public Image image;
    public Text headerText;
    public Text bodyText;

    public void Display(Sprite image, string header, string body)
    {
        panel.gameObject.SetActive(true);
        this.image.sprite = image;
        headerText.text = header;
        bodyText.text = body;

    }

    public void Hide()
    {
        panel.gameObject.SetActive(false);
    }
}
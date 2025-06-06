using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class SongItemUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI channelText;
    public Image thumbnailImage;
    public Button playButton;
    
    private SongData songData;
    
    public void Initialize(SongData data)
    {
        songData = data;
        titleText.text = data.title;
        channelText.text = data.channelTitle;
        
        // Load thumbnail (you'll need to implement this)
        StartCoroutine(LoadThumbnail(data.thumbnailUrl));
        
        playButton.onClick.AddListener(OnPlayClicked);
    }
    
    private IEnumerator LoadThumbnail(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                thumbnailImage.sprite = Sprite.Create(texture, 
                    new Rect(0, 0, texture.width, texture.height), 
                    Vector2.one * 0.5f);
            }
        }
    }
    
    private void OnPlayClicked()
    {
        AudioPlayerManager.Instance.PlayAudioFromUrl(songData.videoId);
    }
}
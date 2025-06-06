using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class YouTubeAPIHandler : MonoBehaviour
{
    public static YouTubeAPIHandler Instance { get; private set; }

    private const string API_KEY = "AIzaSyB3TY4TgQLnOJMvGi6TWGv7qDTDF2kLqlU";
    private const string BASE_URL = "https://www.googleapis.com/youtube/v3/";
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public IEnumerator SearchSongs(string query, System.Action<List<SongData>> callback)
    {
        string url = $"{BASE_URL}search?part=snippet&maxResults=20&q={UnityWebRequest.EscapeURL(query)}&type=video&key={API_KEY}";
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var searchResponse = JsonConvert.DeserializeObject<YouTubeSearchResponse>(webRequest.downloadHandler.text);
                List<SongData> songs = new List<SongData>();
                
                foreach (var item in searchResponse.items)
                {
                    songs.Add(new SongData
                    {
                        videoId = item.id.videoId,
                        title = item.snippet.title,
                        thumbnailUrl = item.snippet.thumbnails.medium.url,
                        channelTitle = item.snippet.channelTitle
                    });
                }
                
                callback(songs);
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
                callback(null);
            }
        }
    }
}

[System.Serializable]
public class YouTubeSearchResponse
{
    public List<YouTubeSearchItem> items;
}

[System.Serializable]
public class YouTubeSearchItem
{
    public YouTubeId id;
    public YouTubeSnippet snippet;
}

[System.Serializable]
public class YouTubeId
{
    public string videoId;
}

[System.Serializable]
public class YouTubeSnippet
{
    public string title;
    public string channelTitle;
    public YouTubeThumbnails thumbnails;
}

[System.Serializable]
public class YouTubeThumbnails
{
    public YouTubeThumbnail medium;
}

[System.Serializable]
public class YouTubeThumbnail
{
    public string url;
}

[System.Serializable]
public class SongData
{
    public string videoId;
    public string title;
    public string thumbnailUrl;
    public string channelTitle;
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public TMP_InputField searchInput;
    public Button searchButton;
    public Transform resultsContainer;
    public GameObject songPrefab;
    public Slider volumeSlider;
    
    private List<GameObject> currentResults = new List<GameObject>();
    
    private void Start()
    {
        searchButton.onClick.AddListener(OnSearch);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }
    
    private void OnSearch()
    {
        string query = searchInput.text;
        if (!string.IsNullOrEmpty(query))
        {
            StartCoroutine(YouTubeAPIHandler.Instance.SearchSongs(query, DisplayResults));
        }
    }
    
    private void DisplayResults(List<SongData> songs)
    {
        // Clear previous results
        foreach (var result in currentResults)
        {
            Destroy(result);
        }
        currentResults.Clear();
        
        if (songs == null || songs.Count == 0)
        {
            Debug.Log("No results found");
            return;
        }
        
        // Display new results
        foreach (var song in songs)
        {
            GameObject songItem = Instantiate(songPrefab, resultsContainer);
            SongItemUI itemUI = songItem.GetComponent<SongItemUI>();
            itemUI.Initialize(song);
            currentResults.Add(songItem);
        }
    }
    
    private void OnVolumeChanged(float value)
    {
        AudioPlayerManager.Instance.SetVolume(value);
    }
}
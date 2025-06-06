using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.Networking;

public class AudioPlayerManager : MonoBehaviour
{
    public static AudioPlayerManager Instance;
    public AudioSource audioSource;
    public AudioMixer audioMixer;

    // For streaming from a legal audio source
    private string currentStreamingUrl;
    private Coroutine currentStreamingCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize audio source
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Legal approach: Play from a licensed audio stream URL
    public void PlayAudioFromUrl(string audioUrl)
    {
        if (currentStreamingCoroutine != null)
        {
            StopCoroutine(currentStreamingCoroutine);
        }

        currentStreamingUrl = audioUrl;
        currentStreamingCoroutine = StartCoroutine(StreamAudioFromUrl(audioUrl));
    }

    private IEnumerator StreamAudioFromUrl(string audioUrl)
    {
        // First stop any currently playing audio
        audioSource.Stop();

        // Check if this is an MP3 stream (most common for audio)
        if (audioUrl.EndsWith(".mp3"))
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = clip;
                    audioSource.Play();
                }
                else
                {
                    Debug.LogError($"Audio stream error: {www.error}");
                }
            }
        }
        else
        {
            Debug.LogError("Unsupported audio format");
        }
    }

    // Alternative approach using YouTube IFrame API (video with hidden player)
    public void PlayYouTubeAudio(string videoId)
    {
        // This would require a WebView or browser integration
        // On mobile, you might open the YouTube app with the video ID
        Application.OpenURL($"https://www.youtube.com/watch?v={videoId}");

        // Note: This will play the full video, not just audio
    }

    public void Pause()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void Resume()
    {
        if (!audioSource.isPlaying && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    public void Stop()
    {
        audioSource.Stop();
        if (currentStreamingCoroutine != null)
        {
            StopCoroutine(currentStreamingCoroutine);
            currentStreamingCoroutine = null;
        }
    }

    public void SetVolume(float volume)
    {
        // Convert linear 0-1 volume to decibel scale
        float dB = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
        audioMixer.SetFloat("MasterVolume", dB);
    }

    public bool IsPlaying
    {
        get { return audioSource.isPlaying; }
    }

    public float CurrentTime
    {
        get { return audioSource.time; }
        set { audioSource.time = value; }
    }

    public float ClipLength
    {
        get { return audioSource.clip != null ? audioSource.clip.length : 0f; }
    }
}
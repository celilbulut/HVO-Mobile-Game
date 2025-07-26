using System.Collections.Generic;
using UnityEngine;

public enum AudioPriority
{
    High = 0,
    Medium = 128,
    Low = 256
}

[System.Serializable]
public class AudioSettings
{
    public AudioClip[] Clips = null;         // Oynatılacak ses klipleri (birden fazla olabilir)
    public float Volume = 1.0f;                // Sesin şiddeti
    public float Pitch = 1.0f;                 // Sesin frekansı
    public bool Loop = false;                // Ses tekrar etmeli mi?
    public float SpatialBlend = 1.0f;          // 0 = 2D ses, 1 = 3D ses
    public float MinDistance = 1.0f;           // Kamera-ses kaynağı minimum mesafe
    public float MaxDistance = 15.0f;          // Kamera-ses kaynağı maksimum mesafe
    public AudioPriority Priority = AudioPriority.Medium;  // Ses önceliği
    public AudioRolloffMode RolloffMode = AudioRolloffMode.Linear;
}

public class AudioManager : SingletonManager<AudioManager>
{
    [SerializeField] private int m_InitialPoolSize = 10;

    private Queue<AudioSource> m_AudioSourcePool;
    private List<AudioSource> m_ActiveSources;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        InitializeAudioPool();
    }

    public void PlaySound(AudioSettings audioSettings, Vector3 position)
    {
        if (audioSettings == null || audioSettings.Clips.Length == 0) return;

        var source = GetAvailableAudioSource();
        Debug.Log(source);
        Debug.Log("Audio Pool: " + m_AudioSourcePool.Count);
        Debug.Log("Active Sounds: " + m_ActiveSources.Count);
    }

    AudioSource GetAvailableAudioSource()
    {
        if (m_AudioSourcePool.Count > 0)
        {
            for (int i = 0; i < m_InitialPoolSize; i++)
            {
                CreateAudioSourceObject();
            }
        }

        AudioSource source = m_AudioSourcePool.Dequeue();
        m_ActiveSources.Add(source);
        return source;
    }

    void InitializeAudioPool()
    {
        m_AudioSourcePool = new();
        m_ActiveSources = new();

        for (int i = 0; i < m_InitialPoolSize; i++)
        {
            CreateAudioSourceObject();
        }
    }

    void CreateAudioSourceObject()
    {
        GameObject audioObject = new("PooledAudioSource");
        audioObject.transform.SetParent(transform);
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        m_AudioSourcePool.Enqueue(audioSource);
    }    
}

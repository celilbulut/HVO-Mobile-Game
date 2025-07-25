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
    public float Volume = 1f;                // Sesin şiddeti
    public float Pitch = 1f;                 // Sesin frekansı
    public bool Loop = false;                // Ses tekrar etmeli mi?
    public float SpatialBlend = 1f;          // 0 = 2D ses, 1 = 3D ses
    public float MinDistance = 5f;           // Kamera-ses kaynağı minimum mesafe
    public float MaxDistance = 50f;          // Kamera-ses kaynağı maksimum mesafe
    public AudioPriority Priority = AudioPriority.Medium;  // Ses önceliği
}

public class AudioManager : SingletonManager<AudioManager>
{
    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}

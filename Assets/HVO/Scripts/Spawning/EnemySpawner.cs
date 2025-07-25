using UnityEngine;

[System.Serializable]
public struct EnemyConfig
{
    public EnemyUnit EnemyPrefab;     // Instantiate edilecek düşman prefab'ı
    public float Propability;         // Bu düşmanın doğma olasılığı (örneğin 0.7 = %70)
}


[System.Serializable]
public struct SpawnWave
{
    public EnemyConfig[] EnemyConfigs; // Bu dalgada doğabilecek düşman türleri
    public float Frequency;            // Kaç saniyede bir düşman doğacak
    public float Duration;             // Bu dalga kaç saniye sürecek
}


public enum SpawnState
{
    Idle,       // Başlangıçta sistem hazır ama başlamamış
    Spawning,   // Düşmanlar aktif olarak doğuyor
    Waiting,    // İki dalga arasında bekleniyor
    Finished    // Tüm dalgalar tamamlandı
}


public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private SpawnWave[] m_SpawnWaves;
    [SerializeField] private Transform[] m_SpawnPoints;
    [SerializeField] private float m_DelayBetweenWaves;

    private int m_CurrentWaveIndex;
    private float m_DelayBetweenWavesTimer;
    private float m_WaveDurationTimer;
    private float m_SpawnFrequencyTimer;
    private SpawnState m_SpawnState = SpawnState.Idle;

    public SpawnState SpawnState => m_SpawnState;

    public void StartUp()
    {
        m_SpawnState = SpawnState.Waiting; // Sistemi başlat ama önce bekle
        m_CurrentWaveIndex = 0;
        InitializeTimers(); // Dalga süresi ve doğurma sıklığını başlat
    }

    void Update()
    {
        if (m_SpawnState == SpawnState.Finished)
        {
            Debug.Log("Spawning Finished");
            return;
        }
        else if (m_SpawnState == SpawnState.Waiting)
        {
            m_DelayBetweenWavesTimer -= Time.deltaTime;

            if (m_DelayBetweenWavesTimer <= 0)
            {
                m_SpawnState = SpawnState.Spawning;
                Debug.Log("Fight wave " + (m_CurrentWaveIndex + 1) + "!");
            }
        }
        else if (m_SpawnState == SpawnState.Spawning)
        {

            m_WaveDurationTimer -= Time.deltaTime;
            m_SpawnFrequencyTimer -= Time.deltaTime;

            if (m_WaveDurationTimer <= 0)
            {
                m_CurrentWaveIndex++;

                if (m_CurrentWaveIndex >= m_SpawnWaves.Length)
                {
                    m_SpawnState = SpawnState.Finished;
                }
                else
                {
                    m_SpawnState = SpawnState.Waiting;
                    InitializeTimers(); // Dalga süresi ve doğurma sıklığını başlat
                }
            }
            else if (m_SpawnFrequencyTimer <= 0)
            {
                Spawn(); // Yeni düşman doğurma
                m_SpawnFrequencyTimer = m_SpawnWaves[m_CurrentWaveIndex].Frequency;
            }
        }
    }

    void InitializeTimers()
    {
        m_DelayBetweenWavesTimer = m_DelayBetweenWaves;
        m_WaveDurationTimer = m_SpawnWaves[m_CurrentWaveIndex].Duration;
        m_SpawnFrequencyTimer = m_SpawnWaves[m_CurrentWaveIndex].Frequency;
    }

    void Spawn()
    {
        float totalPropability = 0;

        foreach (var enemyConfig in m_SpawnWaves[m_CurrentWaveIndex].EnemyConfigs)
        {
            totalPropability += enemyConfig.Propability;
        }

        float randomPropability = Random.Range(0, totalPropability);

        foreach (var enemyConfig in m_SpawnWaves[m_CurrentWaveIndex].EnemyConfigs)
        {
            if (randomPropability <= enemyConfig.Propability)
            {
                var spawnPoint = m_SpawnPoints[Random.Range(0, m_SpawnPoints.Length)];
                Instantiate(enemyConfig.EnemyPrefab, spawnPoint.position, Quaternion.identity);
                break;
            }

            randomPropability -= enemyConfig.Propability;
        }
    }
}

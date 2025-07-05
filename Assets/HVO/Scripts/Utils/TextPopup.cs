using TMPro;
using UnityEngine;

public class TextPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private float m_Duration = 1f;
    [SerializeField] private float m_TextAnimationSize = 5;
    [SerializeField] private AnimationCurve m_FrontSizeCurve;
    [SerializeField] private AnimationCurve m_X_OffsetCurve;
    [SerializeField] private AnimationCurve m_Y_OffsetCurve;
    [SerializeField] private AnimationCurve m_AlphaCurve;


    private float m_ElapsedTime;
    private int m_RandomXdirection = 1;

    void Start()
    {
        m_RandomXdirection = Random.Range(-1, 2);
    }

    public void SetText(string text, Color color)
    {
        m_Text.color = color;
        m_Text.text = text;
    }

    void Update()
    {
        m_ElapsedTime += Time.deltaTime;
        var normalizedTime = m_ElapsedTime / m_Duration;

        if (normalizedTime >= 1)
        {
            Destroy(gameObject);
            return;
        }

        // Opaklık azalması
        var alpha = m_AlphaCurve.Evaluate(normalizedTime);

        // Font boyutu artışı
        m_Text.fontSize += m_FrontSizeCurve.Evaluate(normalizedTime) / m_TextAnimationSize;
        m_Text.color = new Color(m_Text.color.r, m_Text.color.g, m_Text.color.b, alpha);

        // Pozisyon animasyonu (X ve Y offset)
        float xOffset = m_X_OffsetCurve.Evaluate(normalizedTime) * 1.1f * m_RandomXdirection; // saga-sola dogru gidiyor
        float yOffset = m_Y_OffsetCurve.Evaluate(normalizedTime) * 2; // yukari yada asagi dogru

        transform.position += new Vector3(xOffset, yOffset, 0) * Time.deltaTime;
    }
}

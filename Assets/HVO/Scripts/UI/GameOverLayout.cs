using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverLayout : MonoBehaviour
{
    [SerializeField] private Button m_RestartButton;
    [SerializeField] private Button m_QuitButton;
    [SerializeField] private TextMeshProUGUI m_GameOverText;
    [SerializeField] private Image m_GameOverBackgroundImage;

    public UnityAction OnRestartClicked = delegate { };
    public UnityAction OnQuitClicked = delegate { };

    void OnEnable()
    {
        m_RestartButton.onClick.AddListener(RestartGame);
        m_QuitButton.onClick.AddListener(QuitGame);
    }

    public void ShowGameOver(bool isVictory)
    {
        m_GameOverText.text = isVictory ? "Victory!" : "Defeat!";
        gameObject.SetActive(true);
        m_GameOverBackgroundImage.color = new Color(0, 0, 0, 0.3f);
    }

    void OnDisable()
    {
        m_RestartButton.onClick.RemoveListener(RestartGame);
        m_QuitButton.onClick.RemoveListener(QuitGame);
    }

    void RestartGame()
    {
        AudioManager.Get().PlayButtonClick();
        OnRestartClicked.Invoke();
    }

    void QuitGame()
    {
        AudioManager.Get().PlayButtonClick();
        OnQuitClicked.Invoke();
    }
}

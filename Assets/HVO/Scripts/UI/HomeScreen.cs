using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeScreen : MonoBehaviour
{
    [SerializeField] private AudioSettings m_MenuBackgroundAudioSettings;
    [SerializeField] private Button m_PlayButton;
    [SerializeField] private Button m_ExitButton;

    void OnEnable()
    {
        m_PlayButton.onClick.AddListener(OnPlayButtonClicked);
        m_ExitButton.onClick.AddListener(OnExitButtonClicked);
    }

    void OnDisable()
    {
        m_PlayButton.onClick.RemoveListener(OnPlayButtonClicked);
        m_ExitButton.onClick.RemoveListener(OnExitButtonClicked);
    }

    void Start()
    {
        AudioManager.Get().PlayMusic(m_MenuBackgroundAudioSettings);
    }

    void OnPlayButtonClicked()
    {
        AudioManager.Get().PlayButtonClick();
        SceneManager.LoadScene("GameScene"); // Scene adi dogru olmali.
    }

    void OnExitButtonClicked()
    {
        AudioManager.Get().PlayButtonClick();
        Application.Quit();
    }
}

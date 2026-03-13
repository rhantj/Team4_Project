using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtonSounds : MonoBehaviour
{
    [Header("버튼 사운드")]
    [SerializeField] private AudioClip m_BtnSound;

    private SoundManager m_SoundManager;
    public Button[] Buttons;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        BtnSound();

    }

    private void Start()
    {
        m_SoundManager = GameManager.Instance.GetService<SoundManager>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BtnSound();
    }

    private void BtnSound()
    {
        Buttons = Resources.FindObjectsOfTypeAll<Button>();

        foreach (Button button in Buttons)
        {
            button.onClick.AddListener(PlayButtonSound);
        }
    }
    public void PlayButtonSound()
    {
        if (m_BtnSound != null)
        {
            m_SoundManager.PlaySound2D(m_BtnSound.name);
        }
    }
}
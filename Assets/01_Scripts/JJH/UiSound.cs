using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiSound : MonoBehaviour
{

    //버튼 클릭 소리 만들려면?
    //버튼 소리를 지정하고 
    public static UiSound Instance { get; private set; }

    [Header("버튼 사운드")]
    private AudioSource m_AudioSource;
    public AudioClip m_BtnSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            m_AudioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        BtnSound();

    }
   
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //씬이 로드될 때마다 버튼 소리를 초기화하거나 필요한 설정을 할 수 있습니다.
        //예: m_BtnSound = Resources.Load<AudioClip>("ButtonSound");
        BtnSound();
    }
    private void BtnSound()
    {
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();

        foreach (Button button in allButtons)
        {
            button.onClick.AddListener(PlayButtonSound);
        }
    }
    public void PlayButtonSound()
    {
        if (m_BtnSound != null && m_AudioSource != null)
        {
            m_AudioSource.PlayOneShot(m_BtnSound);
        }
    }

}



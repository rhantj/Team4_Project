using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] private Image[] m_TutoImages;

    public Image[] TutoImages => m_TutoImages;

    private void Awake()
    {
        foreach (var image in m_TutoImages)
            image.gameObject.SetActive(false);
    }

    public Image GetImage(int idx)
    {
        var stageManager = GameManager.Instance.GetComponent<StageManager>();

        if (stageManager.StageIdx > 0) return null;
        return m_TutoImages[idx];
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour
{
    [SerializeField] private int m_TutorialIndex;
    [SerializeField] private float m_Duration = 2f;

    private bool m_Once = false;
    private TutorialPanel m_TP;

    private void Start()
    {
        m_TP = GameManager.Instance.GetComponent<TutorialPanel>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !m_Once)
        {
            m_Once = true;
            StartCoroutine(ShowTutorial());
        }
    }

    IEnumerator ShowTutorial()
    {
        var img = m_TP.GetImage(m_TutorialIndex);
        m_Once = true;
        if (!img) yield break;

        img.gameObject.SetActive(true);
        yield return new WaitForSeconds(m_Duration);
        img.gameObject.SetActive(false);
    }
}

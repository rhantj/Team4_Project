using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [Header("튜토리얼 텍스트 및 장소")]
    [SerializeField] private Image m_TutorialImage;
    [SerializeField] private float m_Duration = 2f;

    private bool m_Once = false;



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
        m_Once = true;
        m_TutorialImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(m_Duration);
        m_TutorialImage.gameObject.SetActive(false);
    }



}

using UnityEngine;

public class PlayerReference : MonoBehaviour
{
    [SerializeField] private SOPlayerReference m_Playerref;

    private void Awake()
    {
        m_Playerref.player = this.transform;
    }
}
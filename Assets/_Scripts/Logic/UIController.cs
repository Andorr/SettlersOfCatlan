using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Player Elements")]
    public GameObject actionPanel;

    public void EnableActionPanel(bool enable)
    {
        actionPanel.SetActive(enable);
    }
}

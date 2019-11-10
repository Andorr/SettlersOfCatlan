using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Player Elements")]
    public GameObject actionPanel;

    public void EnableActionPanel(bool enable, UnityAction roadAction = null, UnityAction houseAction = null)
    {
        actionPanel.SetActive(enable);
        var btns = actionPanel.GetComponentsInChildren<Button>();

        if(roadAction != null)
        {
            btns[0].onClick.AddListener(roadAction);
        }

        if(houseAction != null)
        {
            btns[1].onClick.AddListener(houseAction);
        }
    }
}

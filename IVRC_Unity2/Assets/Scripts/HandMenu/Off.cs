using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Off : MonoBehaviour
{
    private TMP_Text textPanel;

    void Start()
    {
        GameObject panel = GameObject.Find("/Canvas/Panel/ModeText");
        if (panel != null)
        {
            textPanel = panel.GetComponent<TMP_Text>();
        }
        else
        {
            Debug.LogError("ModeText GameObject not found");
        }
    }
    public void OnPress()
    {
        if (textPanel != null)
        {
            textPanel.text = "OFF MODE";
        }
    }
}

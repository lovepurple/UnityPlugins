using GOGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientMain : MonoBehaviour
{
    private Text m_receiveMessage = null;
    private Button m_btnSend = null;

    private void Start()
    {
        m_receiveMessage = transform.Find("Text").GetComponent<Text>();
        m_btnSend = transform.Find("Button").GetComponent<Button>();
        m_btnSend.AddClickCallback(OnBtnSendClick);
        
    }

    private void OnBtnSendClick(GameObject btn)
    {
        Debug.Log("nidaye");
    }
}

using DG.Tweening;
using EngineCore;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientMain : MonoBehaviour
{
    private Text m_panelTitle = null;

    private const int Panel_WIDTH = 1080;

    private Image m_btnESCSetting = null;
    private Image m_btnOperate = null;
    private Image m_btnBluetoothSetting = null;

    private RectTransform m_functionPanelRootTransform = null;

    private UIPanelLogicBase[] m_panelGroup = new UIPanelLogicBase[3];

    private Button m_btnCurrentStatus = null;
    private Button m_btnMotorInit = null;
    private Button m_btnSpeedup = null;
    private Button m_btnSpeedDown = null;
    private Button m_btnStop = null;
    private Button m_btnMaxSpeed = null;
    private Button m_btnPowerOff = null;
    private Button m_btnGetCurrentSpeed = null;
    private Button m_btnSetLowSpeed = null;
    private Button m_btnSetHighSpeed = null;

    private SkateMessageHandler m_skateMessageHandler = null;

    private int m_currentPanelIndex = -1;

    private void Start()
    {
        this.m_btnESCSetting = transform.GetComponent<Image>("Menu/MenuItem/Toggle/Background");
        this.m_btnESCSetting.AddClickCallback(obj =>
        {
            SetToPanel(0);
        });

        this.m_btnOperate = transform.GetComponent<Image>("Menu/MenuItem/Toggle (1)/Background");
        this.m_btnOperate.AddClickCallback(obj =>
        {
            SetToPanel(1);
        });

        this.m_btnBluetoothSetting = transform.GetComponent<Image>("Menu/MenuItem/Toggle (2)/Background");
        this.m_btnBluetoothSetting.AddClickCallback(obj =>
        {
            SetToPanel(2);
        });

        m_panelTitle = transform.GetComponent<Text>("Text_PageTitle");

        m_panelGroup[0] = new SkateSettingPanel(transform.Find("PanelScrollRect/Viewport/Content/MotorSetting").GetComponent<RectTransform>());
        m_panelGroup[0].OnCreate();

        m_panelGroup[1] = new SkateOperatorPanel(transform.Find("PanelScrollRect/Viewport/Content/MotorController").GetComponent<RectTransform>());
        m_panelGroup[1].OnCreate();

        m_panelGroup[2] = new BluetoothPanel(transform.Find("PanelScrollRect/Viewport/Content/Connection").GetComponent<RectTransform>());
        m_panelGroup[2].OnCreate();

        m_btnCurrentStatus = transform.Find("GameObject/Button (2)").GetComponent<Button>();
        m_btnCurrentStatus.AddClickCallback(OnBtnBluetoothStatusClick);

        m_btnSpeedup = transform.Find("GameObject/Button (6)").GetComponent<Button>();
        m_btnSpeedup.AddClickCallback(OnBtnSpeedupClick);

        m_btnSpeedDown = transform.Find("GameObject/Button (7)").GetComponent<Button>();
        m_btnSpeedDown.AddClickCallback(OnBtnSpeedDownClick);

        m_btnStop = transform.Find("GameObject/Button (8)").GetComponent<Button>();
        m_btnStop.AddClickCallback(OnBtnStopClick);

        m_btnMaxSpeed = transform.Find("GameObject/Button (9)").GetComponent<Button>();
        m_btnMaxSpeed.AddClickCallback(OnBtnMaxSpeedClick);

        m_btnPowerOff = transform.Find("GameObject/Button (10)").GetComponent<Button>();
        m_btnPowerOff.AddClickCallback(OnBtnPowerOff);

        m_btnGetCurrentSpeed = transform.Find("GameObject/Button (11)").GetComponent<Button>();
        m_btnGetCurrentSpeed.AddClickCallback(OnBtnGetCurrentSpeedClick);

        m_functionPanelRootTransform = transform.Find("PanelScrollRect/Viewport/Content").GetComponent<RectTransform>();

        BluetoothProxy.Intance.InitializeBluetoothProxy();
        m_skateMessageHandler = new SkateMessageHandler(BluetoothProxy.Intance.BluetoothDevice);

        BluetoothEvents.OnErrorEvent += OnLog;
        BluetoothEvents.OnLogEvent += OnLog;


        SetToPanel(1);
    }

    private void OnLog(string logContent)
    {
        Debug.Log(logContent);
    }

    private void SetToPanel(int panelIndex)
    {
        float moveDistanceX = Panel_WIDTH;
        float moveDuration = 0.01f;
        //initial
        if (m_currentPanelIndex == -1)
            moveDistanceX *= -panelIndex;
        else
        {
            moveDistanceX *= m_currentPanelIndex - panelIndex;
            moveDuration = Math.Abs(m_currentPanelIndex - panelIndex) * 0.3f;
        }

        float currentX = this.m_functionPanelRootTransform.anchoredPosition.x;

        this.m_functionPanelRootTransform.DOAnchorPosX(moveDistanceX + currentX, moveDuration).OnComplete(() =>
         {
             if (this.m_currentPanelIndex != -1)
                 this.m_panelGroup[this.m_currentPanelIndex].OnExit();

             this.m_currentPanelIndex = panelIndex;
             this.m_panelGroup[this.m_currentPanelIndex].OnEnter();
             this.m_panelTitle.text = this.m_panelGroup[this.m_currentPanelIndex].PanelName;
         });
    }

    private void OnBtnBluetoothStatusClick(GameObject btn)
    {
        //this.m_receiveMessage.text = BluetoothProxy.Intance.BluetoothDevice.GetBluetoothDeviceStatus().ToString();
    }


    private void OnBtnSpeedupClick(GameObject btn)
    {
    }

    private void OnBtnSpeedDownClick(GameObject btn)
    {

    }

    private void OnBtnStopClick(GameObject btn)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_CORRECT_MIN_POWER);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }

    private void OnBtnMaxSpeedClick(GameObject btn)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_CORRECT_MAX_POWER);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }

    private void OnBtnPowerOff(GameObject btn)
    {

    }

    private void OnBtnGetCurrentSpeedClick(GameObject btn)
    {
        

    }

    private void Update()
    {
        BluetoothProxy.Intance.Tick();
        TimeModule.Instance.Update();
    }
}

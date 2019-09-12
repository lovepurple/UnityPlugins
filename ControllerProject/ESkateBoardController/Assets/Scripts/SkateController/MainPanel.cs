using DG.Tweening;
using EngineCore;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : UIPanelLogicBase
{
    private UIPanelLogicBase[] m_panelGroup = new UIPanelLogicBase[3];
    private const int Panel_WIDTH = 1080;

    private Image m_btnESCSetting = null;
    private Image m_btnOperate = null;
    private Image m_btnBluetoothSetting = null;

    private RectTransform m_functionPanelRootTransform = null;
    private Text m_panelTitle = null;

    private int m_currentPanelIndex = -1;

    private UIPanelLogicBase m_signalPanel;

    public MainPanel(RectTransform uiPanelRootTransfrom) : base(uiPanelRootTransfrom)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();

        m_signalPanel = new SignalPanel(m_panelRootObject.Find("SignalArea") as RectTransform);
        m_signalPanel.OnCreate();

        m_panelGroup[0] = new SkateSettingPanel(m_panelRootObject.Find("PanelScrollRect/Viewport/Content/MotorSetting").GetComponent<RectTransform>());
        m_panelGroup[0].OnCreate();

        m_panelGroup[1] = new SkateOperatorPanel(m_panelRootObject.Find("PanelScrollRect/Viewport/Content/MotorController").GetComponent<RectTransform>());
        m_panelGroup[1].OnCreate();

        m_panelGroup[2] = new BluetoothPanel(m_panelRootObject.Find("PanelScrollRect/Viewport/Content/Connection").GetComponent<RectTransform>());
        m_panelGroup[2].OnCreate();

        this.m_btnESCSetting = m_panelRootObject.GetComponent<Image>("Menu/MenuItem/Toggle/Background");
        this.m_btnOperate = m_panelRootObject.GetComponent<Image>("Menu/MenuItem/Toggle (1)/Background");
        this.m_btnBluetoothSetting = m_panelRootObject.GetComponent<Image>("Menu/MenuItem/Toggle (2)/Background");
        m_panelTitle = m_panelRootObject.GetComponent<Text>("Text_PageTitle");
        m_functionPanelRootTransform = m_panelRootObject.Find("PanelScrollRect/Viewport/Content").GetComponent<RectTransform>();

    }

    public override void OnEnter(params object[] onEnterParams)
    {
        base.OnEnter(onEnterParams);

        this.m_btnESCSetting.AddClickCallback(obj =>
        {
            SetToPanel(0);
        });

        this.m_btnOperate.AddClickCallback(obj =>
        {
            SetToPanel(1);
        });

        this.m_btnBluetoothSetting.AddClickCallback(obj =>
        {
            SetToPanel(2);
        });



        BluetoothEvents.OnErrorEvent += OnLog;
        BluetoothEvents.OnLogEvent += OnLog;

        SetToPanel(1);

        m_signalPanel.OnEnter();
    }

    private void SetToPanel(int panelIndex)
    {
        float moveDistanceX = Panel_WIDTH;
        float moveDuration = 0.01f;

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

    private void OnLog(string logContent)
    {
        Debug.Log(logContent);
    }
}

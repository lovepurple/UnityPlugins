using EngineCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkateOperatorPanel : UIPanelLogicBase
{
    private MaskableGraphic m_btnStartup;
    private MaskableGraphic m_btnStop;

    private MaskableGraphic m_btnGear1;
    private MaskableGraphic m_btnGear2;
    private MaskableGraphic m_btnGear3;
    private MaskableGraphic m_btnGear4;

    private Text m_txtMotorPower = null;

    public SkateOperatorPanel(RectTransform uiPanelRootTransfrom) : base(uiPanelRootTransfrom)
    {
        PanelName = "操作";
    }

    public override void OnCreate()
    {
        m_btnStartup = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button");
        m_btnStop = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (1)");

        m_btnGear1 = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (4)");
        m_btnGear2 = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (5)");
        m_btnGear3 = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (6)");
        m_btnGear4 = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (7)");


        m_txtMotorPower = m_panelRootObject.GetComponent<Text>("InfoPanel/Template/Text (1)");
    }

    public override void OnEnter(params object[] onEnterParams)
    {
        base.OnEnter(onEnterParams);

        m_btnStartup.AddClickCallback(OnBtnStartUpClick);
        m_btnStop.AddClickCallback(OnBtnStopClick);

        m_btnGear1.AddClickCallback(OnSetGear1Click);
        m_btnGear2.AddClickCallback(OnSetGear2Click);
        m_btnGear3.AddClickCallback(OnSetGear3Click);
        m_btnGear4.AddClickCallback(OnSetGear4Click);

        MessageHandler.RegisterMessageHandler((int)MessageDefine.E_D2C_MOTOR_SPEED, OnGetMotorGearResponse);
        BluetoothEvents.OnVolumeKeyEvent += OnVolumeKeyEventHandler;
    }

    private void OnSetGear1Click(GameObject obj)
    {
        SpeedController.Instance.SetGear(1);
    }
    private void OnSetGear2Click(GameObject obj)
    {
        SpeedController.Instance.SetGear(2);
    }
    private void OnSetGear3Click(GameObject obj)
    {
        SpeedController.Instance.SetGear(3);
    }
    private void OnSetGear4Click(GameObject obj)
    {
        SpeedController.Instance.SetGear(4);
    }

    private void OnBtnStartUpClick(GameObject obj)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_NORMAL_START);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }

    private void OnBtnStopClick(GameObject obj)
    {
        //List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_CORRECT_MIN_POWER);

        //BluetoothProxy.Intance.SendData(messageBuffer);
        SpeedController.Instance.SetGear(0);
    }

    private void OnVolumeKeyEventHandler(int keyFlag)
    {
        SpeedController.Instance.SetDeltaGear(keyFlag);
    }

    /// <summary>
    /// 手指抬起，缓慢刹车
    /// </summary>
    private void OnJoyStickMoveEnd()
    {
        if (SpeedController.Instance.SkateSpeed > 0)
            SpeedController.Instance.BrakeSoftly();
    }




    private void OnGetMotorGearResponse(object data)
    {
        this.m_txtMotorPower.text = SpeedController.Instance.Gear.ToString();
    }


    public override void OnExit()
    {
        BluetoothEvents.OnVolumeKeyEvent -= OnVolumeKeyEventHandler;
        MessageHandler.UnRegisterMessageHandler((int)MessageDefine.E_D2C_MOTOR_SPEED, OnGetMotorGearResponse);

        m_btnStartup.RemoveClickCallback(OnBtnStartUpClick);
        m_btnStop.RemoveClickCallback(OnBtnStopClick);

        m_btnGear1.RemoveClickCallback(OnSetGear1Click);
        m_btnGear2.RemoveClickCallback(OnSetGear2Click);
        m_btnGear3.RemoveClickCallback(OnSetGear3Click);
        m_btnGear4.RemoveClickCallback(OnSetGear4Click);
    }
}

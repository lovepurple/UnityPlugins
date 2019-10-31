using EngineCore;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SkateOperatorPanel : UIPanelLogicBase
{
    private const int GearCount = 5; //档位数量

    public ETCJoystick m_ControllerJoyStick;

    private MaskableGraphic m_btnStartup;
    private MaskableGraphic m_btnStop;

    private MaskableGraphic m_btnGear1;
    private MaskableGraphic m_btnGear2;
    private MaskableGraphic m_btnGear3;
    private MaskableGraphic m_btnGear4;
    private MaskableGraphic m_btnGear5;
    private MaskableGraphic m_btnGear6;
    private MaskableGraphic m_btnGear7;
    private MaskableGraphic m_btnGear8;

    private Text m_txtMotorPower = null;

    private bool m_isStarpup = true;


    public SkateOperatorPanel(RectTransform uiPanelRootTransfrom) : base(uiPanelRootTransfrom)
    {
        PanelName = "操作";
    }

    public override void OnCreate()
    {
        m_ControllerJoyStick = m_panelRootObject.GetComponent<ETCJoystick>("InputArea/BoosterStick");

        m_btnStartup = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button");
        m_btnStop = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (1)");

        m_btnGear1 = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (4)");
        m_btnGear2 = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (5)");
        m_btnGear3 = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (6)");
        m_btnGear4 = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (7)");
        m_btnGear5 = m_panelRootObject.GetComponent<MaskableGraphic>("GearGroup/Button (7)");
        m_btnGear6 = m_panelRootObject.GetComponent<MaskableGraphic>("GearGroup/Button (8)");
        m_btnGear7 = m_panelRootObject.GetComponent<MaskableGraphic>("GearGroup/Button (9)");
        m_btnGear8 = m_panelRootObject.GetComponent<MaskableGraphic>("GearGroup/Button (10)");


        m_txtMotorPower = m_panelRootObject.GetComponent<Text>("InfoPanel/Template/Text (1)");
    }

    public override void OnEnter(params object[] onEnterParams)
    {
        base.OnEnter(onEnterParams);

        if (m_isStarpup)
        {
            m_ControllerJoyStick.enabled = true;
            m_ControllerJoyStick.onMove.AddListener(OnJoyStickMove);
            m_ControllerJoyStick.onMoveSpeed.AddListener(OnJoyStickMoveSpeed);
            m_ControllerJoyStick.onMoveStart.AddListener(OnJoyStickMoveStart);
            m_ControllerJoyStick.onMoveEnd.AddListener(OnJoyStickMoveEnd);
        }
        m_btnStartup.AddClickCallback(OnBtnStartUpClick);
        m_btnStop.AddClickCallback(OnBtnStopClick);


        MessageHandler.RegisterMessageHandler((int)MessageDefine.E_D2C_MOTOR_SPEED, OnGetMotorGearResponse);

        if (BluetoothProxy.Intance.BluetoothState == BluetoothStatus.CONNECTED)
        {
            TimeModule.Instance.SetTimeInterval(RequestCurrentGear, 1);
        }

    }

    private void OnBtnStartUpClick(GameObject obj)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_NORMAL_START);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }

    private void OnBtnStopClick(GameObject obj)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_CORRECT_MIN_POWER);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }

    private void OnJoyStickMove(Vector2 delta)
    {
        SpeedController.Instance.SetSpeedByNormalizedPower(delta.y);
    }

    private void OnJoyStickMoveSpeed(Vector2 delta)
    {
    }

    private void OnJoyStickMoveEnd()
    {
    }

    private void OnJoyStickMoveStart()
    {
    }

    private void OnGetMotorGearResponse(object data)
    {
        this.m_txtMotorPower.text = SpeedController.Instance.Gear.ToString();
    }

    private void RequestCurrentGear()
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_GET_SPEED);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }

    public override void OnExit()
    {
        base.OnExit();

        m_ControllerJoyStick.enabled = false;

        m_btnStartup.RemoveClickCallback(OnBtnStartUpClick);
        m_btnStop.RemoveClickCallback(OnBtnStopClick);

        TimeModule.Instance.RemoveTimeaction(RequestCurrentGear);
    }
}

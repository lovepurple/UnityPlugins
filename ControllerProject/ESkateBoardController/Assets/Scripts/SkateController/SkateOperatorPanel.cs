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


    }

    private void OnBtnStartUpClick(GameObject obj)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_NORMAL_START);

        BluetoothProxy.Intance.BluetoothDevice.SendData(messageBuffer);
    }

    private void OnBtnStopClick(GameObject obj)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_CORRECT_MIN_POWER);

        BluetoothProxy.Intance.BluetoothDevice.SendData(messageBuffer);
    }

    private void OnJoyStickMove(Vector2 delta)
    {
        //todo:之后考虑差值
        //char buffer[]
        if (delta.y >= 0)
        {
            //0.1为一档
            //四舍五入
            float gear = Mathf.RoundToInt(delta.y * GearCount) * 1.0f / GearCount;        //Shader中常用方法

            SetSpeed(gear);
        }
    }

    private void SetSpeed(float percentage01)
    {
        int speedThoudsand = (int)MathUtil.Remap(percentage01, 0, 1, 0, 999);

        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_DRIVE);

        messageBuffer.AddRange(Encoding.ASCII.GetBytes(speedThoudsand.ToString()));

        BluetoothProxy.Intance.BluetoothDevice.SendData(messageBuffer);
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

    public override void OnExit()
    {
        base.OnExit();

        m_ControllerJoyStick.enabled = false;

        m_btnStartup.RemoveClickCallback(OnBtnStartUpClick);
        m_btnStop.RemoveClickCallback(OnBtnStopClick);
    }
}

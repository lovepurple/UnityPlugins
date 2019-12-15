using EngineCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkateSettingPanel : UIPanelLogicBase
{
    private MaskableGraphic m_btnEnterProgramming = null;
    private MaskableGraphic m_btnInitialize = null;
    private MaskableGraphic m_btnUp = null;
    private MaskableGraphic m_btnDown = null;
    private MaskableGraphic m_btnPowerOff = null;

    private MaskableGraphic m_btnRefreshBattery = null;

    public SkateSettingPanel(RectTransform uiPanelRootTransfrom) : base(uiPanelRootTransfrom)
    {
        PanelName = "œµÕ≥…Ë÷√";
    }

    public override void OnCreate()
    {
        m_btnEnterProgramming = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (2)");
        m_btnInitialize = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button");
        m_btnPowerOff = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (1)");
        m_btnDown = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (3)");
        m_btnUp = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (4)");
        m_btnRefreshBattery = m_panelRootObject.GetComponent<MaskableGraphic>("ButtonGroup/Button (6)");
    }

    public override void OnEnter(params object[] onEnterParams)
    {
        base.OnEnter(onEnterParams);

        m_btnEnterProgramming.AddClickCallback(OnBtnEnterProgrammingClick);
        m_btnDown.AddClickCallback(OnBtnDownClick);
        m_btnUp.AddClickCallback(OnBtnUpClick);
        m_btnPowerOff.AddClickCallback(OnBtnPowerOffClick);
        m_btnRefreshBattery.AddClickCallback(OnBtnRefreshBatteryClick);
    }


    private void OnBtnMotorInitClick(GameObject btn)
    {
        //List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.);

        //BluetoothProxy.Intance.BluetoothDevice.SendData(messageBuffer);
    }


    private void OnBtnEnterProgrammingClick(GameObject btn)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_INITIALIZE);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }

    private void OnBtnDownClick(GameObject btn)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_CORRECT_MIN_POWER);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }

    private void OnBtnRefreshBatteryClick(GameObject btn)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_REMAINING_POWER);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }

    private void OnBtnUpClick(GameObject btn)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_CORRECT_MAX_POWER);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }

    private void OnBtnPowerOffClick(GameObject btn)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_POWEROFF);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }

    public override void OnExit()
    {
        base.OnExit();

        m_btnPowerOff.RemoveClickCallback(OnBtnPowerOffClick);
        m_btnEnterProgramming.RemoveClickCallback(OnBtnEnterProgrammingClick);
        m_btnDown.RemoveClickCallback(OnBtnDownClick);
        m_btnUp.RemoveClickCallback(OnBtnUpClick);
        m_btnRefreshBattery.RemoveClickCallback(OnBtnRefreshBatteryClick);
    }

}

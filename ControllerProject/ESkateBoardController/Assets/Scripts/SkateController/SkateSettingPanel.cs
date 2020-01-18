using EngineCore;
using EngineCore.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    private SliderSettingComponent m_maxAccelerationSettingComponent;
    private SliderSettingComponent m_gearSettingComponent;
    private SliderSettingComponent m_brakeTimeSettingComponent;

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

        m_maxAccelerationSettingComponent = new SliderSettingComponent(m_panelRootObject.Find("SkateSettingGroup/setting_accelerator_factor").gameObject);
        m_gearSettingComponent = new SliderSettingComponent(m_panelRootObject.Find("SkateSettingGroup/setting_accelerator_gear").gameObject);
        m_brakeTimeSettingComponent = new SliderSettingComponent(m_panelRootObject.Find("SkateSettingGroup/setting_brake_time").gameObject);

        m_maxAccelerationSettingComponent.SliderComponent.maxValue = GlobalDefine.MAX_ACCELERATOR;
        m_gearSettingComponent.SliderComponent.maxValue = GlobalDefine.MAX_GEAR_COUNT;
        m_brakeTimeSettingComponent.SliderComponent.maxValue = GlobalDefine.MAX_BRAKE_TIME;

        m_brakeTimeSettingComponent.SetActive(true);
        m_maxAccelerationSettingComponent.SetActive(true);
        m_gearSettingComponent.SetActive(true);

        m_brakeTimeSettingComponent.SetValue(LocalStorage.GetFloat(LocalSetting.E_SKATE_MAX_BRAKE_TIME));
        m_maxAccelerationSettingComponent.SetValue(LocalStorage.GetFloat(LocalSetting.E_SKATE_MAX_ACCELERATOR));
        m_gearSettingComponent.SetValue(LocalStorage.GetFloat(LocalSetting.E_SKATE_GEAR_COUNT));
    }

    public override void OnEnter(params object[] onEnterParams)
    {
        base.OnEnter(onEnterParams);

        m_btnEnterProgramming.AddClickCallback(OnBtnEnterProgrammingClick);
        m_btnDown.AddClickCallback(OnBtnDownClick);
        m_btnUp.AddClickCallback(OnBtnUpClick);
        m_btnPowerOff.AddClickCallback(OnBtnPowerOffClick);
        m_btnRefreshBattery.AddClickCallback(OnBtnRefreshBatteryClick);

     

        m_brakeTimeSettingComponent.AddOnSliderDragEndCallback(OnBrakeTimeSettingFinishCallback);
        m_maxAccelerationSettingComponent.AddOnSliderDragEndCallback(OnAcceleratorSettingFinishCallback);
        m_gearSettingComponent.AddOnSliderDragEndCallback(OnGearCountSettingFinishCallback);
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

    private void OnBrakeTimeSettingFinishCallback(float brakeTime)
    {
        int brakeTimeMill = (int)(brakeTime * 1000);

        SpeedController.Instance.SetSkateBrakeTime(brakeTimeMill);
    }

    private void OnGearCountSettingFinishCallback(float gearCount)
    {
        int iGearCount = (int)gearCount;

        SpeedController.Instance.SetSkateGearCount(iGearCount);
    }

    private void OnAcceleratorSettingFinishCallback(float maxAccelerator)
    {
        SpeedController.Instance.SetSkateAccelerator(maxAccelerator);
    }


    public override void OnExit()
    {
        base.OnExit();

        m_btnPowerOff.RemoveClickCallback(OnBtnPowerOffClick);
        m_btnEnterProgramming.RemoveClickCallback(OnBtnEnterProgrammingClick);
        m_btnDown.RemoveClickCallback(OnBtnDownClick);
        m_btnUp.RemoveClickCallback(OnBtnUpClick);
        m_btnRefreshBattery.RemoveClickCallback(OnBtnRefreshBatteryClick);

        m_brakeTimeSettingComponent.RemoveOnSliderDragEndCallback(OnBrakeTimeSettingFinishCallback);
        m_maxAccelerationSettingComponent.RemoveOnSliderDragEndCallback(OnAcceleratorSettingFinishCallback);
        m_gearSettingComponent.RemoveOnSliderDragEndCallback(OnGearCountSettingFinishCallback);
    }

}

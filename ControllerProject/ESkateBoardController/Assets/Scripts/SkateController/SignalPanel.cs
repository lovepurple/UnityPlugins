using EngineCore;
using EngineCore.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignalPanel : UIPanelLogicBase
{
    private Image m_imgOnline;
    private Image m_imgOffline;

    private Image[] m_imgBatteryList = new Image[6];
    private Text m_txtBattery;
    private Toggle m_toggleDeviceType = null;

    private Text m_txtMotorRps = null;

    public SignalPanel(RectTransform uiPanelRootTransfrom) : base(uiPanelRootTransfrom)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();

        this.m_imgOffline = m_panelRootObject.GetComponent<Image>("img_offline");
        this.m_imgOnline = m_panelRootObject.GetComponent<Image>("img_online");

        this.m_txtBattery = m_panelRootObject.GetComponent<Text>($"battery_panel/txt_battery");
        this.m_txtMotorRps = m_panelRootObject.GetComponent<Text>("txt_motor_rps");

        for (int i = 0; i <= 5; ++i)
            m_imgBatteryList[i] = m_panelRootObject.GetComponent<Image>($"battery_panel/img_battery_{i}");

        this.m_toggleDeviceType = m_panelRootObject.GetComponent<Toggle>("Toggle");
    }

    public override void OnEnter(params object[] onEnterParams)
    {
        BluetoothEvents.OnBluetoothDeviceStateChangedEvent += OnBluetoothDeviceStateChanged;
        OnBluetoothDeviceStateChanged((int)BluetoothProxy.Intance.BluetoothState);

        MessageHandler.RegisterMessageHandler((int)MessageDefine.E_D2C_REMAINING_POWER, OnReceiveSkaterBatteryPowerHandler);
        MessageHandler.RegisterMessageHandler((int)MessageDefine.E_D2C_MOTOR_RPS, OnReceiveMotorRPSHandler);

        this.m_toggleDeviceType.isOn = BluetoothProxy.Intance.BluetoothDeviceType == BluetoothProxy.EBluetoothDeviceType.BLUETOOTH_LOW_ENERGY;
        this.m_toggleDeviceType.onValueChanged.AddListener(OnBluetoothDeviceTypeChanged);
    }

    private void OnBluetoothDeviceTypeChanged(bool val)
    {
        GlobalEvents.OnBluetoothDeviceChanged.SafeInvoke(val ? BluetoothProxy.EBluetoothDeviceType.BLUETOOTH_LOW_ENERGY : BluetoothProxy.EBluetoothDeviceType.BLUETOOTH_CLASSIC);
    }

    private void OnBluetoothDeviceStateChanged(int status)
    {
        BluetoothStatus bluetoothStatus = (BluetoothStatus)status;

        this.m_imgOffline.gameObject.SetActive(bluetoothStatus != BluetoothStatus.CONNECTED);
        this.m_imgOnline.gameObject.SetActive(bluetoothStatus == BluetoothStatus.CONNECTED);

        //1分钟一次请求剩余电量
        if (bluetoothStatus == BluetoothStatus.CONNECTED)
        {
            TimeModule.Instance.ExecuteOnNextFrame(() => GetSkaterBatteryPower());
            TimeModule.Instance.SetTimeInterval(GetSkaterBatteryPower, 60f);
        }
        else
            TimeModule.Instance.RemoveTimeaction(GetSkaterBatteryPower);
    }

    private void GetSkaterBatteryPower()
    {
        List<byte> msgBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_REMAINING_POWER);
        BluetoothProxy.Intance.SendData(msgBuffer);
    }

    public override void OnExit()
    {
        BluetoothEvents.OnBluetoothDeviceStateChangedEvent -= OnBluetoothDeviceStateChanged;
        MessageHandler.UnRegisterMessageHandler((int)MessageDefine.E_D2C_REMAINING_POWER, OnReceiveSkaterBatteryPowerHandler);
        MessageHandler.UnRegisterMessageHandler((int)MessageDefine.E_D2C_MOTOR_RPS, OnReceiveMotorRPSHandler);
        TimeModule.Instance.RemoveTimeaction(GetSkaterBatteryPower);

        this.m_toggleDeviceType.onValueChanged.RemoveListener(OnBluetoothDeviceTypeChanged);
    }

    private void OnReceiveSkaterBatteryPowerHandler(object recvData)
    {
        char[] batteryPowerData = (char[])recvData;
        uint voltHandred = DigitUtility.GetUInt32(batteryPowerData);
        float volt = voltHandred * 0.01f;

        //太小则说明外部电源没接入
        int percentageRemainPower = SystemController.GetPercentageBatteryPower(volt);
        if (percentageRemainPower < GlobalDefine.MIN_BATTERY_VOLT)
            Debug.Log("主电源开关没打开");

        SetBatteryLevel(percentageRemainPower);
    }

    private void OnReceiveMotorRPSHandler(object recvData)
    {
        float currentSpeed = SpeedController.Instance.SkateSpeed;
        string colorFlag = string.Empty;
        if (currentSpeed <= 2)
            colorFlag = "#FFFFFFFF";
        else if (currentSpeed <= 5)
            colorFlag = "#30FF00FF";
        else
            colorFlag = "#FF2A00FF";

        this.m_txtMotorRps.text = $"<color={colorFlag}>{currentSpeed.ToString("0.00")} km/h</color>";
    }

    private void SetBatteryLevel(int remainPowerPercentage)
    {
        //5个级别
        int batteryLevel = remainPowerPercentage / 20;

        for (int i = 0; i <= 5; ++i)
            m_imgBatteryList[i].gameObject.SetActive(i == batteryLevel);

        this.m_txtBattery.text = $"{remainPowerPercentage}%";
    }
}
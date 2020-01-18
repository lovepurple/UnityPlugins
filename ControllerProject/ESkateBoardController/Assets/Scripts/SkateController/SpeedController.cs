using EngineCore;
using EngineCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpeedController : Singleton<SpeedController>
{
    public const float BRAKE_FORCE_SPEED = 5.0f;       //这个速度可以直接刹停

    private int m_currentGear = 0;
    private uint m_motorRoundPerSecond = 0;      //电机每秒转数

    //同步齿轮齿比
    private readonly float SYNC_GEAR_RATIO = 11.0f / 36.0f;

    //轮子一圈长度
    private readonly float WHEEL_METER_PER_ROUND = 0.2608f;

    public SpeedController()
    {

    }

    public void InitSpeedController()
    {
        if (GearCount == 0)
            GearCount = (int)LocalStorage.GetFloat(LocalSetting.E_SKATE_GEAR_COUNT);

        MessageHandler.RegisterMessageHandler((int)MessageDefine.E_D2C_MOTOR_SPEED, OnGetMotorGearResponse);
        MessageHandler.RegisterMessageHandler((int)MessageDefine.E_D2C_MOTOR_RPS, OnGetMotorRoundPerSecondHandler);
        BluetoothEvents.OnBluetoothDeviceStateChangedEvent += OnBluetoothConnectionStateChangedHandler;
    }


    /// <summary>
    /// 获取档位
    /// </summary>
    /// <param name="normailzdPower"></param>
    /// <returns></returns>
    /// <remarks>power<0 减档</remarks>
    public int GetGear(float normalizdPower)
    {
        if (normalizdPower < 0)
            return m_currentGear - 1 <= 0 ? 0 : m_currentGear - 1;
        else
        {
            normalizdPower = Mathf.Clamp01(normalizdPower);
            //四舍五入
            int gear = Mathf.RoundToInt(MathUtil.Remap(normalizdPower, 0, 1.0f, 0, GearCount));

            return gear;
        }
    }

    public void SetSpeedByNormalizedPower(float normalizedPower)
    {
        int powerToGear = GetGear(normalizedPower);
        SetGear(powerToGear);
    }

    public void SetGear(int gear)
    {
        if (gear <= 0)
            gear = 0;

        if (gear > GearCount)
            return;

        //不能跳档
        if (Mathf.Abs(m_currentGear - gear) > 1)
            return;

        SetSkateBoardSpeedByGear(gear);
    }

    /// <summary>
    /// 获取滑板速度
    /// </summary>
    /// <param name="motorRPS"></param>
    /// <returns></returns>
    public float GetSkateSpeedKilometerPerHour(uint motorRPS)
    {
        uint motorRPH = motorRPS * 3600;
        float wheelRoundPerHour = motorRPH * SYNC_GEAR_RATIO;
        float wheelMeterPerHour = wheelRoundPerHour * WHEEL_METER_PER_ROUND;
        return wheelMeterPerHour * 0.001f;
    }


    /// <summary>
    /// 设置滑板速度
    /// </summary>
    /// <param name="gear"></param>
    private void SetSkateBoardSpeedByGear(int gear)
    {
        int speedTemp = (int)MathUtil.Remap(gear, 0, GearCount, 0, 999);
        //int speedThoudsand = (int)(speedTemp * GEAR_POWER_BASIC);

        List<byte> speedBuffer = DigitUtility.GetFixedLengthBufferList(Encoding.ASCII.GetBytes(speedTemp.ToString()).ToList(), 3, (byte)'0');

        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_DRIVE);

        messageBuffer.AddRange(speedBuffer);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }

    private void OnBluetoothConnectionStateChangedHandler(int bluetoothState)
    {
        if ((BluetoothStatus)bluetoothState == BluetoothStatus.CONNECTED)
        {
            TimeModule.Instance.SetTimeInterval(RequstMotorRPS, 1);
            TimeModule.Instance.SetTimeout(() =>
            {
                SetSkateAccelerator(LocalStorage.GetFloat(LocalSetting.E_SKATE_MAX_ACCELERATOR));
                SetSkateBrakeTime(LocalStorage.GetInt(LocalSetting.E_SKATE_MAX_BRAKE_TIME));
                SetSkateGearCount(LocalStorage.GetInt(LocalSetting.E_SKATE_GEAR_COUNT));
            }, 1f);
        }
        else
            TimeModule.Instance.RemoveTimeaction(RequstMotorRPS);
    }

    private void RequstMotorRPS()
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_RPS);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }

    private void OnGetMotorGearResponse(object data)
    {
        char[] gearData = (char[])data;
        this.m_currentGear = GetGear(((DigitUtility.GetUInt32(gearData) + 1) * 0.001f));
    }

    public void SetSkateGearCount(int gearCount)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_SETTING_SKATE_GEAR_COUNT);

        messageBuffer.AddRange(Encoding.ASCII.GetBytes(gearCount.ToString()));

        BluetoothProxy.Intance.SendData(messageBuffer);

        LocalStorage.SaveSetting(LocalSetting.E_SKATE_GEAR_COUNT, gearCount.ToString());

        GearCount = gearCount;
    }

    public void SetSkateAccelerator(float accelerator01)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_SETTING_SKATE_MAX_ACCLERATOR);
        int iMaxAccelerator = (int)(accelerator01 * 100);

        List<byte> acceleratorBuffer = DigitUtility.GetFixedLengthBufferList(Encoding.ASCII.GetBytes(iMaxAccelerator.ToString()).ToList(), 2, (byte)'0');

        messageBuffer.AddRange(acceleratorBuffer);

        BluetoothProxy.Intance.SendData(messageBuffer);

        LocalStorage.SaveSetting(LocalSetting.E_SKATE_MAX_ACCELERATOR, accelerator01.ToString());
    }

    public void SetSkateBrakeTime(int brakeTimeMill)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_SETTING_SKATE_MAX_ACCLERATOR_BRAKE_TIME);

        List<byte> brakeTimeBuffer = DigitUtility.GetFixedLengthBufferList(Encoding.ASCII.GetBytes(brakeTimeMill.ToString()).ToList(), 4, (byte)'0');

        messageBuffer.AddRange(brakeTimeBuffer);

        BluetoothProxy.Intance.SendData(messageBuffer);

        LocalStorage.SaveSetting(LocalSetting.E_SKATE_MAX_BRAKE_TIME, brakeTimeMill.ToString());
    }

    private void OnGetMotorRoundPerSecondHandler(object recvData)
    {
        char[] motorRpsData = (char[])recvData;
        uint motorRps = DigitUtility.GetUInt32(motorRpsData);
        this.m_motorRoundPerSecond = motorRps;
    }

    /// <summary>
    /// 直接刹停
    /// </summary>
    public void BrakeImmediately()
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_BRAKE_FORCE);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }

    /// <summary>
    /// 柔和刹车
    /// </summary>
    public void BrakeSoftly()
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_BRAKE_LINEAR);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }

    public float SkateSpeed => GetSkateSpeedKilometerPerHour(this.m_motorRoundPerSecond);

    public int GearCount
    {
        get; set;
    }


    public int Gear => this.m_currentGear;
}

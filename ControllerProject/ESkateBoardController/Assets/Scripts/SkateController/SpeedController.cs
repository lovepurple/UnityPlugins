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

    //挡位对应的油门信息(0档为刹车)
    private float[] m_gearAcceleratorInfos = new float[GlobalDefine.GEAR_COUNT + 1];

    public SpeedController()
    {

    }

    public void InitSpeedController()
    {
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
            int intNormalizePower = Mathf.CeilToInt(normalizdPower * 1000);

            for (int i = 0; i < m_gearAcceleratorInfos.Length - 1; ++i)
            {
                int intGearPower = Mathf.CeilToInt(m_gearAcceleratorInfos[i] * 1000);

                if (Math.Abs(intNormalizePower - intGearPower) <= 10)
                    return i;
            }

            return 0;
        }
    }

    public void SetSpeedByNormalizedPower(float normalizedPower)
    {
        int powerToGear = GetGear(normalizedPower);
        SetGear(powerToGear);
    }

    public void SetDeltaGear(int deltaGear)
    {
        int dstGear = m_currentGear + deltaGear;
        if (dstGear > GlobalDefine.GEAR_COUNT - 1)
            return;

        dstGear = Mathf.Max(dstGear, 0);

        SetGear(dstGear);
    }

    public void SetGear(int gear)
    {
        if (gear <= 0)
            gear = 0;

        //不能跳档
        //if (Mathf.Abs(m_currentGear - gear) > 1)
        //    return;

        SetSkateBoardSpeedByGear(gear);


        TimeModule.Instance.SetTimeout(RequestMotorGear, 0.1f);
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
        int speedTemp = (int)(this.m_gearAcceleratorInfos[gear] * 1000);

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
                SendGearAcceleratorToSkate();
            }, 1f);
        }
        else
            TimeModule.Instance.RemoveTimeaction(RequstMotorRPS);
    }

    /// <summary>
    /// 获取电机实时转数
    /// </summary>
    private void RequstMotorRPS()
    {
        if (this.m_currentGear > 0)
        {
            List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_RPS);

            BluetoothProxy.Intance.SendData(messageBuffer);
        }
    }

    private void RequestMotorGear()
    {
        List<byte> sendMsgBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_GET_SPEED);
        BluetoothProxy.Intance.SendData(sendMsgBuffer);
    }

    private void OnGetMotorGearResponse(object data)
    {
        char[] gearData = (char[])data;
        this.m_currentGear = GetGear(((DigitUtility.GetUInt32(gearData) + 1) * 0.001f));
    }



    //public void SetSkateGearCount(int gearCount)
    //{
    //    List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_SETTING_SKATE_GEAR_COUNT);

    //    messageBuffer.AddRange(Encoding.ASCII.GetBytes(gearCount.ToString()));

    //    BluetoothProxy.Intance.SendData(messageBuffer);

    //    LocalStorage.SaveSetting(LocalSetting.E_SKATE_GEAR_COUNT, gearCount.ToString());

    //    GearCount = gearCount;
    //}

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

    public void SetGearAccelerator(int gearID, float accelerator)
    {
        float oldAccelerator = this.m_gearAcceleratorInfos[gearID];
        if (accelerator != oldAccelerator)
        {
            //保留两位小数
            accelerator = float.Parse(accelerator.ToString("0.00"));

            this.m_gearAcceleratorInfos[gearID] = accelerator;

            SendGearAcceleratorToSkate(gearID);

            SaveGearAcceleratorInfo(gearID, accelerator);
        }

    }

    public float GetGearAccelerator(int gearID)
    {
        return this.m_gearAcceleratorInfos[gearID];
    }

    /// <summary>
    /// 发送
    /// </summary>
    public void SendGearAcceleratorToSkate()
    {
        for (int i = 1; i < this.m_gearAcceleratorInfos.Length; ++i)
        {
            this.m_gearAcceleratorInfos[i] = LocalStorage.GetFloat((LocalSetting)i);

            SendGearAcceleratorToSkate(i);
        }
    }

    public void SendGearAcceleratorToSkate(int gearID)
    {
        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_SETTING_SKATE_GEAR_ACCELETOR);
        StringBuilder strGearInfo = new StringBuilder(gearID.ToString());
        strGearInfo.Append((int)(m_gearAcceleratorInfos[gearID] * 100));

        List<byte> gearInfoBuffer = DigitUtility.GetFixedLengthBufferList(Encoding.ASCII.GetBytes(strGearInfo.ToString()).ToList(), 3, (byte)'0');
        messageBuffer.AddRange(gearInfoBuffer);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }


    private void SaveGearAcceleratorInfo(int gearID, float accelerator)
    {
        LocalStorage.SaveSetting((LocalSetting)gearID, accelerator.ToString("0.00"));
    }

    public float SkateSpeed => GetSkateSpeedKilometerPerHour(this.m_motorRoundPerSecond);

    public int Gear => this.m_currentGear;
}

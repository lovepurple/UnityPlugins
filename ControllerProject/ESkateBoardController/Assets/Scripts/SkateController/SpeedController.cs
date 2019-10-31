using EngineCore;
using EngineCore.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpeedController : Singleton<SpeedController>
{
    //挡位数
    public const int GEAR_COUNT = 5;
    public float GEAR_POWER_BASIC = 0.25f;      //实际挡位的基数(0~1) 如果是1 太快,

    private int m_currentGear = 0;

    public SpeedController()
    {
        MessageHandler.RegisterMessageHandler((int)MessageDefine.E_D2C_MOTOR_SPEED, OnGetMotorGearResponse);
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
            int gear = Mathf.RoundToInt(MathUtil.Remap(normalizdPower, 0, 1.0f, 0, GEAR_COUNT));

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

        if (m_currentGear == gear)
            return;

        SetSkateBoardSpeedByGear(gear);
    }


    /// <summary>
    /// 设置滑板速度
    /// </summary>
    /// <param name="gear"></param>
    private void SetSkateBoardSpeedByGear(int gear)
    {
        int speedTemp = (int)MathUtil.Remap(gear, 0, GEAR_COUNT, 0, 999);
        int speedThoudsand = (int)(speedTemp * GEAR_POWER_BASIC);

        List<byte> speedBuffer = DigitUtility.GetFixedLengthBufferList(Encoding.ASCII.GetBytes(speedThoudsand.ToString()).ToList(), 3, (byte)'0');

        List<byte> messageBuffer = SkateMessageHandler.GetSkateMessage(MessageDefine.E_C2D_MOTOR_DRIVE);

        messageBuffer.AddRange(speedBuffer);

        BluetoothProxy.Intance.SendData(messageBuffer);
    }

    private void OnGetMotorGearResponse(object data)
    {
        char[] gearData = (char[])data;
        this.m_currentGear = GetGear(DigitUtility.GetUInt32(gearData) / 999.0f);
    }


    public int Gear => this.m_currentGear;
}

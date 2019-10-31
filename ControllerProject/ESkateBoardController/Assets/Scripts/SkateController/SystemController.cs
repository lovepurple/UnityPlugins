using EngineCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemController
{
    /// <summary>
    /// 获取电量的百分比
    /// </summary>
    /// <param name="currentVolt"></param>
    /// <returns></returns>
    public static int GetPercentageBatteryPower(float currentVolt)
    {
        return (int)MathUtil.Remap01(currentVolt, GlobalDefine.MIN_BATTERY_VOLT, GlobalDefine.MAX_BATTERY_VOLT) * 100;
    }
}

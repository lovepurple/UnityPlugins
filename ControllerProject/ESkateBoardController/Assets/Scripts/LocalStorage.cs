﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocalStorage
{

    public static void SaveSetting(LocalSetting localSetting, string settingValue)
    {
        PlayerPrefs.SetString(localSetting.ToString(), settingValue);
    }

    public static string GetSetting(LocalSetting localSetting)
    {
        return PlayerPrefs.GetString(localSetting.ToString(), string.Empty);
    }

    public static float GetFloat(LocalSetting localSetting)
    {
        string strValue = GetSetting(localSetting);
        if (string.IsNullOrEmpty(strValue))
            return 0;

        return float.Parse(strValue);
    }

    public static int GetInt(LocalSetting localSetting)
    {
        string strValue = GetSetting(localSetting);
        if (string.IsNullOrEmpty(strValue))
            return 0;

        return int.Parse(strValue);
    }

}

public enum LocalSetting
{
    E_BLUETOOTH_DEVICE_TYPE,    //蓝牙类型

    E_SKATE_GEAR_COUNT,          //挡位个数
    E_SKATE_MAX_ACCELERATOR,    //最大油门
    E_SKATE_MAX_BRAKE_TIME,     //最大刹车时间
}

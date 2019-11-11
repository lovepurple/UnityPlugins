using System.Collections;
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

}

public enum LocalSetting
{
    E_BLUETOOTH_DEVICE_TYPE,    //蓝牙类型
}

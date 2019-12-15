using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDefine 
{
    public const float MAX_BATTERY_VOLT = 25.0f;        //应该是25.2V 
    public const float MIN_BATTERY_VOLT = 19.2f;        //以3.2V做为最低放电电压

    //本地存储的配置

    public const float SAME_MESSAGE_INTERVAL = 10f;     //同一个消息连续发送，间隔
}

/// <summary>
/// 客户端与驱动端的通讯协议ID，与Arduino的MessageDefine.h保持一致
/// </summary>
public enum MessageDefine
{
    E_C2D_MOTOR_POWERON = 110,      //电机开机
    E_C2D_MOTOR_POWEROFF = 111,    //电机关机
    E_C2D_MOTOR_CORRECT_MAX_POWER = 112,    //校正最大油门
    E_C2D_MOTOR_CORRECT_MIN_POWER = 113,    //校正最小油门
    E_C2D_MOTOR_DRIVE = 114,            //电机正常调速
    E_C2D_MOTOR_INITIALIZE = 115,       //电调初始化
    E_C2D_MOTOR_NORMAL_START = 116,     //电机正常启动
}

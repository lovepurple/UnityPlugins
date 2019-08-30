#ifndef MESSAGEDEFINE_H
#define MESSAGEDEFINE_H

//C2D  Client To Driver 客户端->驱动器

enum EMessageDefine
{
    E_C2D_MOTOR_POWERON = 110,      //电机开机
    E_C2D_MOTOR_POWEROFF = 111,     //电机关机
    E_C2D_MOTOR_MAX_POWER = 112,    //校正最大油门
    E_C2D_MOTOR_MIN_POWER = 113,    //校正最小油门
    E_C2D_MOTOR_DRIVE = 114,        //电机调速
    E_C2D_MOTOR_INITIALIZE = 115,   //电调初始化
    E_C2D_MOTOR_NORMAL_START = 116, //电机正常启动
};

inline const char *GetMessageName(EMessageDefine messageDefineEnum)
{
    switch (messageDefineEnum)
    {
    case E_C2D_MOTOR_POWERON:
        return "E_C2D_MOTOR_POWERON";
    case E_C2D_MOTOR_POWEROFF:
        return "E_C2D_MOTOR_POWEROFF";
    case E_C2D_MOTOR_MAX_POWER:
        return "E_C2D_MOTOR_MAX_POWER";
    case E_C2D_MOTOR_MIN_POWER:
        return "E_C2D_MOTOR_MIN_POWER";
    case E_C2D_MOTOR_DRIVE:
        return "E_C2D_MOTOR_DRIVE";
    case E_C2D_MOTOR_INITIALIZE:
        return "E_C2D_MOTOR_INITIALIZE";
    case E_C2D_MOTOR_NORMAL_START:
        return "E_C2D_MOTOR_NORMAL_START";
    default:
        break;
    }
    return "";
}

// static inline char *stringFromFruit(enum fruit f)
// {
//     static const char *strings[] = { "apple", "orange", "grape", "banana", /* continue for rest of values */ };

//     return strings[f];
// }
#endif
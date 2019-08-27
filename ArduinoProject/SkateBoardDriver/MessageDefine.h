#ifndef MESSAGEDEFINE_H
#define MESSAGEDEFINE_H

//C2D  Client To Driver 客户端->驱动器

enum EMessageDefine
{
     //128以上暂定都是电机驱动相关协议
    E_C2D_MOTOR_POWERON           = 128,    //电机开机
    E_C2D_MOTOR_POWEROFF          = 129,    //电机关机
    E_C2D_MOTOR_CORRECT_MAX_POWER = 130,    //校正最大油门
    E_C2D_MOTOR_CORRECT_MIN_POWER = 131,    //校正最小油门
    E_C2D_MOTOR_DRIVE             = 132,    //电机正常调速
}

#endif
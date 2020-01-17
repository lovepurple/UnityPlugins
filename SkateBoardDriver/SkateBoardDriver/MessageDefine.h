// MessageDefine.h

#ifndef _MESSAGEDEFINE_h
#define _MESSAGEDEFINE_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif
//C2D  Client To Driver 客户端->驱动器

enum EMessageDefine
{
	E_C2D_SETTING_SKATE_MAX_ACCLERATOR = 95,				//设置滑板最大速度
	E_C2D_SETTING_SKATE_MAX_ACCLERATOR_BRAKE_TIME = 96,		//设置最大速度时的总刹车时间
	E_C2D_SETTING_SKATE_GEAR_COUNT = 97,					//设置滑板档位数量

	E_C2D_MOTOR_POWERON = 110,      //电机开机
	E_C2D_MOTOR_POWEROFF = 111,     //电机关机
	E_C2D_MOTOR_MAX_POWER = 112,    //校正最大油门
	E_C2D_MOTOR_MIN_POWER = 113,    //校正最小油门
	E_C2D_MOTOR_DRIVE = 114,        //电机调速
	E_C2D_MOTOR_INITIALIZE = 115,   //电调初始化
	E_C2D_MOTOR_NORMAL_START = 116, //电机正常启动

	E_C2D_MOTOR_GET_SPEED = 117, //获取电机当前速度
	E_D2C_MOTOR_SPEED = 118,     //返回电机当前速度

	E_C2D_REMAINING_POWER = 119,	//获取剩余电量
	E_D2C_REMAINING_POWER = 120,	//返回剩余电量

	E_C2D_MOTOR_RPS = 121,			//获取电机每秒转数
	E_D2C_MOTOR_RPS = 122,			//返回电机每秒转数
	E_C2D_BRAKE_LINEAR = 123,        //柔和刹车
	E_C2D_BRAKE_FORCE = 124,         //强制刹车

	//accelerator
};


struct Message
{
	EMessageDefine messageID;
	char* messageBody;
	size_t messageBodySize;
};


#endif


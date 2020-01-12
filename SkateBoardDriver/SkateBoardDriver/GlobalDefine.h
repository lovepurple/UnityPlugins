/**
 * 全局宏定义
 */
#include "Utility.h"

 //蓝牙模块引脚
#define BLUETOOTH_RX_PIN 2
#define BLUETOOTH_TX_PIN 5
#define BLUETOOTH_BAUD 9600

//电调控制引脚（使用TimerOne）
#define ESC_A_PIN 9
#define ESC_POWER_PIN 4
#define ESC_POWN_DRIVE_MODE LOW
#define MOTOR_POWER_PIN 7               //一个引脚可以控制两个引脚
#define MOTOR_POWER_DRIVE_MODE  HIGH     //电机电调继电器模式（低电平 还是高电平触发）
#define ECS_FREQUENCY 60               //电调频率

#define MOTOR_MIN_DUTY 0.05             //电机最小速度的占空比
#define MOTOR_MAX_DUTY 0.1              //电机最大速度的占空比
#define GEAR_COUNT 5                    //速度档数量

#define SYNC_GEAR_RATIO 11.0f/36.0f		//todo :同步轮的比例 需要确定小齿与大齿的比值
#define WHEEL_METER_PER_ROUND 0.2608	//轮子一圈周长
#define MOTOR_MAX_RPS 10                //电机最大转数 r/s

#define BATTERY_SENSOR_PIN A7 			//电量测量PIN

//测距模块占用引脚
#define SONAR_TRIG_PIN 13
#define SONAR_ECHO_PIN 12		

//霍尔传感器测速数字引脚
#define HALL_SENSOR_PIN 3

//Debug模式,输出Log
#define DEBUG_MODE 1

//是否Debug 消息与Debug普通的Log拆开
#define DEBUG_MESSAGE 0

//声明另一个类，include 或有循环include的问题 
class MessageHandler;

//需要加上域
typedef void (MessageHandler::* SendMessageDelegate)(byte*);
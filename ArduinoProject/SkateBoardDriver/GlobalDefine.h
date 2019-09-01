/**
 * 全局宏定义
 */

//蓝牙模块引脚
#define BLUETOOTH_RX 2
#define BLUETOOTH_TX 4

//电调控制引脚（使用TimerOne）
#define ESC_A 9
#define ECS_B 10
#define MOTOR_POWER_PIN 7               //一个引脚可以控制两个引脚
#define MOTOR_POWER_DRIVE_MODE  LOW     //电机电调继电器模式（低电平 还是高电平触发）
#define ECS_FREQUENCY 100               //电调频率
#define MOTOR_MIN_DUTY 0.05             //电机最小速度的占空比
#define MOTOR_MAX_DUTY 0.1              //电机最大速度的占空比
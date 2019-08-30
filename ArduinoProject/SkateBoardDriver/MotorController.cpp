#include "MotorController.h"

MotorController::MotorController(uint8_t motorPowerPin,uint8_t ecsPinA,uint8_t ecsPinB)
{
    this->m_motorPowerPin = motorPowerPin;
    this->m_ecsPinA = ecsPinA;

    pinMode(this->m_motorPowerPin,OUTPUT);

    //初始化Timer 50hz = 20000 微秒     占空比 1/20 最低 2/20 最高
    Timer1.initialize(20000);
}

MotorController::~MotorController()
{
}

//Static 方法在cpp中不需要加Static
//一个引脚通过一分二可驱动同时多个信号
void MotorController::PowerOn(){
    digitalWrite(this->m_motorPowerPin,MOTOR_POWER_DRIVE_MODE);
}

void MotorController::PowerOff(){
    digitalWrite(this->m_motorPowerPin,!MOTOR_POWER_DRIVE_MODE);
}

void MotorController::InitializeESC()
{
    MotorMaxPower();
    delay(2000);
    PowerOn();
}

void MotorController::MotorMinPower()
{
    Timer1.pwm(m_ecsPinA,0.05 * 1023);
}

void MotorController::MotorMaxPower()
{
    Timer1.pwm(m_ecsPinA,0.1 * 1023);
}

void SetMotorController(MotorController* motorController);

MotorController* GetMotorController();



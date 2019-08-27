#include "MotorController.h"

MotorController::MotorController(uint8_t motorPowerPin,uint8_t ecsPinA,uint8_t ecsPinB)
{
    this->m_motorPowerPin = motorPowerPin;

    pinMode(this->m_motorPowerPin,OUTPUT);
}

MotorController::~MotorController()
{
}

//Static 方法在cpp中不需要加Static
//一个引脚通过一分二可驱动同时多个信号
void MotorController::PowerOn(){
    digitalWrite(this->m_motorPowerPin,HIGH);
}

void MotorController::PowerOff(){
    digitalWrite(this->m_motorPowerPin,LOW);
}

 void SetMotorController(MotorController* motorController);

    MotorController* GetMotorController();
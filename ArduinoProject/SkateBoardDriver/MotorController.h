/*
    电机驱动控制器
*/
#ifndef MOTORCONTROLLER_H
#define MOTORCONTROLLER_H
#include <Arduino.h>
#include <TimerOne.h>

class MotorController
{
private:
    uint8_t m_motorPowerPin;
    uint8_t m_ecsPinA;

public:
    MotorController(uint8_t motorPowerPin,uint8_t ecsPinA,uint8_t ecsPinB);
    ~MotorController();

    void PowerOn();

    void PowerOff();

    //初始化电调
    void InitializeESC();

    void MotorMinPower();

    void MotorMaxPower();
};

#endif
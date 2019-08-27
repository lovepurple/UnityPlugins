/*
    电机驱动控制器
*/
#ifndef MOTORCONTROLLER_H
#define MOTORCONTROLLER_H
#include <Arduino.h>

class MotorController
{
private:
    uint8_t m_motorPowerPin;

public:
    MotorController(uint8_t motorPowerPin,uint8_t ecsPinA,uint8_t ecsPinB)
    ~MotorController();

    void PowerOn();

    void PowerOff();
};

#endif
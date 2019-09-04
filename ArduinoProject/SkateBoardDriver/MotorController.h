/*
    电机驱动控制器
*/
#ifndef MOTORCONTROLLER_H
#define MOTORCONTROLLER_H
#include <Arduino.h>
#include <TimerOne.h>
#include "GlobalDefine.h"
#include "Utility.h"
#include "MessageDefine.h"

class MotorController
{
private:
    uint8_t m_motorPowerPin;
    uint8_t m_ecsPinA;

    float m_currentMotorDuty = 0.0;

    SendMessageDelegate m_sendMessageDelegate;

public:
    MotorController(uint8_t motorPowerPin, uint8_t ecsPinA, uint8_t ecsPinB);
    ~MotorController();

    bool IsPowerOn();

    void PowerOn();

    void PowerOff();

    //初始化电调
    void InitializeESC();

    void MotorMinPower();

    void MotorMaxPower();

    //使用百分比设置速度
    void SetMotorPower(const float percentage01);

    //获取当前百分比的速度
    float GetMotorPower();

    //使用占空比设置速度
    void SetSpeedByDuty(float pwmDuty);

        /**
     * 设置发消息的代理
     */ 
    void SetSendMessageDelegate(SendMessageDelegate delegate);



    /**************消息处理************************/
    //todo:之后用(void*)
    //处理获取当前速度消息
    //C++常用的方式，对象内存传入，不在函数内分配新的堆内容
    byte* Handle_GetCurrentSpeedMessage(char data[8]);       

    //设置速度处理
    void Handle_SetPercentageSpeedMessage(MessageBody& messageBody);


};

#endif
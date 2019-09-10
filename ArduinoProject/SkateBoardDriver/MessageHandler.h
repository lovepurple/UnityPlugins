/*
    消息处理中心
*/
#ifndef MESSAGEHANDLER_H
#define MESSAGEHANDLER_H

#include <Arduino.h> //系统默认的Serial
#include "NeoSWSerial.h"
#include "DynamicBuffer.h"
#include "MotorController.h"
#include "MessageDefine.h"
#include "QList.h"
#include "TimerOne.h"

class MessageHandler
{
private:
    NeoSWSerial* m_bluetooth;

    byte* m_pTempBuffer;      

    MotorController *m_motorColtroller;

    void OnHandleMessage(Message& message);

    //发送队列
    QList<byte*> m_sendMessageQueue;

    //发消息  todo：BufferCount 
    void SendMessageInternal(char *sendBuffer);

    void TimerInterrupt();

public:
    MessageHandler(uint8_t rx, uint8_t tx);

    void Tick();

    //消息加入发送队列
    void SendMessage(byte* messageBuffer);

    NeoSWSerial *GetBluetoothSerial();

    void SetMotorController(MotorController *motorController);

    MotorController *GetMotorController();

    //消息结束标识
    static char Message_End_Flag;
    // static void SendMessage(byte* messageBuffer);

    void Nidaye();

};
#endif
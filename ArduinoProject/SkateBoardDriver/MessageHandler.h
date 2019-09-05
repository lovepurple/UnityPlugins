/*
    消息处理中心
*/
#ifndef MESSAGEHANDLER_H
#define MESSAGEHANDLER_H

#include <Arduino.h> //系统默认的Serial
#include "NeoSWSerial.h"

#include "MotorController.h"
#include "MessageDefine.h"
#include "QList.h"

class MessageHandler
{
private:
    NeoSWSerial *m_bluetooth;
    uint8_t m_rxPin; //uinit_8 = char
    uint8_t m_txPin;

    byte m_tempBuffer[64];

    MotorController *m_motorColtroller;

    void OnHandleMessage(Message& message);

    //发送队列
    QList<byte*> m_sendMessageQueue;

    //发消息  todo：BufferCount 
    void SendMessageInternal(char *sendBuffer);

public:
    MessageHandler(uint8_t rx, uint8_t tx, uint16_t baudRate = 9600);

    void Tick();

    //消息加入发送队列
    void SendMessage(byte* messageBuffer);

    NeoSWSerial *GetBluetoothSerial();

    void SetMotorController(MotorController *motorController);

    MotorController *GetMotorController();

    //消息结束标识
    static char Message_End_Flag;
    // static void SendMessage(byte* messageBuffer);

};
#endif
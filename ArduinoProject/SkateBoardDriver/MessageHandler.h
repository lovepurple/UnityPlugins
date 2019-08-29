/*
    消息处理中心
*/
#ifndef MESSAGEHANDLER_H
#define MESSAGEHANDLER_H

#include <Arduino.h>            //系统默认的Serial
#include "NeoSWSerial.h"

#include "MotorController.h"
#include "MessageDefine.h"


class MessageHandler
{
private:
     NeoSWSerial* m_bluetooth;
     uint8_t m_rxPin;           //uinit_8 = char
     uint8_t m_txPin;

     byte m_tempBuffer[64];

     MotorController* m_motorColtroller;

     void OnHandleMessage(EMessageDefine messageID,byte* messageBuffer);

public:
    MessageHandler(uint8_t rx,uint8_t tx,uint16_t baudRate = 9600);

    void Tick();

    void SendMessage(char* sendBuffer);

    NeoSWSerial* GetBluetoothSerial();

    void SetMotorController(MotorController* motorController);

    MotorController* GetMotorController();

    //消息结束标识
    static char Message_End_Flag;
};
#endif
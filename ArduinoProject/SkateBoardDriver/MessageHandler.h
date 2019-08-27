/*
    消息处理中心
*/
#ifndef MESSAGEHANDLER_H
#define MESSAGEHANDLER_H

#include <Arduino.h>            //系统默认的Serial
#include "NeoSWSerial.h"


class MessageHandler
{
private:
     NeoSWSerial* m_bluetooth;
     uint8_t m_rxPin;           //uinit_8 = char
     uint8_t m_txPin;


public:
    MessageHandler(uint8_t rx,uint8_t tx,uint16_t baudRate = 9600);

    void Tick();

    void SendMessage(char* sendBuffer);

    NeoSWSerial* GetBluetoothSerial();

};
#endif
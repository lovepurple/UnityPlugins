//#include "ArduinoJson.hpp"
//todo? 消息是\0结尾，每次发送新行，换行符 \n

#include "MessageHandler.h"
#include "MotorController.h"
#include "GlobalDefine.h"
#include "DynamicBuffer.h"
#include "NeoSWSerial.h"
// MessageHandler* pMessageHandler = nullptr;
// MotorController *pMotorController = nullptr;

// NeoSWSerial bluetooth(2, 3);
// NeoSWSerial* pBluetoothSerial;

void setup()
{
    Serial.begin(9600);

    pBluetoothSerial = new NeoSWSerial(2,3);
    pBluetoothSerial->begin(9600);

    while (!Serial)
    {
    }

    // pMessageHandler = &MessageHandler(2,3);
    // pMessageHandler->SetSoftwareBluetooth(&sb);

    // pMotorController = new MotorController(MOTOR_POWER_PIN, ESC_A, ECS_B);
    // pMotorController->SetSendMessageDelegate(&(pMessageHandler->SendMessage),pMessageHandler);      //函数指针的赋值

    // pMessageHandler->SetMotorController(pMotorController);
}

void loop()
{
    // pMessageHandler->Tick();

    while ( pBluetoothSerial->available() > 0)
        Serial.print((char) pBluetoothSerial->read());
    // messageHandler.Tick();
    // if (pMessageHandler != nullptr)
    // {
    //     pMessageHandler->Tick();
    // }

    // while(bt.available())
    // {
    //     Serial.println((char)bt.read());
    // }
}
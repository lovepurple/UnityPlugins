//#include "ArduinoJson.hpp"
//todo? 消息是\0结尾，每次发送新行，换行符 \n

#include "MessageHandler.h"
#include "MotorController.h"
#include "GlobalDefine.h"

MessageHandler *pMessageHandler = nullptr;
MotorController *pMotorController = nullptr;



void setup()
{
    Serial.begin(9600);

    pMessageHandler = new MessageHandler(BLUETOOTH_RX, BLUETOOTH_TX,9600);

    while (!Serial)
    {
    }

    pMotorController = new MotorController(MOTOR_POWER_PIN, ESC_A, ECS_B);
    pMotorController->SetSendMessageDelegate(&(pMessageHandler->SendMessage),pMessageHandler);      //函数指针的赋值

    pMessageHandler->SetMotorController(pMotorController);

}

void loop()
{
    if (pMessageHandler != nullptr)
        pMessageHandler->Tick();

}
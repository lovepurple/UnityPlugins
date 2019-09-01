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
    pMessageHandler = new MessageHandler(BLUETOOTH_RX, BLUETOOTH_TX);

    while (!Serial)
    {
    }

    pMotorController = new MotorController(MOTOR_POWER_PIN, ESC_A, ECS_B);

    pMessageHandler->SetMotorController(pMotorController);
}

void loop()
{
    if (pMessageHandler != nullptr)
        pMessageHandler->Tick();

}
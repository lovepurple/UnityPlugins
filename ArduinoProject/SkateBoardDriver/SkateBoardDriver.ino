//#include "ArduinoJson.hpp"
//todo? 消息是\0结尾，每次发送新行，换行符 \n

#include "MessageHandler.h"
#include "MotorController.h"

//蓝牙模块引脚
#define BLUETOOTH_RX 2
#define BLUETOOTH_TX 4

//电调控制引脚（使用TimerOne）
#define ESC_A 9
#define ECS_B 10
#define MOTOR_POWER_PIN 7       //一个引脚可以控制两个引脚

MessageHandler *pMessageHandler = nullptr;
MotorController* pMotorController = nullptr;

void setup()
{
    Serial.begin(9600);
    pMessageHandler = new MessageHandler(BLUETOOTH_RX,BLUETOOTH_TX);

    while (!Serial)
    {
    }

    pMotorController = new MotorController(MOTOR_POWER_PIN,ESC_A,ECS_B);
    
    pMessageHandler->SetMotorController(pMotorController);
}

void loop()
{
    if (pMessageHandler != nullptr)
        pMessageHandler->Tick();
}
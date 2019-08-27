//#include "ArduinoJson.hpp"
//todo? 消息是\0结尾，每次发送新行，换行符 \n

#include "MessageHandler.h"

//蓝牙模块引脚
#define BLUETOOTH_RX 2
#define BLUETOOTH_TX 4

//电调控制引脚（使用TimerOne）
#define ESC_A 9
#define ECS_B 10

// size_t bufferSize =0;
// char* sendBuffer = new char[128];

//C++ 中，如果成员需要在其它位置初始化（这里只是个引用），需要用指针
// SoftwareSerial *pBluetooth = &(SoftwareSerial(BLUETOOTH_RX, BLUETOOTH_TX));
// MessageHandler *pMessageHandler = nullptr;
// SoftwareSerial bluetooth(BLUETOOTH_RX, BLUETOOTH_TX);

// AltSoftSerial altSerial(2,4);

MessageHandler *pMessageHandler = nullptr;

void setup()
{
    Serial.begin(9600);
    pMessageHandler = new MessageHandler(BLUETOOTH_RX,BLUETOOTH_TX);

    while (!Serial)
    {
    }
    

}

void loop()
{

    if (pMessageHandler != nullptr)
        pMessageHandler->Tick();

    delay(1000);
    char buffer[16];
    ltoa(millis(),buffer,10);
    pMessageHandler->SendMessage(buffer);
    Serial.println("sending ...");
    Serial.println(buffer);

    // while(pBluetooth->available() >0)
    // {
    //     Serial.print((char)pBluetooth->read());
    // }

}

// #include "SoftwareSerial.h"

// SoftwareSerial* pBluetooth;
// void setup() {
// 	pBluetooth = new SoftwareSerial(2, 4);
// 	pBluetooth->begin(38400);

// 	Serial.begin(9600);
// }

// // the loop function runs over and over again until power down or reset
// void loop() {
// 	while (pBluetooth->available() > 0)
// 	{
// 		Serial.print((char)pBluetooth->read());
// 	}
// 	delay(5000);

// 	Serial.println("--------------");
// }
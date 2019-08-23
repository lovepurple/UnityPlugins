//#include "ArduinoJson.hpp"
//todo? 消息是\0结尾，每次发送新行，换行符 \n

#include "ArduinoJson.h"
#include <SoftwareSerial.h>

#define BLUETOOTH_RX 2
#define BLUETOOTH_TX 4
SoftwareSerial bluetooth(BLUETOOTH_RX, BLUETOOTH_TX);

size_t bufferSize =0;
char* sendBuffer = new char[128];

void setup()
{
    Serial.begin(9600);
    bluetooth.begin(38400);

    delay(10 * 1000);

    //测试序列化JSON
    StaticJsonDocument<200> doc;
    doc["platform"] = "Arduino";
    doc["Format"]=1;

    Serial.print(F("Sending: "));
    bufferSize = serializeJson(doc, sendBuffer,128);
    Serial.println();


}


void loop()
{
    // while (bluetooth.available())
    // {
    //     Serial.println((char)bluetooth.read());
    // }
    if(bufferSize >0)
    {
        int index =0;
        while(index < bufferSize)
        {
            bluetooth.write(sendBuffer[index++]);
        }
        bufferSize =0;
    }
}
#include "MessageHandler.h"

MessageHandler::MessageHandler(uint8_t rx,uint8_t tx,uint16_t bandRate = 9600){
    this->m_rxPin = rx;
    this->m_txPin = tx;
    
    this->m_bluetooth = new NeoSWSerial(this->m_rxPin,this->m_txPin);
    this->m_bluetooth->begin(bandRate);
}

void MessageHandler::Tick()
{
    char tempBuffer[64];
    while(this->m_bluetooth->available() > 0){
        Serial.print(m_bluetooth->read());
    }
    
}

void MessageHandler::SendMessage(char* sendBuffer){
    
    int index = 0;
    while (sendBuffer[index])
    {
        m_bluetooth->write(sendBuffer[index]);

        Serial.print(sendBuffer[index]);
        index++;
    }
    this->m_bluetooth->write("\r\n");
}

NeoSWSerial* MessageHandler::GetBluetoothSerial(){
    return this->m_bluetooth;
}


void MessageHandler::SetMotorController(MotorController* motorController){
    this->m_motorColtroller = motorController;
}

MotorController* MessageHandler::GetMotorController(){
    return this->m_motorColtroller;
}
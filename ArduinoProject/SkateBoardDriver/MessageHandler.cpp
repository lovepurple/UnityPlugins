#include "MessageHandler.h"

MessageHandler::MessageHandler(uint8_t rx, uint8_t tx, uint16_t bandRate = 9600)
{
    this->m_rxPin = rx;
    this->m_txPin = tx;

    this->m_bluetooth = new NeoSWSerial(this->m_rxPin, this->m_txPin);
    this->m_bluetooth->begin(bandRate);
}

int m_recvBufferCount = 0;

void MessageHandler::Tick()
{
    //一次发送有可能avaliable() 与发送过来的数量不同，一个Tick没法接收完整
    while (this->m_bluetooth->available() > 0)
    {
        byte recvByte = m_bluetooth->read();

        if (recvByte != MessageHandler::Message_End_Flag)
        {
            m_tempBuffer[m_recvBufferCount++] = recvByte;
            Serial.println(recvByte);
        }
        else
        {
            if (m_recvBufferCount > 0)
            {
                EMessageDefine messageType = (EMessageDefine)m_tempBuffer[0];
                byte messageBuffer[m_recvBufferCount];
                // memcpy(messageBuffer, m_tempBuffer + 1, sizeof(byte) * (m_recvBufferCount-1));
                for(int i =1;i<m_recvBufferCount;++i)
                {
                    messageBuffer[i-1] = m_tempBuffer[i];
                }
                messageBuffer[m_recvBufferCount-1]='\0';

                Serial.println("buffer count");
                Serial.println(m_recvBufferCount);
                Serial.println("**************************************");
                byte* pMessageBuffer = &messageBuffer[0];
                while(*pMessageBuffer)
                {
                    Serial.println(*pMessageBuffer);
                    pMessageBuffer++;
                }
                Serial.println("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");

                OnHandleMessage(messageType, messageBuffer);
            }

            m_recvBufferCount=0;
        }
    }

    while(m_sendMessageQueue.size() >0)
    {
        byte* sendBuffer = m_sendMessageQueue.front();
        Serial.println("sendMessage");
        Serial.println(sendBuffer[0]);
        SendMessageInternal((char*)sendBuffer);
        m_sendMessageQueue.pop_front();
    }
}

void MessageHandler::SendMessageInternal(char *sendBuffer)
{
    Serial.println("Send Message:");
    Serial.println((char)sendBuffer[0]);
    while(*sendBuffer)
    {
        m_bluetooth->write(sendBuffer);

        sendBuffer++;
    }
    this->m_bluetooth->write(MessageHandler::Message_End_Flag);
}

void MessageHandler::OnHandleMessage(EMessageDefine messageID, byte *messageBuffer)
{
    Serial.println("Handle Message");
    Serial.println(GetMessageName(messageID));

    switch (messageID)
    {
    case E_C2D_MOTOR_POWERON: //C++枚举不用写全名
        this->m_motorColtroller->PowerOn();
        break;
    case E_C2D_MOTOR_POWEROFF:
        this->m_motorColtroller->PowerOff();
        break;
    case E_C2D_MOTOR_MAX_POWER:
        this->m_motorColtroller->MotorMaxPower();
        break;
    case E_C2D_MOTOR_MIN_POWER:
        this->m_motorColtroller->MotorMinPower();
        break;
    case E_C2D_MOTOR_DRIVE:
        this->m_motorColtroller->Handle_SetPercentageSpeedMessage(&messageBuffer[0]);
        break;
    case E_C2D_MOTOR_INITIALIZE:
        this->m_motorColtroller->InitializeESC();
        break;
    case E_C2D_MOTOR_NORMAL_START:
        break;
    case E_C2D_MOTOR_GET_SPEED:
        char resultData[8];
        byte* responseBuffer = this-> m_motorColtroller->Handle_GetCurrentSpeedMessage(resultData);
        SendMessage(responseBuffer);
        break;
    default:
        break;
    }
}

NeoSWSerial *MessageHandler::GetBluetoothSerial()
{
    return this->m_bluetooth;
}

void MessageHandler::SetMotorController(MotorController *motorController)
{
    this->m_motorColtroller = motorController;
}

MotorController *MessageHandler::GetMotorController()
{
    return this->m_motorColtroller;
}

void MessageHandler::SendMessage(byte* messageBuffer)
{
    byte* tempBuffer = &messageBuffer[0];       //坑！！！！
    while(*tempBuffer)
    {
        Serial.println((char)*tempBuffer);
        tempBuffer++;
    }

    this->m_sendMessageQueue.push_back(messageBuffer);
}


char MessageHandler::Message_End_Flag = '\n';
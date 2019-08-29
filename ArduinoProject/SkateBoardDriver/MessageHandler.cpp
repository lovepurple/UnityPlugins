#include "MessageHandler.h"

MessageHandler::MessageHandler(uint8_t rx, uint8_t tx, uint16_t bandRate = 9600)
{
    this->m_rxPin = rx;
    this->m_txPin = tx;

    this->m_bluetooth = new NeoSWSerial(this->m_rxPin, this->m_txPin);
    this->m_bluetooth->begin(bandRate);
}

void MessageHandler::Tick()
{
    int bufferIndex = 0;
    
    while (this->m_bluetooth->available() > 0)
    {
        Serial.println("^^^^^^^^^^^^^^^^^^^^^^^^^^");
        Serial.println(bufferIndex);
        Serial.println("%%%%%%%%%%%%%%%%%%%%%%");
        byte recvByte = m_bluetooth->read();

        Serial.println(recvByte);

        if (recvByte != MessageHandler::Message_End_Flag)
        {
            m_tempBuffer[bufferIndex++] = recvByte;
            Serial.println("&&&&&&&&&&&&&&&&&&&&");
            Serial.println(bufferIndex);
            Serial.println("*********************");
        }
        else
        {
            Serial.println("---------------");
            Serial.println(bufferIndex);
             Serial.println("+++++++++++");
            //无效消息
            if (bufferIndex > 0)
            {
                Serial.println(m_tempBuffer[0]);
                EMessageDefine messageType = (EMessageDefine)m_tempBuffer[0];
                Serial.println("---------------");
                Serial.println(messageType);
                byte messageBuffer[bufferIndex];
                memcpy(messageBuffer, m_tempBuffer + 1, sizeof(byte) * bufferIndex);

                OnHandleMessage(messageType, messageBuffer);
            }
            break;
        }
    }
}

void MessageHandler::SendMessage(char *sendBuffer)
{

    int index = 0;
    while (sendBuffer[index])
    {
        m_bluetooth->write(sendBuffer[index]);

        Serial.print(sendBuffer[index]);
        index++;
    }
    this->m_bluetooth->write(MessageHandler::Message_End_Flag);
}

void MessageHandler::OnHandleMessage(EMessageDefine messageID, byte *messageBuffer)
{
    Serial.println("Handle msg");
    Serial.println(messageID);

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
        break;
    case E_C2D_MOTOR_INITIALIZE:
        this->m_motorColtroller->InitializeESC();
        break;
    case E_C2D_MOTOR_NORMAL_START:
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

char MessageHandler::Message_End_Flag = '\n';
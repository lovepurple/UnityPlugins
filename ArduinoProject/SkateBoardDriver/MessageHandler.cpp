#include "MessageHandler.h"

void MessageHandler::TimerInterrupt()
{
    //如果消息太多加入MsTimer2 把发送单独使用Timer跑
}


MessageHandler::MessageHandler(uint8_t rx, uint8_t tx)
{
    // m_bluetooth = &NeoSWSerial(rx,tx);
    // m_bluetooth->begin(9600);

    // m_bluetooth=
    // this->m_rxPin = rx;
    // this->m_txPin = tx;

    // this->m_bluetooth = new NeoSWSerial(this->m_rxPin, this->m_txPin);
    // this->m_bluetooth->begin(bandRate);

    // Timer1.attachInterrupt(TimerInterrupt);
}


//能用小范围就用小范围
size_t m_recvBufferCount = 0;

int freeRam()
{
    extern int __heap_start, *__brkval;
    int v;
    return (int)&v - (__brkval == 0 ? (int)&__heap_start : (int)__brkval);
}

void MessageHandler::Tick()
{
    // Serial.println(" sha qikg ");
    // if(m_bluetooth->available() >0)
    //     Serial.println(m_bluetooth->read());
     
    //一次发送有可能avaliable() 与发送过来的数量不同，一个Tick没法接收完整
    //速度快时，有可能丢包
    // while (m_bluetooth->available() > 0)
    // {
    //     if(m_recvBufferCount==0)
    //         m_pTempBuffer = DynamicBuffer::GetBuffer();

    //     byte recvByte = m_bluetooth->read();

    //     Serial.println("=+++++++++++++++++++++++");
        // if (recvByte != MessageHandler::Message_End_Flag)
        // {
        //     m_pTempBuffer[m_recvBufferCount++] = recvByte;
        //     Serial.println((char)recvByte);
        // }
        // else
        // {
        //     Serial.println("&&&&&&&&&&&&&&&&&&&&&&&");
        //     if (m_recvBufferCount > 0)
        //     {
        //         EMessageDefine messageType = (EMessageDefine)m_pTempBuffer[0];
        //         byte messageBuffer[m_recvBufferCount - 1];
        //         // memset(messageBuffer, 0, m_recvBufferCount - 1); //memset 内存空间初始化
        //         // memcpy(messageBuffer, m_tempBuffer + 1, sizeof(byte) * (m_recvBufferCount-1));

        //         //暂时不跟memcpy较劲了，可能tempBuffer长度不规则
        //         // for (int i = 1; i < m_recvBufferCount; ++i)
        //         // {
        //         //     messageBuffer[i - 1] = m_pTempBuffer[i];
        //         // }
        //         // messageBuffer[m_recvBufferCount - 1] = (byte)'\0';

        //         m_pTempBuffer[m_recvBufferCount-1] = (byte)'\0';

        //         MessageBody messageBody = {
        //             m_pTempBuffer,
        //             m_recvBufferCount - 1};

        //         Message message = {
        //             messageType,
        //             messageBody};

        //         Serial.println(*m_pTempBuffer);

        //         // OnHandleMessage(message);
        //         // DynamicBuffer::RecycleBuffer(m_pTempBuffer);
        //     }

        //     m_recvBufferCount = 0;
        // }
    // }

    // while (m_sendMessageQueue.size() > 0)
    // {
    //     byte *sendBuffer = this->m_sendMessageQueue.front();
    //     Serial.println("sendMessage");
    //     Serial.println(sendBuffer[0]);
    //     this->SendMessageInternal((char *)sendBuffer);
    //    this-> m_sendMessageQueue.pop_front();
    // }
}

void MessageHandler::SendMessageInternal(char *sendBuffer)
{
    Serial.println("Send Message:");
    Serial.println((char)sendBuffer[0]);
    while (*sendBuffer)
    {
        Serial.write(*sendBuffer);
        sendBuffer++;
    }
    Serial.write(MessageHandler::Message_End_Flag);

    DynamicBuffer::RecycleBuffer(&sendBuffer[0]);
}

void MessageHandler::OnHandleMessage(Message &message)
{
    Serial.println("Handle Message");
    Serial.println(GetMessageName(message.messageID));

    return;

    switch (message.messageID)
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
        this->m_motorColtroller->Handle_SetPercentageSpeedMessage(message.messageBody);
        break;
    case E_C2D_MOTOR_INITIALIZE:
        this->m_motorColtroller->InitializeESC();
        break;
    case E_C2D_MOTOR_NORMAL_START:
        break;
    case E_C2D_MOTOR_GET_SPEED:
        byte *responseBuffer = this->m_motorColtroller->Handle_GetCurrentSpeedMessage();
        SendMessage(responseBuffer);
        break;
    }
}

// NeoSWSerial *MessageHandler::GetBluetoothSerial()
// {
//     return this->m_bluetooth;
// }

void MessageHandler::SetMotorController(MotorController *motorController)
{
    this->m_motorColtroller = motorController;
}

MotorController *MessageHandler::GetMotorController()
{
    return this->m_motorColtroller;
}

void MessageHandler::Nidaye()
{
    Serial.println("cao ni da ye");
}

void MessageHandler::SendMessage(byte *messageBuffer)
{
    byte *tempBuffer = &messageBuffer[0]; //坑！！！！
    // while (*tempBuffer)
    // {
    //     Serial.println((char)*tempBuffer);
    //     tempBuffer++;
    // }

    this->m_sendMessageQueue.push_back(messageBuffer);
}

char MessageHandler::Message_End_Flag = '\n';
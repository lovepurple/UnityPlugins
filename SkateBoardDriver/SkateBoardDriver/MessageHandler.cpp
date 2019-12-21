#include "MessageHandler.h"

MessageHandlerClass::MessageHandlerClass()
{
	MessageHandler.m_pBluetooth = new NeoSWSerial(BLUETOOTH_RX_PIN, BLUETOOTH_TX_PIN);
	MessageHandler.m_pBluetooth->begin(BLUETOOTH_BAUD);
	MessageHandler.m_pBluetooth->listen();

	MotorController.init();
}

MessageHandlerClass::~MessageHandlerClass()
{
}

static size_t RecvCount = 0;

void MessageHandlerClass::Tick()
{
	char endMarker = '\n';
	char rc;

	while (MessageHandler.m_pBluetooth->available() > 0)
	{
		rc = MessageHandler.m_pBluetooth->read();

		if (rc != '\n')
		{
			m_tempRecvBuffer[RecvCount++] = rc;
		}
		else
		{
			char* messageBuffer = DynamicBuffer.GetBuffer();

			for (int i = 0; i < RecvCount; ++i)
				messageBuffer[i] = m_tempRecvBuffer[i];

			messageBuffer[RecvCount] = '\0';

			Message message =
			{
				(EMessageDefine)messageBuffer[0],
				 messageBuffer + 1,
				 RecvCount - 1,
			};

			UtilityClass::DebugMessage("RecvMessage:", false);
			UtilityClass::DebugMessage(messageBuffer);

			OnHandleMessage(message);

			RecvCount = 0;
			DynamicBuffer.RecycleBuffer(messageBuffer);
		}
	}

	SendMessageInternal();
}

void MessageHandlerClass::OnHandleMessage(Message& message)
{
	switch (message.messageID)
	{
	case E_C2D_MOTOR_POWERON: //C++枚举不用写全名
		MotorController.PowerOn();
		break;
	case E_C2D_MOTOR_POWEROFF:
		MotorController.PowerOff();
		break;
	case E_C2D_MOTOR_MAX_POWER:
		MotorController.MotorMaxPower();
		break;
	case E_C2D_MOTOR_MIN_POWER:
		MotorController.MotorMinPower();
		break;
	case E_C2D_MOTOR_DRIVE:
		MotorController.Handle_SetPercentageSpeedMessage(message);
		break;
	case E_C2D_MOTOR_INITIALIZE:
		MotorController.InitializeESC();
		break;
	case E_C2D_MOTOR_NORMAL_START:
		MotorController.MotorStarup();
		break;
	case E_C2D_MOTOR_GET_SPEED:
	{
		char* responseBuffer = MotorController.Handle_GetCurrentSpeedMessage();
		SendMessage(responseBuffer);
		break;
	}
	case E_C2D_REMAINING_POWER:
	{
		char* responseBuffer1 = SystemController.Handle_GetSystemRemainingPower();
		SendMessage(responseBuffer1);
		break;
	}
	case E_C2D_MOTOR_RPS:
	{
		char* pMotorRPSBuffer = SpeedMonitor.GetCurrentMotorRPSMesssage();
		SendMessage(pMotorRPSBuffer);
		break;
	}
	}
}

void MessageHandlerClass::SendMessage(char* messageBuffer)
{
	m_sendMessageQueue.push_back(messageBuffer);
}

void MessageHandlerClass::SendMessageInternal()
{
	while (MessageHandler.m_sendMessageQueue.size() > 0)
	{
		char* sendBuffer = MessageHandler.m_sendMessageQueue.front();
		char* pSendBuffer = sendBuffer;

		while (*pSendBuffer)
		{
			MessageHandler.m_pBluetooth->write(*pSendBuffer);
			pSendBuffer++;
		}
		MessageHandler.m_pBluetooth->write('\n');
		MessageHandler.m_pBluetooth->flush();

		UtilityClass::DebugMessage("Sending Message :", false);
		UtilityClass::DebugMessage(String((int)sendBuffer[0]), false);
		char* pSendingBufferContent = &sendBuffer[0];
		UtilityClass::DebugMessage(pSendingBufferContent);

		DynamicBuffer.RecycleBuffer(sendBuffer);

		MessageHandler.m_sendMessageQueue.pop_front();
	}
}

char MessageHandlerClass::Message_End_Flag = '\n';

MessageHandlerClass MessageHandler;		//在CPP里需要再声明一下
// MessageHandler.h

#ifndef _MESSAGEHANDLER_h
#define _MESSAGEHANDLER_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

#include "GlobalDefine.h"
#include "NeoSWSerial.h"
#include "DynamicBuffer.h"
#include "MessageDefine.h"
#include "TimerOne.h"
#include "Utility.h"
#include "MotorController.h"
#include "SystemController.h"
#include "DriverMonitor.h"

/**
 * Arduino 消息处理类
 */
class MessageHandlerClass
{
public:
	MessageHandlerClass();
	~MessageHandlerClass();

	void Tick();

	void OnHandleMessage(Message& message);

	void SendMessage(char* messageBuffer);

	static char Message_End_Flag;
private:
	NeoSWSerial* m_pBluetooth;
	char m_tempRecvBuffer[16];

	void SendMessageInternal();

	QList<char*> m_sendMessageQueue = QList<char*>();
};

extern MessageHandlerClass MessageHandler;


#endif


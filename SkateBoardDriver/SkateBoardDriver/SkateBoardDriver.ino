/*
 Name:		SkateBoardDriver.ino
 Created:	9/8/2019 11:52:35 PM
 Author:	purple
*/

#include "Utility.h"
#include "MotorController.h"
#include "DynamicBuffer.h"
#include "MessageHandler.h"

void setup()
{
	Serial.begin(9600);
	DynamicBuffer.init();
}

void loop() {

	MessageHandler.Tick();

	//if (Serial.available() > 0)
	//{
	//	char* buffer= DynamicBuffer.GetBuffer();
	//	buffer[0] = '2';
	//	buffer[1] = '\0';
	//	MessageHandler.SendMessage(buffer);
	//}
	//char* buffer = MotorController.Handle_GetCurrentSpeedMessage();
	//MessageHandler.SendMessage(buffer);
}
